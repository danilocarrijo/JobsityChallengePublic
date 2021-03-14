using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string signalRId { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
