using SteamHub.ApiContract.Context.Repositories;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.ServiceProxies;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web;
using SteamHub.Web.Services;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://0.0.0.0:8080");
}

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();


builder.Services.AddHttpContextAccessor();

var apiBaseUri = builder.Configuration["ApiSettings:BaseUrl"]!;

builder.Services.AddScoped<IUserDetails, WebUserDetails>();

builder.Services.AddScoped<ISessionService, SessionServiceProxy>();
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<IUserService, UserServiceProxy>();
builder.Services.AddScoped<IGameService, GameServiceProxy>();
builder.Services.AddScoped<IUserGameService, UserGameServiceProxy>();
builder.Services.AddScoped<ICartService, CartServiceProxy>();
builder.Services.AddScoped<IDeveloperService, DeveloperServiceProxy>();
builder.Services.AddScoped<IPointShopService, PointShopServiceProxy>();
builder.Services.AddScoped<IInventoryService, InventoryServiceProxy>();
builder.Services.AddScoped<ITradeService, TradeServiceProxy>();
builder.Services.AddScoped<IMarketplaceService, MarketplaceServiceProxy>();
builder.Services.AddScoped<IAchievementsService, AchievementsServiceProxy>();
builder.Services.AddScoped<IFriendRequestService, FriendRequestServiceProxy>();
builder.Services.AddScoped<IReviewService, ReviewServiceProxy>(_ => new ReviewServiceProxy(apiBaseUri));
builder.Services.AddScoped<IWalletService, WalletServiceProxy>();
builder.Services.AddScoped<IFriendsService, FriendsServiceProxy>();
builder.Services.AddScoped<ICollectionsService, CollectionsServiceProxy>();
builder.Services.AddScoped<IFeaturesService, FeaturesServiceProxy>();

builder.Services.AddScoped<INewsService, NewsServiceProxy>();



builder.Services.AddHttpClient("SteamHubApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUri);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    
}).ConfigurePrimaryHttpMessageHandler(() => new NoSslCertificateValidationHandler());


builder.Services.AddAuthentication("SteamHubAuth")
    .AddCookie("SteamHubAuth", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10); // Set timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization(); // Always place this after UseAuthentication

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=HomePage}/{action=Index}/{id?}");
// app.MapRazorPages();

app.Run();
