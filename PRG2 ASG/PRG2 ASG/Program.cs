// See https://aka.ms/new-console-template for more information

//==========================================================
// Student Number : S10273266
// Partner Number : S10271067
// Student Name : Matthew Tay
// Partner Name : Jovan Soo
//==========================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

class Program
{
    static void Main()
    {
        // Step 1: Load restaurants and food items
        Dictionary<string, Restaurant> restaurants = LoadRestaurants("restaurants.csv");
        int foodItemsLoaded = LoadFoodItems("fooditems.csv", restaurants);

        Console.WriteLine($"{restaurants.Count} restaurants loaded!");
        Console.WriteLine($"{foodItemsLoaded} food items loaded!");

        // Step 2: Load customers and orders
        List<Customer> customers = LoadCustomers("customers.csv");
        int orderCount = LoadOrders();

        Console.WriteLine($"{customers.Count} customers loaded!");
        Console.WriteLine($"{orderCount} orders loaded!");

    }

    static Dictionary<string, Restaurant> LoadRestaurants(string file)
    {       
        if (!File.Exists(file))
            throw new FileNotFoundException($"File not found: {file}");

        Dictionary<string, Restaurant> restaurants = new Dictionary<string, Restaurant>();

        using StreamReader sr = new StreamReader(file);
        sr.ReadLine(); // Skip header

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
        sr.ReadLine(); // Skip header

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

    static List<Customer> LoadCustomers(string file)
    {
    
        if (!File.Exists(file))
            throw new FileNotFoundException($"File not found: {file}");

        List<Customer> customers = new List<Customer>();

        using StreamReader sr = new StreamReader(file);
        sr.ReadLine(); // Skip header

        while (!sr.EndOfStream)
        {
            string line = sr.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');

            // EmailAddress,CustomerName
            string name = parts[0].Trim();
            string email = parts[1].Trim();

            Customer customer = new Customer(email, name);
            customers.Add(customer);

        }
        return customers;

    }

    static int LoadOrders()
    {
        int count = 0;
        string[] lines = File.ReadAllLines("orders.csv");

        // Skip header
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            List<string> fields = new List<string>();
            string field = "";
            bool insideQuotes = false;

            foreach (char c in line)
            {
                if (c == '"')
                    insideQuotes = !insideQuotes;
                else if (c == ',' && !insideQuotes)
                {
                    fields.Add(field);
                    field = "";
                }
                else
                    field += c;
            }
            fields.Add(field);
            int orderId = int.Parse(fields[0]);
            string customerEmail = fields[1];
            string restaurantId = fields[2];

            DateTime deliveryDateTime =
                DateTime.Parse(fields[3] + " " + fields[4]);

            string deliveryAddress = fields[5];

            DateTime createdDateTime =
                DateTime.Parse(fields[6]);

            double totalAmount =
                double.Parse(fields[7]);

            string status = fields[8];
            string itemsColumn = fields[9];

            Order order = new Order(
                orderId,
                createdDateTime,
                totalAmount,
                status,
                deliveryDateTime,
                deliveryAddress,
                "Cash",
                false
            );

            string[] orderedItems = itemsColumn.Split('|');

            foreach (string orderedItem in orderedItems)
            {
                string[] parts = orderedItem.Split(',');

                if (parts.Length != 2)
                    continue; // prevents crash

                string itemName = parts[0].Trim();
                int quantity = int.Parse(parts[1].Trim());

                FoodItem foodItem = new FoodItem(itemName);
                OrderedFoodItem orderedFoodItem =
                    new OrderedFoodItem(foodItem, quantity);

                order.AddOrderedFoodItem(orderedFoodItem);
            }
            count++;
        }
        return count;
    }
}
