using Microsoft.EntityFrameworkCore;
using Feedback_Roulette.Data;

namespace FeedbackRoulette_UnitTest.Services
{
    public class TestDbContextFactory : IDbContextFactory<DataContext>
    {
        private readonly DbContextOptions<DataContext> _options;

        public TestDbContextFactory(DbContextOptions<DataContext> options)
        {
            _options = options;
        }

        public DataContext CreateDbContext()
        {
            return new DataContext(_options);
        }

        public Task<DataContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new DataContext(_options));
        }
    }
}
