using System;
using System.Text;
using System.Text.RegularExpressions;

namespace OldPhonePad.Grouping
{
    /// <summary>
    /// Decodes old T9-style phone keypad input using a grouping approach.
    /// This implementation groups consecutive identical digits before processing them.
    /// </summary>
    public static class OldPhonePadDecoder
    {
        // Keypad mapping - the classic T9 layout
        private static readonly Dictionary<char, string> KeypadMap = new()
        {
            { '1', "&'(" },
            { '2', "abc" },
            { '3', "def" },
            { '4', "ghi" },
            { '5', "jkl" },
            { '6', "mno" },
            { '7', "pqrs" },
            { '8', "tuv" },
            { '9', "wxyz" },
            { '0', " " }
        };

        /// <summary>
        /// Decodes T9-style phone keypad input into readable text using a grouping approach.
        /// </summary>
        /// <param name="input">Input string with digits, spaces, '*' (backspace), and '#' (send).</param>
        /// <returns>The decoded text string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
        /// <exception cref="ArgumentException">Thrown when input doesn't contain '#'.</exception>
        public static string OldPhonePad(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), "Input cannot be null.");
            }

            if (!input.Contains('#'))
            {
                throw new ArgumentException("Input must contain the send character '#'.", nameof(input));
            }

            // Remove everything after # (including #)
            int sendIndex = input.IndexOf('#');
            string relevantInput = input.Substring(0, sendIndex);

            // Process the input by grouping runs of consecutive characters
            return ProcessGroupedInput(relevantInput);
        }

        /// <summary>
        /// Processes the input by identifying and handling groups of consecutive characters.
        /// </summary>
        private static string ProcessGroupedInput(string input)
        {
            var result = new StringBuilder();
            int i = 0;

            while (i < input.Length)
            {
                char current = input[i];

                if (current == ' ')
                {
                    // Space is just a separator, skip it
                    i++;
                    continue;
                }

                if (current == '*')
                {
                    // Backspace - remove last character if there is one
                    if (result.Length > 0)
                    {
                        result.Length--;
                    }
                    i++;
                    continue;
                }

                if (char.IsDigit(current))
                {
                    // Find the group of consecutive identical digits
                    string group = ExtractGroup(input, i);

                    // Convert the group to a character
                    char decodedChar = DecodeGroup(current, group.Length);
                    if (decodedChar != '\0')
                    {
                        result.Append(decodedChar);
                    }

                    // Skip past this group
                    i += group.Length;
                }
                else
                {
                    // Unknown character, just skip it
                    i++;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Extracts a group of consecutive identical characters starting at the given position.
        /// Took a bit to get the grouping logic clean
        /// </summary>
        private static string ExtractGroup(string input, int startIndex)
        {
            if (startIndex >= input.Length)
            {
                return "";
            }

            char targetChar = input[startIndex];
            var group = new StringBuilder();
            int i = startIndex;

            // Collect all consecutive occurrences of the target character
            while (i < input.Length && input[i] == targetChar)
            {
                group.Append(input[i]);
                i++;
            }

            return group.ToString();
        }

        /// <summary>
        /// Alternative implementation using Regex for grouping.
        /// This method demonstrates a more functional approach using pattern matching.
        /// </summary>
        private static List<string> ExtractGroupsWithRegex(string input)
        {
            // Remove spaces first (they're just separators)
            string cleanInput = input.Replace(" ", "|"); // Use | as delimiter

            // Match consecutive identical digits or special characters
            var groups = new List<string>();
            var regex = new Regex(@"(\d)\1*|\*+");

            foreach (Match match in regex.Matches(cleanInput.Replace("|", "")))
            {
                groups.Add(match.Value);
            }

            return groups;
        }

        /// <summary>
        /// Decodes a group of identical digit presses into the corresponding character.
        /// </summary>
        private static char DecodeGroup(char key, int presses)
        {
            if (!KeypadMap.TryGetValue(key, out string? chars))
            {
                return '\0';
            }

            if (string.IsNullOrEmpty(chars))
            {
                return '\0';
            }

            // Had to figure out cycling here - modulo handles the wraparound
            // This handles the case where you press more times than there are letters
            int index = (presses - 1) % chars.Length;
            return char.ToUpper(chars[index]);
        }

        /// <summary>
        /// Alternative implementation: processes input in a single pass with explicit grouping.
        /// This version is more explicit about the grouping strategy.
        /// </summary>
        public static string OldPhonePadWithExplicitGrouping(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), "Input cannot be null.");
            }

            if (!input.Contains('#'))
            {
                throw new ArgumentException("Input must contain the send character '#'.", nameof(input));
            }

            // Extract input up to the send character
            int sendIndex = input.IndexOf('#');
            string relevantInput = input.Substring(0, sendIndex);

            // Split by spaces (which act as group delimiters)
            string[] segments = relevantInput.Split(' ', StringSplitOptions.None);

            var result = new StringBuilder();

            foreach (string segment in segments)
            {
                result.Append(ProcessSegment(segment));
            }

            return result.ToString();
        }

        /// <summary>
        /// Processes a segment (space-delimited part) of the input.
        /// Within a segment, consecutive identical digits form groups.
        /// </summary>
        private static string ProcessSegment(string segment)
        {
            var result = new StringBuilder();
            int i = 0;

            while (i < segment.Length)
            {
                char current = segment[i];

                if (current == '*')
                {
                    // Backspace
                    if (result.Length > 0)
                    {
                        result.Length--;
                    }
                    i++;
                }
                else if (char.IsDigit(current))
                {
                    // Extract group of consecutive identical digits
                    int groupStart = i;
                    while (i < segment.Length && segment[i] == current)
                    {
                        i++;
                    }
                    int groupLength = i - groupStart;

                    // Decode this group
                    char decodedChar = DecodeGroup(current, groupLength);
                    if (decodedChar != '\0')
                    {
                        result.Append(decodedChar);
                    }
                }
                else
                {
                    // Unknown character
                    i++;
                }
            }

            return result.ToString();
        }
    }
}
