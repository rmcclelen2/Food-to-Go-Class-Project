using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FA21.P05.Web.Features.Identity
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        [MaxLength(13)]
        public List<SchedulerDto> Schedule { get; set; }
        public string Role { get; set; }
    }
}