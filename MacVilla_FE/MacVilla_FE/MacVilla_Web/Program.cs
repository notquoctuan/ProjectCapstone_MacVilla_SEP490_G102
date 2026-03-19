using System.Net.Http.Headers;
using MacVilla_Web.Services;

var builder = WebApplication.CreateBuilder(args);

// =======================
// 1. SERVICES
// =======================
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserApiService>();
builder.Services.AddScoped<CartApiService>();
// Named HttpClient
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7262/";
builder.Services.AddHttpClient("MacVillaAPI", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

// Typed HttpClient
builder.Services.AddHttpClient<ProductApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<UserApiService>(client => {
    client.BaseAddress = new Uri("https://localhost:7262/"); 
});
builder.Services.AddHttpClient<DashboardApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001/");
});
// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// =======================
// 2. BUILD
// =======================
var app = builder.Build();

// =======================
// 3. MIDDLEWARE
// =======================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();

// ⭐ BẮT BUỘC PHẢI CÓ
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.Run();
