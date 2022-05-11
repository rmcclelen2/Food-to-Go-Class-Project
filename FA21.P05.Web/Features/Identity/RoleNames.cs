namespace FA21.P05.Web.Features.Identity
{
    public class RoleNames
    {
        public const string Admin = nameof(Admin);
        public const string Staff = nameof(Staff);
        public const string StaffOrAdmin = Admin + "," + Staff;

        public const string KitchenStaff = nameof(KitchenStaff);
        public const string Server = nameof(Server);
        public const string GreetingStaff = nameof(GreetingStaff);
        public const string DeliveryDriver = nameof(DeliveryDriver);
    }
}