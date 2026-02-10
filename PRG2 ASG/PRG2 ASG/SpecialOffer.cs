// See https://aka.ms/new-console-template for more information
//==========================================================
// Student Number : S10273266D
// Student Name : Matthew Tay
// Partner Name : Jovan Soo
//==========================================================

public class SpecialOffer
{
    public string OfferCode { get; private set; }
    public string OfferDesc { get; private set; }
    public double Discount { get; private set; }
    public string OfferType { get; private set; } //Added To show type of Offer

    public SpecialOffer(string offerCode, string offerDesc, double discount, string offerType)
    {
        OfferCode = offerCode;
        OfferDesc = offerDesc;
        Discount = discount;
        OfferType = offerType;
    }

    // Checks what Kind of Offer from OfferType to Either do flat price, Free delivery only if order over $30 or Buy One get One Free
    public double ApplyDiscount(double subtotal, double deliveryFee)
    {
        if (OfferType == "FLAT")
            return subtotal - Discount;   

        if (OfferType == "BOGO")
            return subtotal / 2;

        // Free Delivery Offer skipped here as delivery fee is handled in Program.cs
        // For other Offer Types
        return subtotal;
    }

    public override string ToString()
    {
        return $"{OfferCode} - {OfferDesc}";
    }
}