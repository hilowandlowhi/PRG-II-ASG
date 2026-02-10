// See https://aka.ms/new-console-template for more information
//==========================================================
// Student Number : S10273266D
// Student Name : Matthew Tay
// Partner Name : Jovan Soo
//==========================================================

// This is the Class to get the favourite item of a customer

class FavouriteItem
{
    public FoodItem FoodItem { get; set; }
    public int Quantity { get; set; }
    public string Customisations { get; set; }

    public FavouriteItem(FoodItem foodItem, int quantity, string customisations)
    {
        FoodItem = foodItem;
        Quantity = quantity;
        Customisations = customisations;
    }
}