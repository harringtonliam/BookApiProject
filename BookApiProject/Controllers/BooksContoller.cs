using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookApiProject.Services;
using BookApiProject.Dtos;
using BookApiProject.Models;

namespace BookApiProject.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksContoller : Controller
    {
        IBookRepository _bookRepository;
        IAuthorRepository _authorRepository;
        ICategoryRepository _categoryRepository;
        IReviewRepository _reviewRepository;

        public BooksContoller(IBookRepository bookRepository, IAuthorRepository authorRepository, ICategoryRepository categoryRepository, IReviewRepository reviewRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _reviewRepository = reviewRepository;
        }


        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBooks()
        {
            var books = _bookRepository.GetBooks();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<BookDto> bookDtos = new List<BookDto>();

            foreach (var book in books)
            {
                bookDtos.Add(new BookDto { DatePublished = book.DatePublished, Id = book.Id, Isbn = book.Isbn, Title = book.Title });
            }

            return Ok(bookDtos);
        }


        [HttpGet("{bookId}", Name = "GetBook")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        public IActionResult GetBook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
            {
                return NotFound();
            }

            var book = _bookRepository.GetBook(bookId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BookDto bookDto = new BookDto { DatePublished = book.DatePublished, Id = book.Id, Isbn = book.Isbn, Title = book.Title };

            return Ok(bookDto);

        }

        [HttpGet("ISBN/{isbn}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        public IActionResult GetBook(string isbn)
        {
            if (!_bookRepository.BookExists(isbn))
            {
                return NotFound();
            }

            var book = _bookRepository.GetBook(isbn);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BookDto bookDto = new BookDto { DatePublished = book.DatePublished, Id = book.Id, Isbn = book.Isbn, Title = book.Title };

            return Ok(bookDto);


        }

        [HttpGet("{bookId}/rating")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(decimal))]
        public IActionResult GetBookRating(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
            {
                return NotFound();
            }

            decimal bookRating = _bookRepository.GetBookRating(bookId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



            return Ok(bookRating);

        }

        //api/books?authId=1&authId=2&catId=1,&catId=2
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Book))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateBook([FromQuery] List<int> authId, [FromQuery] List<int> catId,
                                        [FromBody] Book bookToCreate)
        {
            var statusCode = ValidateBook(authId, catId, bookToCreate);
            
            if(!ModelState.IsValid)
            {
                return StatusCode(statusCode.StatusCode);
            }


            if (!_bookRepository.CreateBook(authId, catId, bookToCreate))
            {
                ModelState.AddModelError("", $"Book {bookToCreate.Title}  failed to create");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetBook", new { bookId = bookToCreate.Id }, bookToCreate);


        }


        private StatusCodeResult ValidateBook( List<int> authId, List<int> catid, Book book)
        {
            if (book == null || authId.Count() <= 0 || catid.Count() <= 0)
            {
                ModelState.AddModelError("", "Missing Book, author, or category");
                return BadRequest();
            }

            if (_bookRepository.IsDuplicateISBN(book.Id, book.Isbn))
            {
                ModelState.AddModelError("", "Duplicate ISBN");
                return StatusCode(202);
            }

            foreach (var id in authId)
            {
                if (!_authorRepository.AuthorExists(id))
                {
                    ModelState.AddModelError("", "Author Not Found");
                    return StatusCode(404);
                }
            }

            foreach (var id in catid)
            {
                if (!_categoryRepository.CategoryExists(id))
                {
                    ModelState.AddModelError("", "Category Not Found");
                    return StatusCode(404);
                }
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Critical Error");
                return BadRequest();
            }

            return NoContent();

        }



    }
}
