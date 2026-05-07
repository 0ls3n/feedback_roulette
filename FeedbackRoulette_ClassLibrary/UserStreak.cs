using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackRoulette_ClassLibrary
{
    public class UserStreak
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(ApplicationUserId))]
        public virtual ApplicationUser? User { get; set; }

        public int CurrentStreak { get; set; } = 0;

        public int LongestStreak { get; set; } = 0;

        public DateTime LastFeedbackDate { get; set; } = DateTime.MinValue;

        public DateTime StreakStartedAt { get; set; } = DateTime.MinValue;
    }
}