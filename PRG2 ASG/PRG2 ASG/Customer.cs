using System;

public class Customer
{
    public string EmailAddress { get; private set; }
    public string CustomerName { get; private set; }

    // List of Orders for the Customer
    public List<Order> Orders = new List<Order>();

    public Customer(string email, string name)
    {
        EmailAddress = email;
        CustomerName = name;
    }

    public Customer() { }

    public void AddOrder(Order order) { }

    public void DisplayAllOrders() { }

    public bool RemoveOrder(Order order) { return true; }

    public override string ToString()
    {
        return $"Name: {CustomerName}, Email: {EmailAddress}";
    }

}
