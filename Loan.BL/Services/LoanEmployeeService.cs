using Common.DataTransferObjects;
using Common.Exceptions;
using Common.Interfaces;
using Loan.DAL.Data;
using Loan.DAL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Loan.BL.Services;

public class LoanEmployeeService : ILoanEmployeeService
{
    private readonly ILogger<LoanEmployeeService> _logger;
    private readonly LoanDbContext _loanDbContext;

    public LoanEmployeeService(ILogger<LoanEmployeeService> logger, LoanDbContext loanDbContext)
    {
        _logger = logger;
        _loanDbContext = loanDbContext;
    }

    public async Task<List<TariffDto>> GetTariffsAsync()
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

    public async Task<TariffDto> CreateTariffAsync(CreateTariffRequest request)
    {
        var tariff = new Tariff
        {
            Name = request.Name,
            InterestRate = request.InterestRate
        };

        _loanDbContext.Tariffs.Add(tariff);
        await _loanDbContext.SaveChangesAsync();

        return new TariffDto
        {
            Id = tariff.Id,
            Name = tariff.Name,
            InterestRate = tariff.InterestRate,
            CreatedAt = tariff.CreatedAt
        };
    }

    public async Task DeleteTariffAsync(Guid tariffId)
    {
        var tariff = await _loanDbContext.Tariffs
            .FirstOrDefaultAsync(t => t.Id == tariffId) ?? throw new NotFoundException("Tariff not found.");

        if (tariff != null)
        {
            _loanDbContext.Tariffs.Remove(tariff);
            await _loanDbContext.SaveChangesAsync();
        }
    }

    public async Task<List<LoanDto>> GetClientLoansAsync(Guid clientId)
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
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Status = l.Status,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync();

        return loans;
    }

    public async Task<LoanDetailsDto> GetLoanDetailsAsync(Guid loanId)
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
}