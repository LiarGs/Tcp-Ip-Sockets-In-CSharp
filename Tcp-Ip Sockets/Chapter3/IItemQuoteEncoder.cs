namespace TcpIpSocketsLearn.Chapter3;

public interface IItemQuoteEncoder
{
    byte[] Encode(ItemQuote item);
}