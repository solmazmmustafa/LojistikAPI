// ✅ Tüm gerekli using'ler eksiksiz tanımlandı
using LojistikAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;   // CS0234 çözümü
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────
// 1. VERİTABANI BAĞLANTISI
// ─────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ─────────────────────────────────────────────
// 2. CONTROLLER & API EXPLORER
// ─────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ─────────────────────────────────────────────
// 3. SWAGGER — JWT DESTEKLİ
// CS0234, CS0117, CS0246 → Microsoft.OpenApi.Models using'i ile çözüldü
// ─────────────────────────────────────────────
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Lojistik API",
        Version = "v1"
    });

    // JWT Bearer tanımı
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,   // ✅ ApiKey yerine Http — daha doğru
        Scheme = "bearer",                  // ✅ küçük harf — RFC standardı
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token girin. Örnek: eyJhbGci..."
    });

    // ✅ CS0117 & CS0246 çözümü: OpenApiReference doğru yapılandırıldı
    // ✅ new List<string>() yerine Array.Empty<string>() — daha temiz
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()   // CS0117 çözümü — List<string> import'a gerek kalmadı
        }
    });
});

// ─────────────────────────────────────────────
// 4. JWT KİMLİK DOĞRULAMA
// CS8604 → null-coalescing operatörü ile çözüldü
// ─────────────────────────────────────────────

// ✅ Key appsettings.json'dan okunur; yoksa exception fırlat (sessiz default tehlikelidir)
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Jwt:Key appsettings.json içinde tanımlanmamış!");

builder.Services
    .AddAuthentication(options =>
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
            // ✅ CS8604 çözümü: null ise exception ile erken yakalanıyor
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// ─────────────────────────────────────────────
// 5. CORS POLİTİKASI
// ⚠️  AllowAnyOrigin production'da güvenlik riski — geliştirme için OK
// ─────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ─────────────────────────────────────────────
// UYGULAMA PIPELINE
// ─────────────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lojistik API v1"));
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("AllowAll");        // ✅ Authentication'dan ÖNCE gelmeli
app.UseAuthentication();        // ✅ Sıralama kritik: önce Authentication
app.UseAuthorization();         // ✅ sonra Authorization
app.MapControllers();
app.Run();