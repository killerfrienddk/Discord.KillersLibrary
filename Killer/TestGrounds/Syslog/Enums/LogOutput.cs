using System;

namespace Interaction.TestingGrounds.Services {
	[Flags] public enum LogOutput { 
		Console = 1, 
		File = 2, 

		ConsoleAndFile = Console | File,
	}
}