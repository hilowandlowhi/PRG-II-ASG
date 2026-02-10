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
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.Marshalling;
using System.Security.AccessControl;
using Microsoft.VisualBasic.FileIO;
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
    static Dictionary<string, SpecialOffer> allSpecialOffers = new Dictionary<string, SpecialOffer>();
    // For advanced feature (c) (Matthew Tay)
    static List<FavouriteOrder> allFavourites = new List<FavouriteOrder>();
    static Dictionary<string, List<FavouriteOrder>> customerFavourites = new Dictionary<string, List<FavouriteOrder>>();
    static void Main()
    {
        Console.WriteLine("\nWelcome to the Gruberoo Food Delivery System");

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

        // Additional Feature: Load Special Offers from CSV (Jovan Soo)
        LoadSpecialOffers("specialoffers.csv");

        Console.WriteLine($"{allSpecialOffers.Count} special offers loaded!");
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
            Console.WriteLine("7. Bulk process unprocessed orders for current day");
            Console.WriteLine("8. Display total order amount");
            Console.WriteLine("9. Manage Favourite Orders");
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
                case "7":
                    // Advanced Feature A: Bulk processing of unprocessed orders for a current day
                    // Matthew Tay
                    BulkProcessOrders();
                    break;
                case "8":
                    // Advanced Feature B: Display total order amount
                    // Jovan Soo
                    DisplayTotalOrderAmount();
                    break;
                case "9":
                    // Advanced Feature (c): Shows Customer(s) favourite orders
                    // Matthew Tay
                    ManageFavouriteOrders();
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

    //==========================================================
    // Student Number : S10273266
    // Partner Number : S10271067
    // Student Name : Matthew Tay
    // Partner Name : Jovan Soo
    //==========================================================
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
    //==========================================================
    // Student Number : S10273266
    // Partner Number : S10271067
    // Student Name : Matthew Tay
    // Partner Name : Jovan Soo
    //==========================================================
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

    // Load special offers from CSV file
    static void LoadSpecialOffers(string file)
    {
        if (!File.Exists(file)) return;

        using StreamReader sr = new StreamReader(file);
        sr.ReadLine(); // skip header

        while (!sr.EndOfStream)
        {
            string line = sr.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');

            string code = parts[1].Trim();
            string desc = parts[2].Trim();
            string discountStr = parts[3].Trim();

            double discountAmt = 0;
            string offerType = "";

            // For Special Discounts Requiring to Reduce Cost by Other Means
            if (discountStr == "-")
            {
                if (code == "DELI") offerType = "DELI";
                else if (code == "BOGO") offerType = "BOGO";
                // If Type is Neither DELI nor BOGO ignore
                else offerType = "FLAT";
            }
            else
            {
                discountAmt = double.Parse(discountStr);
                offerType = "FLAT";
            }

            // Only add first occurrence of each code (multiple restaurants share same code)
            // Additionally this is called to show number of special offers loaded with Console.WriteLine($"{allSpecialOffers.Count} special offers loaded!"); above
            if (!allSpecialOffers.ContainsKey(code))
                allSpecialOffers[code] = new SpecialOffer(code, desc, discountAmt, offerType);
        }
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

    //==========================================================
    // Student Number : S10273266
    // Partner Number : S10271067
    // Student Name : Matthew Tay
    // Partner Name : Jovan Soo
    //==========================================================
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

    // Create a new order
    static void CreateNewOrder()
    {
        Console.WriteLine();
        Console.WriteLine("Create New Order");
        Console.WriteLine("================");

        // Get customer email
        Console.Write("Enter Customer Email: ");
        string customerEmail = Console.ReadLine();

        if (!customersByEmail.ContainsKey(customerEmail))
        {
            Console.WriteLine("Customer not found.");
            return;
        }

        // For Advanced Feature (c) (Matthew Tay) Check if customer inputted has favourites
        bool hasFavourites = customerFavourites.ContainsKey(customerEmail) && customerFavourites[customerEmail].Count > 0;
        FavouriteOrder selectedFavourite = null;
        string restaurantId = "";
        string deliveryDateStr = "";
        string deliveryTimeStr = "";
        string deliveryAddress = "";
        List<OrderedFoodItem> orderedItems = new List<OrderedFoodItem>();
        double subtotal = 0;

        if (hasFavourites)
        {
            Customer customer = customersByEmail[customerEmail];
            Console.WriteLine($"Hello. {customer.CustomerName}!");
            Console.WriteLine("You have saved favourite orders.");
            

            while (true)
            {
                Console.Write("Would you like to [1] Order from Favourites or [2] Create New Order? ");
                string choice = Console.ReadLine().Trim();
                if (choice == "1")
                {
                    selectedFavourite = SelectFavourite(customerEmail);

                    if (selectedFavourite != null) // So if user chooses to order from favourite, already filled
                    {
                        
                        Console.Write("Confirm Order? (Y/N): ");
                        string result = Console.ReadLine();
                        if (result.Trim().ToUpper() == "Y")
                        {
                            restaurantId = selectedFavourite.RestaurantId;

                            orderedItems.Clear();
                            subtotal = 0;

                            foreach (FavouriteItem favItem in selectedFavourite.Items)
                            {
                                // Convert favourite item into an OrderedFoodItem
                                OrderedFoodItem ordered = new OrderedFoodItem(favItem.FoodItem, favItem.Quantity);
                                orderedItems.Add(ordered);

                                subtotal += favItem.FoodItem.ItemPrice * favItem.Quantity;
                            }
                            Console.WriteLine("Favourite Order Implemented!");
                            break;
                        }
                        else if (result.Trim().ToUpper() == "N")
                        {
                            Console.WriteLine("Order cancelled, redirecting back...");
                            
                        }
                }
                    else
                    {
                        continue;
                    }
                }
                else if (choice == "2")
                {
                    break;
                }

                else
                {
                    Console.WriteLine("Invalid Option. Please enter 1 or 2.\n");
                }
                
                
            }
        }
        if (selectedFavourite == null) 
        {
            // Get restaurant ID
            Console.Write("Enter Restaurant ID: ");
            restaurantId = Console.ReadLine().Trim();
            // Validate restaurant ID (It is only behind because if this is done in a while loop above, restuarantId will not be saved and causes errors)
            if (!allRestaurants.ContainsKey(restaurantId))
            {
                Console.WriteLine("Invalid Restaurant ID.");
                return;
            }
            // Get delivery date and time
            Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
            deliveryDateStr = Console.ReadLine();
            Console.Write("Enter Delivery Time (hh:mm): ");
            deliveryTimeStr = Console.ReadLine();

            // Get delivery address
            Console.Write("Enter Delivery Address: ");
            deliveryAddress = Console.ReadLine();

            Console.WriteLine();

            // Get the restaurant and display its menu
            Restaurant selectedRestaurant = allRestaurants[restaurantId];
            Console.WriteLine("Available Food Items:");
            Menu menu = selectedRestaurant.GetMenu();
            menu.DisplayFoodItemsNumbered();
            List<FoodItem> items = menu.FoodItems;
            Console.WriteLine();
            // Allow user to select items
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

        }
        else // This is if favourite was selected by user, to get the delivery details now
        {
            // To Validate
            while (true)
            {
                try
                {
                    Console.Write("Enter Delivery Date (dd/MM/yyyy): ");
                    deliveryDateStr = Console.ReadLine();

                    // Try to parse the date to validate format
                    DateTime testDate = DateTime.ParseExact(deliveryDateStr, "dd/MM/yyyy", null);

                    // Valid format - exit loop
                    break;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid date format! Please use dd/MM/yyyy (e.g., 10/02/2026)");
                    Console.WriteLine("Please try again.\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine("Please try again.\n");
                }
            }

            while (true)
            {
                try
                {
                    Console.Write("Enter Delivery Time (HH:mm): ");
                    deliveryTimeStr = Console.ReadLine();

                    // Try to parse the time to validate format
                    DateTime testTime = DateTime.ParseExact(deliveryTimeStr, "HH:mm", null);

                    // Valid format - exit loop
                    break;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid time format! Please use HH:mm (e.g., 14:30)");
                    Console.WriteLine("Please try again.\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine("Please try again.\n");
                }
            }

            while (true)
            {
                Console.Write("Enter Delivery Address: ");
                deliveryAddress = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(deliveryAddress))
                {
                    break; // Valid address
                }
                else
                {
                    Console.WriteLine("Address cannot be empty!");
                    Console.WriteLine("Please try again.\n");
                }
            }

            Console.WriteLine();
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

        // Ask if customer has a special offer code
        Console.Write("Enter special offer code (or press Enter to skip): ");
        string offerCode = Console.ReadLine().Trim().ToUpper();

        double discountAmount = 0;
        double deliveryFee = 5.00;
        SpecialOffer appliedOffer = null;

        if (offerCode != "")
        {
            if (allSpecialOffers.ContainsKey(offerCode))
            {
                appliedOffer = allSpecialOffers[offerCode];

                if (appliedOffer.OfferType == "DELI")
                {
                    if (subtotal >= 30)
                    {
                        deliveryFee = 0;   // Remove Delivery Fee
                        Console.WriteLine($"Offer applied: {appliedOffer.OfferDesc}");
                        Console.WriteLine("Free delivery applied!");
                    }
                    else
                    {
                        Console.WriteLine($"Offer not applicable: subtotal must be $30 or more (yours: ${subtotal:F2}).");
                        appliedOffer = null;
                    }
                }
                else
                {
                    double newSubtotal = appliedOffer.ApplyDiscount(subtotal, deliveryFee);
                    discountAmount = subtotal - newSubtotal;
                    subtotal = newSubtotal;

                    Console.WriteLine($"Offer applied: {appliedOffer.OfferDesc}");
                    Console.WriteLine($"Discount: -${discountAmount:F2}");
                }
            }
            else
            {
                Console.WriteLine("Invalid offer code. No discount applied.");
            }
        }

        // Calculate order total
        double orderTotal = subtotal + deliveryFee;

        // Display order summary
        Console.WriteLine();
        if (appliedOffer != null)
        {
            if (appliedOffer.OfferType == "DELI")
                Console.WriteLine($"Subtotal: ${subtotal:F2} | Delivery: ${deliveryFee:F2}(Free) | Total: ${orderTotal:F2}");
            else
                Console.WriteLine($"Subtotal: ${subtotal:F2} (saved ${discountAmount:F2}) | Delivery: ${deliveryFee:F2} | Total: ${orderTotal:F2}");
        }
        else
        {
            Console.WriteLine($"Subtotal: ${subtotal:F2} | Delivery: ${deliveryFee:F2} | Total: ${orderTotal:F2}");
        }

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
        Console.WriteLine($"Order {newOrderId} created successfully! Status: Pending\n");
    }

    //==========================================================
    // Student Number : S10273266
    // Partner Number : S10271067
    // Student Name : Matthew Tay
    // Partner Name : Jovan Soo
    //==========================================================
    // Process an order
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
            Console.WriteLine("Invalid Restaurant ID. Please try again.\n");
            return; // brings back to option overview cos if not cannot get out of option
        }

        // Clear Queue

        orderQueue.Clear();
        foreach (Order order in allOrders)
        {
            // Only Orders that are in pending, preparing and cancelled can be shown (no "Delivered" or "Rejected")
            if (orderRestaurantId.ContainsKey(order.OrderId) && orderRestaurantId[order.OrderId] == restaurantId && order.OrderStatus != "Delivered" && order.OrderStatus != "Rejected")
            {
                orderQueue.Enqueue(order);
            }
        }

        if (orderQueue.Count == 0)
        {
            Console.WriteLine($"No orders found for restaurant {restaurantId}.");
            return;
        }

        while (orderQueue.Count > 0) // show the information for the customer's orders
        {
            Order currentOrder = orderQueue.Dequeue();


            Console.WriteLine($"\nOrder {currentOrder.OrderId}:");

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



            bool validInp = false;

            while (!validInp)
            {
                // Option for user to choose about the order status
                Console.Write("[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
                string choice = Console.ReadLine().ToUpper(); // in case of lower case
                Console.WriteLine();

                if (choice == "C")
                {
                    if (currentOrder.OrderStatus == "Pending")
                    {
                        currentOrder.UpdateOrderStatus("Preparing");
                        Console.WriteLine($"Order {currentOrder.OrderId} confirmed. Status: Preparing");
                        validInp = true;
                    }
                    else
                    {
                        Console.WriteLine($"Cannot confirm order. Current status is {currentOrder.OrderStatus}. Only Pending orders can be confirmed.\n");

                    }
                }
                else if (choice == "R")
                {
                    if (currentOrder.OrderStatus == "Pending")
                    {
                        currentOrder.UpdateOrderStatus("Rejected");
                        refundStack.Push(currentOrder);
                        Console.WriteLine($"Order {currentOrder.OrderId} rejected. Status: Rejected");
                        Console.WriteLine($"Refund of ${currentOrder.OrderTotal:F2} will be processed for {customerName}.");
                        validInp = true;
                    }
                    else
                    {
                        Console.WriteLine($"Cannot reject order. Current status is {currentOrder.OrderStatus}. Only Pending orders can be rejected.\n");

                    }
                }
                else if (choice == "S")
                {
                    if (currentOrder.OrderStatus == "Cancelled")
                    {
                        Console.WriteLine($"Order {currentOrder.OrderId} skipped.");
                        validInp = true;
                    }
                    else
                    {
                        Console.WriteLine($"Cannot skip order. Current status is {currentOrder.OrderStatus}. Only Cancelled orders can be skipped.\n");

                    }
                }
                else if (choice == "D")
                {
                    if (currentOrder.OrderStatus == "Preparing")
                    {
                        currentOrder.UpdateOrderStatus("Delivered");
                        Console.WriteLine($"Order {currentOrder.OrderId} changed to delivered. Status: {currentOrder.OrderStatus}");
                        validInp = true;
                    }
                    else
                    {
                        Console.WriteLine($"Cannot deliver order. Current status is {currentOrder.OrderStatus}. Only Preparing orders can be delivered.\n");

                    }
                }
                else
                {
                    Console.WriteLine("Invalid option. Please try again.");

                }
            }
        }
    }
    // Modify an existing order
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
                // Display current order details
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
                        // Show current items
                        Console.WriteLine("Current Items:");
                        foreach (OrderedFoodItem item in order.OrderedFoodItem)
                        {
                            int number = order.OrderedFoodItem.IndexOf(item) + 1;
                            Console.WriteLine($"{number}. {item.FoodItem.ItemName} x{item.QtyOrdered}");
                        }

                        Console.WriteLine();
                        Console.WriteLine("What would you like to do?");
                        Console.WriteLine("[A] Add item  [R] Remove item  [Q] Change quantity");
                        string itemChoice = Console.ReadLine().ToUpper();

                        if (itemChoice == "A")
                        {
                            // Get the restaurant's menu for this order
                            string restId = orderRestaurantId[order.OrderId];
                            Menu menu = allRestaurants[restId].GetMenu();
                            menu.DisplayFoodItemsNumbered();
                            List<FoodItem> items = menu.FoodItems;

                            Console.Write("Enter item number to add: ");
                            int itemNum = int.Parse(Console.ReadLine());

                            Console.Write("Enter quantity: ");
                            int qty = int.Parse(Console.ReadLine());

                            FoodItem selected = items[itemNum - 1];
                            OrderedFoodItem newItem = new OrderedFoodItem(selected, qty);
                            order.AddOrderedFoodItem(newItem);

                            Console.WriteLine($"Added {selected.ItemName} x{qty} to order.");
                        }
                        else if (itemChoice == "R")
                        {
                            Console.Write("Enter item number to remove: ");
                            int removeIdx = int.Parse(Console.ReadLine()) - 1;

                            if (removeIdx >= 0 && removeIdx < order.OrderedFoodItem.Count)
                            {
                                OrderedFoodItem toRemove = order.OrderedFoodItem[removeIdx];
                                order.OrderedFoodItem.Remove(toRemove);
                                Console.WriteLine($"Removed {toRemove.FoodItem.ItemName} from order.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid item number.");
                            }
                        }
                        else if (itemChoice == "Q")
                        {
                            Console.Write("Enter item number to update: ");
                            int updateIdx = int.Parse(Console.ReadLine()) - 1;

                            if (updateIdx >= 0 && updateIdx < order.OrderedFoodItem.Count)
                            {
                                Console.Write("Enter new quantity: ");
                                int newQty = int.Parse(Console.ReadLine());

                                OrderedFoodItem old = order.OrderedFoodItem[updateIdx];
                                OrderedFoodItem updated = new OrderedFoodItem(old.FoodItem, newQty);
                                order.OrderedFoodItem[updateIdx] = updated;

                                Console.WriteLine($"Updated {old.FoodItem.ItemName} to x{newQty}.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice.");
                            continue;
                        }
                        break;
                    }

                    else if (choice == "2")
                    {
                        // Update delivery address
                        Console.Write("Enter new delivery address: ");
                        string newAddress = Console.ReadLine();
                        order.UpdateDeliveryAddress(newAddress);
                        Console.WriteLine($"Order {order.OrderId} updated. New Delivery Address: {newAddress}");
                        break;
                    }
                    else if (choice == "3")
                    {
                        // Update delivery time
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

    //==========================================================
    // Student Number : S10273266
    // Partner Number : S10271067
    // Student Name : Matthew Tay
    // Partner Name : Jovan Soo
    //==========================================================
    // Step 8 of PRG2 Assignment
    static void DeleteOrder()
    {
        Console.WriteLine("\nDelete Order");
        Console.WriteLine("============");
        Console.Write("Enter Customer Email: ");
        string customerEmail = Console.ReadLine();

        // Validattion
        if (!customersByEmail.ContainsKey(customerEmail))
        {
            Console.WriteLine("Customer cannot be found.");
            return;
        }

        Customer customer = customersByEmail[customerEmail];
        List<Order> pendorders = customer.Orders.FindAll(o => o.OrderStatus == "Pending"); // Finding the orders that have status pending

        if (pendorders.Count == 0) // for customers with no pending orders
        {
            Console.WriteLine("There are no pending orders found for this customer at all.");
            return;
        }


        Console.WriteLine("Pending Orders:");
        foreach (Order o in pendorders)
        {
            Console.WriteLine(o.OrderId);
        }

        Order orderdelete = null;

        while (true)
        {
            Console.Write("Enter Order ID (or 0 to cancel): ");
            string input = Console.ReadLine().Trim();

            // Validate integer
            if (!int.TryParse(input, out int orderId))
            {
                Console.WriteLine("Invalid input. Please enter a number.\n");
                continue;
            }

            // Allow cancel
            if (orderId == 0)
            {
                Console.WriteLine("Deletion cancelled.\n");
                return;
            }

            // Validate order exists in pending list
            orderdelete = pendorders.Find(o => o.OrderId == orderId);
            if (orderdelete == null)
            {
                Console.WriteLine("Order not found in pending orders. Please try again.\n");
                continue;
            }

            // Valid order found
            break;
        }



        Console.WriteLine($"\nCustomer: {customer.CustomerName}");
        Console.WriteLine("Ordered Items: ");
        foreach (OrderedFoodItem item in orderdelete.OrderedFoodItem)
        {
            Console.WriteLine($"{orderdelete.OrderedFoodItem.IndexOf(item) + 1}. {item.FoodItem.ItemName} - {item.QtyOrdered}");
        }
        Console.WriteLine($"Delivery date/time: {orderdelete.DeliveryDateTime:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"Total Amount: ${orderdelete.OrderTotal:F2}");
        Console.WriteLine($"Order Status: {orderdelete.OrderStatus}");

        while (true)
        // Validation for user, if user doesnt input correct value, reprompt till value accepted
        {
            Console.Write("Confirm deletion? [Y/N]: ");
            string choice = Console.ReadLine().ToUpper();

            if (choice == "Y")
            {
                orderdelete.UpdateOrderStatus("Cancelled");
                refundStack.Push(orderdelete);
                Console.WriteLine($"\nOrder {orderdelete.OrderId} cancelled. Refund of ${orderdelete.OrderTotal:F2} processed");
                break;
            }
            else if (choice == "N")
            {
                Console.WriteLine("\nDeletion cancelled");
                break;
            }
            else
            {
                Console.WriteLine("\nInvalid input. Please try again.");
            }
        }

    }

    // Advanced Feature (a): Bulk processing of unprocessed orders for a current day (Matthew Tay)

    static void BulkProcessOrders()
    {
        Console.WriteLine("Bulk processing of unprocessed orders for the current day\n");

        DateTime today = DateTime.Now.Date;

        List<Order> pendOrders = new List<Order>();


        foreach (Order order in allOrders)
        {
            // Check if status is pending 
            if (order.OrderStatus == "Pending")
            {
                pendOrders.Add(order);
            }
        }
        // To display the total number in the Order Queues with pending status
        Console.WriteLine($"Total number of pending orders: {pendOrders.Count}\n");

        if (pendOrders.Count == 0)
        {
            Console.WriteLine("No pending orders to process for today.\n ");
            return;
        }

        // To add all pending orders to queue
        orderQueue.Clear();
        foreach (Order order in pendOrders)
        {
            orderQueue.Enqueue(order);
        }

        int ordersProcessed = 0;
        int preparingCount = 0;
        int rejectedCount = 0;

        // To process each order that is in the queue
        while (orderQueue.Count > 0)
        {
            Order currentOrder = orderQueue.Dequeue();

            // To get the customer information
            string customerEmail = orderCustomerEmail.ContainsKey(currentOrder.OrderId)
            ? orderCustomerEmail[currentOrder.OrderId] : "";
            string customerName = customersByEmail.ContainsKey(customerEmail)
                ? customersByEmail[customerEmail].CustomerName : "Unknown";

            // below to calculate the time diff in hours from delivery time with order time
            double totalMinutes = (currentOrder.DeliveryDateTime - currentOrder.OrderDateTime).TotalMinutes; // Total Minutes calculates all of the components(Days,Hours...) into a minute count
            double hourstilDelivery = totalMinutes / 60.0;


            if (hourstilDelivery < 1)
            {
                // Set to reject if delivery time less that one hour
                currentOrder.UpdateOrderStatus("Rejected");
                refundStack.Push(currentOrder);
                rejectedCount++;

                Console.WriteLine($"Order {currentOrder.OrderId} - Customer: {customerName}"); // shows which customer made what order that will be rejected
                Console.WriteLine($"Delivery time: {currentOrder.DeliveryDateTime:dd/MM/yyyy HH:mm}"); // shows the delivery time
                Console.WriteLine($"Order time: {currentOrder.OrderDateTime}");
                Console.WriteLine($"Status changed to: Rejected (Delivery time less than 1 hour)"); // shows proof of changing status to rejected for user
                Console.WriteLine($"Refund of ${currentOrder.OrderTotal:F2} will be processed.\n"); // Shows the refund feedback

            }

            else
            {
                // If delivery time not less than one hour, set to preparing
                currentOrder.UpdateOrderStatus("Preparing");
                preparingCount++;
                Console.WriteLine($"Order {currentOrder.OrderId} - Customer: {customerName}"); // shows which customer made what order that will be set to preparing
                Console.WriteLine($"Delivery time: {currentOrder.DeliveryDateTime:dd/MM/yyyy HH:mm}"); // shows order's delivery time
                Console.WriteLine($"Order time: {currentOrder.OrderDateTime}");
                Console.WriteLine($"Status changed to: Preparing\n"); // shows proof of stauts change for user to see
            }
            ordersProcessed++;

        }
        // Summary Statistics
        Console.WriteLine("Summary of the Bulk Processing\n");
        Console.WriteLine($"Number of orders processed: {ordersProcessed}");
        Console.WriteLine($"Number of \"Preparing\" orders: {preparingCount}");
        Console.WriteLine($"Number of \"Rejected\" orders: {rejectedCount}");

        // Calculate the percentage of automatically processed orders against all orders
        double percentageProcess = 0;
        if (pendOrders.Count > 0)
        {
            percentageProcess = ((double)ordersProcessed / allOrders.Count) * 100;
        }

        Console.WriteLine($"Percentage of automatically processed orders: {percentageProcess:F2}% \n");
    }

    static void DisplayTotalOrderAmount()
    {
        Console.WriteLine("Display Total Order Amount");
        Console.WriteLine("=========================");

        double grandTotalOrderAmount = 0;
        double grandTotalRefunds = 0;
        // Iterate through each order to calculate totals
        foreach (var order in allOrders)
        {
            double restaurantTotalOrderAmount = 0;
            double restaurantTotalRefunds = 0;

            // Get restaurant ID for the order
            string restaurantId = orderRestaurantId.ContainsKey(order.OrderId) ? orderRestaurantId[order.OrderId] : "";
            if (allRestaurants.ContainsKey(restaurantId))
            {
                Restaurant restaurant = allRestaurants[restaurantId];
                double deliveryFee = 5.00;
                // Successful orders
                if (order.OrderStatus == "Delivered")
                {
                    double orderAmount = order.OrderTotal - deliveryFee;
                    restaurantTotalOrderAmount += orderAmount;
                }
                // Refunded orders
                if (order.OrderStatus == "Rejected" || order.OrderStatus == "Cancelled")
                {
                    double refundAmount = order.OrderTotal - deliveryFee;
                    grandTotalRefunds += refundAmount;
                }

                Console.WriteLine($"Restaurant: {allRestaurants[restaurantId].RestaurantName}");
                Console.WriteLine($"  Total Order Amount: ${restaurantTotalOrderAmount:F2}");
                Console.WriteLine($"  Total Refunds: ${restaurantTotalRefunds:F2}");
                Console.WriteLine("");

                grandTotalOrderAmount += restaurantTotalOrderAmount;
                grandTotalRefunds += restaurantTotalRefunds;
            }
        }
        double finalAmount = grandTotalOrderAmount - grandTotalRefunds;

        // Overall Summary
        Console.WriteLine("Overall Summary");
        Console.WriteLine("================");
        Console.WriteLine($"Total Order Amount: ${grandTotalOrderAmount:F2}");
        Console.WriteLine($"Total Refunds: ${grandTotalRefunds:F2}");
        Console.WriteLine($"Final Amount Gruberoo earns: ${finalAmount:F2}");
        Console.WriteLine();
    }

    //==========================================================
    // Student Number : S10273266
    // Partner Number : S10271067
    // Student Name : Matthew Tay
    // Partner Name : Jovan Soo
    //==========================================================
    // Advanced Feature (c): Showing customers favourite orders, showing the details, items and price.

    static void ManageFavouriteOrders()
    {
        while (true)
        {
            Console.WriteLine("\nManage Favourite Orders");
            Console.WriteLine("=======================");
            Console.WriteLine("[1] View My Favourites");
            Console.WriteLine("[2] Create New Favourite");
            Console.WriteLine("[3] Delete Favourite");
            Console.WriteLine("[0] Back to Main Menu");
            Console.Write("Enter option: ");

            string option = Console.ReadLine()?.Trim();
            Console.WriteLine();

            switch (option)
            {
                case "1":
                    ViewCustomerFavourites();
                    break;

                case "2":
                    CreateFavouriteOrder();
                    break;

                case "3":
                    DeleteFavouriteOrder();
                    break;
                case "0":
                    return;

                default:
                    Console.WriteLine("Invalid option. Please enter 0, 1, 2, or 3.");
                    continue; // reprompt instead of exiting
            }
        }
    }

    // Viewing Customer Favourites
    static void ViewCustomerFavourites()
    {
        Console.WriteLine("\nView My Favourites\n");
        Console.Write("Enter your email: ");
        string email = Console.ReadLine().Trim().ToLower();

        if (!customersByEmail.ContainsKey(email))
        {
            Console.WriteLine("Customer not found.");
            return;
        }
        
        // Validate if customer has a favourite order
        if (!customerFavourites.ContainsKey(email) || customerFavourites[email].Count == 0)
        {
            Console.WriteLine("You have no favorite orders saved.");
            return;
        }

        Console.WriteLine($"\nShowing Favourite Orders for {customersByEmail[email].CustomerName}:");
        int index = 1;

        foreach(FavouriteOrder fav in customerFavourites[email])
        {
            Console.WriteLine($"\n[{index}] {fav.FavouriteName}");
            Console.WriteLine($"Restaurant: {allRestaurants[fav.RestaurantId].RestaurantName}");
            Console.WriteLine($"Items: {fav.Items.Count}");
            Console.WriteLine($"Total: ${fav.GetTotalPrice():F2}");
            Console.WriteLine($"Created: {fav.CreatedDate:dd/MM/yyyy}");
            index++;
        }
    }

    // Creating A Customer's Favourite Order

    static void CreateFavouriteOrder()
    {
        Console.WriteLine("\nCreating A New Favorite Order\n");

        Console.Write("Enter your email: ");
        string email = Console.ReadLine().Trim().ToLower();

        if (!customersByEmail.ContainsKey(email))
        {
            Console.WriteLine("Customer not found.");
            return;
        }

        Customer customer = customersByEmail[email];

        // Display all stalls
        Console.WriteLine("\nAvailable Restaurants:");
        foreach (var kvp in allRestaurants)
        {
            Console.WriteLine($"{kvp.Key} - {kvp.Value.RestaurantName}");
        }

        string restaurantId = "";
        
        while (true)
        {

            // Get restaurant for food item
            Console.Write("Enter Restaurant ID: ");
            restaurantId = Console.ReadLine().Trim();


            if (!allRestaurants.ContainsKey(restaurantId))
            {
                Console.WriteLine("Invalid Restaurant ID.");
                return;
            }

            else
            {
                break;
            }
        }
        Restaurant restaurant = allRestaurants[restaurantId];
        Console.WriteLine($"\nRestaurant: {restaurant.RestaurantName}");

        // Display the Menu
        Console.WriteLine("\nAvailable Items:");
        restaurant.GetMenu().DisplayFoodItemsNumbered();
        List<FoodItem> items = restaurant.GetMenu().FoodItems;

        // Creating the favourite ID
        string favouriteId = "FAV" + (allFavourites.Count + 1).ToString("D3");

        Console.Write("\nEnter a name for this favourite order: ");
        string favoriteName = Console.ReadLine().Trim();

        if (string.IsNullOrEmpty(favoriteName))
        {
            favoriteName = "Favorite " + (allFavourites.Count + 1);
        }

        FavouriteOrder favourite = new FavouriteOrder(favouriteId, email, favoriteName, restaurantId);
        
        // To add items for favourite order
        while (true)
        {
            Console.Write("\nEnter item number (or 0 to finish): ");
            if (!int.TryParse(Console.ReadLine(), out int itemChoice) || itemChoice < 0 || itemChoice > items.Count)
            {
                Console.WriteLine("Invalid choice. Please try again.");
                continue;
            }

            if (itemChoice == 0)
            {
                break;
            }

            FoodItem selectedItem = items[itemChoice - 1];

            // Prompt user to enter quantity
            Console.Write($"Enter quantity for {selectedItem.ItemName}: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
            {
                Console.WriteLine("Invalid quantity. Please try again.");
                continue;
            }
            Console.Write("Any customisations? (press Enter to skip): ");
            string customisations = Console.ReadLine().Trim();

            FavouriteItem favouriteItem = new FavouriteItem(selectedItem, quantity, customisations);
            favourite.Items.Add(favouriteItem);
            Console.WriteLine($"Added {quantity} x {selectedItem.ItemName}");

        }
        // If user decided not to make new favourite
        if (favourite.Items.Count == 0)
        {
            Console.WriteLine("No items added. Favoruite not created.");
            return;
        }

        allFavourites.Add(favourite);

        if (!customerFavourites.ContainsKey(email))
        {
            customerFavourites[email] = new List<FavouriteOrder>();
        }
        customerFavourites[email].Add(favourite);

        Console.WriteLine("\nFavourite order has been created successfully!");
        favourite.DisplayFavouriteDetails();
    }

    static void DeleteFavouriteOrder()
    {
        Console.WriteLine("\nDelete Favourite Order");

        Console.Write("Enter your email: ");
        string email = Console.ReadLine().Trim().ToLower();

        if (!customersByEmail.ContainsKey(email))
        {
            Console.WriteLine("Customer not found.");
            return;
        }

        if (!customerFavourites.ContainsKey(email) || customerFavourites[email].Count == 0)
        {
            Console.WriteLine("You have no favourite orders to delete.");
            return;
        }

        // Display customers favourite(if any)
        Console.WriteLine($"\nYour Favourite Orders:");
        for (int i = 0; i < customerFavourites[email].Count; i++)
        {
            FavouriteOrder fav = customerFavourites[email][i];
            Console.WriteLine($"{i + 1}. {fav.FavouriteName} ({fav.Items.Count} items, ${fav.GetTotalPrice():F2})");
        }
        Console.Write("\nEnter the number of the favourite to delete (or 0 to cancel): ");
        if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > customerFavourites[email].Count)
        {
            Console.WriteLine("Invalid choice.");
            return;
        }
        if (choice == 0)
        {
            Console.WriteLine("Deletion cancelled.");
            return;
        }

        FavouriteOrder toDelete = customerFavourites[email][choice - 1];
        customerFavourites[email].RemoveAt(choice - 1);
        allFavourites.Remove(toDelete);
        Console.WriteLine($"Favourite '{toDelete.FavouriteName}' deleted successfully.");
    }

    // To Order from favourite saved (used in CreateNewOrder() )

    static FavouriteOrder SelectFavourite(string email)
    {   
        while (true)
        {
            Console.WriteLine("\nOrder from Favourite\n");
            Console.WriteLine("Your Favourite Orders:");

            for (int i = 0; i < customerFavourites[email].Count; i++)
            {
                FavouriteOrder fav = customerFavourites[email][i];
                Console.WriteLine($"\n[{i + 1}] {fav.FavouriteName}");
                Console.WriteLine($"Restaurant: {allRestaurants[fav.RestaurantId].RestaurantName}");
                Console.WriteLine($"Items: {fav.Items.Count}");
                Console.WriteLine($"Total: ${fav.GetTotalPrice():F2}");
            }

            Console.Write("\nSelect favourite number (or 0 to cancel): ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid choice. Please enter a number.\n");
                continue;
            }

            if (choice == 0)
            {
                Console.WriteLine("Order cancelled.\n");
                return null;
            }

            if (choice < 1 || choice > customerFavourites[email].Count)
            {
                Console.WriteLine("Invalid choice. Please select from the list.\n");
                continue;
            }

            FavouriteOrder selectedFavourite = customerFavourites[email][choice - 1];

            // show selected favourite (so user can confirm it correct)
            Console.WriteLine($"\nUsing favourite: {selectedFavourite.FavouriteName}");
            Console.WriteLine($"Restaurant: {allRestaurants[selectedFavourite.RestaurantId].RestaurantName}");
            Console.WriteLine("\nItems in your favourite:");
            foreach (var fi in selectedFavourite.Items)
            {
                Console.WriteLine($"{fi.FoodItem.ItemName} x {fi.Quantity}");
            }

            return selectedFavourite;
        }
    }



}