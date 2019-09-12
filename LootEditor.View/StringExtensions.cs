namespace LootEditor.View
{
    public static class StringExtensions
    {
        public static bool IsLower(this string str)
        {
            foreach (var ch in str)
                if (char.IsUpper(ch))
                    return false;
            return true;
        }
    }
}
