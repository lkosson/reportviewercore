namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal enum Token : byte
	{
		Null = 0,
		Object = 1,
		Reference = 2,
		Enum = 3,
		GlobalReference = 4,
		Hashtable = 232,
		Serializable = 233,
		SqlGeometry = 234,
		SqlGeography = 235,
		DateTimeWithKind = 236,
		DateTimeOffset = 237,
		ByteArray = 238,
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
