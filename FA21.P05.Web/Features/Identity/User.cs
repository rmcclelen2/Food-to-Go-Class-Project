using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FA21.P05.Web.Features.Identity
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; }
        [MaxLength(13)]
        public List<SchedulerDto> Schedule { get; set; }
       
        public virtual ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
    }
}