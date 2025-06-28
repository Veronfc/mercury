using backend.Models;
using Microsoft.EntityFrameworkCore;

DotNetEnv.Env.Load("../.env");

string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? throw new ArgumentNullException("CONNECTION_STRING");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BackendDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddIdentityApiEndpoints<User>().AddEntityFrameworkStores<BackendDbContext>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();
app.MapGroup("/user").CustomMapIdentityApi<User>();
app.MapControllers();

app.Run();
