using backend.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApiDbContext : IdentityDbContext<User>
{
  public ApiDbContext(DbContextOptions <ApiDbContext> options) : base(options) { }
}