using System.ComponentModel.DataAnnotations;

namespace MessageNowApi.Models
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public List<Message> Messages { get; set; }
        [Required] 
        public List<UserConvConnector> UserConvConnectors { get; set; }
        public Message? LastMessage { get; set; }
    }
}
