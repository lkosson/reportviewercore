using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class EscherComplexProperty : EscherProperty
	{
		internal byte[] complexData = new byte[0];

		internal virtual byte[] ComplexData
		{
			get
			{
				return complexData;
			}
			set
			{
				complexData = value;
			}
		}

		internal override int PropertySize => 6 + complexData.Length;

		internal EscherComplexProperty(ushort id, byte[] complexData)
			: base(id)
		{
			this.complexData = complexData;
		}

		internal EscherComplexProperty(ushort propertyNumber, bool isBlipId, byte[] complexData)
			: base(propertyNumber, isComplex: true, isBlipId)
		{
			this.complexData = complexData;
		}

		internal override int serializeSimplePart(BinaryWriter dataWriter)
		{
			dataWriter.Write(Id);
			dataWriter.Write(complexData.Length);
			return 6;
		}

		internal override int serializeComplexPart(BinaryWriter dataWriter)
		{
			dataWriter.Write(complexData);
			return complexData.Length;
		}

		public override int GetHashCode()
		{
			return Id * 11;
		}
	}
}
