using System.ComponentModel.DataAnnotations;

namespace MessageNowApi.Models
{
    public class UserConvConnector
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Conversation Conversation { get; set; }
        [Required]
        public MessageNowUser User { get; set; }
    }
}
