using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SteamProfileWeb.Services;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Models;
using SteamProfileWeb.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Connection strings and config
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7262/api/";

// Add Identity DbContext with fully qualified name
builder.Services.AddDbContext<SteamProfileWeb.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add BusinessLayer DbContext with fully qualified name and retry policy
builder.Services.AddDbContext<BusinessLayer.DataContext.ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Default Identity services
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<SteamProfileWeb.Data.ApplicationDbContext>();

// Add MVC and HTTP context accessors
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Auth Manager for web app authentication
builder.Services.AddScoped<IAuthManager, AuthManager>();

// Http Clients
builder.Services.AddHttpClient("AuthApi", client =>
{
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient("SteamWebApi", client =>
{
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Authentication
builder.Services.AddAuthentication("SteamWebApi")
    .AddCookie("SteamWebApi", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    });

// Session and Cache
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register Repositories
builder.Services.AddScoped<IForumRepository, ForumRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IUserProfilesRepository, UserProfilesRepository>();
builder.Services.AddScoped<IFeaturesRepository, FeaturesRepository>();
builder.Services.AddScoped<IAchievementsRepository, AchievementsRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<ICollectionsRepository, CollectionsRepository>();
builder.Services.AddScoped<IFriendshipsRepository, FriendshipsRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

// Register Services - Note the order: SessionService needs to be registered before UserService
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IForumService, ForumService>();
builder.Services.AddScoped<IFeaturesService, FeaturesService>();
builder.Services.AddScoped<IAchievementsService, AchievementsService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IFriendsService, FriendsService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

// Add Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSessionRestoration();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapControllerRoute(
    name: "profile",
    pattern: "Profile/{userId?}",
    defaults: new { controller = "Profile", action = "Index" });

app.Run();