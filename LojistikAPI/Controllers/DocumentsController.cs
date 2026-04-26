using LojistikAPI.Data;
using LojistikAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace LojistikAPI.Controllers
{
    //[Authorize]

    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DocumentsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] int shipmentId, [FromForm] string documentType, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Lütfen geçerli bir dosya seçin." });

            // 1. İlan gerçekten var mı kontrol edelim
            var shipmentExists = await _context.Shipments.AnyAsync(s => s.Id == shipmentId);
            if (!shipmentExists)
                return NotFound(new { message = "Bu belgeyi eklemeye çalıştığınız ilan bulunamadı!" });

            // 2. Güvenli Klasör Yolu Bulma (WebRootPath bazen boş olabilir, bunu engelliyoruz)
            string webRootPath = string.IsNullOrWhiteSpace(_env.WebRootPath)
                ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                : _env.WebRootPath;

            string uploadsFolder = Path.Combine(webRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // 3. Benzersiz dosya ismi
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 4. Dosyayı fiziksel olarak kaydet
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 5. Veritabanı kaydı
            var document = new ShipmentDocument
            {
                ShipmentId = shipmentId,
                DocumentType = documentType,
                FilePath = $"/uploads/{uniqueFileName}"
            };

            // İŞTE DÜZELTİLEN SATIR BURASI (Fazladan 's' harfi kaldırıldı)
            _context.ShipmentDocuments.Add(document);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Dosya başarıyla yüklendi!",
                documentPath = document.FilePath
            });
        }
    }
}