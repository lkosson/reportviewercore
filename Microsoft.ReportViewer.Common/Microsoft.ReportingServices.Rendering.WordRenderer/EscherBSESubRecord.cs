using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class EscherBSESubRecord : EscherRecord
	{
		internal const int MD4HASH_LENGTH = 16;

		private byte[] mHash;

		private byte mBoundary = byte.MaxValue;

		private byte[] mImage;

		internal override int RecordSize => 8 + mHash.Length + ((mImage != null) ? (1 + mImage.Length) : 0);

		internal override string RecordName => "BSESub";

		internal virtual byte[] Hash
		{
			get
			{
				return mHash;
			}
			set
			{
				mHash = value;
			}
		}

		internal virtual byte[] Image
		{
			get
			{
				return mImage;
			}
			set
			{
				mImage = value;
			}
		}

		internal EscherBSESubRecord()
		{
		}

		internal override int Serialize(BinaryWriter dataWriter)
		{
			dataWriter.Write(getOptions());
			dataWriter.Write(GetRecordId());
			dataWriter.Write(RecordSize - 8);
			dataWriter.Write(mHash);
			dataWriter.Write(mBoundary);
			dataWriter.Write(mImage);
			return RecordSize;
		}
	}
}
