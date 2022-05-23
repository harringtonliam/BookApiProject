using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApiProject.Services;
using BookApiProject.Dtos;
using Microsoft.AspNetCore.Mvc;
using BookApiProject.Models;

namespace BookApiProject.Controllers
{

    [Route("api/reviews")]
    [ApiController]
    public class ReviewsController : Controller
    {
        private IReviewRepository _reviewRepository;
        private IReviewerRepository _reviewerRepository;
        private IBookRepository _bookRepository;

        public ReviewsController(IReviewRepository reviewRepository, IReviewerRepository reviewerRepository, IBookRepository bookRepository)
        {
            _reviewRepository = reviewRepository;
            _reviewerRepository = reviewerRepository;
            _bookRepository = bookRepository;
        }

        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviews()
        {
            var reviews = _reviewRepository.GetReviews();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<ReviewDto> reviewDtos = new List<ReviewDto>();
            foreach (var review in reviews)
            {
                reviewDtos.Add(new ReviewDto {  Headline = review.Headline, Id = review.Id, Rating = review.Rating, ReviewText = review.ReviewText });
            }

            return Ok(reviewDtos);
        }


        //GetReview
        [HttpGet("{reviewId}", Name = "GetReview")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ReviewDto))]
        public IActionResult GetReview( int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }

            var review = _reviewRepository.GetReview(reviewId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            ReviewDto reviewDto = new ReviewDto { Headline = review.Headline, Id = review.Id, Rating = review.Rating, ReviewText = review.ReviewText };

            return Ok(reviewDto);
        }

        [HttpGet("{reviewId}/books")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        public IActionResult GetBookOfAReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }

            var book = _reviewRepository.GetBookOfAReview(reviewId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BookDto bookDto = new BookDto { DatePublished = book.DatePublished, Id = book.Id, Isbn = book.Isbn, Title = book.Title };

            return Ok( bookDto);


        }



        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviewsOfABook(int bookId)
        {

            // TODO check book exisst

            var reviews = _reviewRepository.GetReviewsOfABook(bookId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<ReviewDto> reviewDtos = new List<ReviewDto>();
            foreach (var review in reviews)
            {
                reviewDtos.Add(new ReviewDto { Headline = review.Headline, Id = review.Id, Rating = review.Rating, ReviewText = review.ReviewText });
            }

            return Ok(reviewDtos);

        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Review))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateReview([FromBody] Review reviewToCreate)
        {
            if (reviewToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewerRepository.ReviewerExists(reviewToCreate.Reviewer.Id))
            {
                ModelState.AddModelError("", "Reviewer doesn't exist");
            }
            if (!_bookRepository.BookExists(reviewToCreate.Book.Id))
            {
                ModelState.AddModelError("", "Book doesn't exist");
            }

            reviewToCreate.Book = _bookRepository.GetBook(reviewToCreate.Book.Id);
            reviewToCreate.Reviewer = _reviewerRepository.GetReviewer(reviewToCreate.Reviewer.Id);

            if (!ModelState.IsValid)
            {
                return StatusCode(404, ModelState);
            }

            if (!_reviewRepository.CreateReview(reviewToCreate))
            {
                ModelState.AddModelError("", $"Review failed to create");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetReview", new { reviewId = reviewToCreate.Id }, reviewToCreate);


        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateReview(int reviewId, [FromBody] Review reviewToUpdate)
        {
            if (reviewToUpdate == null)
            {
                return BadRequest(ModelState);
            }

            if (reviewId != reviewToUpdate.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewerRepository.ReviewerExists(reviewToUpdate.Reviewer.Id))
            {
                ModelState.AddModelError("", "Reviewer doesn't exist");
            }
            if (!_bookRepository.BookExists(reviewToUpdate.Book.Id))
            {
                ModelState.AddModelError("", "Book doesn't exist");
            }
            if (!_reviewRepository.ReviewExists(reviewToUpdate.Id))
            {
                ModelState.AddModelError("", "Review doesn't exist");
            }

            reviewToUpdate.Book = _bookRepository.GetBook(reviewToUpdate.Book.Id);
            reviewToUpdate.Reviewer = _reviewerRepository.GetReviewer(reviewToUpdate.Reviewer.Id);

            if (!ModelState.IsValid)
            {
                return StatusCode(404, ModelState);
            }

            if (!_reviewRepository.UpdateReview(reviewToUpdate))
            {
                ModelState.AddModelError("", $"Review failed to update");
                return StatusCode(500, ModelState);
            }

            return NoContent();


        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }

            var reviewToDelete = _reviewRepository.GetReview(reviewId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewRepository.DeleteReview(reviewToDelete))
            {
                ModelState.AddModelError("", $"Review {reviewId}  failed to delete");
                return StatusCode(500, ModelState);
            }

            return NoContent(); ;

        }

    }
}
