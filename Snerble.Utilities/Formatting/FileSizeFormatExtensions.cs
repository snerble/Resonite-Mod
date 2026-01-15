namespace Snerble.Utilities.Formatting
{
	public static class FileSizeFormatExtensions
	{
		/// <summary>
		/// Returns a formatted string depicting an amount of data in multiples of the byte unit.
		/// </summary>
		/// <param name="length"></param>
		/// <param name="decimals">The amount of fractional digits to include in the output</param>
		/// <param name="asBinaryUnit">If true, formats the size as a binary unit rather than as an SI unit.</param>
		public static string FormatDataUnit(this long length, int decimals = 2, bool asBinaryUnit = false)
		{
			var unitBase = asBinaryUnit ? 1024 : 1000;
			var magnitude = Math.Clamp((int)Math.Log(length, unitBase), 0, 8);

			var unit = magnitude switch
			{
				0 => "bytes",
				1 => asBinaryUnit ? "KiB" : "kB",
				2 => asBinaryUnit ? "MiB" : "MB",
				3 => asBinaryUnit ? "GiB" : "GB",
				4 => asBinaryUnit ? "TiB" : "TB",
				5 => asBinaryUnit ? "PiB" : "PB",
				6 => asBinaryUnit ? "EiB" : "EB",
				7 => asBinaryUnit ? "ZiB" : "ZB",
				_ => asBinaryUnit ? "YiB" : "YB"
			};

			return $"{Math.Round(length / Math.Pow(unitBase, magnitude), decimals)} {unit}";
		}
	}
}
