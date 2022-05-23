using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApiProject.Models;

namespace BookApiProject.Services
{
    public interface IBookRepository
    {
        ICollection<Book> GetBooks();

        Book GetBook(int bookId);

        Book GetBook(string isbn);

        bool BookExists(int bookId);

        bool BookExists(string isbn);

        bool IsDuplicateISBN(int bookid, string isbn);

        decimal GetBookRating(int bookId);

        bool CreateBook(List<int> authorsId, List<int> categiesId, Book book);
        bool UpdateBook(List<int> authorsId, List<int> categiesId, Book book);
        bool DeleteBook(Book book);
        bool Save();

    }
}
