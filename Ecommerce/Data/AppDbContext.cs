using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data
{
	public class AppDbContext : DbContext
	{

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

    }
}
