using System.ComponentModel.DataAnnotations;

namespace aspnet_booklog.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string Title { get; set; } = "";

        [MaxLength(120)]
        public string? Author { get; set; }

        [Required]
        public string Status { get; set; } = "unread"; // unread|reading|done

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
