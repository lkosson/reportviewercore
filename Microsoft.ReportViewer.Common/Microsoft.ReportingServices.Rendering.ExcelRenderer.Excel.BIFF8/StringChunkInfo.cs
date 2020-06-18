namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal class StringChunkInfo
	{
		private bool mCompressed;

		private byte[] mData = EMPTYARRAY;

		private int mCharPos;

		private int mCharsTotal;

		private static byte[] EMPTYARRAY = new byte[0];

		internal byte[] Bytes => mData;

		internal int CharPos
		{
			get
			{
				return mCharPos;
			}
			set
			{
				mCharPos = value;
			}
		}

		internal int CharsTotal
		{
			get
			{
				return mCharsTotal;
			}
			set
			{
				mData = EMPTYARRAY;
				mCharPos = 0;
				mCharsTotal = value;
			}
		}

		internal byte[] Data
		{
			get
			{
				return mData;
			}
			set
			{
				if (value == null)
				{
					mData = EMPTYARRAY;
				}
				else
				{
					mData = value;
				}
			}
		}

		internal bool Compressed
		{
			get
			{
				return mCompressed;
			}
			set
			{
				mCompressed = value;
			}
		}

		internal bool HasMore => mCharPos < mCharsTotal;
	}
}
