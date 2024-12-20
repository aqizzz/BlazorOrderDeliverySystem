﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderDeliverySystem.Share.Data.Models;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs.ReviewDTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace OrderDeliverySystemApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        // Customer can add a review
        [HttpPost("addReview")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewRequestDTO reviewDto)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
                return BadRequest(new { Error = "Customer not found or mismatched customer ID." });

            var review = new Review
            {
                OrderId = reviewDto.OrderId,
                CustomerId = customer.CustomerId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                CreatedAt = DateTime.UtcNow,
                Order = await _context.Orders.FindAsync(reviewDto.OrderId),
                Customer = customer
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return Ok("Review added successfully.");
        }

        // Admin can delete a review
        [HttpDelete("deleteReview/{reviewId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
                return NotFound(new { Error = "Review not found." });

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return Ok("Review deleted successfully.");
        }

        // Customer can view reviews based on different merchantId
        [HttpGet("customerReviews/{userId}")]
        public async Task<IActionResult> customerGetReviews(int userId)
        {
            var merchant = await _context.Merchants.FirstOrDefaultAsync(c => c.UserId == userId);

            if (merchant == null)
            {
                return NotFound(new { Error = "Merchant not found." });
            }

            int merchantId = merchant.MerchantId;

            var reviews = await _context.Reviews
                .Include(r => r.Order)
                .Where(r => r.Order.MerchantId == merchantId)
                .Select(r => new GetReviewResponseDTO
                {
                    ReviewId = r.ReviewId,
                    OrderId = r.OrderId,
                    CustomerId = r.CustomerId,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt,
                    Reply = r.Reply,
                    ReplyCreatedAt = r.ReplyCreatedAt
                })
                .ToListAsync();

            if (!reviews.Any())
                return NotFound(new { Error = "No reviews found for this merchant." });

            return Ok(reviews);
        }

        [HttpGet("orderReview/{orderId}")]
        public async Task<IActionResult> GetReviewByOrderId(int orderId)
        {
            var result = await _context.Reviews.FirstOrDefaultAsync(r => r.OrderId == orderId);

            if (result == null)
                return Ok(new GetReviewResponseDTO());

            var review = new GetReviewResponseDTO
            {
                ReviewId = result.ReviewId,
                OrderId = orderId,
                CustomerId = result.CustomerId,
                Comment = result.Comment ?? "",
                Rating = result.Rating,
                CreatedAt = result.CreatedAt,
                Reply = result.Reply ?? "",
                ReplyCreatedAt = result.ReplyCreatedAt
            };

            return Ok(review);
        }

        // Merchant can view its own reviews
        [HttpGet("merchantReviews")]
        [Authorize(Roles = "Merchant")]
        public async Task<IActionResult> MerchantGetReviews()
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var merchant = await _context.Merchants.FirstOrDefaultAsync(c => c.UserId == userId);


            if (merchant == null)
            {
                return NotFound(new { Error = "Merchant not found." });
            }

            int merchantId = merchant.MerchantId;


            var reviews = await _context.Reviews
                .Include(r => r.Order)
                .Where(r => r.Order.MerchantId == merchantId)
                .Select(r => new GetReviewResponseDTO
                {
                    ReviewId = r.ReviewId,
                    OrderId = r.OrderId,
                    CustomerId = r.CustomerId,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt,
                    Reply = r.Reply,
                    ReplyCreatedAt = r.ReplyCreatedAt
                })
                .ToListAsync();

            if (!reviews.Any())
                return NotFound(new { Error = "No reviews found for your merchant." });

            return Ok(reviews);
            
        }


        // Admin can view all reviews
        [HttpGet("adminReviews")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminReviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Order)
                .Select(r => new GetReviewResponseDTO
                {
                    ReviewId = r.ReviewId,
                    OrderId = r.OrderId,
                    CustomerId = r.CustomerId,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt,
                    Reply = r.Reply,
                    ReplyCreatedAt = r.ReplyCreatedAt
                })
                .ToListAsync();

            if (!reviews.Any())
                return NotFound(new { Error = "No reviews found." });

            return Ok(reviews);
        }




        // Merchant can update the reply and reply time to his own restaurant's orders
        [HttpPut("updateReply/{reviewId}")]
        [Authorize(Roles = "Merchant")]
        public async Task<IActionResult> UpdateReply(int reviewId, [FromBody] UpdateReplyRequestDTO replyDto)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var merchant = await _context.Merchants.FirstOrDefaultAsync(c => c.UserId == userId);


            if (merchant == null)
            {
                return NotFound(new { Error = "Merchant not found." });
            }

            int merchantId = merchant.MerchantId;

            var review = await _context.Reviews
                .Include(r => r.Order)
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId && r.Order.MerchantId == merchantId);

            if (review == null)
                return NotFound(new { Error = "Review not found or you are not authorized to update this review." });

            review.Reply = replyDto.Reply;
            review.ReplyCreatedAt = replyDto.ReplyCreatedAt ?? DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok("Reply updated successfully.");
        }
    }
}
