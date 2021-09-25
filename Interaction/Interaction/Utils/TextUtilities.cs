using System.Text.RegularExpressions;
using System.Text;
using System;

namespace Interaction.Utilities {


	public static partial class TextUtilities {

		public static void AppendLineIf(StringBuilder sb, bool condition, string text) {
			if(condition) sb.AppendLine(text);
		} 

		public static string GetDefaultValueString(object val) {
			if(val == null) return "Null";

			var valStr = val.ToString();

			return (valStr.Length > 0) ? valStr : "\"\"";
		}


		// ---- // 

		




	}


	

	// 
	// String formatting utilities
	// 
	public static partial class TextUtilities {
	
		public static string DateString(DateTimeOffset date, bool printTimeOfDay = true) {
			if(printTimeOfDay) 	return date.ToString("yyyy/MM/dd HH:mm:ss");
			else 				return date.ToString("yyyy/MM/dd");
		} 
		public static string TimeSpanString(TimeSpan timeSpan) {
			if(timeSpan < TimeSpan.Zero) return timeSpan.ToString("yyyy/MM/dd HH:mm:ss");

			var days = timeSpan.Days % 365;
			var years = timeSpan.Days / 365;

			if(timeSpan.TotalMinutes < 1)	return $"{timeSpan.Seconds}s";
			if(timeSpan.TotalHours < 1)	 	return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
			if(timeSpan.TotalDays < 1)	 	return timeSpan.ToString("hh\\:mm\\:ss");
			if(timeSpan.TotalDays < 3)	 	return $"{days}d {timeSpan.ToString("hh\\:mm")}";
			if(years < 1) 				 	return $"{days}d {timeSpan.ToString("%h")}h";
			
			
			return $"{years}y {days}d";
		}

		public static string Format(DateTimeOffset date, bool printTimeOfDay = true) 
			=> DateString(date, printTimeOfDay);
		public static string Format(TimeSpan timeSpan) => TimeSpanString(timeSpan);

		public static string Format(double num) {
			if(Math.Abs(num) < 1000)		return $"{num:N2} ";

			num /= 1000;
			if(Math.Abs(num) < 1000)		return $"{num:N2}K";

			num /= 1000;
			if(Math.Abs(num) < 1000)		return $"{num:N2}M";

			num /= 1000;
			if(Math.Abs(num) < 1000)		return $"{num:N2}B";

			return $"{num:N2}B";
		}



		/*
			OwOifies text.
		*/
		public static string Owoify(this string text) {

			// Function to replace ! with random owo faces.
			MatchEvaluator myEvaluator = new MatchEvaluator((Match match) => {
				string[] faces = { "(・ω・)",";;w;;","owo","UwU",">w<","^w^" };
				return $" {faces[ new Random().Next(faces.Length - 1) ]} ";
			});

			// Return empty if there is nothing.
			if(string.IsNullOrWhiteSpace(text)) 
				return "";
			
			// Apply the OwO rules
			text = Regex.Replace(text, @"(r|l)", "w");			// r or l 	=> w
			text = Regex.Replace(text, @"(R|L)", "W");			// R or L 	=> W
			text = Regex.Replace(text, @"n([aeiou])", "ny$1");	// no 		=> nyo
			text = Regex.Replace(text, @"N([aeiou])", "Ny$1");	// No 		=> Nyo
			text = Regex.Replace(text, @"N([AEIOU])", "Ny$1");	// NO 		=> NyO
			text = Regex.Replace(text, @"ove", "uv");			// Love 	=> Luv
			text = Regex.Replace(text, @"(\!)", myEvaluator);	// Yes! 	=> Yes >w<

			return text;
		}

	
	} // TextUtils.Format()

	// 
	// String formatting utilities
	//  
	public static partial class TextUtilities { 
	
		public static bool TryReadHex(string str, out int val) {
			val = 0;
			try {
				if(str.StartsWith('#')) 		str = str.Substring(1);
				else if(str.StartsWith("0x")) 	str = str.Substring(2);
				val = Convert.ToInt32(str, 16);
				return true;
			} catch(Exception) {}
			return false;
		}

		public static bool TryParseHex(string str, out uint val) {
			val = 0;
			try {
				if(str.StartsWith('#')) 		str = str.Substring(1);
				else if(str.StartsWith("0x")) 	str = str.Substring(2);
				val = Convert.ToUInt32(str, 16);
				return true;
			} catch(Exception) {}
			return false;
		}

		public static bool TryParseHex(string str, out ulong val) {
			val = 0;
			try {
				if(str.StartsWith('#')) 		str = str.Substring(1);
				else if(str.StartsWith("0x")) 	str = str.Substring(2);
				val = Convert.ToUInt64(str, 16);
				return true;
			} catch(Exception) {}
			return false;
		}
	
	}



	// 
	// MarkDown utilities
	// 
	public static partial class TextUtilities {
	
		// ---- // Complex Markdownd

		public static string MakeLink(string text, string url) => $"[{text}]({url})";




		// ---- // Simple Markdownd


		public static string ToItalics(string text) => $"*{text}*";
		public static string ToBold(string text) => $"**{text}**";
		public static string ToUnderline(string text) => $"__{text}__";
		public static string ToStrikeThrough(string text) => $"~~{text}~~";


		public static string ToCodeBlockSingleline(string text) => $"`{text}`";
		public static string ToCodeBlockMultiline(string text) => $"```\n{text}```";
		public static string ToCodeBlockMultiline(string text, string languageCode) => $"```{languageCode}\n{text}```";

		public static string ToBlockQuoteSingleline(string text) => $"> {text}";
		public static string ToBlockQuoteMultiline(string text) => $">>> {text}";

		public static string ToSpoiler(string text) => $"||{text}||";




		// ---- // Composite Markdownd

		public static string ToBoldItalics(string text) => ToItalics(ToBold(text)); 
		public static string ToUnderlineItalics(string text) => ToUnderline(ToItalics(text)); 
		public static string ToUnderlineBold(string text) => ToUnderline(ToBold(text)); 
		public static string ToUnderlineBoldItalics(string text) => ToUnderline(ToBold(ToItalics(text))); 
		


		// ---- // Custom Stuff

		public const char BAR_FULL = '■';
		public const char BAR_EMPTY = '□';


		/*
			Creates a progress bar of specified size.
		*/
		public static string ProgressBar(double score, int width) {
			if(width < 2)
				throw new ArgumentOutOfRangeException(nameof(width), width, $"Value must be at least {2}!");

			score = Math.Clamp(score, 0, 1);

			int full = (int) Math.Round(score * width);
			int empty = width - full;

			return $"{new String(BAR_FULL, full)}{new String(BAR_EMPTY, empty)}";
		}


		public static string GetInviteUrl(string code)
			=> $"https://discord.gg/{code}";
	

		/*
			Gets the timespan tag for this point in time.
		*/
		public static string GetTimestamp(DateTimeOffset date, char format = 'f')
			=> GetTimestamp((ulong) date.ToUnixTimeSeconds(), format);
		public static string GetTimestamp(DateTime date, char format = 'f')
			=> GetTimestamp((ulong) date.ToSeconds(), format);
		public static string GetTimestamp(TimeSpan ts, char format = 'f')
			=> GetTimestamp(DateTimeOffset.Now + ts, format);


		/*
			Gets the timespan tag for this point in time.
			- date is the number of seconds since Epoch (01/01/1970 01:00).
			- format is the character describing which date format we want to use.
		*/
		public static string GetTimestamp(ulong date, char format = 'f') 
			=> $"<t:{date}:{format}>";



		/*
			Formats
				d = 10/07/2021
				f = 10 July 2021 19:21
				t = 19:21
				D = 10 July 2021
				F = Saturday, 10 July 2021 19:21
				R = a month ago
				T = 19:21:08
		*/


	}
}