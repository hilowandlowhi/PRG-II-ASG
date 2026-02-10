// See https://aka.ms/new-console-template for more information
//==========================================================
// Student Number : S10273266D
// Student Name : Matthew Tay
// Partner Name : Jovan Soo
//==========================================================

// This is the favourite order class, to get the favourite orders from a customer
class FavouriteOrder
{
    public string FavouriteId { get; set; }
    public string CustomerId { get; set; }
    public string FavouriteName { get; set; }
    public string RestaurantId { get; set; }
    public List<FavouriteItem> Items { get; set; }
    public DateTime CreatedDate { get; set; }

    public FavouriteOrder(string favouriteId, string customerId, string favouriteName, string restaurantId)
    {
        FavouriteId = favouriteId;
        CustomerId = customerId;
        FavouriteName = favouriteName;
        RestaurantId = restaurantId;
        Items = new List<FavouriteItem>();
        CreatedDate = DateTime.Now;
    }

    // Get the total price of the favourite order
    public double GetTotalPrice()
    {
        double total = 0;
        foreach (FavouriteItem item in Items)
        {
            total += item.FoodItem.ItemPrice * item.Quantity;
        }
        return total;
    }

    // Show details of favourite orders
    public void DisplayFavouriteDetails()
    {
        Console.WriteLine($"\nFavourite: {FavouriteName}");
        Console.WriteLine($"Restaurant ID: {RestaurantId}");
        Console.WriteLine("Items:");
        int index = 1;
        foreach (FavouriteItem item in Items)
        {
            Console.WriteLine($"{index}. {item.FoodItem.ItemName} x {item.Quantity} (${item.FoodItem.ItemPrice:F2} each)");
            if (!string.IsNullOrEmpty(item.Customisations))
            {
                Console.WriteLine($"Customisations: {item.Customisations}");

            }
            index++;
        }
        Console.WriteLine($"Estimated Total: ${GetTotalPrice():F2}");


    }
}