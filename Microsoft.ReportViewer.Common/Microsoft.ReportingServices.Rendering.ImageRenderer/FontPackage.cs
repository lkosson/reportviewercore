using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RichText;
using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class FontPackage
	{
		private class CompareDirectoryEntryOffsets : IComparer
		{
			int IComparer.Compare(object object1, object object2)
			{
				DirectoryEntry directoryEntry = (DirectoryEntry)object1;
				DirectoryEntry directoryEntry2 = (DirectoryEntry)object2;
				if (directoryEntry.TableOffset < directoryEntry2.TableOffset)
				{
					return -1;
				}
				if (directoryEntry.TableOffset == directoryEntry2.TableOffset)
				{
					return 0;
				}
				return 1;
			}
		}

		private class DirectoryEntry
		{
			private uint m_tableLength;

			private uint m_tableOffset;

			private uint m_tableTag;

			private int m_directoryIndex;

			public uint TableLength => m_tableLength;

			public uint TableOffset => m_tableOffset;

			public uint TableTag => m_tableTag;

			public int DirectoryIndex => m_directoryIndex;

			private uint ToUInt32BigEndian(byte[] buffer, ushort offset)
			{
				return (uint)((int)((buffer[offset] << 24) & 4278190080u) | ((buffer[offset + 1] << 16) & 0xFF0000) | ((buffer[offset + 2] << 8) & 0xFF00) | (buffer[offset + 3] & 0xFF));
			}

			public DirectoryEntry(byte[] rawData, int directoryIndex)
			{
				m_directoryIndex = directoryIndex;
				m_tableLength = ToUInt32BigEndian(rawData, 12);
				m_tableOffset = ToUInt32BigEndian(rawData, 8);
				m_tableTag = BitConverter.ToUInt32(rawData, 0);
			}
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr AllocateMemory(IntPtr size);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr ReAllocateMemory(IntPtr memPointer, IntPtr size);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void FreeMemory(IntPtr memPointer);

		private const ushort OS2V0_WEIGHTCLASS_OFFSET = 4;

		private const ushort OS2V0_SELECTION_OFFSET = 62;

		private const ushort SIZEOF_OS2V0_TABLE = 64;

		private const uint TAG_TABLE_NAME_OS2V0 = 841962319u;

		private const byte TMPF_TRUETYPE = 4;

		private const int FW_SEMIBOLD = 600;

		private const ushort SFNT_DIRECTORYENTRY_TAG = 0;

		private const ushort SFNT_DIRECTORYENTRY_CHECKSUM = 4;

		private const ushort SFNT_DIRECTORYENTRY_TABLEOFFSET = 8;

		private const ushort SFNT_DIRECTORYENTRY_TABLELENGTH = 12;

		private const ushort SIZEOF_SFNT_DIRECTORYENTRY = 16;

		private const ushort SFNT_OFFSETTABLE_NUMOFFSETS = 4;

		private const ushort SIZEOF_SFNT_OFFSETTABLE = 12;

		private const uint tag_TTCF = 1717793908u;

		private const uint GDI_ERROR = uint.MaxValue;

		private const ushort TTFCFP_FLAGS_SUBSET = 1;

		private const ushort TTFCFP_FLAGS_GLYPHLIST = 8;

		private const int EMBED_PREVIEWPRINT = 1;

		private const int EMBED_EDITABLE = 2;

		private const int EMBED_INSTALLABLE = 3;

		private const int EMBED_NOEMBEDDING = 4;

		private const int E_NONE = 0;

		private static void ThrowNativeException(string source, int error, bool dump)
		{
			string message = string.Format(CultureInfo.InvariantCulture, ImageRendererRes.Win32ErrorInfo, source, error);
			if (dump)
			{
				throw new Exception(message);
			}
			throw new ReportRenderingException(message);
		}

		private static void CheckGetFontDataResult(uint result)
		{
			if (result == uint.MaxValue)
			{
				throw new Exception(string.Format(CultureInfo.InvariantCulture, ImageRendererRes.Win32ErrorInfo, "GetFontData", uint.MaxValue));
			}
		}

		private static ushort ToUInt16BigEndian(byte[] buffer, ushort offset)
		{
			return (ushort)(((buffer[offset] << 8) & 0xFF00) | (buffer[offset + 1] & 0xFF));
		}

		private static bool GetWeightClassAndSelection(Win32DCSafeHandle hdc, out ushort weightClass, out ushort selection)
		{
			weightClass = 0;
			selection = 0;
			byte[] array = new byte[64];
			if (GetFontData(hdc, 841962319u, 0u, array, (uint)array.Length) == array.Length)
			{
				weightClass = ToUInt16BigEndian(array, 4);
				selection = ToUInt16BigEndian(array, 62);
				return true;
			}
			return false;
		}

		internal static void CheckSimulatedFontStyles(Win32DCSafeHandle hdc, Microsoft.ReportingServices.Rendering.RichText.Win32.TEXTMETRIC textMetric, ref bool simulateItalic, ref bool simulateBold)
		{
			simulateItalic = false;
			simulateBold = false;
			if ((textMetric.tmPitchAndFamily & 4) == 4 && (textMetric.tmWeight >= 600 || textMetric.tmItalic > 0) && GetWeightClassAndSelection(hdc, out ushort weightClass, out ushort selection))
			{
				if (textMetric.tmItalic > 0 && (selection & 1) == 0)
				{
					simulateItalic = true;
				}
				if (textMetric.tmWeight >= 600 && weightClass < textMetric.tmWeight)
				{
					simulateBold = true;
				}
			}
		}

		internal static bool CheckEmbeddingRights(Win32DCSafeHandle hdc)
		{
			uint status = 0u;
			if (TTGetEmbeddingType(hdc, ref status) == 0)
			{
				return status != 4;
			}
			return false;
		}

		internal static byte[] Generate(Win32DCSafeHandle hdc, string fontFamily, ushort[] glyphIdArray)
		{
			IntPtr intPtr = IntPtr.Zero;
			uint fontDataLength = 0u;
			IntPtr puchFontPackageBuffer = IntPtr.Zero;
			uint pulFontPackageBufferSize = 0u;
			byte[] array = null;
			try
			{
				if (GetFontData(hdc, 1717793908u, 0u, IntPtr.Zero, 0u) == uint.MaxValue)
				{
					fontDataLength = GetFontData(hdc, 0u, 0u, IntPtr.Zero, 0u);
					CheckGetFontDataResult(fontDataLength);
					intPtr = Marshal.AllocHGlobal((int)fontDataLength);
					CheckGetFontDataResult(GetFontData(hdc, 0u, 0u, intPtr, fontDataLength));
				}
				else
				{
					byte[] tTCFontData = GetTTCFontData(hdc, ref fontDataLength);
					intPtr = Marshal.AllocHGlobal((int)fontDataLength);
					Marshal.Copy(tTCFontData, 0, intPtr, (int)fontDataLength);
				}
				ushort usFlags = 9;
				uint pulBytesWritten = 0u;
				short num = CreateFontPackage(intPtr, fontDataLength, out puchFontPackageBuffer, ref pulFontPackageBufferSize, ref pulBytesWritten, usFlags, 0, 0, 0, 3, ushort.MaxValue, glyphIdArray, (ushort)glyphIdArray.Length, new AllocateMemory(AllocateFontBufferMemory), new ReAllocateMemory(ReAllocateFontBufferMemory), new FreeMemory(FreeFontBufferMemory), IntPtr.Zero);
				if (num != 0)
				{
					ThrowNativeException(string.Format(CultureInfo.InvariantCulture, "CreateFontPackage(fontFamily={0})", fontFamily), num, dump: false);
				}
				array = new byte[pulBytesWritten];
				Marshal.Copy(puchFontPackageBuffer, array, 0, (int)pulBytesWritten);
				return array;
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
				if (puchFontPackageBuffer != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(puchFontPackageBuffer);
				}
			}
		}

		private static byte[] Swap(byte[] source)
		{
			byte[] array = new byte[source.Length];
			for (int i = 0; i < source.Length; i++)
			{
				array[i] = source[source.Length - i - 1];
			}
			return array;
		}

		private static byte[] GetTTCFontData(Win32DCSafeHandle hdc, ref uint fontDataLength)
		{
			byte[] array = null;
			fontDataLength = 0u;
			byte[] array2 = null;
			byte[] array3 = new byte[2];
			CheckGetFontDataResult(GetFontData(hdc, 0u, 4u, array3, 2u));
			ushort num = BitConverter.ToUInt16(Swap(array3), 0);
			ushort num2 = (ushort)(12 + num * 16);
			array2 = new byte[num2];
			CheckGetFontDataResult(GetFontData(hdc, 0u, 0u, array2, num2));
			ArrayList arrayList = new ArrayList();
			uint num3 = num2;
			for (int i = 0; i < num; i++)
			{
				byte[] array4 = new byte[16];
				Array.Copy(array2, 12 + i * 16, array4, 0, 16);
				DirectoryEntry directoryEntry = new DirectoryEntry(array4, i);
				num3 += directoryEntry.TableLength;
				arrayList.Add(directoryEntry);
			}
			array = new byte[num3];
			Array.Copy(array2, array, num2);
			fontDataLength += num2;
			arrayList.Sort(new CompareDirectoryEntryOffsets());
			foreach (DirectoryEntry item in arrayList)
			{
				byte[] array5 = new byte[item.TableLength];
				CheckGetFontDataResult(GetFontData(hdc, item.TableTag, 0u, array5, (uint)array5.Length));
				int destinationIndex = 12 + item.DirectoryIndex * 16 + 8;
				byte[] array6 = Swap(BitConverter.GetBytes(fontDataLength));
				Array.Copy(array6, 0, array, destinationIndex, array6.Length);
				Array.Copy(array5, 0L, array, fontDataLength, array5.Length);
				fontDataLength += (uint)array5.Length;
			}
			return array;
		}

		private static IntPtr AllocateFontBufferMemory(IntPtr size)
		{
			return Marshal.AllocHGlobal(size);
		}

		private static IntPtr ReAllocateFontBufferMemory(IntPtr memPointer, IntPtr size)
		{
			if (memPointer == IntPtr.Zero)
			{
				return Marshal.AllocHGlobal(size);
			}
			return Marshal.ReAllocHGlobal(memPointer, size);
		}

		private static void FreeFontBufferMemory(IntPtr memPointer)
		{
			Marshal.FreeHGlobal(memPointer);
		}

		[DllImport("GDI32")]
		private static extern uint GetFontData(Win32DCSafeHandle hdc, uint dwTable, uint dwOffset, IntPtr lpvBuffer, uint cbData);

		[DllImport("GDI32")]
		private static extern uint GetFontData(Win32DCSafeHandle hdc, uint dwTable, uint dwOffset, byte[] lpvBuffer, uint cbData);

		[DllImport("FontSub")]
		private static extern short CreateFontPackage(IntPtr puchSrcBuffer, uint ulSrcBufferSize, out IntPtr puchFontPackageBuffer, ref uint pulFontPackageBufferSize, ref uint pulBytesWritten, ushort usFlags, ushort usTTCIndex, ushort usSubsetFormat, ushort usSubsetLanguage, ushort usSubsetPlatform, ushort usSubsetEncoding, ushort[] pusSubsetKeepList, ushort usSubsetKeepListCount, Delegate lpfnAllocate, Delegate lpfnReAllocate, Delegate lpfnFree, IntPtr lpvReserved);

		[DllImport("T2Embed")]
		private static extern int TTGetEmbeddingType(Win32DCSafeHandle hdc, ref uint status);
	}
}
