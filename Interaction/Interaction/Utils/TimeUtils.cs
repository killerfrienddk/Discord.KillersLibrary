namespace Interaction.Services {
	public static class TimeUtils {
		public const long TICKS_TO_MS 		=  10_000;
		public const long TICKS_TO_SECONDS	=  10_000_000;
		public const long TICKS_TO_MINUTES 	=  600_000_000;
		public const long TICKS_TO_HOURS 	=  36_000_000_000;
		public const long TICKS_TO_DAYS 	=  864_000_000_000;

		public static long TicksToMilliseconds(long ticks) 
			=> ticks / TICKS_TO_MS;
		public static long TicksToSeconds(long ticks) 
			=> ticks / TICKS_TO_SECONDS;
		public static long TicksToMinutes(long ticks) 
			=> ticks / TICKS_TO_MINUTES;
		public static long TicksToHours(long ticks) 
			=> ticks / TICKS_TO_HOURS;
		public static long TicksToDays(long ticks) 
			=> ticks / TICKS_TO_DAYS;
	}
}