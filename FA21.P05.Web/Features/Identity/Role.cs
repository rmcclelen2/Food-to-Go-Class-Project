using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FA21.P05.Web.Features.Identity
{
    public class Role : IdentityRole<int>
    {
        public virtual ICollection<UserRole> Users { get; set; } = new List<UserRole>();
    }
}