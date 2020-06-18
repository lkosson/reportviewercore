using System.Collections;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class EscherOptRecord : EscherRecord
	{
		private class AnonymousClassComparator : IComparer
		{
			private EscherOptRecord enclosingInstance;

			internal AnonymousClassComparator(EscherOptRecord enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}

			private void InitBlock(EscherOptRecord enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			public virtual int Compare(object o1, object o2)
			{
				EscherProperty escherProperty = (EscherProperty)o1;
				EscherProperty escherProperty2 = (EscherProperty)o2;
				if (escherProperty.PropertyNumber >= escherProperty2.PropertyNumber)
				{
					if (escherProperty.PropertyNumber != escherProperty2.PropertyNumber)
					{
						return 1;
					}
					return 0;
				}
				return -1;
			}
		}

		internal static ushort RECORD_ID = 61451;

		internal const string RECORD_DESCRIPTION = "msofbtOPT";

		private IList m_properties = new ArrayList();

		internal override int RecordSize => 8 + PropertiesSize;

		internal override string RecordName => "Opt";

		private int PropertiesSize
		{
			get
			{
				int num = 0;
				IEnumerator enumerator = m_properties.GetEnumerator();
				while (enumerator.MoveNext())
				{
					EscherProperty escherProperty = (EscherProperty)enumerator.Current;
					num += escherProperty.PropertySize;
				}
				return num;
			}
		}

		internal virtual IList EscherProperties => m_properties;

		internal EscherOptRecord()
		{
		}

		internal override int Serialize(BinaryWriter dataWriter)
		{
			dataWriter.Write(getOptions());
			dataWriter.Write(GetRecordId());
			dataWriter.Write(PropertiesSize);
			int num = 8;
			IEnumerator enumerator = m_properties.GetEnumerator();
			while (enumerator.MoveNext())
			{
				EscherProperty escherProperty = (EscherProperty)enumerator.Current;
				num += escherProperty.serializeSimplePart(dataWriter);
			}
			IEnumerator enumerator2 = m_properties.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				EscherProperty escherProperty2 = (EscherProperty)enumerator2.Current;
				num += escherProperty2.serializeComplexPart(dataWriter);
			}
			return num;
		}

		internal override ushort getOptions()
		{
			setOptions((ushort)((m_properties.Count << 4) | 3));
			return base.getOptions();
		}

		internal virtual EscherProperty getEscherProperty(int index)
		{
			return (EscherProperty)m_properties[index];
		}

		internal virtual EscherProperty getEscherPropertyByID(int id)
		{
			for (int i = 0; i < m_properties.Count; i++)
			{
				EscherProperty escherProperty = (EscherProperty)m_properties[i];
				if (escherProperty.Id == id)
				{
					return escherProperty;
				}
			}
			return null;
		}

		internal virtual void addEscherProperty(EscherProperty prop)
		{
			m_properties.Add(prop);
		}

		internal virtual void sortProperties()
		{
			((ArrayList)m_properties).Sort(new AnonymousClassComparator(this));
		}
	}
}
