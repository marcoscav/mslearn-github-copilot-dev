using Library.ApplicationCore.Entities;

namespace Library.ApplicationCore;

public interface IBookRepository
{
    Task<Book?> SearchBookByTitle(string title);
    Task<List<Loan>> GetLoansByBookId(int bookId);
}
