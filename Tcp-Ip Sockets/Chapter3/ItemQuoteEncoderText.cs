using System.Text;

namespace TcpIpSocketsLearn.Chapter3;

public class ItemQuoteEncoderText : IItemQuoteEncoder
{
    public Encoding Encoding; // Character encoding

    public ItemQuoteEncoderText() : this(ItemQuoteTextConst.DEFAULT_CHAR_ENC)
    {
    }

    public ItemQuoteEncoderText(string encodingDesc)
    {
        Encoding = Encoding.GetEncoding(encodingDesc);
    }

    public byte[] Encode(ItemQuote item)
    {
        var encodedString = item.ItemNumber + " ";

        if (item.ItemDescription.Contains('\n'))
            throw new IOException("Invalid description (contains newline)");

        encodedString = encodedString + item.ItemDescription + "\n";
        encodedString = encodedString + item.Quantity        + " ";
        encodedString = encodedString + item.UnitPrice       + " ";

        if (item.Discounted)
            encodedString += "d"; // Only include 'd' if discounted
        if (item.InStock)
            encodedString += "s"; // Only include 's' if in stock
        encodedString += "\n";

        if (encodedString.Length > ItemQuoteTextConst.MAX_WIRE_LENGTH)
            throw new IOException("Encoded length too long");

        var buf = Encoding.GetBytes(encodedString);

        return buf;
    }
}