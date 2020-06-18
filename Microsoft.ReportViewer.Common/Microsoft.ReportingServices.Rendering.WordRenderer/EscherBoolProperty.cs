namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class EscherBoolProperty : EscherSimpleProperty
	{
		internal virtual bool True => m_propertyValue != 0;

		internal virtual bool False => m_propertyValue == 0;

		internal EscherBoolProperty(ushort propertyNumber, int value_Renamed)
			: base(propertyNumber, isComplex: false, isBlipId: false, value_Renamed)
		{
		}
	}
}
