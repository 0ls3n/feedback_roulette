using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackRoulette_ClassLibrary
{
    public class UserFollow
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FollowerUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(FollowerUserId))]
        public virtual ApplicationUser? FollowerUser { get; set; }

        [Required]
        public string FollowedUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(FollowedUserId))]
        public virtual ApplicationUser? FollowedUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
