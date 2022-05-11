using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FA21.P05.Web.Features.Identity
{
    public class UserLoginDto
    {
        [Required]
        public string UserName { get; set; }
        public string Name { get; set; }
        [MaxLength(13)]
        public List<SchedulerDto> Schedule { get; set; }
        [Required]
        public string Password { get; set; }
    }
}