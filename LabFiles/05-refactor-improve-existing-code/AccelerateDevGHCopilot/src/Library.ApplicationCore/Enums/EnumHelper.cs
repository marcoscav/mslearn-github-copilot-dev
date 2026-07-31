namespace Library.ApplicationCore.Enums;

public static class EnumHelper
{
    private static readonly IReadOnlyDictionary<LoanExtensionStatus, string> LoanExtensionDescriptions =
        new Dictionary<LoanExtensionStatus, string>
        {
            [LoanExtensionStatus.Success] = "Book loan extension was successful.",
            [LoanExtensionStatus.LoanNotFound] = "Loan not found.",
            [LoanExtensionStatus.LoanExpired] = "Cannot extend book loan as it already has expired. Return the book instead.",
            [LoanExtensionStatus.MembershipExpired] = "Cannot extend book loan due to expired patron's membership.",
            [LoanExtensionStatus.LoanReturned] = "Cannot extend book loan as the book is already returned.",
            [LoanExtensionStatus.Error] = "Cannot extend book loan due to an error."
        };

    private static readonly IReadOnlyDictionary<LoanReturnStatus, string> LoanReturnDescriptions =
        new Dictionary<LoanReturnStatus, string>
        {
            [LoanReturnStatus.Success] = "Book was successfully returned.",
            [LoanReturnStatus.LoanNotFound] = "Loan not found.",
            [LoanReturnStatus.AlreadyReturned] = "Cannot return book as the book is already returned.",
            [LoanReturnStatus.Error] = "Cannot return book due to an error."
        };

    private static readonly IReadOnlyDictionary<MembershipRenewalStatus, string> MembershipRenewalDescriptions =
        new Dictionary<MembershipRenewalStatus, string>
        {
            [MembershipRenewalStatus.Success] = "Membership renewal was successful.",
            [MembershipRenewalStatus.PatronNotFound] = "Patron not found.",
            [MembershipRenewalStatus.TooEarlyToRenew] = "It is too early to renew the membership.",
            [MembershipRenewalStatus.LoanNotReturned] = "Cannot renew membership due to an outstanding loan.",
            [MembershipRenewalStatus.Error] = "Cannot renew membership due to an error."
        };

    public static string GetDescription(Enum? value)
    {
        if (value is null)
            return string.Empty;

        return value switch
        {
            LoanExtensionStatus extensionStatus when LoanExtensionDescriptions.TryGetValue(extensionStatus, out var extensionDescription) => extensionDescription,
            LoanReturnStatus returnStatus when LoanReturnDescriptions.TryGetValue(returnStatus, out var returnDescription) => returnDescription,
            MembershipRenewalStatus membershipStatus when MembershipRenewalDescriptions.TryGetValue(membershipStatus, out var membershipDescription) => membershipDescription,
            _ => value.ToString() ?? string.Empty
        };
    }
}