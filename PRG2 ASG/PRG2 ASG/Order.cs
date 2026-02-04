//==========================================================
// Student Number : S10271067
// Student Name : Jovan Soo
// Partner Name : Matthew Tay
//==========================================================

using System;
using System.Net.Mail;


public class Order
{
    public int OrderId { get; private set; }
    public DateTime OrderDateTime { get; private set; } 
    public double OrderTotal { get; private set; }
    public string OrderStatus { get; private set; }
    public DateTime DeliveryDateTime { get; private set; }
    public string DeliveryAddress { get; private set; }
    public string OrderPaymentMethod { get; private set; }
    public bool OrderPaid { get; private set; }

    // List of OrderedFoodItems in the Order
    public List<OrderedFoodItem> OrderedFoodItem = new List<OrderedFoodItem>();

    public Order(int orderId, DateTime orderDateTime, double orderTotal, string orderStatus,
                    DateTime deliveryDateTime, string deliveryAddress,
                    string orderPaymentMethod, bool orderPaid)
    {
        OrderId = orderId;
        OrderDateTime = orderDateTime;
        OrderTotal = orderTotal;
        OrderStatus = orderStatus;
        DeliveryDateTime = deliveryDateTime;
        DeliveryAddress = deliveryAddress;
        OrderPaymentMethod = orderPaymentMethod;
        OrderPaid = orderPaid;
    }

    public Order() { }

    public double CalculateTax(double taxRate)
    {
        return OrderTotal * taxRate;
    }

    public void AddOrderedFoodItem(OrderedFoodItem orderedfoodItem) 
    { 
        OrderedFoodItem.Add(orderedfoodItem);
    }

    public bool RemoveOrderedFoodItem(OrderedFoodItem orderedfoodItem) { return true; }

    // For Step 6 needs to add
    public void DisplayOrderedFoodItems()
    {
        foreach (var item in OrderedFoodItem)
        {
            Console.WriteLine($" - {item.FoodItem.ItemName} x {item.QtyOrdered}");
        }
    }

    public void UpdateOrderStatus(string newStatus)
    {
        OrderStatus = newStatus;
    }


    public override string ToString()
    {
        return $"Order ID: {OrderId}, Date: {OrderDateTime}, Total: {OrderTotal}, Status: {OrderStatus}, " +
                $"Delivery Date: {DeliveryDateTime}, Address: {DeliveryAddress}, " +
                $"Payment Method: {OrderPaymentMethod}, Paid: {OrderPaid}";
    }
}
