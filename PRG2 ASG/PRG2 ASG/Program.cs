// See https://aka.ms/new-console-template for more information

//==========================================================
// Student Number : S10273266
// Partner Number : S10271067
// Student Name : Matthew Tay
// Partner Name : Jovan Soo
//==========================================================
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Net.Mail;
using System.Runtime.InteropServices.Marshalling;
using static System.Collections.Specialized.BitVector32;

// To Do: Validations (and feedback)
class Program
{
    // For Step 4, Use name to reference email
    static Dictionary<string, Customer> customersByEmail = new Dictionary<string, Customer>();
    static List<Order> allOrders = new List<Order>();
    static Dictionary<int, string> orderCustomerEmail = new Dictionary<int, string>();
    static Dictionary<int, string> orderRestaurantId = new Dictionary<int, string>();
    static Dictionary<string, string> restaurantNameById = new Dictionary<string, string>();
    static Dictionary<string, Restaurant> allRestaurants = new Dictionary<string, Restaurant>();
    static Queue<Order> orderQueue = new Queue<Order>();
    static Stack<Order> refundStack = new Stack<Order>();
    static void Main()
    {
        Console.WriteLine("Welcome to the Gruberoo Food Delivery System");

        // Step 1: Load restaurants and food items
        Dictionary<string, Restaurant> restaurants = LoadRestaurants("restaurants.csv");
        allRestaurants = restaurants;
        int foodItemsLoaded = LoadFoodItems("fooditems.csv", restaurants);

        Console.WriteLine($"{restaurants.Count} restaurants loaded!");
        Console.WriteLine($"{foodItemsLoaded} food items loaded!");

        // Step 2: Load customers and orders
        Dictionary<string, Customer> customers = LoadCustomers("customers.csv");
        int orderCount = LoadOrders();

        Console.WriteLine($"{customers.Count} customers loaded!");
        Console.WriteLine($"{orderCount} orders loaded!");
        Console.WriteLine();

        while (true)
        {
            Console.WriteLine("===== Gruberoo Food Delivery System =====");
            Console.WriteLine("1. List all restaurants and menu items");
            Console.WriteLine("2. List all orders");
            Console.WriteLine("3. Create a new order");
            Console.WriteLine("4. Process an order");
            Console.WriteLine("5. Modify an existing order");
            Console.WriteLine("6. Delete an existing order");
            Console.WriteLine("0. Exit");
            Console.Write("Enter your choice: ");
            string Choice = Console.ReadLine();
            Console.WriteLine();

            switch (Choice)
            {
                case "1":
                    // Step 3: Display all restaurants and menu items
                    DisplayAllRestaurantsAndMenuItems(restaurants);
                    break;
                case "2":
                    // Step 4: Display All Orders Made
                    DisplayAllOrders();
                    break;
                case "3":
                    // Step 5: Create a New Order
                    CreateNewOrder();
                    break;
                case "4":
                    // Step 6: Process An Order
                    ProcessOrder();
                    break;
                case "5":
                    // Step 7: Modify An Existing Order
                    ModifyOrder();
                    break;
                case "6":
                    // Step 8: Delete An Existing Order
                    DeleteOrder();
                    break;
                    
                case "0":
                    // Exit
                    Console.WriteLine("Exiting the system. Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    // Load restaurants from CSV file
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

            restaurantNameById[id] = name;

            restaurants[id] = new Restaurant(id, name, email);
        }

        return restaurants;
    }

    // Load food items from CSV file
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

    // Load customers from CSV file
    static Dictionary<string, Customer> LoadCustomers(string file) // Changed to Dictionary For Step 4
    {

        if (!File.Exists(file))
            throw new FileNotFoundException($"File not found: {file}");

        Dictionary<string, Customer> customers = new Dictionary<string, Customer>();

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




            customers[name] = new Customer(email, name);

            customersByEmail[email] = customers[name];

        }
        return customers;

    }

    // Load orders from CSV file
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
            // Now fields contains all columns for the current line
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

            allOrders.Add(order);
            orderCustomerEmail[orderId] = customerEmail;
            orderRestaurantId[orderId] = restaurantId;

            // Add order to customer's order list
            if (customersByEmail.ContainsKey(customerEmail))
            {
                customersByEmail[customerEmail].Orders.Add(order);
            }

            string[] orderedItems = itemsColumn.Split('|');

            // Add ordered food items to the order
            foreach (string orderedItem in orderedItems)
            {
                string[] parts = orderedItem.Split(',');

                if (parts.Length != 2)
                    continue; // prevents crash

                string itemName = parts[0].Trim();
                int quantity = int.Parse(parts[1].Trim());

                FoodItem foodItem = new FoodItem(itemName);
                OrderedFoodItem orderedFoodItem = new OrderedFoodItem(foodItem, quantity);

                order.AddOrderedFoodItem(orderedFoodItem);
            }
            count++;
        }
        return count;
    }

    // Display all restaurants and their menu items
    static void DisplayAllRestaurantsAndMenuItems(Dictionary<string, Restaurant> restaurants)
    {
        Console.WriteLine();
        Console.WriteLine("All Restaurants and Menu Items");
        Console.WriteLine("==============================");

        // Display each restaurant and its menu
        foreach (Restaurant restaurant in restaurants.Values)
        {
            restaurant.DisplayMenu();
            Console.WriteLine();
        }
    }

    // Display All Orders 
    static void DisplayAllOrders()
    {
        Console.WriteLine();
        Console.WriteLine("All Orders");
        Console.WriteLine("==========");
        Console.WriteLine("Order ID    Customer      Restaurant       Delivery Date/Time   Amount    Status");
        Console.WriteLine("--------    ----------    -------------    ------------------   ------    ---------");

        foreach (Order o in allOrders)
        {
            // Assign the data value for each column(Order Id, Customoer...)
            string email = orderCustomerEmail.ContainsKey(o.OrderId) ? orderCustomerEmail[o.OrderId] : "";
            string restaurant = orderRestaurantId.ContainsKey(o.OrderId) ? orderRestaurantId[o.OrderId] : "";

            string custName = customersByEmail.ContainsKey(email) ? customersByEmail[email].CustomerName : "";

            string restName = restaurantNameById.ContainsKey(restaurant) ? restaurantNameById[restaurant] : "";

            string amountText = $"${o.OrderTotal:0.00}"; // Sticks the $ sign with the amount

            Console.WriteLine($"{o.OrderId,-12}{custName,-14}{restName,-17}{o.DeliveryDateTime,-20:dd/MM/yyyy HH:mm} {amountText,-7}   {o.OrderStatus}");

        }

    }
    static void CreateNewOrder()
    {
        Console.WriteLine();
        Console.WriteLine("Create New Order");
        Console.WriteLine("================");

        // Get customer email
        Console.Write("Enter Customer Email: ");
        string customerEmail = Console.ReadLine();

        // Get restaurant ID
        Console.Write("Enter Restaurant ID: ");
        string restaurantId = Console.ReadLine().Trim();

        // Get delivery date and time
        Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
        string deliveryDateStr = Console.ReadLine();
        Console.Write("Enter Delivery Time (hh:mm): ");
        string deliveryTimeStr = Console.ReadLine();

        // Get delivery address
        Console.Write("Enter Delivery Address: ");
        string deliveryAddress = Console.ReadLine();

        Console.WriteLine();

        // Validate restaurant ID
        if (!allRestaurants.ContainsKey(restaurantId))
        {
            Console.WriteLine("Invalid Restaurant ID.");
            return;
        }

        // Get the restaurant and display its menu
        Restaurant selectedRestaurant = allRestaurants[restaurantId];
        Console.WriteLine("Available Food Items:");
        Menu menu = selectedRestaurant.GetMenu();
        menu.DisplayFoodItemsNumbered();
        List<FoodItem> items = menu.FoodItems;
        Console.WriteLine();

        // Allow user to select items
        List<OrderedFoodItem> orderedItems = new List<OrderedFoodItem>();
        double subtotal = 0;

        while (true)
        {
            Console.Write("Enter item number (0 to finish): ");
            string input = Console.ReadLine();
            int itemNumber = int.Parse(input);

            if (itemNumber == 0)
                break;

            if (itemNumber < 1 || itemNumber > items.Count)
            {
                Console.WriteLine("Invalid item number.");
                continue;
            }

            Console.Write("Enter quantity: ");
            int quantity = int.Parse(Console.ReadLine());

            FoodItem selectedItem = items[itemNumber - 1];
            OrderedFoodItem orderedItem = new OrderedFoodItem(selectedItem, quantity);
            orderedItems.Add(orderedItem);

            subtotal += selectedItem.ItemPrice * quantity;
        }

        // Ask for special request
        Console.Write("Add special request? [Y/N]: ");
        string specialRequestChoice = Console.ReadLine().ToUpper();

        string specialRequest = null;
        if (specialRequestChoice == "Y")
        {
            Console.Write("Enter special request: ");
            specialRequest = Console.ReadLine();
        }

        // Calculate order total (subtotal + delivery fee)
        double deliveryFee = 5.00;
        double orderTotal = subtotal + deliveryFee;

        // Display order summary
        Console.WriteLine();
        Console.WriteLine($"Order Total: ${subtotal:F2} + ${deliveryFee:F2} (delivery) = ${orderTotal:F2}");

        // Ask for payment method
        Console.Write("Proceed to payment? [Y/N]: ");
        string proceedChoice = Console.ReadLine().ToUpper();

        if (proceedChoice != "Y")
        {
            Console.WriteLine("Order cancelled.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Payment method:");
        Console.Write("[CC] Credit Card / [PP] Paypal / [CD] Cash on Delivery: ");
        string paymentMethod = Console.ReadLine().ToUpper();

        string paymentMethodFull = "";
        switch (paymentMethod)
        {
            case "CC":
                paymentMethodFull = "Credit Card";
                break;
            case "PP":
                paymentMethodFull = "Paypal";
                break;
            case "CD":
                paymentMethodFull = "Cash on Delivery";
                break;
            default:
                Console.WriteLine("Invalid payment method.");
                return;
        }

        // Generate a new order ID
        int newOrderId = allOrders.Count > 0 ? allOrders[allOrders.Count - 1].OrderId + 1 : 1001;
        DateTime deliveryDateTime = DateTime.Parse($"{deliveryDateStr} {deliveryTimeStr}");

        // Create the new order
        Order newOrder = new Order(
            newOrderId,
            DateTime.Now,
            orderTotal,
            "Pending",
            deliveryDateTime,
            deliveryAddress,
            paymentMethodFull,
            false
        );

        // Add ordered food items to the order
        foreach (var orderedItem in orderedItems)
        {
            newOrder.AddOrderedFoodItem(orderedItem);
        }

        // Update tracking dictionaries
        allOrders.Add(newOrder);
        orderCustomerEmail[newOrderId] = customerEmail;
        orderRestaurantId[newOrderId] = restaurantId;

        // Add order to customer's order list
        if (customersByEmail.ContainsKey(customerEmail))
        {
            customersByEmail[customerEmail].AddOrder(newOrder);
        }

        // Append to orders.csv
        string itemsString = "";
        foreach (var item in orderedItems)
        {
            if (itemsString.Length > 0)
                itemsString += "|";
            itemsString += $"{item.FoodItem.ItemName},{item.QtyOrdered}";
        }

        string orderLine = $"{newOrderId},{customerEmail},{restaurantId}," +
                          $"{deliveryDateTime:dd/MM/yyyy},{deliveryDateTime:HH:mm}," +
                          $"{deliveryAddress},{DateTime.Now:dd/MM/yyyy HH:mm:ss}," +
                          $"{orderTotal:F2},Pending,\"{itemsString}\"";

        File.AppendAllText("orders.csv", Environment.NewLine + orderLine);

        // Display confirmation
        Console.WriteLine();
        Console.WriteLine($"Order {newOrderId} created successfully! Status: Pending");
    }

    static void ProcessOrder()
    {
        Console.WriteLine();
        Console.WriteLine("Process Order");
        Console.WriteLine("=============");

        // Get restaurant ID
        Console.Write("Enter Restaurant ID: ");
        string restaurantId = Console.ReadLine().Trim();

        // Validatation
        if (!allRestaurants.ContainsKey(restaurantId))
        {
            Console.WriteLine("Invalid Restaurant ID.");
            return;
        }

        // Clear Queue

        orderQueue.Clear();
        foreach (Order order in allOrders)
        {
            if (orderRestaurantId.ContainsKey(order.OrderId) &&
                orderRestaurantId[order.OrderId] == restaurantId)
            {
                orderQueue.Enqueue(order);
            }
        }

        if (orderQueue.Count == 0)
        {
            Console.WriteLine($"No orders found for restaurant {restaurantId}.");
            return;
        }

        while (orderQueue.Count > 0)
        {
            Order currentOrder = orderQueue.Dequeue();

            Console.WriteLine();
            Console.WriteLine($"Order {currentOrder.OrderId}:");

            string customerEmail = orderCustomerEmail.ContainsKey(currentOrder.OrderId)
                ? orderCustomerEmail[currentOrder.OrderId] : "";
            string customerName = customersByEmail.ContainsKey(customerEmail)
                ? customersByEmail[customerEmail].CustomerName : "Unknown";

            Console.WriteLine($"Customer: {customerName}");

            Console.WriteLine("Ordered Items:");
            foreach (OrderedFoodItem item in currentOrder.OrderedFoodItem)
            {
                Console.WriteLine($"{currentOrder.OrderedFoodItem.IndexOf(item) + 1}. {item.FoodItem.ItemName} - {item.QtyOrdered}");
            }
            Console.WriteLine($"Delivery date/time: {currentOrder.DeliveryDateTime:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Total Amount: ${currentOrder.OrderTotal:F2}");
            Console.WriteLine($"Order Status: {currentOrder.OrderStatus}");
            Console.WriteLine();

            // Option
            Console.Write("[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
            string option = Console.ReadLine().ToUpper(); // in case of lower case
            Console.WriteLine();

            if (option == "C")
            {
                if (currentOrder.OrderStatus == "Pending")
                {
                    currentOrder.UpdateOrderStatus("Preparing");
                    Console.WriteLine($"Order {currentOrder.OrderId} confirmed. Status: Preparing");
                }
                else
                {
                    Console.WriteLine($"Cannot confirm order. Current status is {currentOrder.OrderStatus}. Only Pending orders can be confirmed.");
                }
            }

            else if (option == "R")
            {

                if (currentOrder.OrderStatus == "Pending")
                {
                    currentOrder.UpdateOrderStatus("Rejected");
                    refundStack.Push(currentOrder);
                    Console.WriteLine($"Order {currentOrder.OrderId} rejected. Status: Rejected");
                    Console.WriteLine($"Refund of ${currentOrder.OrderTotal:F2} will be processed for {customerName}.");
                }
                else
                {
                    Console.WriteLine($"Cannot reject order. Current status is {currentOrder.OrderStatus}. Only Pending orders can be rejected.");
                }
            }

            else if (option == "S")
            {
                if (currentOrder.OrderStatus == "Cancelled")
                {
                    Console.WriteLine($"Order {currentOrder.OrderId} skipped.");
                }
                else
                {
                    Console.WriteLine($"Skipping order {currentOrder.OrderId} (Status: {currentOrder.OrderStatus}).");
                }
            }
            else if (option == "D")
            {
                if (currentOrder.OrderStatus == "Preparing")
                {
                    currentOrder.UpdateOrderStatus("Delivered");
                    Console.WriteLine($"Order {currentOrder.OrderId} changed to delivered. Status: {currentOrder.OrderStatus}");

                }
                else
                {
                    Console.WriteLine($"Cannot deliver order. Current status is {currentOrder.OrderStatus}. Only Preparing orders can be delivered.");
                }
            }

            else
            {
                Console.WriteLine("Invalid Option, skipping order.");
            }
        }
    }
    static void ModifyOrder()
    {

        Console.WriteLine("Modify Order");
        Console.WriteLine("============");

        Console.Write("Enter Customer Email: ");
        string customerEmail = Console.ReadLine();

        if (!customersByEmail.ContainsKey(customerEmail))
        {
            Console.WriteLine("Customer not found.");
            return;
        }
        else
        {
            //Display all orders from the Order List that are "Pending"
            Customer customer = customersByEmail[customerEmail];
            List<Order> pendingOrders = customer.Orders.FindAll(o => o.OrderStatus == "Pending");
            if (pendingOrders.Count == 0)
            {
                Console.WriteLine("No pending orders found for this customer.");
                return;
            }
            Console.WriteLine("Pending Orders:");
            foreach (Order o in pendingOrders)
            {
                Console.WriteLine($"{o.OrderId}");
            }
            Console.Write("Enter Order ID: ");
            int orderId = int.Parse(Console.ReadLine());
            Order orderToModify = pendingOrders.Find(o => o.OrderId == orderId);
            if (orderToModify == null)
            {
                Console.WriteLine("Order not found or not pending.");
                return;
            }
            else
            {
                Order order = orderToModify;
                Console.WriteLine("Order Items: ");
                foreach (OrderedFoodItem item in order.OrderedFoodItem)
                {
                    Console.WriteLine($"{order.OrderedFoodItem.IndexOf(item) + 1}. {item.FoodItem.ItemName} - {item.QtyOrdered}");
                }
                Console.WriteLine("Address: ");
                Console.WriteLine(order.DeliveryAddress);
                Console.WriteLine("Delivery Date/Time: ");
                Console.WriteLine(order.DeliveryDateTime.ToString("dd/MM/yyyy, HH:mm"));

                while (true)
                {
                    Console.WriteLine();

                    Console.Write("Modify: [1] Items [2] Address [3] Delivery Time: ");
                    string choice = Console.ReadLine();

                    if (choice == "1")
                    {
                        Console.WriteLine("Modifying Items is not implemented in this version.");
                        continue;
                    }
                    else if (choice == "2")
                    {
                        Console.Write("Enter new delivery address: ");
                        string newAddress = Console.ReadLine();
                        order.UpdateDeliveryAddress(newAddress);
                        Console.WriteLine($"Order {order.OrderId} updated. New Delivery Address: {newAddress}");
                        break;
                    }
                    else if (choice == "3")
                    {
                        Console.Write("Enter new delivery time (hh:mm): ");
                        string newTime = Console.ReadLine();
                        DateTime newDateTime = DateTime.Parse($"{order.DeliveryDateTime.ToString("dd/MM/yyyy")} {newTime}");
                        order.UpdateDeliveryDateTime(newDateTime);
                        Console.WriteLine();
                        Console.WriteLine($"Order {order.OrderId} updated. New Delivery Time: {order.DeliveryDateTime.ToString("HH:mm")}");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice.");
                        continue;
                    }


                }

                Console.WriteLine();
            }
        }
    }
    // Step 8 of PRG2 Assignment
    static void DeleteOrder()
    {
        Console.WriteLine("\nDelete Order");
        Console.WriteLine("============");
        Console.Write("Enter Customer Email: ");
        string customerEmail = Console.ReadLine();

        if (!customersByEmail.ContainsKey(customerEmail))
        {
            Console.WriteLine("Customer cannot be found.");
            return;
        }

        Customer customer = customersByEmail[customerEmail];
        List<Order> pendorders = customer.Orders.FindAll(o => o.OrderStatus == "Pending");

        if (pendorders.Count == 0)
        {
            Console.WriteLine("There are no pending orders found for this customer at all.");
            return;
        }

        Console.WriteLine("Pending Orders:");
        foreach (Order o in pendorders)
        {
            Console.WriteLine(o.OrderId);
        }

        Console.Write("Enter Order ID: ");
        int orderId = Convert.ToInt32(Console.ReadLine());
        Order orderdelete = pendorders.Find(o => o.OrderId == orderId);

        if (orderdelete == null)
        {
            Console.WriteLine("Order not found or is not in pending.");
            return;
        }

        Console.WriteLine($"Customer: {customer.CustomerName}");
        Console.WriteLine("Ordered Items: ");
        foreach (OrderedFoodItem item in orderdelete.OrderedFoodItem)
        {
            Console.WriteLine($"{orderdelete.OrderedFoodItem.IndexOf(item) + 1}. {item.FoodItem.ItemName} - {item.QtyOrdered}");
        }
        Console.WriteLine($"Delivery date/time: {orderdelete.DeliveryDateTime:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"Total Amount: ${orderdelete.OrderTotal:F2}");
        Console.WriteLine($"Order Status: {orderdelete.OrderStatus}");
        Console.Write("Confirm deletion? [Y/N]: ");
        string choice = Console.ReadLine().ToUpper();

        if (choice == "Y")
        {
            orderdelete.UpdateOrderStatus("Cancelled");
            refundStack.Push(orderdelete);
            Console.WriteLine($"Order {orderdelete.OrderId} cancelled. Refund of ${orderdelete.OrderTotal:F2} processed");
        }
        else
        {
            Console.WriteLine("Deletion cancelled");
        }
    }
}
    
