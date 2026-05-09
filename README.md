# 🔐 JWT Authentication — ASP.NET Core

نظام **JWT Authentication** كامل مبني بـ **ASP.NET Core** باستخدام **Clean Architecture**، يوفر تسجيل مستخدمين جدد وتسجيل دخول مع إصدار JWT Token.

---

## 📁 هيكل المشروع

```
JWTAuthentication/
│
├── 📦 Shared/                            # DTOs و Settings مشتركة
│   ├── JwtOptions.cs                     # إعدادات JWT (Issuer, Audience, SecretKey)
│   └── Dtos/IdentityDtos/
│       ├── LoginDto.cs                   # بيانات تسجيل الدخول
│       ├── RegisterDto.cs                # بيانات التسجيل
│       └── UserDto.cs                    # الرد بعد Login/Register
│
├── 🧠 Core/                              # Business Logic (لا يعتمد على أي Layer تانية)
│   ├── Domain/
│   │   ├── Entities/IdentityModule/
│   │   │   ├── ApplicationUser.cs        # User Entity (يرث من IdentityUser)
│   │   │   └── Address.cs               # Address Entity (One-to-One مع User)
│   │   ├── Contracts/
│   │   │   └── IDataInitializer.cs      # Interface لـ Seeding البيانات
│   │   └── Exceptions/
│   │       └── ValidationException.cs   # Custom Exception للأخطاء
│   ├── Services.Abstraction/
│   │   └── IAuthenticationService.cs    # Interface للـ Authentication Service
│   └── Services/
│       └── AuthenticationService.cs     # تنفيذ Login و Register وإنشاء JWT Token
│
├── 🏗️ Infrastructure/
│   ├── Presentation/
│   │   ├── ApiController.cs             # Base Controller
│   │   └── AuthenticationController.cs  # Endpoints: /Login و /Register
│   └── Presistence/
│       └── Data/
│           ├── DbContexts/
│           │   └── StoreIdentityDbContext.cs  # Identity DbContext
│           ├── DataSeed/
│           │   └── IdentityDataInitializer.cs # Seed Roles و Default Users
│           └── Migrations/                    # EF Core Migrations
│
└── 🚀 JWT Authentication/               # Entry Point (Web API)
    ├── Program.cs                        # تسجيل الـ Services والـ Middleware
    ├── Extensions/
    │   └── WebApplicationRegistration.cs # Extension Methods للـ Migration و Seeding
    └── appsettings.json                  # Connection String و JWT Settings
```

---

## ⚙️ المتطلبات

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (أو SQL Server Express)
- Visual Studio 2022 أو VS Code

---

## 🚀 طريقة التشغيل

### 1. Clone المشروع

```bash
git clone https://github.com/NourEldeenSaber/JWTAuthentication.git
cd JWTAuthentication
```

### 2. تعديل Connection String

افتح ملف `JWT Authentication/appsettings.json` وعدّل الـ Connection String حسب السيرفر عندك:

```json
{
  "ConnectionStrings": {
    "IdentityConnection": "Server=YOUR_SERVER; DataBase=Store.Identity; Trusted_Connection=True; TrustServerCertificate=True"
  }
}
```

> **مثال:** لو عندك SQL Server Express: `Server=.\SQLEXPRESS`

### 3. تشغيل المشروع

```bash
dotnet run --project "JWT Authentication"
```

عند التشغيل، المشروع بيعمل **تلقائياً**:
- ✅ يطبّق الـ Migrations ويكوّن قاعدة البيانات
- ✅ يضيف الـ Roles: `Admin` و `SuperAdmin`
- ✅ يضيف يوزرين افتراضيين (Admin و SuperAdmin)

---

## 🗄️ قاعدة البيانات

### الجداول المُنشأة

| الجدول | الوصف |
|--------|-------|
| `Users` | بيانات المستخدمين (يرث من AspNetUsers) |
| `Roles` | الأدوار: Admin, SuperAdmin |
| `UserRoles` | ربط المستخدمين بالأدوار |
| `Addresses` | عناوين المستخدمين (اختياري) |

### البيانات الافتراضية (Seeded)

| DisplayName | Email | Password | Role |
|-------------|-------|----------|------|
| admin | admin@gmail.com | P@ssw0rd | Admin |
| superAdmin | superAdmin@gmail.com | P@ssw0rd | SuperAdmin |

---

## 🔑 إعداد JWT

في ملف `appsettings.json`:

```json
{
  "JWTOptions": {
    "SecretKey": "da447425e611e9c5d94ca0bd8a37ce852bf7ecffcf11352cddcb0b8571bb994b",
    "Issuer": "https://localhost:7199/",
    "Audience": "https://localhost:7199/"
  }
}
```

| الإعداد | الوصف |
|---------|-------|
| `SecretKey` | المفتاح السري لتوقيع الـ Token (يجب أن يكون طويل وعشوائي في Production) |
| `Issuer` | الجهة المُصدِرة للـ Token (رابط API) |
| `Audience` | الجهة المستهدفة بالـ Token |

> ⚠️ **تحذير:** لا تستخدم نفس الـ SecretKey في Production — اتعمل Key جديد وخبّيه في Environment Variables أو Azure Key Vault.

---

## 📡 الـ API Endpoints

### 📥 Register — تسجيل مستخدم جديد

```
POST /api/Authentication/Register
Content-Type: application/json
```

**Request Body:**
```json
{
  "email": "user@example.com",
  "displayName": "Ahmed Mohamed",
  "userName": "ahmed123",
  "password": "P@ssw0rd",
  "phoneNumber": "01012345678"
}
```

**Response (200 OK):**
```json
{
  "email": "user@example.com",
  "displayName": "Ahmed Mohamed",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

### 🔓 Login — تسجيل الدخول

```
POST /api/Authentication/Login
Content-Type: application/json
```

**Request Body:**
```json
{
  "email": "admin@gmail.com",
  "password": "P@ssw0rd"
}
```

**Response (200 OK):**
```json
{
  "email": "admin@gmail.com",
  "displayName": "admin",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

### 🔒 استخدام الـ Token في الـ Requests المحمية

بعد الـ Login، بعت الـ Token في كل Request في الـ Header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 🔍 كيف يشتغل الكود؟ (شرح تفصيلي)

### 1️⃣ تسجيل الـ Services في `Program.cs`

```csharp
// ربط DbContext بـ SQL Server
builder.Services.AddDbContext<StoreIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

// تسجيل Authentication Service
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// إعداد ASP.NET Identity
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<StoreIdentityDbContext>();

// ربط JWT Settings من appsettings.json بـ JwtOptions Class
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWTOptions"));

// إعداد JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,       // تحقق من Issuer
        ValidateAudience = true,     // تحقق من Audience
        ValidateLifetime = true,     // تحقق من صلاحية الـ Token
        ValidIssuer = builder.Configuration["JWTOptions:Issuer"],
        ValidAudience = builder.Configuration["JWTOptions:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWTOptions:SecretKey"]!))
    };
});
```

---

### 2️⃣ عملية التسجيل `RegisterAsync`

```csharp
public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
{
    // إنشاء User Object جديد
    var user = new ApplicationUser()
    {
        Email = registerDto.Email,
        DisplayName = registerDto.DisplayName,
        UserName = registerDto.UserName,
        PhoneNumber = registerDto.PhoneNumber,
    };

    // تخزين الـ User في قاعدة البيانات مع تشفير الباسورد تلقائياً
    var result = await _userManager.CreateAsync(user, registerDto.Password);

    // لو في أخطاء (باسورد ضعيف، إيميل مكرر...) → throw ValidationException
    if (!result.Succeeded)
    {
        var errors = result.Errors.Select(e => e.Description).ToList();
        throw new ValidationException(errors);
    }

    // إنشاء JWT Token وإرجاع بيانات الـ User
    var token = await CreateTokenAsync(user);
    return new UserDto(user.Email, user.DisplayName, token);
}
```

---

### 3️⃣ عملية تسجيل الدخول `LoginAsync`

```csharp
public async Task<UserDto> LoginAsync(LoginDto loginDto)
{
    // البحث عن الـ User بالإيميل
    var user = await _userManager.FindByEmailAsync(loginDto.Email);
    if (user is null)
        throw new Exception("User Invalid");

    // التحقق من الباسورد
    var isValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
    if (!isValid)
        throw new Exception("User Invalid");

    // إنشاء JWT Token وإرجاع بيانات الـ User
    var token = await CreateTokenAsync(user);
    return new UserDto(user.Email!, user.DisplayName, token);
}
```

---

### 4️⃣ إنشاء الـ JWT Token `CreateTokenAsync`

هنا القلب الحقيقي للـ JWT Authentication:

```csharp
private async Task<string> CreateTokenAsync(ApplicationUser user)
{
    var jwtOptions = _options.Value;

    // 1️⃣ إنشاء الـ Claims (بيانات داخل الـ Token)
    var claims = new List<Claim>()
    {
        new Claim(JwtRegisteredClaimNames.Email, user.Email!),
        new Claim(JwtRegisteredClaimNames.Name, user.DisplayName),
    };

    // إضافة Roles الـ User للـ Claims
    var roles = await _userManager.GetRolesAsync(user);
    foreach (var role in roles)
        claims.Add(new Claim(ClaimTypes.Role, role));

    // 2️⃣ إنشاء Signing Credentials بالـ Secret Key
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    // 3️⃣ بناء الـ Token
    var token = new JwtSecurityToken(
        issuer: jwtOptions.Issuer,
        audience: jwtOptions.Audience,
        expires: DateTime.UtcNow.AddDays(2),    // صالح لمدة يومين
        claims: claims,
        signingCredentials: credentials
    );

    // 4️⃣ تحويل الـ Token لـ String وإرجاعه
    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

**مكونات الـ JWT Token:**

```
Header.Payload.Signature
  ↓        ↓          ↓
eyJhbG... eyJlbW... SflKxwRJ...
```

| الجزء | المحتوى |
|-------|---------|
| Header | Algorithm (HmacSha256) + Type (JWT) |
| Payload | Claims: Email, DisplayName, Roles, ExpiryDate |
| Signature | HMAC(Header + Payload, SecretKey) — لضمان عدم التلاعب |

---

### 5️⃣ الـ Data Seeding التلقائي

عند أول تشغيل للمشروع:

```csharp
// في Program.cs
await app.MigrateIdentityDatabase();   // تطبيق الـ Migrations
await app.IdentitySeedDatabaseAsync(); // إضافة البيانات الافتراضية
```

الـ `IdentityDataInitializer` بيتحقق لو في Roles أو Users مضافين قبل كده، ولو لأ بيضيفهم:

```csharp
// إنشاء الـ Roles لو مش موجودة
if (!_roleManager.Roles.Any())
{
    await _roleManager.CreateAsync(new IdentityRole("Admin"));
    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
}

// إنشاء الـ Default Users لو مش موجودين
if (!_userManager.Users.Any())
{
    await _userManager.CreateAsync(user01, "P@ssw0rd");
    await _userManager.AddToRoleAsync(user01, "Admin");
    // ...
}
```

---

## 🏛️ الـ Architecture Pattern

المشروع بيتبع **Clean Architecture**:

```
┌─────────────────────────────────────────┐
│           Presentation Layer            │  ← Controllers (HTTP Requests)
│        (AuthenticationController)       │
├─────────────────────────────────────────┤
│            Application Layer            │  ← Services (Business Logic)
│         (AuthenticationService)         │
├─────────────────────────────────────────┤
│              Domain Layer               │  ← Entities + Interfaces (Core)
│      (ApplicationUser, IAuthService)    │
├─────────────────────────────────────────┤
│          Infrastructure Layer           │  ← DbContext + DataSeed
│    (StoreIdentityDbContext, Seeding)    │
└─────────────────────────────────────────┘
```

**الفائدة:** كل Layer معزولة — تقدر تغير الـ Database أو الـ Authentication Logic من غير ما تأثر على بقية الكود.

---

## 🛡️ حماية الـ Endpoints

لحماية أي Endpoint بالـ JWT Token، بستخدم `[Authorize]`:

```csharp
// السماح للمستخدمين المسجلين فقط
[Authorize]
[HttpGet("profile")]
public IActionResult GetProfile() { ... }

// السماح لـ Admin فقط
[Authorize(Roles = "Admin")]
[HttpGet("admin-only")]
public IActionResult AdminDashboard() { ... }

// السماح لـ Admin أو SuperAdmin
[Authorize(Roles = "Admin,SuperAdmin")]
[HttpGet("management")]
public IActionResult Management() { ... }
```

---

## 📦 الـ NuGet Packages المستخدمة

| Package | الاستخدام |
|---------|-----------|
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | ASP.NET Identity مع EF Core |
| `Microsoft.EntityFrameworkCore.SqlServer` | SQL Server Provider |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT Bearer Authentication |
| `System.IdentityModel.Tokens.Jwt` | إنشاء والتحقق من JWT Tokens |
| `Microsoft.IdentityModel.Tokens` | SymmetricSecurityKey و SigningCredentials |

---

## 🧪 تجربة الـ API

### عن طريق Swagger

بعد التشغيل، افتح المتصفح على:
```
https://localhost:7199/swagger
```

للـ Endpoints المحمية في Swagger، اضغط **Authorize** وادخل:
```
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### عن طريق curl

```bash
# Register
curl -X POST https://localhost:7199/api/Authentication/Register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","displayName":"Test","userName":"test","password":"P@ssw0rd","phoneNumber":"01000000000"}'

# Login
curl -X POST https://localhost:7199/api/Authentication/Login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@gmail.com","password":"P@ssw0rd"}'
```


---

## 👨‍💻 المطور

**Nour Eldeen Saber** — [GitHub](https://github.com/NourEldeenSaber)
