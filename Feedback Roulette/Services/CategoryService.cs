using FeedbackRoulette_ClassLibrary;
using Feedback_Roulette.Data;
using Microsoft.EntityFrameworkCore;

namespace Feedback_Roulette.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IDbContextFactory<DataContext> _contextFactory;

        public CategoryService(IDbContextFactory<DataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Categories.ToListAsync();
        }
    }
}
