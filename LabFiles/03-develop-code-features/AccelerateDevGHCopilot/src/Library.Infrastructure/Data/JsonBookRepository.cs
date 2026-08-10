using Library.ApplicationCore;
using Library.ApplicationCore.Entities;

namespace Library.Infrastructure.Data;

public class JsonBookRepository : IBookRepository
{
    private readonly JsonData _jsonData;

    public JsonBookRepository(JsonData jsonData)
    {
        _jsonData = jsonData;
    }

    public async Task<Book?> SearchBookByTitle(string title)
    {
        await _jsonData.EnsureDataLoaded();
        
        // Case-insensitive search
        return _jsonData.Books?.FirstOrDefault(b => 
            b.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<Loan>> GetLoansByBookId(int bookId)
    {
        await _jsonData.EnsureDataLoaded();
        
        var loans = new List<Loan>();
        foreach (var loan in _jsonData.Loans!)
        {
            var bookItem = _jsonData.BookItems?.FirstOrDefault(bi => bi.Id == loan.BookItemId);
            if (bookItem?.BookId == bookId)
            {
                loans.Add(loan);
            }
        }
        return loans;
    }
}
