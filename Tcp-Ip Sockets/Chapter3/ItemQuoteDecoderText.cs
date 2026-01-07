using System.Text;

namespace TcpIpSocketsLearn.Chapter3;

public class ItemQuoteDecoderText : IItemQuoteDecoder
{
    public Encoding Encoding; // Character encoding

    public ItemQuoteDecoderText() : this(ItemQuoteTextConst.DEFAULT_CHAR_ENC)
    {
    }

    public ItemQuoteDecoderText(string encodingDesc)
    {
        Encoding = Encoding.GetEncoding(encodingDesc);
    }

    public ItemQuote Decode(Stream wire)
    {
        var space   = Encoding.GetBytes(" ");
        var newline = Encoding.GetBytes("\n");

        var itemNo      = Encoding.GetString(Framer.NextToken(wire, space));
        var description = Encoding.GetString(Framer.NextToken(wire, newline));
        var quant       = Encoding.GetString(Framer.NextToken(wire, space));
        var price       = Encoding.GetString(Framer.NextToken(wire, space));
        var flags       = Encoding.GetString(Framer.NextToken(wire, newline));

        return new ItemQuote(long.Parse(itemNo),
            description,
            int.Parse(quant),
            int.Parse(price),
            flags.Contains('d'),
            flags.Contains('s'));
    }

    public ItemQuote Decode(byte[] packet)
    {
        Stream payload = new MemoryStream(packet, 0, packet.Length, false);
        return Decode(payload);
    }
}