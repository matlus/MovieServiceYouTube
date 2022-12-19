using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace Testing.Shared.TestingHelpers
{
    [ExcludeFromCodeCoverage]
    public static class RandomStringGenerator
    {
        private static readonly int[] s_punctuationACIICodes = { 33, 35, 36, 37, 38, 40, 41, 42, 43, 44, 45, 46, 47, 58, 59, 60, 61, 62, 63, 64, 91, 93, 94, 95, 123, 124, 125, 126 };

        public static string GetRandomAciiString(int length)
        {
            const int NUMERIC = 0;
            const int LOWERCASE = 1;
            const int UPPERCASE = 2;
            const int PUNCT = 3;

            StringBuilder sb = new StringBuilder();

            //ensure at least one of each type occurs 
            sb.Append(Convert.ToChar(RandomNumberGenerator.GetInt32(97, 122))); //lowercase
            sb.Append(Convert.ToChar(RandomNumberGenerator.GetInt32(65, 90))); //uppercase
            sb.Append(Convert.ToChar(RandomNumberGenerator.GetInt32(48, 57))); //numeric
            sb.Append(Convert.ToChar(s_punctuationACIICodes[RandomNumberGenerator.GetInt32(0, s_punctuationACIICodes.Length)])); //punctuation

            char ch;
            for (int i = 0; i < length - 4; i++)
            {
                int rnd = RandomNumberGenerator.GetInt32(0, 3);
                ch = rnd switch
                {
                    LOWERCASE => Convert.ToChar(RandomNumberGenerator.GetInt32(97, 122)),
                    UPPERCASE => Convert.ToChar(RandomNumberGenerator.GetInt32(65, 90)),
                    NUMERIC => Convert.ToChar(RandomNumberGenerator.GetInt32(48, 57)),
                    PUNCT => Convert.ToChar(s_punctuationACIICodes[RandomNumberGenerator.GetInt32(0, s_punctuationACIICodes.Length)]),
                    _ => Convert.ToChar(RandomNumberGenerator.GetInt32(97, 122)),
                };
                sb.Append(ch);
            }
            return sb.ToString();
        }

        public static string GetRandomUnicodeString(int length)
        {
            length *= 2;

            byte[] str = new byte[length];

            for (int i = 0; i < length; i += 2)
            {
                int chr = RandomNumberGenerator.GetInt32(0xD7FF);
                str[i + 1] = (byte)((chr & 0xFF00) >> 8);
                str[i] = (byte)(chr & 0xFF);
            }

            return Encoding.Unicode.GetString(str);
        }

        public static int GetRandomInt(int minValue, int maxValue)
        {
            return RandomNumberGenerator.GetInt32(minValue, maxValue);
        }
    }
}