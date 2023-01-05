namespace Boggle
{
    static class BoggleGlobals
    {
        public static int g_numBlockSides = 6;
        public static int g_iGridSize = 4;
        public static int g_minWordLength = 3;
        public static int g_maxWordLength = g_iGridSize * g_iGridSize;

        public static string PryDiadicHeader(string str)
        {
            char[] array = str.ToCharArray(0, 2);
            string header = new string(array);
            return header;
        }

        public static string PryTriadicHeader(string str)
        {
            char[] array = str.ToCharArray(0, 3);
            string header = new string(array);
            return header;
        }
    }
}
