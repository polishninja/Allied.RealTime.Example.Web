using System.ComponentModel.DataAnnotations;

namespace Allied.RealTime.Example.Web.Models
{
    public class ProfileRequest
    {
        [Required]
        public string Username { get; set; }
    }
}