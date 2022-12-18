using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCondensor
{
    public class TextCondenser
    {
        public List<string> CondenseText(IEnumerable<string>? linesOfTextToCondense)
        {
            List<string> condensedTextList = new List<string>();
            if (linesOfTextToCondense == null) return condensedTextList;
            
            string shortestUniqueText = "";
            string longestUniqueText = "";
            foreach(string currentKey in linesOfTextToCondense)
            {
                if (string.IsNullOrWhiteSpace(shortestUniqueText))
                {
                    shortestUniqueText = currentKey;
                    longestUniqueText = currentKey;
                    continue;
                }

                if (currentKey.StartsWith(longestUniqueText) || longestUniqueText.StartsWith(currentKey))
                {
                    shortestUniqueText = currentKey.Length < shortestUniqueText.Length
                        ? currentKey
                        : shortestUniqueText;
                    longestUniqueText = currentKey.Length > longestUniqueText.Length
                        ? currentKey
                        : longestUniqueText;
                    continue;
                }

                // current line of text is part of a new set
                condensedTextList.Add(LongestUniquePhrase(longestUniqueText));
                shortestUniqueText = "";
                longestUniqueText = "";
            }

            // Account for last line of text
            condensedTextList.Add(LongestUniquePhrase(longestUniqueText));

            return condensedTextList;
        }

        private string LongestUniquePhrase(string longestUniqueText)
        {
            // this is repeated, this is repeated, but with more text. this is repeated, but with more text. and then some
            // これはぺん。これはぺん。でも、これは犬。これはペン。でも、これは犬。そして終わり。

            // find first unique string (key)
            var fullSpan = longestUniqueText.AsSpan();
            var key = ExtractUniqueSpanFromFront(fullSpan, ReadOnlySpan<char>.Empty);

            // Return the original string if it is unique
            if (key.Length == 0 || key.Length == fullSpan.Length)
                return longestUniqueText;

            // Otherwise, slice out the key from the front and see if we can add on to it.
            var currentSlice = fullSpan;
            while (currentSlice.Length > key.Length)
            {
                currentSlice = currentSlice.Slice(key.Length);
                key = ExtractUniqueSpanFromFront(currentSlice, key);
            }

            return key.ToString();
        }

        private static ReadOnlySpan<char> ExtractUniqueSpanFromFront(ReadOnlySpan<char> span, ReadOnlySpan<char> key)
        {
            bool atLeastOneKeyFound = false;
            
            for (int i = key.Length + 1; i < span.Length; i++)
            {
                ReadOnlySpan<char> subSpan = span.Slice(0, i);
                ReadOnlySpan<char> remainingSpan = span.Slice(i, span.Length - i);

                if (!remainingSpan.Contains(subSpan, StringComparison.InvariantCulture))
                    break;

                key = subSpan;
                atLeastOneKeyFound = true;
            }

            if (!atLeastOneKeyFound)
                return span;

            return key;
        }
    }
}
