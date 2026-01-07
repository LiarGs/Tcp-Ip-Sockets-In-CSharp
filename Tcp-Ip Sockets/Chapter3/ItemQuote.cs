namespace TcpIpSocketsLearn.Chapter3;

public class ItemQuote
{
    public bool   Discounted;      // Price reflect a discount?
    public bool   InStock;         // Item(s) ready to ship?
    public string ItemDescription; // String description of item
    public long   ItemNumber;      // Item identification number
    public int    Quantity;        // Number of items in quote (always >= 1)
    public int    UnitPrice;       // Price (in cents) per item

    public ItemQuote(long itemNumber, string itemDescription,
        int               quantity,   int    unitPrice, bool discounted, bool inStock)
    {
        ItemNumber      = itemNumber;
        ItemDescription = itemDescription;
        Quantity        = quantity;
        UnitPrice       = unitPrice;
        Discounted      = discounted;
        InStock         = inStock;
    }

    public override string ToString()
    {
        var EOLN = "\n";
        var value = "Item# = "        + ItemNumber      + EOLN +
                    "Description = "  + ItemDescription + EOLN +
                    "Quantity = "     + Quantity        + EOLN +
                    "Price (each) = " + UnitPrice       + EOLN +
                    "Total Price = "  + Quantity * UnitPrice;

        if (Discounted)
            value += " (discounted)";
        if (InStock)
            value += EOLN + "In Stock" + EOLN;
        else
            value += EOLN + "Out of Stock" + EOLN;

        return value;
    }
}