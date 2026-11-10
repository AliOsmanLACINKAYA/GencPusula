using System.Text;
using GencPusula.Web.Data;
using GencPusula.Web.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using GencPusula.Web.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

// --- Veritabanı ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection tanimli degil.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// --- Identity ---
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// --- JWT ---
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key tanimli degil veya 32 karakterden kisa.");
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "GencPusula";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "GencPusulaMobile";

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json; charset=utf-8";

                await context.Response.WriteAsJsonAsync(new
                {
                    kod = "Yetkisiz",
                    mesaj = "Bu isteği yapmak için giriş yapmalısınız."
                });
            }
        };
    });

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddScoped<GencPusula.Web.Services.ProfilKayitServisi>();

var app = builder.Build();

// --- Roller ve başlangıç verileri ---
using (var scope = app.Services.CreateScope())
{
    var rolYoneticisi = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var rol in new[] { Roller.Yonetici, Roller.Kullanici })
    {
        if (!await rolYoneticisi.RoleExistsAsync(rol))
            await rolYoneticisi.CreateAsync(new IdentityRole(rol));
    }

    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await KategoriSeeder.CalistirAsync(db);
    await FirsatSeeder.CalistirAsync(db);

    var adminEmail = builder.Configuration["Seed:AdminEmail"];
    var adminPassword = builder.Configuration["Seed:AdminPassword"];

    if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
    {
        var kullaniciYoneticisi = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var adminUser = await kullaniciYoneticisi.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var olusturmaSonucu = await kullaniciYoneticisi.CreateAsync(adminUser, adminPassword);
            if (!olusturmaSonucu.Succeeded)
            {
                app.Logger.LogWarning(
                    "Admin kullanıcısı oluşturulamadı: {Hatalar}",
                    string.Join(" ", olusturmaSonucu.Errors.Select(e => e.Description)));
                adminUser = null;
            }
        }

        if (adminUser is not null &&
            !await kullaniciYoneticisi.IsInRoleAsync(adminUser, Roller.Yonetici))
        {
            await kullaniciYoneticisi.AddToRoleAsync(adminUser, Roller.Yonetici);
        }
    }
}

// --- Ara katmanlar ---
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();