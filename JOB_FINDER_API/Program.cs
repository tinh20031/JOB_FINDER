using CloudinaryDotNet;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.Services;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ICvSnapshotService, CvSnapshotService>();
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<SemanticMatchingService>();
builder.Services.Configure<GeminiConfig>(builder.Configuration.GetSection("Gemini"));
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

var cloudinarySettings = builder.Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
var account = new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret);
var cloudinary = new Cloudinary(account);

builder.Services.AddSingleton(cloudinary);
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.OperationFilter<UploadFileOperationFilter>(); // Thêm filter này
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // Check if the XML file exists before including it
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    else
    {
        Console.WriteLine($"Warning: XML documentation file not found at {xmlPath}");
    }

    // Thêm cấu hình bảo mật JWT
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

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder.WithOrigins("https://job-finder-fe.vercel.app", "http://localhost:3000", "http://localhost:5194")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});

builder.Services.AddDbContext<JobFinderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
        ClockSkew = TimeSpan.Zero,
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.EnableFilter(); 
    });
}



app.UseCors("AllowReactApp");
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
app.Run();
