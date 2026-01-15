namespace Snerble.Utilities.Formatting
{
	public static class TimeSpanExtensions
    {
		/// <summary>
		/// Returns a formatted string depicting the elapsed time in the closest time unit;
		/// nanoseconds, microseconds or milliseconds.
		/// </summary>
		public static string FormatClosestUnit(this TimeSpan timeSpan)
		{
			if (timeSpan.Ticks < 10)
				return (timeSpan.Ticks * 100) + " ns";
			if (timeSpan.Ticks < 10000)
				return (timeSpan.Ticks / 10) + " µs";
			return (int)timeSpan.TotalMilliseconds + " ms";
		}

		public static string FormatShort(this TimeSpan timeSpan)
		{
			return timeSpan.TotalHours > 1
				? $"{(int)timeSpan.TotalHours}:{timeSpan:mm':'ss}"
				: $"{timeSpan.Minutes}:{timeSpan:ss}";
		}

		public static string FormatDisplay(this TimeSpan timeSpan)
		{
			return new[]
			{
				((int)timeSpan.TotalDays) != 0 ? $"{timeSpan.Days} {(timeSpan.Days == 1 ? "day" : "days")}" : null,
				((int)timeSpan.TotalHours) != 0 ? $"{timeSpan.Hours} {(timeSpan.Hours == 1 ? "hour" : "hours")}" : null,
				((int)timeSpan.TotalMinutes) != 0 ? $"{timeSpan.Minutes} {(timeSpan.Minutes == 1 ? "minute" : "minutes")}" : null,
				$"{timeSpan.Seconds} {(timeSpan.Seconds == 1 ? "second" : "seconds")}",
			}.JoinEnumeration()!;
		}

		public static string FormatDisplayShort(this TimeSpan timeSpan)
		{
			return new[]
			{
				((int)timeSpan.TotalDays) != 0 ? $"{timeSpan.Days}d" : null,
				((int)timeSpan.TotalHours) != 0 ? $"{timeSpan.Hours}h" : null,
				((int)timeSpan.TotalMinutes) != 0 ? $"{timeSpan.Minutes}m" : null,
				((int)timeSpan.TotalSeconds) == 1 ? $"{timeSpan.Seconds}s" : null,
				$"{timeSpan.Seconds}s",
			}.JoinNotNull(" ")!;
		}
	}
}
