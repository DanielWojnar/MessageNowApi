using MessageNowApi.Models;
using System.ComponentModel.DataAnnotations;

namespace MessageNowApi.Dtos
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public List<Message> Messages { get; set; }
        public Message? LastMessage { get; set; }
        public string Name { get; set; }
    }
}
