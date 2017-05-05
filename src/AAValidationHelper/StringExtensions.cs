namespace AAValidationHelper
{
    public static class StringExtensions
    {
        public static string LowerFirstLetter(this string str)
        {
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }
    }
}