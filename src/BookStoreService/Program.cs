using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

using BookStoreService.Models.BookStore;
using BookStoreService.Models.BookStore.Extensions;
using BookStoreService.Models.Identify;
using BookStoreService.Services.Identify;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// TODO: split the the code below to separated files and functions.

builder.Services.AddDbContext<IdentifyContext>(opt => opt.UseInMemoryDatabase("IdentifyDB"));
builder.Services.AddDbContext<BookStoreContext>(opt => opt.UseInMemoryDatabase("BookStoreDB"));

// Specify identity requirements
// Must be added before .AddAuthentication otherwise a 404 is thrown on authorized endpoints
builder.Services
    .AddIdentityCore<User>(opt => { // IdentityRole is not defined, cannot use AddIdentity<TU, TR>() here. will cause authentication issue.
        opt.SignIn.RequireConfirmedAccount = false;
        opt.SignIn.RequireConfirmedPhoneNumber = false;
        opt.SignIn.RequireConfirmedEmail = false;
        opt.User.RequireUniqueEmail = true;
        opt.Password.RequireDigit = true;
        opt.Password.RequiredLength = 8;
        opt.Password.RequireNonAlphanumeric = true;
        opt.Password.RequireUppercase = true;
        opt.ClaimsIdentity.UserNameClaimType = ClaimTypes.Email;
    })
    .AddEntityFrameworkStores<IdentifyContext>()
    .AddApiEndpoints();

// set the key with command: dotnet user-secrets set "Authentication:JwtSecretKey" "811271ef95278a2857f84727dcb9f221"
JwtTokenService.Initialize(
    builder.Configuration.GetValue<string>("Authentication:JwtValidIssuer") ?? string.Empty,
    builder.Configuration.GetValue<string>("Authentication:JwtValidAudience") ?? string.Empty,
    builder.Configuration.GetValue<string>("Authentication:JwtRegisteredClaimNamesSub") ?? string.Empty,
    builder.Configuration.GetValue<string>("Authentication:JwtSecretKey") ?? string.Empty,
    builder.Configuration.GetValue<int?>("Authentication:JwtExpirationMinutes") ?? 30,
    builder.Configuration.GetValue<bool?>("Authentication:JwtValidateLifetime") ?? true,
    builder.Configuration.GetValue<bool?>("Authentication:JwtValidateIssuer") ?? false,
    builder.Configuration.GetValue<bool?>("Authentication:JwtValidateAudience") ?? false);

builder.Services.AddScoped<JwtTokenService, JwtTokenService>();

builder.Services
    .AddAuthentication(opt => {
        opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(opt => {
        opt.RequireHttpsMetadata = true;
        opt.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtTokenService.SecretKey)),
            ValidateLifetime = JwtTokenService.ValidateLifetime,
            ValidateIssuer = JwtTokenService.ValidateIssuer,
            ValidateAudience = JwtTokenService.ValidateAudience,
            ClockSkew = TimeSpan.Zero
        };
        opt.Events = new JwtBearerEvents { // for debugging.
            OnAuthenticationFailed = ctx => {
                Console.WriteLine("Authentication failed: " + ctx.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx => {
                Console.WriteLine("Token validated: " + ctx.SecurityToken);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();


builder.Services.AddControllers()
    .AddJsonOptions(opt => { // Support string to enum conversions
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddRouting(opt => opt.LowercaseUrls = true);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {

    // to resolve the issue: or Linux or non-Windows operating systems, file names and paths can be case-sensitive.
    // For example, a TodoApi.XML file is valid on Windows but not Ubuntu.
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme {
        Description = "JWT Authorization header using the Bearer scheme.Don't need to add bearer at first",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});



// monitor & health
builder.Services.AddHealthChecks();
builder.Services.AddHttpLogging(opt => {
    opt.CombineLogs = true;
    opt.LoggingFields = HttpLoggingFields.Request | HttpLoggingFields.ResponseStatusCode | HttpLoggingFields.Duration;
    opt.RequestBodyLogLimit = 1024;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enforce HTTPS
app.UseHttpsRedirection();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();
//app.MapIdentityApi<User>(); // use the APIs in UsersController
app.MapHealthChecks("/health");

// init sample data for development env
if (app.Environment.IsDevelopment()) {

    using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope()) {

        // set the key with command like: dotnet user-secrets set "Authentication:DefaultUserName" "Admin"
        var defaultUserName = builder.Configuration.GetValue<string>("Authentication:DefaultUserName");
        var defaultUserEmail = builder.Configuration.GetValue<string>("Authentication:DefaultUserEmail");
        var defaultUserPassword = builder.Configuration.GetValue<string>("Authentication:DefaultUserPassword");
        if (defaultUserName != null && defaultUserEmail != null && defaultUserPassword != null) {
            var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
            if (userManager != null) {
                await userManager.CreateAsync(new User {
                    UserName = defaultUserName,
                    Email = defaultUserEmail,
                    Role = "Admin"
                }, defaultUserPassword);
            }
        }
        serviceScope.ServiceProvider.GetService<BookStoreContext>()?.EnsureSeedData();
    }
}



await app.RunAsync();
