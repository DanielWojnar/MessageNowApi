using System.ComponentModel.DataAnnotations;

namespace MessageNowApi.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public MessageNowUser User { get; set; }
        [Required]
        public Conversation Conversation { get; set; }
        [Required]
        public bool Read { get; set; }
    }
}
