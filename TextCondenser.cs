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
        /// <summary>
        /// Takes in a list of lines and attempts to remove repetitions.
        /// </summary>
        /// <param name="linesOfTextToCondense">list of strings that needs to be condensed to unique lines.</param>
        /// <returns>List of condensed strings</returns>
        /// <remarks>
        /// This function assumes the following when condensing string: <br/>
        /// 1. Repetitions and partial repetitions of strings are adjacent. <br/>
        /// 2. Partial repetitions within a string itself are possible and need to be reduced as well. <br/>
        ///
        /// <code>
        /// Given:
        /// this is
        /// this is new
        /// this is new this is new text
        /// this is
        /// this is new
        /// this is different text
        ///
        /// Output:
        /// this is new text
        /// this is different text
        /// </code>
        /// </remarks>
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
                shortestUniqueText = currentKey;
                longestUniqueText = currentKey;
            }

            // Account for last line of text
            condensedTextList.Add(LongestUniquePhrase(longestUniqueText));

            return condensedTextList;
        }

        /// <summary>
        /// Parses out the longest unique sequence of characters assuming repeated text will start at the beginning of the string.
        /// </summary>
        /// <param name="longestUniqueText">String to parse</param>
        private string LongestUniquePhrase(string longestUniqueText)
        {
            // this is repeated, this is repeated, but with more text. this is repeated, but with more text. and then some
            // これはぺん。これはぺん。でも、これは犬。これはペン。でも、これは犬。そして終わり。

            // find first unique string (key)
            var fullSpan = longestUniqueText.AsSpan();
            var key = ExtractUniqueSpanFromFront(fullSpan, ReadOnlySpan<char>.Empty);

            // Return the original string if it is unique
            if (key.Length == fullSpan.Length)
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

        /// <summary>
        /// Locates the span of characters in the front of the current span which is repeated one or more times.
        /// </summary>
        /// <param name="span">Existing span of characters being searched/sliced.</param>
        /// <param name="key">Start key. If the length of this span is zero then the search will start at the front of the span.</param>
        /// <returns>span corresponding to the located key (repeated characters), or the original span itself if no repetition was observed or if the length of the found key is 1.</returns>
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

            if (!atLeastOneKeyFound || key.Length <= 1)
                return span;

            return key;
        }
    }
}
