using backend.User;
using Microsoft.EntityFrameworkCore;

DotNetEnv.Env.Load("../.env");

string[] frontendOrigins = ["https://localhost:5173/", "https://localhost:5173"];

string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? throw new ArgumentNullException("CONNECTION_STRING");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendOrigin", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(frontendOrigins);
    });
});
builder.Services.AddDbContext<ApiDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddIdentityApiEndpoints<User>().AddEntityFrameworkStores<ApiDbContext>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontendOrigin");
app.UseAuthorization();
app.MapGroup("/user").CustomMapIdentityApi<User>();
app.MapControllers();

app.Run();
