using UnityEngine;

namespace Bserg.Controller.Util
{
    public static class PrettyPrint
    {
        static readonly string[] postfix = { "", "K", "M", "B", "T" };
        public static string DecimalThousandsFormat (long number, string format = "")
        {
            // Limit to thousands
            int postfixIndex = 0;
            long divisor = 1, nextDivisor = 1000;

            while (Mathf.Abs(number) > nextDivisor && postfixIndex < postfix.Length)
            {
                divisor = nextDivisor;
                nextDivisor *= 1000;
                postfixIndex++;
            }

            float numberLessThanThousand = number / (float)divisor;
            
            if (format == "")
                format = numberLessThanThousand >= 100 ? "0" : numberLessThanThousand >= 10 ? ".0" : ".00";
            
            return numberLessThanThousand.ToString(format) + " " + postfix[postfixIndex];
        }
    }

}
