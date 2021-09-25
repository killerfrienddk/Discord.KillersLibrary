using System;

namespace Interaction.Services {
	[Flags] public enum LogOutput { 
		Console = 1, 
		File = 2, 

		ConsoleAndFile = Console | File,
	}
}