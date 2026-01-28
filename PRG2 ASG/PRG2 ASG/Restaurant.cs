// See https://aka.ms/new-console-template for more information
public class Restaurant
{
    private string RestaurantId;
    private string RestaurantName;
    private string RestaurantEmail;
    private Menu Menu;

    public Restaurant(string id, string name, string email)
    {
        RestaurantId = id;
        RestaurantName = name;
        RestaurantEmail = email;

        // One Menu per Restaurant
        Menu = new Menu(id + "_MENU", name + " Menu");
    }

    // Put in Step 1 even though not stated
    public Menu GetMenu()
    {
        return Menu;
    }

    // Put in Step 1 even though not stated
    public void DisplayMenu()
    {
        System.Console.WriteLine($"Restaurant: {RestaurantName} ({RestaurantId})");
        Menu.DisplayFoodItems();
    }

    public override string ToString()
    {
        return $"{RestaurantName} ({RestaurantId}) - {RestaurantEmail}";
    }
}
