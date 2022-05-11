using Microsoft.AspNetCore.Identity;

namespace FA21.P05.Web.Features.Identity
{
    public class UserRole : IdentityUserRole<int>
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}