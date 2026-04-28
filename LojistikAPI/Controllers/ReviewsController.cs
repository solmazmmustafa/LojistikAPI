using LojistikAPI.Data;
using LojistikAPI.Entities;
using LojistikAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LojistikAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            // 1. Yeni değerlendirmeyi veritabanına ekliyoruz
            var review = new Review
            {
                ShipmentId = dto.ShipmentId,
                FromUserId = userId,
                ToUserId = dto.ToUserId,
                Score = dto.Score,
                Comment = dto.Comment
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync(); // Önce kaydediyoruz ki aşağıdaki ortalama hesabına dahil olsun

            // 2. FİRMANIN PUAN ORTALAMASINI GÜNCELLEME (Akıllı Sistem)
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.UserId == dto.ToUserId);
            if (company != null)
            {
                // Firmanın geçmişteki tüm puanlarını getir
                var allScores = await _context.Reviews
                    .Where(r => r.ToUserId == dto.ToUserId)
                    .Select(r => r.Score)
                    .ToListAsync();

                // Matematiği yapıp ortalamayı al
                double newAverage = allScores.Average();

                // Firmanın profiline yeni puanı işle (Örn: 4.3)
                company.Rating = Math.Round(newAverage, 1);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Değerlendirme başarıyla kaydedildi! Firmanın not ortalaması güncellendi." });
        }
    }
}