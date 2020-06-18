using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class ListLevelOnFile
	{
		internal enum WordNumberFormat
		{
			Arabic = 0,
			Lowercase = 4,
			RomanLower = 2,
			Bullet = 23
		}

		internal const int TAB = 0;

		internal const int SPACE = 1;

		internal const int NOTHING = 2;

		private int _iStartAt;

		private byte _nfc;

		private byte _info;

		private byte[] _rgbxchNums;

		private byte _ixchFollow;

		private int _dxaSpace;

		private int _dxaIndent;

		private short _reserved;

		private byte[] _grpprlPapx;

		private byte[] _grpprlChpx;

		private char[] _bulletText;

		private RPLFormat.ListStyles m_style;

		internal virtual int SizeInBytes => 28 + _grpprlChpx.Length + _grpprlPapx.Length + _bulletText.Length * 2 + 2;

		internal virtual byte FollowChar => _ixchFollow;

		internal RPLFormat.ListStyles ListStyle => m_style;

		internal ListLevelOnFile(int level, RPLFormat.ListStyles listStyle, Word97Writer writer)
		{
			m_style = listStyle;
			_iStartAt = 1;
			int num = 0;
			_grpprlPapx = new byte[0];
			_dxaSpace = 360;
			_rgbxchNums = new byte[9];
			_dxaIndent = 0;
			_ixchFollow = 0;
			string fontName = "Arial";
			switch (listStyle)
			{
			case RPLFormat.ListStyles.Numbered:
				_rgbxchNums[0] = 1;
				_bulletText = new char[2]
				{
					(char)level,
					'.'
				};
				switch (level % 3)
				{
				case 0:
					setNumberFormatInternal(WordNumberFormat.Arabic);
					break;
				case 1:
					setNumberFormatInternal(WordNumberFormat.RomanLower);
					break;
				case 2:
					setNumberFormatInternal(WordNumberFormat.Lowercase);
					break;
				}
				break;
			case RPLFormat.ListStyles.Bulleted:
				setNumberFormatInternal(WordNumberFormat.Bullet);
				switch (level % 3)
				{
				case 0:
					_bulletText = new char[1]
					{
						'·'
					};
					fontName = "Symbol";
					break;
				case 1:
					_bulletText = new char[1]
					{
						'o'
					};
					fontName = "Courier New";
					break;
				case 2:
					_bulletText = new char[1]
					{
						'§'
					};
					fontName = "Wingdings";
					break;
				}
				break;
			default:
				setNumberFormatInternal(WordNumberFormat.Bullet);
				_bulletText = new char[1]
				{
					' '
				};
				break;
			}
			_grpprlChpx = new byte[20];
			num = 0;
			int param = writer.WriteFont(fontName);
			num += Word97Writer.AddSprm(_grpprlChpx, num, 19023, param, null);
			num += Word97Writer.AddSprm(_grpprlChpx, num, 19038, param, null);
			num += Word97Writer.AddSprm(_grpprlChpx, num, 19024, param, null);
			num += Word97Writer.AddSprm(_grpprlChpx, num, 19025, param, null);
			int param2 = 20;
			num += Word97Writer.AddSprm(_grpprlChpx, num, 19011, param2, null);
		}

		internal void Write(BinaryWriter writer)
		{
			writer.Write(_iStartAt);
			writer.Write(_nfc);
			writer.Write(_info);
			writer.Flush();
			writer.Write(_rgbxchNums, 0, _rgbxchNums.Length);
			writer.Write(_ixchFollow);
			writer.Write(_dxaSpace);
			writer.Write(_dxaIndent);
			writer.Write((byte)_grpprlChpx.Length);
			writer.Write((byte)_grpprlPapx.Length);
			writer.Write(_reserved);
			writer.Flush();
			writer.Write(_grpprlPapx, 0, _grpprlPapx.Length);
			writer.Write(_grpprlChpx, 0, _grpprlChpx.Length);
			writer.Flush();
			writer.Write((short)_bulletText.Length);
			byte[] bytes = Encoding.Unicode.GetBytes(_bulletText);
			writer.Flush();
			writer.Write(bytes, 0, bytes.Length);
		}

		private void setNumberFormatInternal(WordNumberFormat nfc)
		{
			_nfc = (byte)nfc;
		}
	}
}
