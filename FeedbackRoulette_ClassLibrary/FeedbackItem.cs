

using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackRoulette_ClassLibrary;

public class FeedbackItem
{
    public int Id { get; set; }
    
    public required string Title { get; set; }
    public required string Description { get; set; }
    
    public string? ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    
    public string? FileUrl  { get; set; }
    public string? FileType  { get; set; }
    public string? FileSize { get; set; }
    
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    
    public List<Feedback> Feedbacks { get; set; }
    
    [NotMapped]
    public double AverageRating
    {
        get
        {
            if (Feedbacks == null || !Feedbacks.Any())
                return 0;
            
            var scores = Feedbacks.Select(f =>
                f.HasPositiveFeedback && !f.HasNegativeFeedback ? 1 :
                !f.HasPositiveFeedback && f.HasNegativeFeedback ? -1 :
                0);
            
            var averageScore = scores.Average();
            return (averageScore + 1) * 2 + 1;
        }
    }
}