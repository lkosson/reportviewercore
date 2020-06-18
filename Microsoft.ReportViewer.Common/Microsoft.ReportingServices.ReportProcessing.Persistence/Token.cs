namespace Microsoft.ReportingServices.ReportProcessing.Persistence
{
	internal enum Token : byte
	{
		Null = 0,
		Object = 1,
		EndObject = 2,
		Reference = 3,
		Enum = 4,
		TypedArray = 5,
		Array = 6,
		Declaration = 7,
		DataFieldInfo = 8,
		Guid = 239,
		String = 240,
		DateTime = 241,
		TimeSpan = 242,
		Char = 243,
		Boolean = 244,
		Int16 = 245,
		Int32 = 246,
		Int64 = 247,
		UInt16 = 248,
		UInt32 = 249,
		UInt64 = 250,
		Byte = 251,
		SByte = 252,
		Single = 253,
		Double = 254,
		Decimal = byte.MaxValue
	}
}
