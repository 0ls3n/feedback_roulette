using FeedbackRoulette_ClassLibrary;

namespace Feedback_Roulette.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategoriesAsync();
    }
}
