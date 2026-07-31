using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Library.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace UnitTests.Infrastructure.JsonLoanRepositoryTests;

public class GetLoanTest
{
    private readonly ILoanRepository _mockLoanRepository;
    private readonly JsonLoanRepository _jsonLoanRepository;
    private readonly IConfiguration _configuration;
    private readonly JsonData _jsonData;

    public GetLoanTest()
    {
        _mockLoanRepository = Substitute.For<ILoanRepository>();

        var jsonDirectory = Path.Combine(AppContext.BaseDirectory, "Json");
        if (!Directory.Exists(jsonDirectory))
        {
            var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
            jsonDirectory = Path.Combine(projectRoot, "src", "Library.Console", "Json");
        }

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JsonPaths:Authors"] = Path.Combine(jsonDirectory, "Authors.json"),
                ["JsonPaths:Books"] = Path.Combine(jsonDirectory, "Books.json"),
                ["JsonPaths:BookItems"] = Path.Combine(jsonDirectory, "BookItems.json"),
                ["JsonPaths:Patrons"] = Path.Combine(jsonDirectory, "Patrons.json"),
                ["JsonPaths:Loans"] = Path.Combine(jsonDirectory, "Loans.json")
            })
            .Build();

        _jsonData = new JsonData(_configuration);
        _jsonLoanRepository = new JsonLoanRepository(_jsonData);
    }

    [Fact(DisplayName = "JsonLoanRepository.GetLoan: Returns loan when ID is found")]
    public async Task GetLoan_ReturnsLoanWhenIdIsFound()
    {
        var loanId = 1;
        var expectedLoan = new Loan
        {
            Id = loanId,
            BookItemId = 17,
            PatronId = 22,
            LoanDate = DateTime.Parse("2023-12-08T00:40:43.1808862"),
            DueDate = DateTime.Parse("2023-12-22T00:40:43.1808862"),
            ReturnDate = null
        };

        _mockLoanRepository.GetLoan(loanId).Returns(expectedLoan);

        var actualLoan = await _jsonLoanRepository.GetLoan(loanId);

        Assert.NotNull(actualLoan);
        Assert.Equal(expectedLoan.Id, actualLoan!.Id);
    }

    [Fact(DisplayName = "JsonLoanRepository.GetLoan: Returns null when ID is not found")]
    public async Task GetLoan_ReturnsNullWhenIdIsNotFound()
    {
        var loanId = 999;
        _mockLoanRepository.GetLoan(loanId).Returns((Loan?)null);

        var actualLoan = await _jsonLoanRepository.GetLoan(loanId);

        Assert.Null(actualLoan);
    }
}
