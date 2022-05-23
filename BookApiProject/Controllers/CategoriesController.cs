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
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : Controller
    {

        private ICategoryRepository _categoryRepository;

        public  CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetCategories().ToList();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryDto = new List<CategoryDto>();
            foreach (var category in categories)
            {
                categoryDto.Add(new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name
                });

            }

            return Ok(categoryDto);
        }

        [HttpGet("{categoryId}", Name = "GetCategory")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CategoryDto))]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }

            var category = _categoryRepository.GetCategory(categoryId);


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CategoryDto categoryDto = new CategoryDto { Id = category.Id, Name = category.Name };

            return Ok(categoryDto);

        }

        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetAllCategoriesForABook(int bookId)
        {
            //todo validate book exists

            var categories = _categoryRepository.GetAllCategoriesForABook(bookId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryDtos = new List<CategoryDto>();
            foreach (var category in categories)
            {
                categoryDtos.Add(new CategoryDto { Id = category.Id, Name = category.Name });
            }

            return Ok(categoryDtos);

        }


        [HttpGet("{categoryId}/books")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBooksForACategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }

            var books = _categoryRepository.GetBooksForCategory(categoryId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bookDtos = new List<BookDto>();

            foreach (var book in books)
            {
                bookDtos.Add(new BookDto { Id = book.Id, Title = book.Title, DatePublished = book.DatePublished, Isbn = book.Isbn });
            }

            return Ok(bookDtos);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Category))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateCategory([FromBody] Category categoryToCreate)
        {
            if (categoryToCreate == null)
            {
                return BadRequest(ModelState);
            }

            var category = _categoryRepository.GetCategories().Where(c => c.Name.Trim().ToUpper() == categoryToCreate.Name.Trim().ToUpper()).FirstOrDefault();

            if (category != null)
            {
                ModelState.AddModelError("", $"Category {category.Name}  already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.CreateCategory(categoryToCreate))
            {
                ModelState.AddModelError("", $"Category {categoryToCreate.Name}  failed to create");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategory", new { categoryId = categoryToCreate.Id }, categoryToCreate);
   

        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] Category updatedCategoryInfo)
        {
            if (updatedCategoryInfo == null)
            {
                return BadRequest(ModelState);
            }

            if (categoryId != updatedCategoryInfo.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }

            if (_categoryRepository.IsDuplicateCategory(categoryId, updatedCategoryInfo.Name))
            {
                ModelState.AddModelError("", $"Category {updatedCategoryInfo.Name}  already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.UpdateCategory(updatedCategoryInfo))
            {
                ModelState.AddModelError("", $"Category {updatedCategoryInfo.Name}  failed to update");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult DeleteCountry(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }

            var categoryToDelete = _categoryRepository.GetCategory(categoryId);

            if (_categoryRepository.GetBooksForCategory(categoryId).Count() > 0)
            {
                ModelState.AddModelError("", $"Category {categoryToDelete.Name}  cannot be deleted because it used by at least one book");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.DeleteCategory(categoryToDelete))
            {
                ModelState.AddModelError("", $"Category {categoryToDelete.Name}  failed to delete");
                return StatusCode(500, ModelState);
            }

            return NoContent(); ;

        }
    }
}
