

namespace FeedbackRoulette_ClassLibrary;

public class Feedback
{
    public int Id { get; set; }
    
    public string? ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    
    public bool HasPositiveFeedback { get; set; }
    public bool HasNegativeFeedback { get; set; }
    public bool HasSuggestion { get; set; }
    
    public string? PositiveFeedback { get; set; }
    public string? NegativeFeedback { get; set; }
    public string? Suggestion { get; set; }
}