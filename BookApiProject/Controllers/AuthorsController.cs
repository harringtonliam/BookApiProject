using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookApiProject.Dtos;
using BookApiProject.Services;
using BookApiProject.Models;



namespace BookApiProject.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorsController : Controller
    {
        IAuthorRepository _authorRepository;
        ICountryRepository _countryRepository;

        public AuthorsController(IAuthorRepository authorRepository, ICountryRepository countryRepository)
        {
            _authorRepository = authorRepository;
            _countryRepository = countryRepository;

        }

        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthors()
        {
            var authors = _authorRepository.GetAuthors();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<AuthorDto> authorDtos = new List<AuthorDto>();

            foreach (var author in authors)
            {
                authorDtos.Add(new AuthorDto { FirstName = author.FirstName, Id = author.Id, LastName = author.LastName });
            }

            return Ok(authorDtos);


        }

        [HttpGet("{authorId}", Name = "GetAuthor")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(AuthorDto))]
        public IActionResult GetAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var author = _authorRepository.GetAuthor(authorId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AuthorDto authorDto = new AuthorDto { FirstName = author.FirstName, Id = author.Id, LastName = author.LastName };


            return Ok(authorDto);

        }

        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable< AuthorDto>))]
        public IActionResult GetAuthorsOfABook(int bookId)
        {
            var authors = _authorRepository.GetAuthorsOfABook(bookId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<AuthorDto> authorDtos = new List<AuthorDto>();

            foreach (var author in authors)
            {
                authorDtos.Add(new AuthorDto { FirstName = author.FirstName, Id = author.Id, LastName = author.LastName });
            }

            return Ok(authorDtos);
        }


        [HttpGet("{authorId}/books")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBooksByAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var books = _authorRepository.GetBooksByAuthor(authorId);

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


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Author))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateAuthor([FromBody] Author authorToCreate)
        {
            if (authorToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (authorToCreate.Country == null)
            {
                return BadRequest(ModelState);
            }

            if (!_countryRepository.CountryExists(authorToCreate.Country.Id))
            {
                ModelState.AddModelError("", $"Country {authorToCreate.Country.Id}  does not exist");
                return StatusCode(404, ModelState);
            }

            var country = _countryRepository.GetCountry(authorToCreate.Country.Id);

            authorToCreate.Country = country;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_authorRepository.CreateAuthor(authorToCreate))
            {
                ModelState.AddModelError("", $"Author {authorToCreate.LastName}  failed to create");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetAuthor", new { authorId = authorToCreate.Id }, authorToCreate);


        }

        [HttpPut("{authorId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateAuthor(int authorId, [FromBody] Author updatedAuthorInfo)
        {
            if (updatedAuthorInfo == null)
            {
                return BadRequest(ModelState);
            }

            if (authorId != updatedAuthorInfo.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            if (updatedAuthorInfo.Country == null)
            {
                return BadRequest(ModelState);
            }

            if (!_countryRepository.CountryExists(updatedAuthorInfo.Country.Id))
            {
                ModelState.AddModelError("", $"Country {updatedAuthorInfo.Country.Id}  does not exist");
                return StatusCode(404, ModelState);
            }

            var country = _countryRepository.GetCountry(updatedAuthorInfo.Country.Id);

            updatedAuthorInfo.Country = country;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_authorRepository.UpdateAuthor(updatedAuthorInfo))
            {
                ModelState.AddModelError("", $"Category {updatedAuthorInfo.LastName}  failed to update");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }


        [HttpDelete("{authorId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult DeleteAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var authorToDelete = _authorRepository.GetAuthor(authorId);

            if (_authorRepository.GetBooksByAuthor(authorId).Count() > 0)
            {
                ModelState.AddModelError("", $"Author {authorToDelete.LastName}  cannot be deleted because they have at least one book");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_authorRepository.DeleteAuthor(authorToDelete))
            {
                ModelState.AddModelError("", $"Country {authorToDelete.FirstName}  failed to delete");
                return StatusCode(500, ModelState);
            }

            return NoContent(); ;

        }

    }
}
