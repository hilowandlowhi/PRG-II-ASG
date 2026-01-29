// See https://aka.ms/new-console-template for more information
//==========================================================
// Student Number : S10273266D
// Student Name : Matthew Tay
// Partner Name : Jovan Soo
//==========================================================

public class SpecialOffer
{
    private string OfferCode;
    private string OfferDesc;
    private double Discount;

    public SpecialOffer(string offerCode, string offerDesc, double discount)
    {
        OfferCode = offerCode;
        OfferDesc = offerDesc;
        Discount = discount;
    }


    public override string ToString()
    {
        return $"{OfferCode} - {OfferDesc} ({Discount})";
    }

}