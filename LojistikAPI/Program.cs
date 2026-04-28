using LojistikAPI.Data;
using LojistikAPI.Hubs;
using LojistikAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // Swagger için gereken asıl kütüphane
using System.Text;
using LojistikAPI.Middlewares;


var builder = WebApplication.CreateBuilder(args);


// MediatR'ı sisteme kaydediyoruz
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// ─────────────────────────────────────────────
// 1. VERİTABANI VE TEMEL AYARLAR
// ─────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR(); // Canlı harita (Telsiz) altyapısı
// Arka plan işçisini (Görünmez İşçi) sisteme kaydediyoruz

builder.Services.AddHostedService<FuelPriceUpdateService>();
// E-Posta servisini sistemin her yerinde (Controller'larda) kullanılabilir hale getiriyoruz
builder.Services.AddSingleton<EmailService>();
builder.Services.AddScoped<IOfferService, OfferService>();

// ─────────────────────────────────────────────
// 2. İŞTE İSTEDİĞİN SWAGGER KODLARI 
// ─────────────────────────────────────────────
builder.Services.AddSwaggerGen(c =>
{
    // Swagger sayfasının ana başlığı
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Lojistik API", Version = "v1" });

    // Swagger'ın sağ üst köşesine "Authorize" (Kilit) butonunu ekleyen kod
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Giriş yaptıktan sonra aldığınız Token'ı buraya yapıştırın."
    });

    // Kilit butonuna token girdiğimizde, bunu diğer tüm sayfalara (ilanlar, dosyalar vs.) otomatik taşıyan kod
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ─────────────────────────────────────────────
// 3. GÜVENLİK (JWT) VE İZİNLER (CORS)
// ─────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key tanımlanmamış!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.WithOrigins("http://localhost:5199", "https://seninsiten.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// ─────────────────────────────────────────────
// 4. UYGULAMAYI AYAĞA KALDIRMA (PIPELINE)
// ─────────────────────────────────────────────
var app = builder.Build();
// Tüm hataları merkezden yakalayan kalkanımız
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    // Swagger'ı tarayıcıda çalıştıran tetikleyiciler
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lojistik API v1"));
}

app.UseStaticFiles(); // Yüklediğimiz resimlerin tarayıcıda görünmesini sağlar
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Telsiz merkezimizin yayın adresini belirliyoruz
app.MapHub<TrackingHub>("/trackingHub");
// Canlı bildirim merkezinin yayın adresini belirliyoruz
app.MapHub<NotificationHub>("/notificationHub");

app.Run();