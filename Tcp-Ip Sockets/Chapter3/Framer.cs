namespace TcpIpSocketsLearn.Chapter3;

public static class Framer
{
    public static byte[]? NextToken(Stream input, byte[] delimiter)
    {
        int nextByte;

        // If the stream has already ended, return null
        if ((nextByte = input.ReadByte()) == -1)
            return null;

        var tokenBuffer = new MemoryStream();
        do
        {
            tokenBuffer.WriteByte((byte)nextByte);
            var currentToken = tokenBuffer.ToArray();
            if (EndsWith(currentToken, delimiter))
            {
                var tokenLength = currentToken.Length - delimiter.Length;
                var token       = new byte[tokenLength];
                Array.Copy(currentToken, 0, token, 0, tokenLength);
                return token;
            }
        } while ((nextByte = input.ReadByte()) != -1); // Stop on EOS

        return tokenBuffer.ToArray(); // Received at least one byte
    }

    // Returns true if value ends with the bytes in the suffix
    private static bool EndsWith(byte[] value, byte[] suffix)
    {
        if (value.Length < suffix.Length)
            return false;

        for (var offset = 1; offset <= suffix.Length; offset++)
            if (value[^offset] != suffix[^offset])
                return false;

        return true;
    }
}