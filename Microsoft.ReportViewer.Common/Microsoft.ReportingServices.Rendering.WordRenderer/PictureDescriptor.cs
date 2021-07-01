using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal class PictureDescriptor
	{
		internal static byte[] INVALIDIMAGEDATA;

		internal const string BMP_MIME = "image/bmp";

		internal const string JPEG_MIME = "image/jpeg";

		internal const string PNG_MIME = "image/png";

		private const float INCH_IN_TWIPS = 1440f;

		private const float DEFAULT_DPI = 96f;

		private const ushort SHAPERECORD_ID = 61450;

		private const ushort OPTIONSRECORD_ID = 61451;

		private const ushort ANCHORRECORD_ID = 61456;

		private const ushort CONTAINERRECORD_ID = 61444;

		private const ushort BSERECORD_ID = 61447;

		private const string FILENAME = "..\\bl.jpg\0";

		internal const ushort BASE_SHAPE_ID = 1024;

		internal const ushort IDPROP_ID = 16644;

		private const ushort FILENAMEPROP_ID = 49413;

		private const ushort BLIPFLAGSPROP_ID = 262;

		private const ushort NOLINEPROP_ID = 511;

		private const int ASPECT_ID = 127;

		private const int ASPECT_NOT_LOCKED = 8388608;

		private const int RELATIVE_ID = 831;

		private const int IS_NOT_RELATIVE = 1048576;

		private static readonly BitField m_fFrameEmpty;

		private static readonly BitField m_fBitmap;

		private static readonly BitField m_fDrawHatch;

		private static readonly BitField m_fError;

		private static readonly BitField m_bpp;

		internal const int SIZE = 68;

		internal const int TIFF = 98;

		internal const int BMP = 99;

		internal const int ESCHER = 100;

		private int m_totalSize;

		private short m_picSize = 68;

		private short m_mappingMode;

		private short m_xExt;

		private short m_yExt;

		private short m_hMF;

		private byte[] m_metaData = new byte[14];

		private short m_dxaGoal;

		private short m_dyaGoal;

		private short m_mX;

		private short m_mY;

		private int m_dxaCropLeft;

		private int m_dyaCropTop;

		private int m_dxaCropRight;

		private int m_dyaCropBottom;

		private short m_info;

		private int m_brcTop;

		private int m_brcLeft;

		private int m_brcBottom;

		private int m_brcRight;

		private short m_xOrigin;

		private short m_yOrigin;

		private short m_cProps;

		private byte[] m_unknownData;

		private ArrayList m_recordList;

		private short m_originalHeight;

		private short m_originalWidth;

		private EscherBSERecord m_bse;

		private EscherSpRecord m_shape;

		private EscherOptRecord m_options;

		private byte[] m_imgData;

		private byte m_blipType = 1;

		private RPLFormat.Sizings m_sizing;

		private float m_xDensity;

		private float m_yDensity;

		private EscherClientAnchorRecord m_anchorRecord;

		static PictureDescriptor()
		{
			INVALIDIMAGEDATA = Microsoft.ReportingServices.InvalidImage.ImageData;
			m_fFrameEmpty = new BitField(16);
			m_fBitmap = new BitField(32);
			m_fDrawHatch = new BitField(64);
			m_fError = new BitField(64);
			m_bpp = new BitField(65280);
		}

		internal PictureDescriptor(byte[] imgData, byte[] hash, int aWidth, int aHeight, RPLFormat.Sizings sizing, int imgIndex)
		{
			m_mX = 1000;
			m_mY = 1000;
			m_dxaGoal = (short)aWidth;
			m_dyaGoal = (short)aHeight;
			m_originalWidth = m_dxaGoal;
			m_originalHeight = m_dyaGoal;
			m_mappingMode = 100;
			m_brcTop = 1073741824;
			m_brcLeft = 1073741824;
			m_brcBottom = 1073741824;
			m_brcRight = 1073741824;
			m_unknownData = new byte[0];
			m_sizing = sizing;
			m_imgData = imgData;
			if (m_imgData == null)
			{
				m_imgData = INVALIDIMAGEDATA;
			}
			ParseImageData();
			CreateDefaultEscherRecords(hash, imgIndex);
			if (sizing == RPLFormat.Sizings.Clip)
			{
				m_dxaCropLeft = 0;
				m_dyaCropTop = 0;
				m_dxaCropRight = m_dxaGoal - aWidth;
				m_dyaCropBottom = m_dyaGoal - aHeight;
			}
		}

		internal void Serialize(Stream data)
		{
			m_totalSize = 68 + m_unknownData.Length;
			for (int i = 0; i < m_recordList.Count; i++)
			{
				EscherRecord escherRecord = (EscherRecord)m_recordList[i];
				m_totalSize += escherRecord.RecordSize;
			}
			BinaryWriter binaryWriter = new BinaryWriter(data);
			binaryWriter.Write(m_totalSize);
			binaryWriter.Write(m_picSize);
			binaryWriter.Write(m_mappingMode);
			binaryWriter.Write(m_xExt);
			binaryWriter.Write(m_yExt);
			binaryWriter.Write(m_hMF);
			binaryWriter.Write(m_metaData);
			binaryWriter.Write(m_dxaGoal);
			binaryWriter.Write(m_dyaGoal);
			binaryWriter.Write(m_mX);
			binaryWriter.Write(m_mY);
			binaryWriter.Write((short)m_dxaCropLeft);
			binaryWriter.Write((short)m_dyaCropTop);
			binaryWriter.Write((short)m_dxaCropRight);
			binaryWriter.Write((short)m_dyaCropBottom);
			binaryWriter.Write(m_info);
			binaryWriter.Write(m_brcTop);
			binaryWriter.Write(m_brcLeft);
			binaryWriter.Write(m_brcBottom);
			binaryWriter.Write(m_brcRight);
			binaryWriter.Write(m_xOrigin);
			binaryWriter.Write(m_yOrigin);
			binaryWriter.Write(m_cProps);
			if (m_unknownData.Length != 0)
			{
				binaryWriter.Write(m_unknownData);
			}
			for (int j = 0; j < m_recordList.Count; j++)
			{
				((EscherRecord)m_recordList[j]).Serialize(binaryWriter);
			}
			binaryWriter.Flush();
		}

		private void CreateDefaultEscherRecords(byte[] hash, int imgIndex)
		{
			m_recordList = new ArrayList(2);
			EscherContainerRecord escherContainerRecord = new EscherContainerRecord();
			escherContainerRecord.SetRecordId(61444);
			escherContainerRecord.setOptions(15);
			m_shape = new EscherSpRecord();
			m_shape.ShapeId = 1025;
			m_shape.Flags = 2560;
			m_shape.setOptions(1202);
			m_shape.SetRecordId(61450);
			escherContainerRecord.addChildRecord(m_shape);
			m_options = new EscherOptRecord();
			m_options.setOptions(67);
			m_options.SetRecordId(61451);
			m_options.addEscherProperty(new EscherSimpleProperty(16644, 1));
			try
			{
				EscherComplexProperty prop = new EscherComplexProperty(49413, Encoding.GetEncoding("utf-16").GetBytes("..\\bl.jpg\0"));
				m_options.addEscherProperty(prop);
			}
			catch (IOException innerException)
			{
				throw new ReportRenderingException(innerException);
			}
			m_options.addEscherProperty(new EscherSimpleProperty(262, 2));
			m_options.addEscherProperty(new EscherBoolProperty(511, 524288));
			escherContainerRecord.addChildRecord(m_options);
			m_anchorRecord = new EscherClientAnchorRecord();
			m_anchorRecord.RemainingData = new byte[4]
			{
				0,
				0,
				0,
				8
			};
			m_anchorRecord.ShortRecord = true;
			m_anchorRecord.setOptions(0);
			m_anchorRecord.SetRecordId(61456);
			escherContainerRecord.addChildRecord(m_anchorRecord);
			m_recordList.Add(escherContainerRecord);
			m_bse = new EscherBSERecord();
			m_bse.Unused2 = 9;
			m_bse.Unused3 = 1;
			m_bse.Tag = 255;
			m_bse.Ref = 1;
			m_bse.SetRecordId(61447);
			m_bse.SubRecord = new EscherBSESubRecord();
			InitImage(hash, imgIndex);
			m_recordList.Add(m_bse);
		}

		private void ParseImageData()
		{
			if (m_imgData == null)
			{
				return;
			}
			bool flag = false;
			try
			{
				using (System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(m_imgData)))
				{
					m_dxaGoal = (short)((float)image.Width / image.HorizontalResolution * 1440f);
					m_dyaGoal = (short)((float)image.Height / image.VerticalResolution * 1440f);
					m_xDensity = image.HorizontalResolution;
					m_yDensity = image.VerticalResolution;
					if (image.RawFormat == ImageFormat.Jpeg)
					{
						m_blipType = 5;
					}
					else if (image.RawFormat == ImageFormat.Png)
					{
						m_blipType = 6;
					}
					else
					{
						using (MemoryStream memoryStream = new MemoryStream())
						{
							image.Save(memoryStream, ImageFormat.Png);
							m_imgData = memoryStream.ToArray();
						}
						m_blipType = 6;
					}
				}
			}
			catch (ArgumentException)
			{
				flag = true;
			}
			catch (ExternalException)
			{
				flag = true;
			}
			if (m_imgData != INVALIDIMAGEDATA && flag)
			{
				m_imgData = INVALIDIMAGEDATA;
				ParseImageData();
			}
		}

		private void InitImage(byte[] aHash, int imgIndex)
		{
			InitSizing();
			m_bse.BlipTypeMacOS = m_blipType;
			m_bse.BlipTypeWin32 = m_blipType;
			m_bse.setOptions((ushort)((m_blipType << 4) | 2));
			m_bse.Uid = aHash;
			m_bse.Size = m_imgData.Length + 25;
			EscherBSESubRecord subRecord = m_bse.SubRecord;
			subRecord.Hash = aHash;
			ushort num = 0;
			ushort num2 = 0;
			byte blipType = m_blipType;
			if (blipType == 5)
			{
				num = 61469;
				num2 = 18080;
			}
			else
			{
				num = 61470;
				num2 = 28160;
			}
			subRecord.Image = m_imgData;
			subRecord.SetRecordId(num);
			subRecord.setOptions(num2);
			if (m_xDensity == 0f)
			{
				m_xDensity = 96f;
			}
			if (m_yDensity == 0f)
			{
				m_yDensity = 96f;
			}
			InitIndex(imgIndex);
		}

		private void InitIndex(int index)
		{
			int num = 1 + index;
			int shapeId = 1024 + num;
			m_shape.ShapeId = shapeId;
			((EscherSimpleProperty)m_options.getEscherPropertyByID(16644)).PropertyValue = num;
		}

		private void InitSizing()
		{
			switch (m_sizing)
			{
			case RPLFormat.Sizings.FitProportional:
			{
				float num = (float)m_originalWidth / (float)m_dxaGoal;
				float num2 = (float)m_originalHeight / (float)m_dyaGoal;
				if (num2 < num)
				{
					num = num2;
				}
				m_dyaGoal = (short)((float)m_dyaGoal * num);
				m_dxaGoal = (short)((float)m_dxaGoal * num);
				break;
			}
			case RPLFormat.Sizings.Fit:
				m_dxaGoal = m_originalWidth;
				m_dyaGoal = m_originalHeight;
				break;
			case RPLFormat.Sizings.Clip:
				m_dxaCropLeft = 0;
				m_dyaCropTop = 0;
				m_dxaCropRight = m_dxaGoal - m_originalWidth;
				m_dyaCropBottom = m_dyaGoal - m_originalWidth;
				break;
			}
		}
	}
}
