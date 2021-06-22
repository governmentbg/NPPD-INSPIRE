namespace Inspire.Utilities.Extensions
{
    public static class StringExt
    {
        public static string ExtendToSearch(this string text)
        {
            return text.IsNotNullOrEmpty()
                ? $"%{text.Replace(' ', '%')}%".Trim()
                : null;
        }

        public static string ToDbData(this string text)
        {
            return PrepareToDb(text);
        }

        public static string PrepareToDb(string text)
        {
            text = text?.Trim();
            return text.IsNullOrEmpty() ? null : text;
        }
    }
}