// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main()
    {
        Dictionary<string, Restaurant> restaurants = LoadRestaurants("restaurants.csv");
        int foodItemsLoaded = LoadFoodItems("fooditems.csv", restaurants);

        Console.WriteLine($"{restaurants.Count} restaurants loaded!");
        Console.WriteLine($"{foodItemsLoaded} food items loaded!");
    }

    static Dictionary<string, Restaurant> LoadRestaurants(string file)
    {       
        if (!File.Exists(file))
            throw new FileNotFoundException($"File not found: {file}");

        Dictionary<string, Restaurant> restaurants = new Dictionary<string, Restaurant>();

        using StreamReader sr = new StreamReader(file);
        sr.ReadLine(); // skip header

        while (!sr.EndOfStream)
        {
            string line = sr.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');

            string id = parts[0].Trim();
            string name = parts[1].Trim();
            string email = parts[2].Trim();

            restaurants[id] = new Restaurant(id, name, email);
        }

        return restaurants;
    }

    static int LoadFoodItems(string file, Dictionary<string, Restaurant> restaurants)
    {
        if (!File.Exists(file))
            throw new FileNotFoundException($"File not found: {file}");

        int count = 0;

        using StreamReader sr = new StreamReader(file);
        sr.ReadLine(); // skip header

        while (!sr.EndOfStream)
        {
            string line = sr.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');

            // RestaurantId,ItemName,Description,Price
            string restaurantId = parts[0].Trim();
            string itemName = parts[1].Trim();
            string desc = parts[2].Trim();
            double price = double.Parse(parts[3].Trim());

            if (restaurants.ContainsKey(restaurantId))
            {
                FoodItem item = new FoodItem(itemName, desc, price);
                restaurants[restaurantId].GetMenu().AddFoodItem(item);
                count++;
            }
        }

        return count;
    }
}
