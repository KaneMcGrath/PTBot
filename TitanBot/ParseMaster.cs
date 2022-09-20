namespace TitanBot
{
    /// <summary>
    /// Parse Master
    /// some helper functions to make parsing data easier
    /// </summary>
    public static class ParseMaster
    {
        /// <summary>
        /// specify a delimiter and it will return the first and outermost string that is encapsulated by it
        /// if there are no encapsulated strings it will return the input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string FirstEncapsulatedString(string input, char delimiter)
        {
            int first = -1;
            int last = -1;
            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                if (ch == delimiter)
                {
                    if (first == -1)
                    {
                        first = i;

                    }
                    else
                    {
                        last = i;
                    }
                }
            }

            if (first == -1) return input;
            if (last == -1) return input;
            if (first == last) return input;

            return input.Substring(first + 1, last - first);
        }
        /// <summary>
        /// Specifiy two delimiters ex. "[" and "]" and it will return the first string encapsulated by the two not including the delimiters
        /// if there are no encapsulated strings it will return the input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="firstDelimiter"></param>
        /// <param name="secondDelimiter"></param>
        /// <returns></returns>
        public static string FirstEncapsulatedString(string input, char firstDelimiter, char secondDelimiter)
        {
            int first = -1;
            int last = -1;
            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                if (ch == firstDelimiter)
                {
                    if (first == -1)
                    {
                        first = i;

                    }
                }
                if (ch == secondDelimiter)
                {
                    if (first != -1)
                    {
                        last = i;
                    }
                }
            }

            if (first == -1) return input;
            if (last == -1) return input;
            if (first == last) return input;

            return input.Substring(first + 1, last - first - 1);
        }

        public static Range RangeOfFirstEncapsulation(string input, char firstDelimiter, char secondDelimiter, bool includeDelimiters = false)
        {
            int first = -1;
            int last = -1;
            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                if (ch == firstDelimiter)
                {
                    if (first == -1)
                    {
                        first = i;

                    }
                }
                if (ch == secondDelimiter)
                {
                    if (first != -1)
                    {
                        last = i;
                    }
                }
            }

            if (first == -1) return Range.Empty;
            if (last == -1) return Range.Empty;
            if (first == last) return Range.Empty;

            Range result;
            if (includeDelimiters)
            {
                result = new Range(first, last + 1);
            }
            else
            {
                result = new Range(first + 1, last);
            }

            return result;
        }

    }

    public struct Range
    {
        public static Range Empty = new Range();

        public int start;
        public int end;


        public Range(int start, int end)
        {
            this.start = start; this.end = end;
        }

        public int Length()
        {
            return end - start;
        }

        public bool isEmpty()
        {
            if (end == 0) return true;
            return false;
        }
    }
}
