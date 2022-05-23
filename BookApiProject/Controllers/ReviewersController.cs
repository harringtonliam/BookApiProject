using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApiProject.Services;
using BookApiProject.Dtos;
using BookApiProject.Models;

namespace BookApiProject.Controllers
{

    [Route("api/reviewers")]
    [ApiController]
    public class ReviewersController : Controller
    {
        IReviewerRepository _reviewerRepository;
        IReviewRepository _reviewRepository;

        public ReviewersController(IReviewerRepository reviewerRepository, IReviewRepository reviewRepository)
        {
            _reviewerRepository = reviewerRepository;
            _reviewRepository = reviewRepository;
        }

        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        public IActionResult GetReviewers()
        {
            var reviewers = _reviewerRepository.GetReviewers();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<ReviewerDto> reviewerDtos = new List<ReviewerDto>();
            foreach (var reviewer in reviewers)
            {
                reviewerDtos.Add(new ReviewerDto { FirstName = reviewer.FirstName, Id = reviewer.Id, LastName = reviewer.LastName });
            }

            return Ok(reviewerDtos);
        }

        [HttpGet("{reviewerId}", Name = "GetReviewer")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }

            var reviewer = _reviewerRepository.GetReviewer(reviewerId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ReviewerDto reviewerDto = new ReviewerDto{ FirstName = reviewer.FirstName, Id = reviewer.Id, LastName = reviewer.LastName };
    
            return Ok(reviewerDto);
        }

        [HttpGet("reviews/{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewerOfAReview(int reviewId)
        {
            var reviewer = _reviewerRepository.GetReviewerOfAReview(reviewId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ReviewerDto reviewerDto = new ReviewerDto { FirstName = reviewer.FirstName, Id = reviewer.Id, LastName = reviewer.LastName };

            return Ok(reviewerDto);

        }

        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviewsByReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }

            var reviews = _reviewerRepository.GetReviewsByReviewer(reviewerId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewDtos = new List<ReviewDto>();

            foreach (var review in reviews)
            {
                reviewDtos.Add(new ReviewDto { Headline = review.Headline, Id = review.Id, Rating = review.Rating, ReviewText = review.ReviewText });
            }

            return Ok(reviewDtos);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateCountry([FromBody] Reviewer reviewerToCreate)
        {
            if (reviewerToCreate == null)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewerRepository.CreateReviewer(reviewerToCreate))
            {
                ModelState.AddModelError("", $"Reviewer {reviewerToCreate.LastName}  failed to create");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetReviewer", new { reviewerId = reviewerToCreate.Id }, reviewerToCreate);

        }

        [HttpPut("{reviwerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCountry(int reviwerId, [FromBody] Reviewer updatedReviewerInfo)
        {
            if (updatedReviewerInfo == null)
            {
                return BadRequest(ModelState);
            }

            if (reviwerId != updatedReviewerInfo.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewerRepository.ReviewerExists(reviwerId))
            {
                return NotFound();
            }

    

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewerRepository.UpdateReviewer(updatedReviewerInfo))
            {
                ModelState.AddModelError("", $"Reviewer {updatedReviewerInfo.LastName}  failed to update");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }


        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteReviwer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }

            var reviewerToDelete = _reviewerRepository.GetReviewer(reviewerId);

            var reviewsToDelete = _reviewerRepository.GetReviewsByReviewer(reviewerId).ToList<Review>();
     

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewerRepository.DeleteReviewer(reviewerToDelete))
            {
                ModelState.AddModelError("", $"Reviewer {reviewerToDelete.LastName}  failed to delete");
                return StatusCode(500, ModelState);
            }

            if (reviewsToDelete.Count > 0)
            {
                if (_reviewRepository.DeleteReviews(reviewsToDelete))
                {
                    ModelState.AddModelError("", $"Reviewes failed to delete");
                    return StatusCode(500, ModelState);
                }
            }

            return NoContent(); ;

        }
    }
}
