using MessageNowApi.Models;
using System.ComponentModel.DataAnnotations;

namespace MessageNowApi.Dtos
{
    public class FullConversationDto
    {
        public int Id { get; set; }
        public List<Message> Messages { get; set; }
        public string Username { get; set; }
    }
}
