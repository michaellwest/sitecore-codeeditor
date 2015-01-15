namespace Sitecore.SharedSource.Extensions
{
    public static class IntegerExtensions
    {
        public static bool IsBetween(this int value, int minimum = int.MinValue, int maximum = int.MaxValue)
        {
            return value >= minimum && value <= maximum;
        }
    }
}