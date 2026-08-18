using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Library.ApplicationCore.Enums;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly Func<DateTime> _nowProvider;

    public LoanService(ILoanRepository loanRepository)
        : this(loanRepository, () => DateTime.Now)
    {
    }

    public LoanService(ILoanRepository loanRepository, Func<DateTime> nowProvider)
    {
        _loanRepository = loanRepository;
        _nowProvider = nowProvider;
    }

    public async Task<LoanReturnStatus> ReturnLoan(int loanId)
    {
        Loan? loan = await _loanRepository.GetLoan(loanId);
        if (loan == null)
        {
            return LoanReturnStatus.LoanNotFound;
        }

        // check if already returned
        if (loan.ReturnDate != null)
        {
            return LoanReturnStatus.AlreadyReturned;
        }

        loan.ReturnDate = DateTime.Now;
        try
        {
            await _loanRepository.UpdateLoan(loan);
            return LoanReturnStatus.Success;
        }
        catch (Exception e)
        {
            return LoanReturnStatus.Error;
        }
    }

    public const int ExtendByDays = 14;

    public async Task<LoanExtensionStatus> ExtendLoan(int loanId)
    {
        var loan = await _loanRepository.GetLoan(loanId);

        if (loan == null)
            return LoanExtensionStatus.LoanNotFound;

        var now = _nowProvider();

        // Check if patron's membership is expired
        if (loan.Patron!.MembershipEnd <= now)
            return LoanExtensionStatus.MembershipExpired;

        if (loan.ReturnDate != null)
            return LoanExtensionStatus.LoanReturned;

        if (loan.DueDate <= now)
            return LoanExtensionStatus.LoanExpired;

        loan.DueDate = loan.DueDate.AddDays(ExtendByDays);
        try
        {
            await _loanRepository.UpdateLoan(loan);
            return LoanExtensionStatus.Success;
        }
        catch (Exception e)
        {
            return LoanExtensionStatus.Error;
        }
    }
}