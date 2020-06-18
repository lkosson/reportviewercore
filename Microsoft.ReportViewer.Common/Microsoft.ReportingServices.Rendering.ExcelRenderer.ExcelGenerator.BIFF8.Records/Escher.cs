using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8;
using Microsoft.ReportingServices.Rendering.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records
{
	internal sealed class Escher
	{
		internal class EscherHeader
		{
			private uint m_escherHeader;

			private uint m_cbLength;

			internal const uint CbLengthOffset = 4u;

			internal uint Instance
			{
				set
				{
					if (value < 4096)
					{
						m_escherHeader &= 4294901775u;
						m_escherHeader |= value << 4;
					}
				}
			}

			internal virtual uint Length
			{
				get
				{
					return m_cbLength;
				}
				set
				{
					m_cbLength = value;
				}
			}

			internal EscherHeader(ushort ver, uint inst, EscherType fbt, uint cbLength)
			{
				if (ver < 16)
				{
					m_escherHeader = ver;
				}
				if (inst < 4096)
				{
					m_escherHeader |= inst << 4;
				}
				if (65535 >= (int)fbt)
				{
					m_escherHeader = ((ushort)m_escherHeader | ((uint)fbt << 16));
				}
				m_cbLength = cbLength;
			}

			internal virtual byte[] GetData()
			{
				byte[] array = new byte[8];
				LittleEndianHelper.WriteIntU(m_escherHeader, array, 0);
				LittleEndianHelper.WriteIntU(m_cbLength, array, 4);
				return array;
			}
		}

		internal sealed class DrawingGroupContainer : EscherHeader
		{
			internal sealed class CheckSumImage
			{
				private byte[] m_checkSum;

				private int m_streamIndex;

				internal byte[] CheckSum => m_checkSum;

				internal int StreamIndex => m_streamIndex;

				internal CheckSumImage(byte[] checkSum, int streamIndex)
				{
					m_checkSum = checkSum;
					m_streamIndex = streamIndex;
				}
			}

			internal class ImageStream
			{
				private Stream m_stream;

				private string m_name;

				private int m_offset;

				internal Stream Stream => m_stream;

				internal string Name => m_name;

				internal int Offset
				{
					get
					{
						return m_offset;
					}
					set
					{
						RSTrace.ExcelRendererTracer.Assert(value < m_stream.Length, "The current position in the stream cannot exceed the stream length");
						m_offset = value;
					}
				}

				internal ImageStream(Stream stream, string name, EscherType escherType)
				{
					m_stream = stream;
					m_name = name;
					if (escherType == EscherType.MSOFBTBLIP_DIB)
					{
						m_offset = 14;
					}
				}
			}

			private DrawingGroup m_drawingGroup;

			private BlipStoreContainer m_bStoreContainer;

			private List<ImageStream> m_images;

			private Dictionary<int, ushort> m_clusters;

			private Dictionary<string, CheckSumImage> m_imageTable;

			internal const int BitmapFileHeaderSize = 14;

			internal List<ImageStream> StreamList => m_images;

			internal override uint Length => 50 + m_bStoreContainer.Length + m_drawingGroup.Length + 8;

			internal byte[] DrawingGroupContainerData
			{
				get
				{
					base.Length = Length;
					return GetData();
				}
			}

			internal byte[] DrawingGroupData
			{
				get
				{
					if (m_drawingGroup == null)
					{
						return null;
					}
					return m_drawingGroup.GetData();
				}
			}

			internal byte[] BStoreContainerData
			{
				get
				{
					if (m_bStoreContainer == null)
					{
						return null;
					}
					return m_bStoreContainer.GetData();
				}
			}

			internal Hashtable BSEList
			{
				get
				{
					if (m_bStoreContainer == null)
					{
						return null;
					}
					return m_bStoreContainer.BSEList;
				}
			}

			internal ArrayList BlipList
			{
				get
				{
					if (m_bStoreContainer == null)
					{
						return null;
					}
					return m_bStoreContainer.BlipList;
				}
			}

			internal byte[] ShapePropertyData => ShapeProperty.GetData();

			internal DrawingGroupContainer()
				: base(15, 0u, EscherType.MSOFBTDGGCONTAINER, 0u)
			{
				m_drawingGroup = new DrawingGroup();
			}

			private void UpdateImageStreamDGC(string name, Stream imageData, EscherType escherType, out int streamIndex)
			{
				if (m_images != null)
				{
					for (int i = 0; i < m_images.Count; i++)
					{
						if (m_images[i].Name.Equals(name))
						{
							streamIndex = i;
							return;
						}
					}
				}
				else
				{
					m_images = new List<ImageStream>();
				}
				ImageStream item = new ImageStream(imageData, name, escherType);
				m_images.Add(item);
				streamIndex = m_images.Count - 1;
			}

			internal uint AddImage(Stream imageData, ImageFormat format, string imageName, int workSheetId, out uint startSPID, out ushort dgID)
			{
				EscherType escherType = EscherType.MSOFBTUNKNOWN;
				BlipType blipType = BlipType.MSOBLIPUNKNOWN;
				BlipSignature blipSignature = BlipSignature.MSOBIUNKNOWN;
				if (format.Equals(ImageFormat.Bmp))
				{
					escherType = EscherType.MSOFBTBLIP_DIB;
					blipType = BlipType.MSOBLIPDIB;
					blipSignature = BlipSignature.MSOBIDIB;
				}
				else if (format.Equals(ImageFormat.Jpeg))
				{
					escherType = EscherType.MSOFBTBLIP_JPEG;
					blipType = BlipType.MSOBLIPJPEG;
					blipSignature = BlipSignature.MSOBIJFIF;
				}
				else if (format.Equals(ImageFormat.Gif))
				{
					escherType = EscherType.MSOFBTBLIP_GIF;
					blipType = BlipType.MSOBLIPPNG;
					blipSignature = BlipSignature.MSOBIPNG;
				}
				else if (format.Equals(ImageFormat.Png))
				{
					escherType = EscherType.MSOFBTBLIP_GIF;
					blipType = BlipType.MSOBLIPPNG;
					blipSignature = BlipSignature.MSOBIPNG;
				}
				if (m_clusters == null)
				{
					m_clusters = new Dictionary<int, ushort>();
				}
				if (m_bStoreContainer == null)
				{
					m_bStoreContainer = new BlipStoreContainer();
				}
				if (m_imageTable == null)
				{
					m_imageTable = new Dictionary<string, CheckSumImage>();
				}
				int num = (int)imageData.Length;
				CheckSumImage checkSumImage;
				if (m_imageTable.ContainsKey(imageName))
				{
					checkSumImage = m_imageTable[imageName];
				}
				else
				{
					byte[] checkSum = CheckSum(imageData);
					int streamIndex = m_bStoreContainer.GetStreamPosFromCheckSum(checkSum);
					if (streamIndex == -1)
					{
						UpdateImageStreamDGC(imageName, imageData, escherType, out streamIndex);
					}
					checkSumImage = new CheckSumImage(checkSum, streamIndex);
					m_imageTable.Add(imageName, checkSumImage);
				}
				if (escherType == EscherType.MSOFBTBLIP_DIB)
				{
					num -= 14;
					StreamList[checkSumImage.StreamIndex].Offset = 14;
				}
				uint result = AddImage(checkSumImage.CheckSum, checkSumImage.StreamIndex, num, escherType, blipType, blipSignature, workSheetId);
				dgID = m_clusters[workSheetId];
				startSPID = m_drawingGroup.GetStartingSPID(dgID);
				return result;
			}

			private uint AddImage(byte[] checkSum, int streamIndex, int imageLength, EscherType escherType, BlipType blipType, BlipSignature blipSignature, int workSheetId)
			{
				if (m_clusters.ContainsKey(workSheetId))
				{
					int dgID = m_clusters[workSheetId];
					m_drawingGroup.IncrementShapeCount(dgID);
				}
				else
				{
					m_clusters.Add(workSheetId, (ushort)(m_clusters.Count + 1));
					int dgID = m_clusters.Count;
					m_drawingGroup.AddCluster(dgID);
					m_drawingGroup.IncrementShapeCount(dgID);
				}
				return m_bStoreContainer.AddImage(checkSum, streamIndex, imageLength, escherType, blipType, blipSignature);
			}
		}

		internal sealed class FIDCL
		{
			internal uint m_dgid;

			internal uint m_cspidCurr;

			internal FIDCL(uint dgid, uint cspidCurr)
			{
				m_dgid = dgid;
				m_cspidCurr = cspidCurr;
			}
		}

		internal sealed class DrawingGroup : EscherHeader
		{
			private ArrayList m_dgCluster;

			private uint m_cdgSaved;

			private const uint FixedRecordLength = 16u;

			internal override uint Length => 16 + m_cdgSaved * 8;

			internal DrawingGroup()
				: base(0, 0u, EscherType.MSOFBTDGG, 0u)
			{
			}

			internal uint GetStartingSPID(int dgID)
			{
				if (m_dgCluster == null || dgID < 1)
				{
					return 0u;
				}
				if (dgID == 1)
				{
					return 1024u;
				}
				return m_cdgSaved * 1024;
			}

			internal void AddCluster(int dgID)
			{
				if (m_dgCluster == null)
				{
					m_dgCluster = new ArrayList();
				}
				FIDCL value = new FIDCL((uint)dgID, 1u);
				if (dgID > m_dgCluster.Count)
				{
					ArrayList arrayList = new ArrayList();
					arrayList.Add(value);
					m_dgCluster.Add(arrayList);
				}
				else
				{
					((ArrayList)m_dgCluster[dgID - 1]).Add(value);
				}
				m_cdgSaved++;
			}

			internal uint GetCurrentSpid(int dgID)
			{
				if (m_dgCluster == null || dgID < 1 || dgID > m_dgCluster.Count)
				{
					return 0u;
				}
				ArrayList obj = (ArrayList)m_dgCluster[dgID - 1];
				return ((FIDCL)obj[obj.Count - 1]).m_cspidCurr;
			}

			internal void IncrementShapeCount(int dgID)
			{
				if (m_dgCluster != null && dgID >= 1 && dgID <= m_dgCluster.Count)
				{
					if (GetCurrentSpid(dgID) % 1024u == 0)
					{
						AddCluster(dgID);
						return;
					}
					ArrayList obj = (ArrayList)m_dgCluster[dgID - 1];
					((FIDCL)obj[obj.Count - 1]).m_cspidCurr++;
				}
			}

			internal override byte[] GetData()
			{
				byte[] array = new byte[Length + 8];
				base.Length = Length;
				int num = 0;
				byte[] data = base.GetData();
				data.CopyTo(array, num);
				num += data.Length;
				ArrayList arrayList = (ArrayList)m_dgCluster[m_dgCluster.Count - 1];
				data = BitConverter.GetBytes(m_cdgSaved * 1024 + ((FIDCL)arrayList[arrayList.Count - 1]).m_cspidCurr);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_cdgSaved + 1);
				data.CopyTo(array, num);
				num += data.Length;
				uint num2 = 0u;
				int num3 = 0;
				byte[] array2 = new byte[m_cdgSaved * 8];
				for (int i = 0; i < m_dgCluster.Count; i++)
				{
					ArrayList arrayList2 = (ArrayList)m_dgCluster[i];
					for (int j = 0; j < arrayList2.Count; j++)
					{
						data = BitConverter.GetBytes(((FIDCL)arrayList2[j]).m_dgid);
						data.CopyTo(array2, num3);
						num3 += data.Length;
						num2 += ((FIDCL)arrayList2[j]).m_cspidCurr;
						data = BitConverter.GetBytes(((FIDCL)arrayList2[j]).m_cspidCurr);
						data.CopyTo(array2, num3);
						num3 += data.Length;
					}
				}
				data = BitConverter.GetBytes(num2);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_dgCluster.Count);
				data.CopyTo(array, num);
				num += data.Length;
				array2.CopyTo(array, num);
				return array;
			}
		}

		internal class BlipStoreContainer : EscherHeader
		{
			private Hashtable m_bSEList;

			private ArrayList m_blipList;

			private int m_totalLength;

			private const int BSELength = 44;

			internal Hashtable BSEList => m_bSEList;

			internal ArrayList BlipList => m_blipList;

			internal override uint Length => (uint)(8 + m_totalLength);

			internal uint ShapeCount => (uint)m_blipList.Count;

			internal BlipStoreContainer()
				: base(15, 0u, EscherType.MSOFBTBSTORECONTAINER, 0u)
			{
			}

			internal uint AddImage(byte[] checkSum, int streamIndex, int imageLength, EscherType escherType, BlipType blipType, BlipSignature blipSignature)
			{
				if (m_bSEList == null)
				{
					m_bSEList = new Hashtable();
				}
				string @string = Encoding.ASCII.GetString(checkSum);
				if (m_bSEList.ContainsKey(@string))
				{
					BlipStoreEntry obj = (BlipStoreEntry)m_bSEList[@string];
					obj.ReferenceCount++;
					return obj.ReferenceIndex;
				}
				if (m_blipList == null)
				{
					m_blipList = new ArrayList();
				}
				Blip blip = new Blip(checkSum, streamIndex, imageLength, escherType, blipSignature);
				m_blipList.Add(blip);
				base.Instance = (uint)m_blipList.Count;
				BlipStoreEntry value = new BlipStoreEntry(checkSum, blipType, blip.Length, (uint)m_blipList.Count);
				m_bSEList.Add(@string, value);
				m_totalLength += 44 + imageLength + 8 + 16 + 1;
				return (uint)m_blipList.Count;
			}

			internal int GetStreamPosFromCheckSum(byte[] checkSum)
			{
				if (m_bSEList == null || !m_bSEList.ContainsKey(Encoding.ASCII.GetString(checkSum)))
				{
					return -1;
				}
				BlipStoreEntry blipStoreEntry = (BlipStoreEntry)m_bSEList[Encoding.ASCII.GetString(checkSum)];
				return ((Blip)m_blipList[(int)(blipStoreEntry.ReferenceIndex - 1)]).StreamIndex;
			}

			internal override byte[] GetData()
			{
				if (m_blipList == null)
				{
					return null;
				}
				base.Length = (uint)m_totalLength;
				return base.GetData();
			}
		}

		internal sealed class BlipStoreEntry : EscherHeader
		{
			private byte m_btWin32;

			private byte m_btMacOS;

			private byte[] m_rgbUID;

			private ushort m_tag = 255;

			private uint m_size;

			private uint m_cRef;

			private uint m_MSOFO;

			private byte usage;

			private byte cbName;

			private byte unused2;

			private byte unused3;

			private uint m_referenceIndex;

			private const ushort RecordLength = 36;

			internal uint ReferenceIndex => m_referenceIndex;

			internal uint ReferenceCount
			{
				get
				{
					return m_cRef;
				}
				set
				{
					m_cRef = value;
				}
			}

			internal BlipStoreEntry(byte[] checkSum, BlipType blipType, uint atomLength, uint referenceIndex)
				: base(2, (uint)blipType, EscherType.MSOFBTBSE, (ushort)(atomLength + 36 + 8))
			{
				m_btWin32 = (byte)blipType;
				m_btMacOS = (byte)blipType;
				m_rgbUID = checkSum;
				m_size = atomLength + 8;
				m_referenceIndex = referenceIndex;
				m_cRef = 1u;
			}

			internal override byte[] GetData()
			{
				byte[] array = new byte[44];
				int num = 0;
				byte[] data = base.GetData();
				data.CopyTo(array, num);
				num += data.Length;
				array[num] = m_btWin32;
				num++;
				array[num] = m_btMacOS;
				num++;
				m_rgbUID.CopyTo(array, num);
				num += m_rgbUID.Length;
				data = BitConverter.GetBytes(m_tag);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_size);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_cRef);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_MSOFO);
				data.CopyTo(array, num);
				num += data.Length;
				array[num] = usage;
				num++;
				array[num] = cbName;
				num++;
				array[num] = unused2;
				num++;
				array[num] = unused3;
				return array;
			}
		}

		internal sealed class Blip : EscherHeader
		{
			private byte[] m_rgbUID;

			private byte m_bTag;

			private int m_imageLength;

			private int m_streamIndex = -1;

			internal int StreamIndex => m_streamIndex;

			internal override uint Length => (uint)(m_imageLength + 16 + 1);

			internal byte[] CheckSum => m_rgbUID;

			internal Blip(byte[] checkSum, int streamIndex, int imageLength, EscherType escherType, BlipSignature blipSignature)
				: base(0, (uint)blipSignature, escherType, (uint)(imageLength + 16 + 1))
			{
				m_rgbUID = checkSum;
				m_bTag = byte.MaxValue;
				m_streamIndex = streamIndex;
				m_imageLength = imageLength;
			}

			internal byte[] GetHeaderData()
			{
				byte[] array = new byte[8 + m_rgbUID.Length + 1];
				int num = 0;
				byte[] data = GetData();
				data.CopyTo(array, num);
				num += data.Length;
				m_rgbUID.CopyTo(array, num);
				num += m_rgbUID.Length;
				array[num] = m_bTag;
				return array;
			}
		}

		internal sealed class ShapeProperty
		{
			internal static byte[] GetData()
			{
				return new byte[50]
				{
					51,
					0,
					11,
					240,
					18,
					0,
					0,
					0,
					191,
					0,
					8,
					0,
					8,
					0,
					129,
					1,
					65,
					0,
					0,
					8,
					192,
					1,
					64,
					0,
					0,
					8,
					64,
					0,
					30,
					241,
					16,
					0,
					0,
					0,
					13,
					0,
					0,
					8,
					12,
					0,
					0,
					8,
					23,
					0,
					0,
					8,
					247,
					0,
					0,
					16
				};
			}
		}

		internal sealed class DrawingContainer : EscherHeader
		{
			private Drawing m_drawing;

			private ShapeGroupContainer m_shapeGroupContainer;

			private ArrayList m_shapeContainer;

			internal DrawingContainer(ushort drawingID)
				: base(15, 0u, EscherType.MSOFBTDGCONTAINER, 0u)
			{
				m_drawing = new Drawing(drawingID);
				m_shapeGroupContainer = new ShapeGroupContainer();
			}

			internal int AddShape(uint spid, string imageName, ClientAnchor.SPRC clientAnchorInfo, uint referenceIndex)
			{
				if (m_shapeContainer == null)
				{
					m_shapeContainer = new ArrayList();
				}
				if (m_shapeContainer.Count == 0)
				{
					uint spid2 = spid / 1024u * 1024;
					m_shapeContainer.Add(new ShapeContainer(spid2, ShapeType.MSOSPTMIN, (ShapeFlag)5));
				}
				m_shapeContainer.Add(new ShapeContainer(spid, ShapeType.MSOSPTPICTUREFRAME, (ShapeFlag)2560, clientAnchorInfo, referenceIndex, imageName));
				m_drawing.LastSPID = spid;
				m_drawing.ShapeCount = (uint)m_shapeContainer.Count;
				return m_shapeContainer.Count;
			}

			internal int AddShape(uint spid, string imageName, ClientAnchor.SPRC clientAnchorInfo, uint referenceIndex, string hyperLinkName, HyperlinkType hyperLinkType)
			{
				if (m_shapeContainer == null)
				{
					m_shapeContainer = new ArrayList();
				}
				if (m_shapeContainer.Count == 0)
				{
					uint spid2 = spid / 1024u * 1024;
					m_shapeContainer.Add(new ShapeContainer(spid2, ShapeType.MSOSPTMIN, (ShapeFlag)5));
				}
				m_shapeContainer.Add(new ShapeContainer(spid, ShapeType.MSOSPTPICTUREFRAME, (ShapeFlag)2560, clientAnchorInfo, referenceIndex, imageName, hyperLinkName, hyperLinkType));
				m_drawing.LastSPID = spid;
				m_drawing.ShapeCount = (uint)m_shapeContainer.Count;
				return m_shapeContainer.Count;
			}

			internal override byte[] GetData()
			{
				return null;
			}

			internal void WriteToStream(BinaryWriter output)
			{
				if (m_shapeContainer == null)
				{
					return;
				}
				long position = output.BaseStream.Position;
				RecordFactory.WriteHeader(output, 236, 0);
				int num = 0;
				long position2 = output.BaseStream.Position;
				byte[] data = base.GetData();
				output.BaseStream.Write(data, 0, data.Length);
				num += data.Length;
				data = m_drawing.GetData();
				output.BaseStream.Write(data, 0, data.Length);
				num += data.Length;
				long position3 = output.BaseStream.Position + 4;
				data = m_shapeGroupContainer.GetData();
				output.BaseStream.Write(data, 0, data.Length);
				num += data.Length;
				int num2 = 0;
				int num3 = num;
				ushort num4 = 1;
				for (int i = 0; i < m_shapeContainer.Count; i++)
				{
					data = ((ShapeContainer)m_shapeContainer[i]).GetData();
					if (i < 2)
					{
						num += data.Length;
					}
					else
					{
						num4 = (ushort)(num4 + 1);
						RecordFactory.WriteHeader(output, 236, data.Length);
					}
					output.BaseStream.Write(data, 0, data.Length);
					num2 += data.Length;
					num3 += data.Length;
					if (i > 0)
					{
						RecordFactory.OBJ(output, num4);
					}
				}
				long position4 = output.BaseStream.Position;
				int value = num3 - 8;
				output.BaseStream.Position = position2 + 4;
				byte[] bytes = BitConverter.GetBytes((uint)value);
				output.BaseStream.Write(bytes, 0, bytes.Length);
				output.BaseStream.Position = position3;
				bytes = BitConverter.GetBytes((uint)num2);
				output.BaseStream.Write(bytes, 0, bytes.Length);
				bytes = BitConverter.GetBytes((ushort)num);
				output.BaseStream.Position = position + 2;
				output.BaseStream.Write(bytes, 0, bytes.Length);
				output.BaseStream.Position = position4;
			}
		}

		internal sealed class Drawing : EscherHeader
		{
			private uint m_csp;

			private uint m_spidCur;

			internal uint LastSPID
			{
				set
				{
					m_spidCur = value;
				}
			}

			internal uint ShapeCount
			{
				set
				{
					m_csp = value;
				}
			}

			internal Drawing(ushort drawingID)
				: base(0, drawingID, EscherType.MSOFBTDG, 8u)
			{
			}

			internal override byte[] GetData()
			{
				byte[] array = new byte[16];
				int num = 0;
				byte[] data = base.GetData();
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_csp);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_spidCur);
				data.CopyTo(array, num);
				return array;
			}
		}

		internal sealed class ShapeGroupContainer : EscherHeader
		{
			internal ShapeGroupContainer()
				: base(15, 0u, EscherType.MSOFBTSPGRCONTAINER, 0u)
			{
			}

			internal override byte[] GetData()
			{
				byte[] array = new byte[8];
				int index = 0;
				base.GetData().CopyTo(array, index);
				return array;
			}
		}

		internal sealed class ShapeContainer : EscherHeader
		{
			private ShapeGroup m_shapeGroup;

			private Shape m_shape;

			private DrawingOpt m_drawingOpt;

			private ClientAnchor m_clientAnchor;

			private ClientData m_clientData;

			internal ShapeContainer(uint spid, ShapeType shapeType, ShapeFlag shapeFlags)
				: base(15, 0u, EscherType.MSOFBTSPCONTAINER, 0u)
			{
				m_shapeGroup = new ShapeGroup(0u, 0u, 0u, 0u);
				m_shape = new Shape(shapeType, shapeFlags, spid);
			}

			internal ShapeContainer(uint spid, ShapeType shapeType, ShapeFlag shapeFlags, ClientAnchor.SPRC clientAnchorInfo, uint refIndex, string imageName)
				: base(15, 0u, EscherType.MSOFBTSPCONTAINER, 0u)
			{
				m_shape = new Shape(shapeType, shapeFlags, spid);
				m_drawingOpt = new DrawingOpt(imageName, refIndex);
				m_clientAnchor = new ClientAnchor(clientAnchorInfo);
				m_clientData = new ClientData();
			}

			internal ShapeContainer(uint spid, ShapeType shapeType, ShapeFlag shapeFlags, ClientAnchor.SPRC clientAnchorInfo, uint refIndex, string imageName, string hyperLinkName, HyperlinkType hyperLinkType)
				: base(15, 0u, EscherType.MSOFBTSPCONTAINER, 0u)
			{
				m_shape = new Shape(shapeType, shapeFlags, spid);
				m_drawingOpt = new DrawingOpt(imageName, refIndex, hyperLinkName, hyperLinkType);
				m_clientAnchor = new ClientAnchor(clientAnchorInfo);
				m_clientData = new ClientData();
			}

			internal override byte[] GetData()
			{
				MemoryStream memoryStream = new MemoryStream();
				byte[] data = base.GetData();
				memoryStream.Write(data, 0, data.Length);
				if (m_shapeGroup != null)
				{
					data = m_shapeGroup.GetData();
					memoryStream.Write(data, 0, data.Length);
					data = m_shape.GetData();
					memoryStream.Write(data, 0, data.Length);
				}
				else
				{
					data = m_shape.GetData();
					memoryStream.Write(data, 0, data.Length);
					data = m_drawingOpt.GetData();
					memoryStream.Write(data, 0, data.Length);
					data = m_clientAnchor.GetData();
					memoryStream.Write(data, 0, data.Length);
					data = m_clientData.GetData();
					memoryStream.Write(data, 0, data.Length);
				}
				BitConverter.GetBytes((uint)(memoryStream.Length - 8)).CopyTo(memoryStream.GetBuffer(), 4L);
				memoryStream.Position = 0L;
				return memoryStream.ToArray();
			}
		}

		internal sealed class ShapeGroup : EscherHeader
		{
			private uint m_left;

			private uint m_top;

			private uint m_right;

			private uint m_bottom;

			internal override uint Length => 16u;

			internal ShapeGroup(uint left, uint right, uint top, uint bottom)
				: base(1, 0u, EscherType.MSOFBTSPGR, 16u)
			{
				m_left = left;
				m_top = top;
				m_right = right;
				m_bottom = bottom;
			}

			internal override byte[] GetData()
			{
				byte[] array = new byte[24];
				int num = 0;
				byte[] data = base.GetData();
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_left);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_top);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_right);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_bottom);
				data.CopyTo(array, num);
				return array;
			}
		}

		internal sealed class Shape : EscherHeader
		{
			private uint m_spid;

			private ShapeFlag m_shapeFlag;

			internal override uint Length => 8u;

			internal Shape(ShapeType shapeType, ShapeFlag shapeFlags, uint spid)
				: base(2, (uint)shapeType, EscherType.MSOFBTSP, 8u)
			{
				m_spid = spid;
				m_shapeFlag = shapeFlags;
			}

			internal override byte[] GetData()
			{
				byte[] array = new byte[16];
				int num = 0;
				byte[] data = base.GetData();
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_spid);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes((uint)m_shapeFlag);
				data.CopyTo(array, num);
				return array;
			}
		}

		internal sealed class DrawingOpt : EscherHeader
		{
			private string m_imageName;

			private uint m_referenceIndex;

			private string m_hyperLinkName;

			private HyperlinkType m_hyperLinkType;

			private const ushort PropertyIDShapeCount = 16644;

			private const ushort PropertyIDImageName = 49413;

			private const ushort PropertyPihlShape = 50050;

			private const ushort BookMarkLength = 48;

			private const ushort HyperlinkLength = 64;

			private const int RecordLength = 14;

			internal DrawingOpt(string imageName, uint refIndex)
				: base(3, 2u, EscherType.MSOFBTOPT, 0u)
			{
				m_imageName = imageName;
				m_referenceIndex = refIndex;
			}

			internal DrawingOpt(string imageName, uint refIndex, string hyperLinkName, HyperlinkType hyperLinkType)
				: base(3, 5u, EscherType.MSOFBTOPT, 0u)
			{
				m_imageName = imageName;
				m_referenceIndex = refIndex;
				m_hyperLinkName = hyperLinkName;
				m_hyperLinkType = hyperLinkType;
			}

			internal override byte[] GetData()
			{
				int num = 14 + m_imageName.Length * 2;
				if (m_hyperLinkName != null && m_hyperLinkName.Length > 0)
				{
					num = ((m_hyperLinkType != HyperlinkType.BOOKMARK) ? (num + (m_hyperLinkName.Length * 2 + 64)) : (num + (m_hyperLinkName.Length * 2 + 48)));
				}
				Length = (uint)num;
				byte[] array = new byte[num + 8];
				int num2 = 0;
				byte[] data = base.GetData();
				data.CopyTo(array, num2);
				num2 += data.Length;
				data = BitConverter.GetBytes((ushort)16644);
				data.CopyTo(array, num2);
				num2 += data.Length;
				data = BitConverter.GetBytes(m_referenceIndex);
				data.CopyTo(array, num2);
				num2 += data.Length;
				data = BitConverter.GetBytes((ushort)49413);
				data.CopyTo(array, num2);
				num2 += data.Length;
				data = BitConverter.GetBytes((uint)(m_imageName.Length * 2 + 2));
				data.CopyTo(array, num2);
				num2 += data.Length;
				if (m_hyperLinkName != null && m_hyperLinkName.Length > 0)
				{
					byte[] array2 = new byte[6]
					{
						191,
						1,
						1,
						0,
						1,
						0
					};
					byte[] obj = new byte[6]
					{
						191,
						3,
						8,
						0,
						8,
						0
					};
					array2.CopyTo(array, num2);
					num2 += 6;
					data = BitConverter.GetBytes((ushort)50050);
					data.CopyTo(array, num2);
					num2 += data.Length;
					uint value = (uint)((m_hyperLinkType != HyperlinkType.BOOKMARK) ? (44 + (m_hyperLinkName.Length + 1) * 2) : (28 + (m_hyperLinkName.Length + 1) * 2));
					data = BitConverter.GetBytes(value);
					data.CopyTo(array, num2);
					num2 += data.Length;
					obj.CopyTo(array, num2);
					num2 += 6;
				}
				UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
				data = unicodeEncoding.GetBytes(m_imageName);
				data.CopyTo(array, num2);
				num2 += data.Length + 2;
				if (m_hyperLinkName != null && m_hyperLinkName.Length > 0)
				{
					data = new Guid("79EAC9D0-BAF9-11CE-8C82-00AA004BA90B").ToByteArray();
					data.CopyTo(array, num2);
					num2 += data.Length;
					uint value2;
					if (m_hyperLinkType == HyperlinkType.BOOKMARK)
					{
						value2 = (uint)(m_hyperLinkName.Length + 1);
						uint value3 = 8u;
						data = BitConverter.GetBytes(2u);
						data.CopyTo(array, num2);
						num2 += data.Length;
						data = BitConverter.GetBytes(value3);
						data.CopyTo(array, num2);
						num2 += data.Length;
					}
					else
					{
						value2 = (uint)((m_hyperLinkName.Length + 1) * 2);
						uint value4 = 3u;
						data = BitConverter.GetBytes(2u);
						data.CopyTo(array, num2);
						num2 += data.Length;
						data = BitConverter.GetBytes(value4);
						data.CopyTo(array, num2);
						num2 += data.Length;
						data = new Guid("79EAC9E0-BAF9-11CE-8C82-00AA004BA90B").ToByteArray();
						data.CopyTo(array, num2);
						num2 += data.Length;
					}
					data = BitConverter.GetBytes(value2);
					data.CopyTo(array, num2);
					num2 += data.Length;
					data = unicodeEncoding.GetBytes(m_hyperLinkName);
					data.CopyTo(array, num2);
				}
				return array;
			}
		}

		internal sealed class ClientAnchor : EscherHeader
		{
			internal sealed class SPRC
			{
				internal sealed class ORC
				{
					internal ushort m_colL;

					internal short m_dxL;

					internal ushort m_rwT;

					internal short m_dyT;

					internal ushort m_colR;

					internal short m_dxR;

					internal ushort m_rwB;

					internal short m_dyB;
				}

				internal ushort wFlags;

				internal ORC m_orc;

				internal SPRC(ushort leftTopColumn, short leftTopOffset, ushort topLeftRow, short topLeftOffset, ushort rightBottomColumn, short rightBottomOffset, ushort bottomRightRow, short bottomRightOffset)
				{
					m_orc = new ORC();
					m_orc.m_colL = leftTopColumn;
					m_orc.m_dxL = leftTopOffset;
					m_orc.m_rwT = topLeftRow;
					m_orc.m_dyT = topLeftOffset;
					m_orc.m_colR = rightBottomColumn;
					m_orc.m_dxR = rightBottomOffset;
					m_orc.m_rwB = bottomRightRow;
					m_orc.m_dyB = bottomRightOffset;
				}
			}

			private SPRC m_sprc;

			private const uint RecordLength = 18u;

			internal override uint Length => 18u;

			internal ClientAnchor(SPRC clientAnchorInfo)
				: base(0, 0u, EscherType.MSOFBTCLIENTANCHOR, 18u)
			{
				m_sprc = clientAnchorInfo;
			}

			internal override byte[] GetData()
			{
				byte[] array = new byte[26];
				int num = 0;
				byte[] data = base.GetData();
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_sprc.wFlags);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_sprc.m_orc.m_colL);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_sprc.m_orc.m_dxL);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_sprc.m_orc.m_rwT);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_sprc.m_orc.m_dyT);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_sprc.m_orc.m_colR);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_sprc.m_orc.m_dxR);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_sprc.m_orc.m_rwB);
				data.CopyTo(array, num);
				num += data.Length;
				data = BitConverter.GetBytes(m_sprc.m_orc.m_dyB);
				data.CopyTo(array, num);
				return array;
			}
		}

		internal sealed class ClientData : EscherHeader
		{
			internal ClientData()
				: base(0, 0u, EscherType.MSOFBTCLIENTDATA, 0u)
			{
			}
		}

		internal enum EscherType : ushort
		{
			MSOFBTUNKNOWN = 0,
			MSOFBTDGGCONTAINER = 61440,
			MSOFBTDGG = 61446,
			MSOFBTCLSID = 61462,
			MSOFBTOPT = 61451,
			MSOFBTBSTORECONTAINER = 61441,
			MSOFBTBSE = 61447,
			MSOFBTBLIP = 61464,
			MSOFBTBLIP_JPEG = 61469,
			MSOFBTBLIP_GIF = 61470,
			MSOFBTBLIP_PNG = 61470,
			MSOFBTBLIP_DIB = 61471,
			MSOFBTDGCONTAINER = 61442,
			MSOFBTDG = 61448,
			MSOFBTSPGRCONTAINER = 61443,
			MSOFBTSPCONTAINER = 61444,
			MSOFBTSPGR = 61449,
			MSOFBTSP = 61450,
			MSOFBTCLIENTANCHOR = 61456,
			MSOFBTCLIENTDATA = 61457
		}

		internal enum BlipType
		{
			MSOBLIPERROR = 0,
			MSOBLIPUNKNOWN = 1,
			MSOBLIPEMF = 2,
			MSOBLIPWMF = 3,
			MSOBLIPPICT = 4,
			MSOBLIPJPEG = 5,
			MSOBLIPPNG = 6,
			MSOBLIPDIB = 7,
			MSOBLITIFF = 17,
			MSOBLIPCMYKJPEG = 18,
			MSLBLIPFIRSTCLIENT = 0x20,
			MSLBLIPLASTCLIENT = 0xFF
		}

		internal enum BlipUsage
		{
			MSOBLIPUSAGEDEFAULT = 0,
			MSOBLIPUSAGETEXTURE = 1,
			MSOBLIPUSAGEMAX = 0xFF
		}

		internal enum BlipSignature
		{
			MSOBIUNKNOWN = 0,
			MSOBIWMF = 534,
			MSOBIEMF = 980,
			MSOBIPICT = 1346,
			MSOBIPNG = 1760,
			MSOBIJFIF = 1130,
			MSOBIJPEG = 1130,
			MSOBIDIB = 1960,
			MSOBICMYKJPEG = 1762,
			MSOBITIFF = 1764,
			MSOBICLIENT = 0x800
		}

		internal enum ShapeType
		{
			MSOSPTMIN = 0,
			MSOSPTPICTUREFRAME = 75
		}

		internal enum ShapeFlag
		{
			NONE = 0,
			GROUP = 1,
			CHILD = 2,
			PATRIARCH = 4,
			DELETED = 8,
			OLESHAPE = 0x10,
			HAVEMASTER = 0x20,
			FLIPH = 0x40,
			FLIPV = 0x80,
			CONNECTOR = 0x100,
			HAVEANCHOR = 0x200,
			BACKGROUND = 0x400,
			HAVESPT = 0x800
		}

		internal enum HyperlinkType
		{
			URL,
			LOCALFILE,
			UNC,
			BOOKMARK
		}

		internal const ushort RecordHeaderLength = 8;

		private const int ClusterSize = 1024;

		private const ushort ContainerVersion = 15;

		internal static byte[] CheckSum(Stream imageBits)
		{
			if (imageBits == null)
			{
				return null;
			}
			imageBits.Position = 0L;
			return new OfficeImageHasher(imageBits).Hash;
		}
	}
}
