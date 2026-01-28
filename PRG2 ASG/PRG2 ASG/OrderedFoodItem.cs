//==========================================================
// Student Number : S10271067
// Student Name : Jovan Soo
// Partner Name : Matthew Tay
//==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class OrderedFoodItem
{
    public int QtyOrdered { get; private set; }
    public double SubTotal { get; private set; }
    public FoodItem Item { get; private set; }


    public OrderedFoodItem(int qtyOrdered, double subTotal)
    {
        QtyOrdered = qtyOrdered;
        SubTotal = subTotal;
    }

    public OrderedFoodItem(FoodItem item, int qty)
    {
        Item = item;
        QtyOrdered = qty;
    }

    public OrderedFoodItem() { }

    public double CalcuateSubTotal() { return 0; }
}