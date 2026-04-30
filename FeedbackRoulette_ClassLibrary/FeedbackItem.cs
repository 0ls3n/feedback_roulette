

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
    public required Category Category { get; set; }
    
    public List<Feedback> Feedbacks { get; set; }
}