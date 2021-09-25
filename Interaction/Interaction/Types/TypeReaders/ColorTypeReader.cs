using System.Threading.Tasks;
using System;
using Discord.Commands;
using Discord;


namespace TypeReaders.Types.TypeReaders {

	public class ColorTypeReader : TypeReader {

		public ColorTypeReader() { }

		public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services) {
			uint val;

			// Tries to read the input as an unsigned integer
			if(uint.TryParse(input, out val))
				return Task.FromResult(TypeReaderResult.FromSuccess(new TypeReaderValue(ColorUtils.CreateColor(val), 1.0f)));

			// Tries to read the input as an unsigned integer in base-16
			if(TextUtils.TryParseHex(input, out val))
				return Task.FromResult(TypeReaderResult.FromSuccess(new TypeReaderValue(ColorUtils.CreateColor(val), 1.0f)));

			// Tries to read the input as a ColorCode
			try {
				var colorCode = (ColorCodes) Enum.Parse(typeof(ColorCodes), input, true);
				return Task.FromResult(TypeReaderResult.FromSuccess(new TypeReaderValue(colorCode.ToColor(), 1.0f)));
			} catch(Exception) {}

			return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"Failed to parse {typeof(Color).Name}."));
		}
		
	}
}