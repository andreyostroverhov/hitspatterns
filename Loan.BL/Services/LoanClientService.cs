using Common.DataTransferObjects;
using Common.Enums;
using Common.Exceptions;
using Common.Interfaces;
using Loan.DAL.Data;
using Loan.DAL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Loan.BL.Services;

public class LoanClientService : ILoanClientService
{

    private readonly ILogger<LoanClientService> _logger;
    private readonly LoanDbContext _loanDbContext;

    public LoanClientService(ILogger<LoanClientService> logger, LoanDbContext loanDbContext)
    {
        _logger = logger;
        _loanDbContext = loanDbContext;
    }

    public async Task<LoanDto> TakeLoanAsync(TakeLoanRequest request, Guid clientId)
    {
        var tariff = await _loanDbContext.Tariffs
            .FirstOrDefaultAsync(t => t.Id == request.TariffId) ?? throw new NotFoundException("Tariff not found.");

        var loan = new DAL.Data.Entities.Loan
        {
            ClientId = clientId,
            AccountId = request.AccountId,
            TariffId = request.TariffId,
            Amount = request.Amount.Amount,
            Currency = request.Amount.Currency,
            RemainingAmount = request.Amount.Amount, // Оставшаяся сумма равна сумме кредита
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(request.NumberOfMonths),
            Status = LoanStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _loanDbContext.Loans.Add(loan);
        await _loanDbContext.SaveChangesAsync();

        return new LoanDto
        {
            Id = loan.Id,
            ClientId = loan.ClientId,
            AccountId = loan.AccountId,
            TariffId = loan.TariffId,
            Amount = loan.Amount,
            RemainingAmount = loan.RemainingAmount,
            Currency = loan.Currency,
            StartDate = loan.StartDate,
            EndDate = loan.EndDate,
            Status = loan.Status,
            CreatedAt = loan.CreatedAt
        };
    }

    public async Task<RepayLoanResponse> RepayLoanAsync(RepayLoanRequest request)
    {
        var loan = await _loanDbContext.Loans
            .FirstOrDefaultAsync(l => l.Id == request.LoanId) ?? throw new NotFoundException("Loan not found.");

        if (request.Amount > loan.RemainingAmount)
        {
            throw new ValidationException("The amount to reach the remaining loan amount.");
        }

        loan.RemainingAmount -= request.Amount;

        if (loan.RemainingAmount <= 0)
        {
            loan.Status = LoanStatus.Paid;
            loan.EndDate = DateTime.UtcNow;
        }

        var payment = new LoanPayment
        {
            LoanId = loan.Id,
            Amount = request.Amount,
            PaymentDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _loanDbContext.LoanPayments.Add(payment);
        await _loanDbContext.SaveChangesAsync();

        return new RepayLoanResponse
        {
            LoanId = loan.Id,
            RemainingAmount = loan.RemainingAmount,
            Currency = loan.Currency,
            Message = loan.RemainingAmount > 0
                ? "Кредит частично погашен."
                : "Кредит полностью погашен."
        };
    }

    public async Task<List<TariffDto>> GetAvailableTariffsForClient()
    {
        var tariffs = await _loanDbContext.Tariffs
            .AsNoTracking()
            .Select(t => new TariffDto
            {
                Id = t.Id,
                Name = t.Name,
                InterestRate = t.InterestRate,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();

        return tariffs;
    }

    public async Task<List<LoanDto>> GetLoansClient(Guid clientId)
    {
        var loans = await _loanDbContext.Loans
            .AsNoTracking()
            .Where(l => l.ClientId == clientId)
            .Select(l => new LoanDto
            {
                Id = l.Id,
                ClientId = l.ClientId,
                AccountId = l.AccountId,
                TariffId = l.TariffId,
                Amount = l.Amount,
                RemainingAmount = l.RemainingAmount,
                Currency = l.Currency,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Status = l.Status,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync();

        return loans;
    }

    public async Task<LoanDetailsDto> GetLoanDetailsClient(Guid loanId)
    {
        // Получаем информацию о кредите
        var loan = await _loanDbContext.Loans
            .AsNoTracking()
            .Include(l => l.Tariff)
            .FirstOrDefaultAsync(l => l.Id == loanId) ?? throw new NotFoundException("Loan not found.");

        // Получаем платежи по кредиту
        var payments = await _loanDbContext.LoanPayments
            .AsNoTracking()
            .Where(p => p.LoanId == loanId)
            .ToListAsync();

        // Получаем график платежей по кредиту
        var schedule = await _loanDbContext.LoanSchedules
            .AsNoTracking()
            .Where(s => s.LoanId == loanId)
            .ToListAsync();

        return new LoanDetailsDto
        {
            Id = loan.Id,
            ClientId = loan.ClientId,
            AccountId = loan.AccountId,
            TariffId = loan.TariffId,
            Amount = loan.Amount,
            RemainingAmount = loan.RemainingAmount,
            Currency = loan.Currency,
            StartDate = loan.StartDate,
            EndDate = loan.EndDate,
            Status = loan.Status,
            CreatedAt = loan.CreatedAt,
            Tariff = new TariffDto
            {
                Id = loan.Tariff.Id,
                Name = loan.Tariff.Name,
                InterestRate = loan.Tariff.InterestRate,
                CreatedAt = loan.Tariff.CreatedAt
            },
            Payments = payments.Select(p => new LoanPaymentDto
            {
                Id = p.Id,
                LoanId = p.LoanId,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                CreatedAt = p.CreatedAt
            }).ToList(),
            Schedule = schedule.Select(s => new LoanScheduleDto
            {
                Id = s.Id,
                LoanId = s.LoanId,
                PaymentDate = s.PaymentDate,
                Amount = s.Amount,
                Status = s.Status,
                CreatedAt = s.CreatedAt
            }).ToList()
        };
    }

    public async Task<List<LoanScheduleDto>> GetOverduePayments(Guid clientId)
    {
        var loans = await _loanDbContext.Loans
            .Where(l => l.ClientId == clientId)
            .Select(l => l.Id)
            .ToListAsync();

        return await _loanDbContext.LoanSchedules
            .Where(s => loans.Contains(s.LoanId))
            .Where(s => s.PaymentDate < DateTime.UtcNow && s.Status == LoanStatus.Pending)
            .Select(s => new LoanScheduleDto
            {
                Id = s.Id,
                LoanId = s.LoanId,
                PaymentDate = s.PaymentDate,
                Amount = s.Amount,
                Status = s.Status,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<int> GetCreditRating(Guid clientId)
    {
        var overdueCount = await _loanDbContext.LoanSchedules
            .Join(_loanDbContext.Loans,
                s => s.LoanId,
                l => l.Id,
                (s, l) => new { s, l })
            .Where(x => x.l.ClientId == clientId && x.s.Status == LoanStatus.Overdue)
            .CountAsync();

        return Math.Max(300, 800 - overdueCount * 50);
    }
}

