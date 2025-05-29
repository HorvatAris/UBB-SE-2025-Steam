using System.Text.Json.Serialization;
using BusinessLayer.Repositories;
using SteamHub.Api.Context;
using SteamHub.Api.Context.Repositories;
using SteamHub.ApiContext.Services;
using SteamHub.ApiContract.Context.Repositories;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.ServiceProxies;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add JWT authentication
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
        ValidateAudience = false,  // No audience in token
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "SteamHubApi",  // Match the token's issuer
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "YourTemporarySecretKeyHere32CharsMini")),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = async context =>
        {
            var sessionId = context.Principal.FindFirst("sessionId")?.Value;
            if (!string.IsNullOrEmpty(sessionId) && Guid.TryParse(sessionId, out var sessionGuid))
            {
                var sessionService = context.HttpContext.RequestServices.GetRequiredService<ISessionService>();
                await sessionService.RestoreSessionFromDatabaseAsync(sessionGuid);
            }
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"Challenge issued: {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

// Configure DbContext with proper concurrency handling
builder.Services.AddDbContext<DataContext>(options => 
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll); // Use tracking by default
});

// Register repositories with scoped lifetime
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IPointShopItemRepository, PointShopItemRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IUserPointShopItemInventoryRepository, UserPointShopItemInventoryRepository>();
builder.Services.AddScoped<IUsersGamesRepository, UsersGamesRepository>();
builder.Services.AddScoped<IStoreTransactionRepository, StoreTransactionRepository>();
builder.Services.AddScoped<IItemTradeRepository, ItemTradeRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IUserInventoryRepository, UserInventoryRepository>();
builder.Services.AddScoped<IItemTradeDetailRepository, ItemTradeDetailRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();

builder.Services.AddScoped<IAchievementsRepository, AchievementsRepository>();
builder.Services.AddScoped<IAchievementsService, AchievementsService>();

builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IUserGameService, UserGameService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IDeveloperService, DeveloperService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IPointShopService, PointShopService>();
builder.Services.AddScoped<ITradeService, TradeService>();
builder.Services.AddScoped<IMarketplaceService, MarketplaceService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// Add authentication middleware before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();