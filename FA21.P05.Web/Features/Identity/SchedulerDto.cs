using System.ComponentModel.DataAnnotations;
namespace FA21.P05.Web.Features.Identity
{
    public class SchedulerDto
    {
        [Key]
        public int Day { get; set; }

        public string DailySchedule { get; set; }
    }
}