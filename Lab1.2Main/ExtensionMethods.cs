namespace RedaktorMain
{
    public static class ExtensionMethods
    {
        public static IEnumerable<T> FastReverse<T>(this IList<T> items)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                yield return items[i];
            }
        }

        public static int Gcd(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                {
                    a %= b;
                }
                else
                {
                    b %= a;
                }
            }

            return Math.Max(a, b);
        }
    }
}