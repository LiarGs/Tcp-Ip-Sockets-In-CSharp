namespace TcpIpSocketsLearn.Chapter4;

public static class MCIPAddress
{
    public static bool IsValid(string ip)
    {
        try
        {
            var octet1 = int.Parse(ip.Split(['.'], 4)[0]);
            if (octet1 >= 224 && octet1 <= 239) return true;
        }
        catch (Exception)
        {
            // ignored
        }

        return false;
    }
}