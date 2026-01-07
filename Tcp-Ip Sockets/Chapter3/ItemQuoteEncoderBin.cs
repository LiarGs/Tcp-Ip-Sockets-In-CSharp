using System.Net;
using System.Text;

namespace TcpIpSocketsLearn.Chapter3;

public class ItemQuoteEncoderBin : IItemQuoteEncoder
{
    public Encoding Encoding; // Character encoding

    public ItemQuoteEncoderBin() : this(ItemQuoteBinConst.DEFAULT_CHAR_ENC)
    {
    }

    public ItemQuoteEncoderBin(string encodingDesc)
    {
        Encoding = Encoding.GetEncoding(encodingDesc);
    }

    public byte[] Encode(ItemQuote item)
    {
        var mem    = new MemoryStream();
        var output = new BinaryWriter(new BufferedStream(mem));

        output.Write(IPAddress.HostToNetworkOrder(item.ItemNumber));
        output.Write(IPAddress.HostToNetworkOrder(item.Quantity));
        output.Write(IPAddress.HostToNetworkOrder(item.UnitPrice));

        byte flags = 0;
        if (item.Discounted)
            flags |= ItemQuoteBinConst.DISCOUNT_FLAG;
        if (item.InStock)
            flags |= ItemQuoteBinConst.IN_STOCK_FLAG;
        output.Write(flags);

        var encodedDesc = Encoding.GetBytes(item.ItemDescription);
        if (encodedDesc.Length > ItemQuoteBinConst.MAX_DESC_LEN)
            throw new IOException("Item Description exceeds encoded length limit");
        output.Write((byte)encodedDesc.Length);
        output.Write(encodedDesc);

        output.Flush();

        return mem.ToArray();
    }
}