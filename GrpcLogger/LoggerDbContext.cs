using GrpcLogger.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcLogger
{
    public class LoggerDbContext: DbContext
    {
        public DbSet<Log> Logs { get; set; }
        public LoggerDbContext(DbContextOptions<LoggerDbContext> options): base(options)
        {
        }
    }
}
