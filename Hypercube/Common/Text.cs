using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ZBase.Common {
    public static class Text {
        const string RegexString = "[^A-Za-z0-9!\\^\\~$%&/()=?{}\t\\[\\]\\\\ ,\\\";.:\\-_#'+*<>|@]|&.$|&.(&.)";
		const string ColorCodeRegex = "\\&[A-Fa-f0-9]";
        /// <summary>
        /// Replaces invalid chat characters with "*".
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CleanseString(string input) {
            input = input.Replace("§E", Configuration.Settings.Formats.Error);
            input = input.Replace("§S", Configuration.Settings.Formats.System);
            input = input.Replace("§D", Configuration.Settings.Formats.Divider);

            var matcher = new Regex(RegexString, RegexOptions.Multiline);
            return matcher.Replace(input, "*");
        }

        public static string CleanseStringCP437(string input) {
            input = input.Replace("§E", Configuration.Settings.Formats.Error);
            input = input.Replace("§S", Configuration.Settings.Formats.System);
            input = input.Replace("§D", Configuration.Settings.Formats.Divider);
            return input;
        }
        /// <summary>
        /// Returns true if an illegal character is inside of the given string.
        /// </summary>
        /// <returns></returns>
        public static bool StringMatches(string input) {
            var matcher = new Regex(RegexString, RegexOptions.Multiline);
            return matcher.IsMatch(input);
        }

        /// <summary>
        /// Removes color codes from messages.
        /// </summary>
        /// <param name="input">The text to strip color codes from.</param>
        /// <returns>Non-colored text</returns>
        public static string RemoveColors(string input) {
			var matcher = new Regex(ColorCodeRegex, RegexOptions.Multiline);
			return matcher.Replace(input, "");
        }

		public static List<string> SplitBrs(string input) {
			var builder = new List<string>();

			while (input.IndexOf("<br>", StringComparison.OrdinalIgnoreCase) >= 0) {
				int index = input.IndexOf("<br>", StringComparison.OrdinalIgnoreCase);
				builder.Add(input.Substring(0, index)); // -- Add to our string builder
				input = input.Substring(index + 4, input.Length - (index + 4)); // -- Remove from Input the string, and discard the <br>.
			}

			// -- If there's any leftovers that wern't split, we will need to go ahead and add that as well.
			if (input != "")
				builder.Add(input);

			// -- If we miracously made it here without having to break the line, we will need to do this.
			if (builder.Count == 0)
				builder.Add(input);

			return builder;
		}

		/// <summary>
		/// Splits a long message into multiple lines as needed. Appends ">>" as needed. This will also pad messages if they are of incorrect length.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string[] SplitLines(string input) {
			var builder = new List<string>();

			if (input.Length <= 64 && input.IndexOf("<br>", StringComparison.OrdinalIgnoreCase) <= 0)
				return new[] { input.PadRight(64) };

			// -- The string is longer than 64 characters, or contains '<br>'.
			builder.AddRange(SplitBrs(input));
			string temp;

			// -- First, going to insert our own <br>'s wherever the string is too long.
			for (var i = 0; i < builder.Count; i++) { // -- For each item in the builders array (1 or more strings)
				temp = "";

				while (builder[i].Length > 0) { // -- Going to use temp here so we don't mess up our original string
					if (builder[i].Length > 64) {
						int thisIndex = builder[i].Substring(0, 60).LastIndexOf(' '); // -- Split by words.

						if (thisIndex == -1) // -- Just incase it's one spaceless string.
							thisIndex = 60;

						temp += builder[i].Substring(0, thisIndex) + "<br>"; // -- Put the string before, with the seperator, and our break.

						// -- Finally, Remove this part of the string from the original Builder[i], and add our newline seperators.
						builder[i] = builder[i].Substring(thisIndex, builder[i].Length - (thisIndex)); // -- It will now loop again for any subsequent breaks.
					} else {
						// -- Since Builder[i] is not (or is no longer) greater than 64 characters long, we can simply remove the whole thing :)
						temp += builder[i];
						builder[i] = "";
					}
				}

				builder[i] = temp;
			}

			// -- Next, remove any "<br>"'s, and split up the line on either side of it.
			for (var z = 0; z < builder.Count; z++) {
				while (builder[z].IndexOf("<br>", StringComparison.OrdinalIgnoreCase) >= 0) {
					temp = builder[z];
					int index = builder[z].IndexOf("<br>", StringComparison.OrdinalIgnoreCase);
					builder[z] = temp.Substring(0, index).PadRight(64);
					builder.Insert(z + 1, temp.Substring(index + 4, temp.Length - (index + 4)));
				}

				// -- If there's any leftovers that wern't split, we will need to go ahead and add that as well.
				if (builder[z] != "")
					builder[z] = builder[z].PadRight(64);
 
			}

			// -- If we miracously made it here without having to break the line, we will need to do this.
			if (builder.Count == 0)
				builder.Add(input.PadRight(64));

			return builder.ToArray(); // -- Return our nice array'd string :)
		}
	}
}
