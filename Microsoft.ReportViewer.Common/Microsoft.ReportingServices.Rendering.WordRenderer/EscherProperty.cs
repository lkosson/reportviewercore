using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal abstract class EscherProperty
	{
		private ushort id;

		internal virtual ushort Id => id;

		internal virtual ushort PropertyNumber => (ushort)(id & 0x3FFF);

		internal virtual bool Complex => (id & 0x8000) != 0;

		internal virtual bool BlipId => (id & 0x4000) != 0;

		internal virtual int PropertySize => 6;

		internal EscherProperty(ushort id)
		{
			this.id = id;
		}

		internal EscherProperty(ushort propertyNumber, bool isComplex, bool isBlipId)
		{
			id = (ushort)(propertyNumber + (isComplex ? 32768 : 0) + (isBlipId ? 16384 : 0));
		}

		internal abstract int serializeSimplePart(BinaryWriter dataWriter);

		internal abstract int serializeComplexPart(BinaryWriter dataWriter);
	}
}
