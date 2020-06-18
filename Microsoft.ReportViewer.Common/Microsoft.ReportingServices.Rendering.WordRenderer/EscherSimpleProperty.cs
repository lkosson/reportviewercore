using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class EscherSimpleProperty : EscherProperty
	{
		protected internal int m_propertyValue;

		internal virtual int PropertyValue
		{
			get
			{
				return m_propertyValue;
			}
			set
			{
				m_propertyValue = value;
			}
		}

		internal EscherSimpleProperty(ushort id, int propertyValue)
			: base(id)
		{
			m_propertyValue = propertyValue;
		}

		internal EscherSimpleProperty(ushort propertyNumber, bool isComplex, bool isBlipId, int propertyValue)
			: base(propertyNumber, isComplex, isBlipId)
		{
			m_propertyValue = propertyValue;
		}

		internal override int serializeSimplePart(BinaryWriter dataWriter)
		{
			dataWriter.Write(Id);
			dataWriter.Write(m_propertyValue);
			return 6;
		}

		internal override int serializeComplexPart(BinaryWriter dataWriter)
		{
			return 0;
		}

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (!(o is EscherSimpleProperty))
			{
				return false;
			}
			EscherSimpleProperty escherSimpleProperty = (EscherSimpleProperty)o;
			if (m_propertyValue != escherSimpleProperty.m_propertyValue)
			{
				return false;
			}
			if (Id != escherSimpleProperty.Id)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return m_propertyValue;
		}
	}
}
