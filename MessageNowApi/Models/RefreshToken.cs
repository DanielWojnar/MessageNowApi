using System.ComponentModel.DataAnnotations;

namespace MessageNowApi.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public bool Active { get; set; }
    }
}
