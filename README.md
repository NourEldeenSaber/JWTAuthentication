# 🔐 JWT Authentication — ASP.NET Core

A complete **JWT Authentication** system built with **ASP.NET Core 8** following **Clean Architecture** principles. Provides user registration, login, role-based authorization, and JWT token generation.

---

## 📁 Project Structure

```
JWTAuthentication/
│
├── 📦 Shared/                               # Shared DTOs & Settings (no dependencies)
│   ├── JwtOptions.cs                        # JWT settings (Issuer, Audience, SecretKey)
│   └── Dtos/IdentityDtos/
│       ├── LoginDto.cs                      # Login request body
│       ├── RegisterDto.cs                   # Registration request body
│       └── UserDto.cs                       # Response after Login/Register
│
├── 🧠 Core/                                 # Business Logic (depends on nothing)
│   ├── Domain/
│   │   ├── Entities/IdentityModule/
│   │   │   ├── ApplicationUser.cs           # User entity (extends IdentityUser)
│   │   │   └── Address.cs                  # Address entity (One-to-One with User)
│   │   ├── Contracts/
│   │   │   └── IDataInitializer.cs         # Interface for database seeding
│   │   └── Exceptions/
│   │       └── ValidationException.cs      # Custom exception for validation errors
│   ├── Services.Abstraction/
│   │   └── IAuthenticationService.cs       # Authentication service contract
│   └── Services/
│       └── AuthenticationService.cs        # Login, Register & JWT token creation
│
├── 🏗️ Infrastructure/
│   ├── Presentation/
│   │   ├── ApiController.cs                # Base controller
│   │   └── AuthenticationController.cs     # /Login & /Register endpoints
│   └── Presistence/
│       └── Data/
│           ├── DbContexts/
│           │   └── StoreIdentityDbContext.cs   # EF Core Identity DbContext
│           ├── DataSeed/
│           │   └── IdentityDataInitializer.cs  # Seeds default roles & users
│           └── Migrations/                     # EF Core auto-generated migrations
│
└── 🚀 JWT Authentication/                   # Entry Point (Web API)
    ├── Program.cs                           # DI registration & middleware pipeline
    ├── Extensions/
    │   └── WebApplicationRegistration.cs   # Migration & seeding extension methods
    └── appsettings.json                     # Connection string & JWT settings
```

---

## ⚙️ Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server or SQL Server Express
- Visual Studio 2022 / Rider / VS Code

---

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/NourEldeenSaber/JWTAuthentication.git
cd JWTAuthentication
```

### 2. Configure the connection string

Open `JWT Authentication/appsettings.json` and update the connection string to match your server:

```json
{
  "ConnectionStrings": {
    "IdentityConnection": "Server=YOUR_SERVER; DataBase=Store.Identity; Trusted_Connection=True; TrustServerCertificate=True"
  }
}
```

> **Example for SQL Server Express:** `Server=.\SQLEXPRESS`

### 3. Run the project

```bash
dotnet run --project "JWT Authentication"
```

On startup, the app automatically:
- ✅ Applies pending EF Core migrations
- ✅ Creates the database schema
- ✅ Seeds default roles (`Admin`, `SuperAdmin`) and users

---

## 🗄️ Database

### Tables created by Identity

| Table | Description |
|-------|-------------|
| `Users` | Application users (extends AspNetUsers) |
| `Roles` | Application roles |
| `UserRoles` | User-to-role mapping |
| `Addresses` | Optional user addresses (One-to-One) |

### Seeded default users

| DisplayName | Email | Password | Role |
|-------------|-------|----------|------|
| admin | admin@gmail.com | P@ssw0rd | Admin |
| superAdmin | superAdmin@gmail.com | P@ssw0rd | SuperAdmin |

---

## 🔑 JWT Configuration

```json
{
  "JWTOptions": {
    "SecretKey": "da447425e611e9c5d94ca0bd8a37ce852bf7ecffcf11352cddcb0b8571bb994b",
    "Issuer": "https://localhost:7199/",
    "Audience": "https://localhost:7199/"
  }
}
```

| Setting | Description |
|---------|-------------|
| `SecretKey` | Secret used to sign tokens — must be long and random in production |
| `Issuer` | Who issued the token (your API URL) |
| `Audience` | Who the token is intended for |

> ⚠️ **Never commit the real SecretKey to source control.** Use environment variables or a secrets manager in production.

---

## 📡 API Endpoints

### Register — create a new account

```
POST /api/Authentication/Register
Content-Type: application/json
```

**Request body:**
```json
{
  "email": "user@example.com",
  "displayName": "Ahmed Mohamed",
  "userName": "ahmed123",
  "password": "P@ssw0rd",
  "phoneNumber": "01012345678"
}
```

**Response `200 OK`:**
```json
{
  "email": "user@example.com",
  "displayName": "Ahmed Mohamed",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

### Login — authenticate and receive a token

```
POST /api/Authentication/Login
Content-Type: application/json
```

**Request body:**
```json
{
  "email": "admin@gmail.com",
  "password": "P@ssw0rd"
}
```

**Response `200 OK`:**
```json
{
  "email": "admin@gmail.com",
  "displayName": "admin",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

### Using the token on protected endpoints

Include the token in every request as a Bearer header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 🔍 How It Works — Step by Step

### Step 1 — Plan the layers

Before writing any code, map each project to a Clean Architecture layer:

```
Shared                →  DTOs & settings          (no dependencies)
Core / Domain         →  Entities & interfaces     (no dependencies)
Core / Services       →  Business logic            (depends on Domain & Shared)
Infrastructure / Persistence  →  Database          (depends on Services)
Infrastructure / Presentation →  Controllers       (depends on Services.Abstraction)
Entry Point           →  Program.cs                (wires everything together)
```

**NuGet packages at this step:** none — just create the projects.

---

### Step 2 — Build the Shared layer

`Shared` holds the DTOs that travel between layers. It has no NuGet dependencies — only built-in .NET types.

```csharp
// LoginDto.cs
public record LoginDto([EmailAddress] string Email, string Password);

// RegisterDto.cs
public record RegisterDto(
    [EmailAddress] string Email,
    string DisplayName,
    string UserName,
    string Password,
    [Phone] string PhoneNumber);

// UserDto.cs — the response returned after login or register
public record UserDto(string Email, string DisplayName, string Token);

// JwtOptions.cs — bound from appsettings.json via Options Pattern
public class JwtOptions
{
    public string Issuer    { get; set; } = default!;
    public string Audience  { get; set; } = default!;
    public string SecretKey { get; set; } = default!;
}
```

**NuGet packages in this layer:** none.

---

### Step 3 — Build the Domain layer

`Domain` contains entities and contracts. It has one package because `ApplicationUser` inherits from `IdentityUser`.

```csharp
// ApplicationUser.cs
public class ApplicationUser : IdentityUser
{
    public string   DisplayName { get; set; } = default!;
    public Address? Address     { get; set; }
}

// IDataInitializer.cs
public interface IDataInitializer
{
    Task InitializeAsync();
}

// ValidationException.cs
public sealed class ValidationException : Exception
{
    public IEnumerable<string> Errors { get; set; } = [];
    public ValidationException(IEnumerable<string> messages) : base("Validation failed")
        => Errors = messages;
}
```

**NuGet packages in this layer:**

| Package | Version | Why |
|---------|---------|-----|
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 8.0.26 | Provides `IdentityUser` that `ApplicationUser` inherits from |

---

### Step 4 — Define the service contract

`Services.Abstraction` declares *what* the service does without revealing *how*. No packages needed.

```csharp
// IAuthenticationService.cs
public interface IAuthenticationService
{
    Task<UserDto> LoginAsync(LoginDto loginDto);
    Task<UserDto> RegisterAsync(RegisterDto registerDto);
}
```

**NuGet packages in this layer:** none.

---

### Step 5 — Implement the Authentication Service

This is the heart of the JWT system. `CreateTokenAsync` builds the token in four stages:

```csharp
private async Task<string> CreateTokenAsync(ApplicationUser user)
{
    var jwtOptions = _options.Value;

    // 1. Build Claims — data embedded inside the token payload
    var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Email, user.Email!),
        new Claim(JwtRegisteredClaimNames.Name,  user.DisplayName),
    };

    var roles = await _userManager.GetRolesAsync(user);
    foreach (var role in roles)
        claims.Add(new Claim(ClaimTypes.Role, role));

    // 2. Create Signing Credentials using the secret key
    var key  = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));
    var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    // 3. Build the token object
    var token = new JwtSecurityToken(
        issuer:             jwtOptions.Issuer,
        audience:           jwtOptions.Audience,
        expires:            DateTime.UtcNow.AddDays(2),
        claims:             claims,
        signingCredentials: cred);

    // 4. Serialize it to a compact string
    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

**How a JWT token is structured:**

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9    ← Header  (algorithm + type)
.eyJlbWFpbCI6ImFkbWluQGdtYWlsLmNvbSJ9    ← Payload (claims: email, name, roles, expiry)
.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQ  ← Signature (HMAC-SHA256 — prevents tampering)
```

**NuGet packages in this layer:**

| Package | Version | Why |
|---------|---------|-----|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.26 | Provides `JwtSecurityToken`, `JwtSecurityTokenHandler`, `SymmetricSecurityKey`, and `SigningCredentials` |

---

### Step 6 — Build the Persistence layer

`Persistence` handles everything database-related: the DbContext, migrations, and seeding.

```csharp
// StoreIdentityDbContext.cs
public class StoreIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public StoreIdentityDbContext(DbContextOptions<StoreIdentityDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Address>().ToTable("Addresses");
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
    }
}
```

`IdentityDataInitializer` runs at startup and seeds default roles and users only if the database is empty:

```csharp
public async Task InitializeAsync()
{
    if (!_roleManager.Roles.Any())
    {
        await _roleManager.CreateAsync(new IdentityRole("Admin"));
        await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
    }

    if (!_userManager.Users.Any())
    {
        var admin = new ApplicationUser
        {
            DisplayName = "admin", UserName = "admin",
            Email = "admin@gmail.com", PhoneNumber = "01012131415"
        };
        await _userManager.CreateAsync(admin, "P@ssw0rd");
        await _userManager.AddToRoleAsync(admin, "Admin");
        // ... same for superAdmin
    }
}
```

**NuGet packages in this layer:**

| Package | Version | Why |
|---------|---------|-----|
| `Microsoft.EntityFrameworkCore.SqlServer` | 8.0.26 | SQL Server provider — makes `options.UseSqlServer()` work |

---

### Step 7 — Build the Presentation layer

Controllers receive HTTP requests and delegate to the service. They know nothing about the database.

```csharp
// ApiController.cs — base class
[ApiController]
[Route("api/[controller]")]
public class ApiController { }

// AuthenticationController.cs
public class AuthenticationController : ApiController
{
    private readonly IAuthenticationService _service;

    public AuthenticationController(IAuthenticationService service)
        => _service = service;

    [HttpPost("Login")]
    public async Task<ActionResult<UserDto>> LoginAsync([FromBody] LoginDto loginDto)
        => Ok(await _service.LoginAsync(loginDto));

    [HttpPost("Register")]
    public async Task<ActionResult<UserDto>> RegisterAsync(RegisterDto registerDto)
        => Ok(await _service.RegisterAsync(registerDto));
}
```

**NuGet packages in this layer:**

| Package | Version | Why |
|---------|---------|-----|
| `FrameworkReference: Microsoft.AspNetCore.App` | built-in | Not a NuGet package — provides `ControllerBase`, `ActionResult`, and the full MVC stack at no extra cost |

---

### Step 8 — Wire everything in Program.cs

Register all services and configure the JWT validation middleware. Order matters.

```csharp
// 1. Register DbContext with SQL Server
builder.Services.AddDbContext<StoreIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

// 2. Register application services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddKeyedScoped<IDataInitializer, IdentityDataInitializer>("Identity");

// 3. Register ASP.NET Identity
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<StoreIdentityDbContext>();

// 4. Bind JWT settings from appsettings.json via Options Pattern
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWTOptions"));

// 5. Configure JWT Bearer Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer   = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer      = builder.Configuration["JWTOptions:Issuer"],
        ValidAudience    = builder.Configuration["JWTOptions:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWTOptions:SecretKey"]!))
    };
});

// 6. Run migrations and seeding automatically on startup
await app.MigrateIdentityDatabase();
await app.IdentitySeedDatabaseAsync();

// 7. Middleware pipeline — UseAuthentication MUST come before UseAuthorization
app.UseAuthentication();
app.UseAuthorization();
```

**NuGet packages in this layer:**

| Package | Version | Why |
|---------|---------|-----|
| `Swashbuckle.AspNetCore` | 6.6.2 | Generates the `/swagger` UI for testing endpoints during development |
| `Microsoft.EntityFrameworkCore.Tools` | 8.0.26 | Enables `dotnet ef migrations` — only used at development time (`PrivateAssets=all` means it is not deployed to production) |

---

### Step 9 — Protect endpoints with [Authorize]

```csharp
// Any authenticated user
[Authorize]
[HttpGet("profile")]
public IActionResult GetProfile() { ... }

// Admin role only
[Authorize(Roles = "Admin")]
[HttpGet("dashboard")]
public IActionResult AdminDashboard() { ... }

// Admin or SuperAdmin
[Authorize(Roles = "Admin,SuperAdmin")]
[HttpGet("management")]
public IActionResult Management() { ... }
```

**The four test scenarios — all four must pass before the implementation is complete:**

| Scenario | Expected result |
|----------|----------------|
| Request with no token | `401 Unauthorized` |
| Request with wrong or expired token | `401 Unauthorized` |
| Request with valid token but wrong role | `403 Forbidden` |
| Request with valid token and correct role | `200 OK` |

---

## 📦 NuGet Packages — Complete Summary per Layer

| Layer | Package | Version |
|-------|---------|---------|
| **Shared** | — | none |
| **Domain** | `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 8.0.26 |
| **Services.Abstraction** | — | none |
| **Services** | `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.26 |
| **Persistence** | `Microsoft.EntityFrameworkCore.SqlServer` | 8.0.26 |
| **Presentation** | `FrameworkReference: Microsoft.AspNetCore.App` | built-in |
| **Entry Point** | `Swashbuckle.AspNetCore` | 6.6.2 |
| **Entry Point** | `Microsoft.EntityFrameworkCore.Tools` | 8.0.26 |

> The key principle: the Core layers (Domain + Services) contain no infrastructure packages. The only exception is `JwtBearer` in Services, because that is where the token is actually built.

---

## 🏛️ Architecture Overview

```
┌──────────────────────────────────────────┐
│           Presentation Layer             │  HTTP in → delegate to service
│        (AuthenticationController)        │
├──────────────────────────────────────────┤
│            Application Layer             │  Business rules live here
│         (AuthenticationService)          │
├──────────────────────────────────────────┤
│              Domain Layer                │  Entities & interfaces — pure C#
│    (ApplicationUser · IAuthService)      │
├──────────────────────────────────────────┤
│          Infrastructure Layer            │  Database, migrations, seeding
│  (StoreIdentityDbContext · DataSeed)     │
└──────────────────────────────────────────┘
```

Each layer only depends on the layer below it. The Domain knows nothing about SQL Server, EF Core, or HTTP.

---

## 🧪 Testing the API

### Swagger (built-in)

Navigate to `https://localhost:7199/swagger` after running the project.

For protected endpoints, click **Authorize** and enter:
```
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### curl

```bash
# Register a new user
curl -X POST https://localhost:7199/api/Authentication/Register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","displayName":"Test","userName":"test","password":"P@ssw0rd","phoneNumber":"01000000000"}'

# Login
curl -X POST https://localhost:7199/api/Authentication/Login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@gmail.com","password":"P@ssw0rd"}'
```

### Decode the token

Paste any returned token into [jwt.io](https://jwt.io) to inspect the claims (email, display name, roles, expiry) and verify the signature.

