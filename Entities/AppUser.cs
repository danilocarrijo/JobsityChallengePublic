using Microsoft.AspNetCore.Identity;

namespace Entities
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string signalRId { get; set; }
        public string Password { get; set; }
    }
}
