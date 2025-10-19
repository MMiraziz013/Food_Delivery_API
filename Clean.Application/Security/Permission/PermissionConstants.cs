namespace Clean.Application.Security.Permission;

public static class PermissionConstants 
{
    public static class Restaurants
    {
        public const string View = "Permissions.Restaurants.View";
        public const string Manage = "Permissions.Restaurants.Manage";
    }

    public static class Orders
    {
        public const string View = "Permissions.Orders.View";
        public const string Create = "Permissions.Orders.Create";
        public const string Manage = "Permissions.Orders.Manage";
    }

    public static class Couriers
    {
        public const string View = "Permissions.Couriers.View";
        public const string Manage = "Permissions.Couriers.Manage";
    }
    
    public static class Menus
    {
        public const string View = "Permissions.Menus.View";
        public const string Manage = "Permissions.Menus.Manage";

    }

    public static class User
    {
        public const string Manage = "Permissions.Profile.Manage";
    }
}