﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApiProject.Models;

namespace BookApiProject.Services
{
    public interface IAuthorRepository
    {
        ICollection<Author> GetAuthors();

        Author GetAuthor(int AuthorId);

        ICollection<Author> GetAuthorsOfABook(int bookId);

        ICollection<Book> GetBooksByAuthor(int authorId);

        bool AuthorExists(int authorId);

        bool CreateAuthor(Author author);
        bool UpdateAuthor(Author author);
        bool DeleteAuthor(Author author);
        bool Save();

    }
}
