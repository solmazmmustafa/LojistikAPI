using LojistikAPI.Models;
using System.Net;

namespace LojistikAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // İsteğin normal akışına devam etmesine izin ver
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // Kod patlarsa buraya düşer! Geliştirici için terminale (konsola) tam hatayı yaz:
                _logger.LogError($"SİSTEMDE HATA: {ex}");

                // Müşteriye ise güvenli/sansürlü versiyonunu gönder:
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // Varsayılan 500 hatası

            // Standart, güvenli hata mesajı (SQL kodlarını gizler)
            var message = "Sunucu tarafında beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.";

            // Eğer bizim kasıtlı fırlattığımız özel bir uyarıysa (Örn: "DİKKAT: Veritabanında ilan yok")
            // O zaman bu mesajı sansürlemeden doğrudan müşteriye gösterebiliriz.
            if (exception.Message.StartsWith("DİKKAT"))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest; // 400 Bad Request
                message = exception.Message;
            }

            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            }.ToString());
        }
    }
}