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

    public async Task<LoanDto> TakeLoanAsync(TakeLoanRequest request)
    {
        var tariff = await _loanDbContext.Tariffs
            .FirstOrDefaultAsync(t => t.Id == request.TariffId) ?? throw new NotFoundException("Tariff not found.");

        var loan = new DAL.Data.Entities.Loan
        {
            ClientId = request.ClientId,
            AccountId = request.AccountId,
            TariffId = request.TariffId,
            Amount = request.Amount,
            RemainingAmount = request.Amount, // Оставшаяся сумма равна сумме кредита
            StartDate = DateTime.UtcNow,
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
            Message = loan.RemainingAmount > 0
                ? "Кредит частично погашен."
                : "Кредит полностью погашен."
        };
    }

}

