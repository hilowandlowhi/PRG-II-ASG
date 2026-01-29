// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;
//==========================================================
// Student Number : S10273266D
// Student Name : Matthew Tay
// Partner Name : Jovan Soo
//==========================================================
public class Menu
{
    private string MenuId;
    private string MenuName;
    private List<FoodItem> FoodItems;

    public Menu(string id, string name)
    {
        MenuId = id;
        MenuName = name;
        FoodItems = new List<FoodItem>();
    }

    public void AddFoodItem(FoodItem item)
    {
        if (item != null)
            FoodItems.Add(item);
    }

    // Put in Step 1 even though not stated
    public bool RemoveFoodItem(FoodItem item)
    {
        return FoodItems.Remove(item);
    }

    // Put in Step 1 even though not stated
    public void DisplayFoodItems()
    {
        foreach (var item in FoodItems)
            System.Console.WriteLine("  - " + item);
    }

    public override string ToString()
    {
        return $"{MenuName} ({MenuId})";
    }
}
