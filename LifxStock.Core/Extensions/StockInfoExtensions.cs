using System.Linq;

namespace LifxStock.Core.Extensions
{
    public static class StockInfoExtensions
    {
        public static string GetFormattedChangeText(this string value, double changeValue)
        {
            if (value == "N/A") return "N/A";

            var minusIndexNthValue = changeValue >= 0 ? 1 : 2;
            var plusIndexNthValue = changeValue >= 0 ? 2 : 3;
            var changeCharacter = changeValue >= 0 ? '+' : '-';

            var minusIndex = value.NthIndexOf('-', minusIndexNthValue);
            var plusIndex = value.NthIndexOf(changeCharacter, plusIndexNthValue);

            var changedFirstPart = value.Substring(0, minusIndex - 1);
            var changedSecondPart = "(" + value.Substring(plusIndex, value.Length - plusIndex) + ")";
            return changedFirstPart + " " + changedSecondPart;            
        }

        public static int NthIndexOf(this string s, char c, int n)
        {
            var takeCount = s.TakeWhile(x => (n -= (x == c ? 1 : 0)) > 0).Count();
            return takeCount == s.Length ? -1 : takeCount;
        }
    }
}
