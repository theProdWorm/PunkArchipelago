using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace PunkArchipelago;

public static class StringUtils
{
    [Pure]
    public static string Fill(string input, int targetLength)
    {
        char[] chars = new char[targetLength];
        for (int i = 0; i < targetLength; i++)
        {
            if (i < input.Length)
                chars[i] = input[i];
            else
                chars[i] = ' ';
        }

        return new string(chars);
    }
}