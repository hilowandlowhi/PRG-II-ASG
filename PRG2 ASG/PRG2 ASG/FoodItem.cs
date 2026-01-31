// See https://aka.ms/new-console-template for more information
//==========================================================
// Student Number : S10273266D
// Student Name : Matthew Tay
// Partner Name : Jovan Soo
//==========================================================
public class FoodItem
{
    public string ItemName;
    public string ItemDesc;
    public double ItemPrice;
    public string customise;

    public FoodItem(string name) { ItemName = name; }
    public FoodItem(string name, string desc, double price)
    {
        ItemName = name;
        ItemDesc = desc;
        ItemPrice = price;
        customise = "";
    }

    // Getters (needed by Menu when displaying later)
    public string GetItemName() => ItemName;
    public string GetItemDesc() => ItemDesc;
    public double GetItemPrice() => ItemPrice;

    public override string ToString()
    {
        return $"{ItemName}: {ItemDesc} - ${ItemPrice:0.00}";
    }
}
