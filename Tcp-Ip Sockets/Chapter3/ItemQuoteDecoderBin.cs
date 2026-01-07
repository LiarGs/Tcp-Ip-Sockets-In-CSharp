using System.Net;
using System.Text;

namespace TcpIpSocketsLearn.Chapter3;

public class ItemQuoteDecoderBin : IItemQuoteDecoder
{
    public Encoding Encoding; // Character encoding

    public ItemQuoteDecoderBin() : this(ItemQuoteTextConst.DEFAULT_CHAR_ENC)
    {
    }

    public ItemQuoteDecoderBin(string encodingDesc)
    {
        Encoding = Encoding.GetEncoding(encodingDesc);
    }

    public ItemQuote Decode(Stream wire)
    {
        var src = new BinaryReader(new BufferedStream(wire));

        var itemNumber = IPAddress.NetworkToHostOrder(src.ReadInt64());
        var quantity   = IPAddress.NetworkToHostOrder(src.ReadInt32());
        var unitPrice  = IPAddress.NetworkToHostOrder(src.ReadInt32());
        var flags      = src.ReadByte();
        var discounted = (flags & ItemQuoteBinConst.DISCOUNT_FLAG) == ItemQuoteBinConst.DISCOUNT_FLAG;
        var inStock    = (flags & ItemQuoteBinConst.IN_STOCK_FLAG) == ItemQuoteBinConst.IN_STOCK_FLAG;

        var stringLength = src.Read(); // Returns an unsigned byte as an int
        if (stringLength == -1)
            throw new EndOfStreamException();
        var stringBuf = new byte[stringLength];
        src.Read(stringBuf, 0, stringLength);
        var itemDesc = Encoding.GetString(stringBuf);

        return new ItemQuote(itemNumber, itemDesc, quantity, unitPrice, discounted, inStock);
    }

    public ItemQuote Decode(byte[] packet)
    {
        Stream payload = new MemoryStream(packet, 0, packet.Length, false);
        return Decode(payload);
    }
}