namespace TcpIpSocketsLearn.Chapter3;

public interface IItemQuoteDecoder
{
    ItemQuote Decode(Stream source);
    ItemQuote Decode(byte[] packet);
}