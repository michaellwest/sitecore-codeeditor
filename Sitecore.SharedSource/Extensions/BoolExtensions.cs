namespace Sitecore.SharedSource.Extensions
{
    public static class BoolExtensions
    {
        public static bool ToBool(this string value)
        {
            return value == "1";
        }
    }
}