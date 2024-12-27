namespace Home.Core.Extensions {

    internal static class StringExtensions {

        // Adapted from https://stackoverflow.com/a/27073919
        public static string FirstCharToUpperCase(this string s) {
            if (string.IsNullOrEmpty(s)) return s;
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

    }

}
