using CloudinaryDotNet;
using FirebaseAdmin;
using FireSharp.Config;
using FireSharp.Interfaces;
using Google.Apis.Auth.OAuth2;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Hubs;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.Services;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Supabase;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var isProduction = builder.Environment.IsProduction();

// Create keys directory if it doesn't exist
var keysDirectory = new DirectoryInfo("/app/keys");
if (!keysDirectory.Exists)
{
    keysDirectory.Create();
}

// Configure data protection with key encryption
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(keysDirectory)
    .SetApplicationName("JobFinderApp")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90)); // Optional: set key lifetime

string firebaseJsonPath = Path.Combine(builder.Environment.ContentRootPath, "Configs", "job-32b5d-firebase-adminsdk-fbsvc-55164bc3ae.json");
try
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromFile(firebaseJsonPath)
    });
    Console.WriteLine("FirebaseApp initialized successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error initializing FirebaseApp: {ex.Message}");
    throw;
}
builder.Services.AddSingleton<IFirebaseClient>(sp =>
{
    IFirebaseConfig config = new FirebaseConfig
    {
        AuthSecret = "sdmhGaGzaKEdYsWdtAEqe5eCsKKUuMuhm7m4GnGz",
        BasePath = "https://job-32b5d-default-rtdb.firebaseio.com/"
    };
    return new FireSharp.FirebaseClient(config);
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddMemoryCache();

// Custom services
builder.Services.AddScoped<ICvSnapshotService, CvSnapshotService>();
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ProfileStrengthService>();
builder.Services.AddScoped<SemanticMatchingService>();
builder.Services.AddScoped<IUserService, UserService>();
// Configurations
builder.Services.Configure<GeminiConfig>(builder.Configuration.GetSection("Gemini"));
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

var cloudinarySettings = builder.Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
var account = new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret);
var cloudinary = new Cloudinary(account);
builder.Services.AddSingleton(cloudinary);

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.OperationFilter<UploadFileOperationFilter>();

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    else
    {
        Console.WriteLine($"Warning: XML documentation file not found at {xmlPath}");
    }

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

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
            new string[] {}
        }
    });
});

// Add this to your services configuration
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    //options.LowercaseQueryStrings = true;
});

// CORS
// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            var allowedOrigins = new[]
            {
                "https://job-finder-fe.vercel.app",
                "http://localhost:3000",
                "https://job-finder-kjt2.onrender.com",
                "http://job-finder-kjt2.onrender.com",
                "http://localhost:5194",
                "http://10.0.2.2:5195",
                "http://localhost:8081",
                "http://192.168.1.226:5195"
            };

            builder
                .SetIsOriginAllowed(origin => string.IsNullOrEmpty(origin) || allowedOrigins.Contains(origin))
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
                //.AllowCredentials();
        });
});

// DbContext
builder.Services.AddDbContext<JobFinderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Authentication & JWT & Google
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = "External";
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role,
        ClockSkew = TimeSpan.Zero
    };

    // Allow token via query string for SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
})
.AddCookie("External", options =>
{
    options.Cookie.Name = ".AspNetCore.External";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = isProduction ? SameSiteMode.None : SameSiteMode.Lax; 
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
    options.Cookie.IsEssential = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);

  
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        }
    };
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = "/api/auth/google-response";
    options.SignInScheme = "External";
    options.SaveTokens = true;

    // Configure scope
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    // Configure cookie settings consistently
    options.CorrelationCookie.SameSite = isProduction ? SameSiteMode.None : SameSiteMode.Lax;
    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
    options.CorrelationCookie.HttpOnly = true;
    options.CorrelationCookie.IsEssential = true;

    options.Events = new OAuthEvents
    {
        OnTicketReceived = context =>
        {
            Console.WriteLine("Google authentication ticket received successfully");
            return Task.CompletedTask;
        },
        OnRemoteFailure = context =>
        {
            Console.WriteLine($"Google authentication failed: {context.Failure?.Message}");
            context.Response.Redirect($"https://job-finder-fe.vercel.app/auth/error?message={Uri.EscapeDataString(context.Failure?.Message ?? "Unknown error")}");
            context.HandleResponse();
            return Task.CompletedTask;
        },
        OnRedirectToAuthorizationEndpoint = context =>
        {
            Console.WriteLine($"Redirecting to authorization endpoint: {context.RedirectUri}");
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        }
    };
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Authorization
builder.Services.AddAuthorization();
builder.WebHost.UseUrls("http://0.0.0.0:5195");
var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor,

});

// Development tools
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.EnableFilter());
}

// CORS Middleware
app.UseCors("AllowReactApp");

// Log applied CORS origins
app.Use(async (context, next) =>
{
    var origin = context.Request.Headers["Origin"].ToString();
    Console.WriteLine($"CORS applied for {context.Request.Path}, Origin: {(string.IsNullOrEmpty(origin) ? "null/empty" : origin)}");
    await next();
});

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

app.Run();