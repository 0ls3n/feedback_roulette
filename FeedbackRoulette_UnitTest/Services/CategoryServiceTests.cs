using Microsoft.VisualStudio.TestTools.UnitTesting;
using Feedback_Roulette.Services;
using Feedback_Roulette.Data;
using FeedbackRoulette_ClassLibrary;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FeedbackRoulette_UnitTest.Services
{
    [TestClass]
    public class CategoryServiceTests
    {
        private DataContext _context;
        private CategoryService _service;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);
            
            _context.Categories.Add(new Category { Id = 1, Name = "Music", Description = "Music category" });
            _context.Categories.Add(new Category { Id = 2, Name = "Programming", Description = "Programming category" });
            _context.Categories.Add(new Category { Id = 3, Name = "Design", Description = "Design category" });
            _context.SaveChanges();

            var dbFactory = new TestDbContextFactory(options);
            _service = new CategoryService(dbFactory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetCategoriesAsync_ShouldReturnAllCategories()
        {
            var categories = await _service.GetCategoriesAsync();

            Assert.AreEqual(3, categories.Count);
        }

        [TestMethod]
        public async Task GetCategoriesAsync_ShouldReturnCorrectCategoryNames()
        {
            var categories = await _service.GetCategoriesAsync();
            var names = categories.Select(c => c.Name).ToList();

            Assert.IsTrue(names.Contains("Music"));
            Assert.IsTrue(names.Contains("Programming"));
            Assert.IsTrue(names.Contains("Design"));
        }

        [TestMethod]
        public async Task GetCategoriesAsync_ShouldReturnEmptyListWhenNoCategories()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var emptyContext = new DataContext(options);
            var dbFactory = new TestDbContextFactory(options);
            var emptyService = new CategoryService(dbFactory);

            var categories = await emptyService.GetCategoriesAsync();

            Assert.AreEqual(0, categories.Count);
            
            emptyContext.Dispose();
        }
    }
}
