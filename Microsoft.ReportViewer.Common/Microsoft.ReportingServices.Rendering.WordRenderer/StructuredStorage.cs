using Microsoft.Win32.SafeHandles;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.ReportingServices.Rendering.WordRenderer
{
	internal static class StructuredStorage
	{
		internal sealed class OLEStructuredStorage
		{
			[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			[Guid("0000000a-0000-0000-C000-000000000046")]
			internal interface UCOMILockBytes
			{
				void ReadAt(ulong offset, [Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pv, int cb, out int pcbRead);

				void WriteAt(ulong offset, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pv, int cb, out int pcbWritten);

				void Flush();

				void SetSize(int cb);

				void LockRegion(int libOffset, int cb, long dwLoclType);

				void UnlockRegion(int libOffset, int cb, long dwLoclType);

				void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag);
			}

			[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			[Guid("0000000d-0000-0000-C000-000000000046")]
			internal interface IEnumSTATSTG
			{
				void Next(int celt, out System.Runtime.InteropServices.ComTypes.STATSTG rgelt, out int pceltFetched);
			}

			[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			[Guid("0000000b-0000-0000-C000-000000000046")]
			internal interface UCOMIStorage
			{
				void CreateStream([MarshalAs(UnmanagedType.LPWStr)] string wcsName, int grfMode, int reserved1, int reserved2, out IStream stream);

				void OpenStream([MarshalAs(UnmanagedType.LPWStr)] string wcsName, IntPtr reserved1, int grfMode, int reserved2, out IStream stream);

				void CreateStorage([MarshalAs(UnmanagedType.LPWStr)] string wcsName, int grfMode, int reserved1, int reserved2, out UCOMIStorage storage);

				void OpenStorage([MarshalAs(UnmanagedType.LPWStr)] string wcsName, UCOMIStorage pstgPriority, int grfMode, IntPtr[] snbExclude, int reserved1, out UCOMIStorage storage);

				void CopyTo(int ciidExclude, IntPtr[] rgiidExclude, IntPtr[] snbExclude, out UCOMIStorage storage);

				void MoveTo([MarshalAs(UnmanagedType.LPWStr)] string wcsName, UCOMIStorage pstgDest, [MarshalAs(UnmanagedType.LPWStr)] string pwcsNewName, int grfFlags);

				void Commit(int grfCommitFlags);

				void Revert();

				void EnumElements(int reserved1, IntPtr reserved2, int reserved3, out IEnumSTATSTG ppenum);

				void DestroyElement([MarshalAs(UnmanagedType.LPWStr)] string pwcsName);

				void RenameElement([MarshalAs(UnmanagedType.LPWStr)] string pwcsOldName, [MarshalAs(UnmanagedType.LPWStr)] string pwcsNewName);

				void SetElementTimes([MarshalAs(UnmanagedType.LPWStr)] string pwcsName, ref System.Runtime.InteropServices.ComTypes.FILETIME pctime, ref System.Runtime.InteropServices.ComTypes.FILETIME patime, ref System.Runtime.InteropServices.ComTypes.FILETIME pmtime);

				void SetClass(ref Guid clsid);
			}

			[ComImport]
			[ComVisible(true)]
			[Guid("0000013A-0000-0000-C000-000000000046")]
			[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			internal interface IPropertySetStorage
			{
				uint Create([In] [MarshalAs(UnmanagedType.Struct)] ref Guid rfmtid, [In] IntPtr pclsid, [In] int grfFlags, [In] int grfMode, out IPropertyStorage propertyStorage);

				int Open([In] [MarshalAs(UnmanagedType.Struct)] ref Guid rfmtid, [In] int grfMode, [Out] IPropertyStorage propertyStorage);
			}

			[ComImport]
			[ComVisible(true)]
			[Guid("00000138-0000-0000-C000-000000000046")]
			[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			internal interface IPropertyStorage
			{
				int ReadMultiple(uint numProperties, PropSpec[] propertySpecifications, PropVariant[] propertyValues);

				int WriteMultiple(uint numProperties, ref PropSpec propertySpecification, ref PropVariant propertyValues, int propIDNameFirst);

				uint Commit(int commitFlags);
			}

			public struct PropSpec
			{
				[MarshalAs(UnmanagedType.U4)]
				public int ulKind;

				public IntPtr strPtr;
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
			public struct PropVariant : IDisposable
			{
				public short variantType;

				public short wReserved1;

				public short wReserved2;

				public short wReserved3;

				public HGlobalSafeHandle pointerValue;

				public void FromString(string str)
				{
					variantType = 31;
					pointerValue = new HGlobalSafeHandle(Marshal.StringToHGlobalUni(str));
				}

				public void Dispose()
				{
					if (pointerValue != null)
					{
						pointerValue.Close();
						pointerValue = null;
					}
				}

				public string PointerValue()
				{
					if (pointerValue != null)
					{
						return pointerValue.MarshalToString();
					}
					return null;
				}
			}

			internal class HGlobalSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
			{
				public HGlobalSafeHandle(IntPtr handle)
					: base(ownsHandle: true)
				{
					SetHandle(handle);
				}

				[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
				protected override bool ReleaseHandle()
				{
					Marshal.FreeHGlobal(handle);
					return true;
				}

				public string MarshalToString()
				{
					return Marshal.PtrToStringUni(handle);
				}
			}

			internal const long MEMORY_THRESHOLD = 11534336L;

			internal const int STGM_SIMPLE = 134217728;

			internal const int STGM_READ = 0;

			internal const int STGM_READWRITE = 2;

			internal const int STGM_SHARE_DENY_NONE = 64;

			internal const int STGM_SHARE_EXCLUSIVE = 16;

			internal const int STGM_CREATE = 4096;

			internal const int STGM_DELETEONRELEASE = 67108864;

			internal const int PIDSI_TITLE = 2;

			internal const int PIDSI_AUTHOR = 4;

			internal const int PIDSI_COMMENTS = 6;

			internal const short VT_LPWSTR = 31;

			internal const int PRSPEC_PROPID = 1;

			[DllImport("Ole32.dll")]
			internal static extern int StgCreateDocfile([MarshalAs(UnmanagedType.LPWStr)] string wcsName, int grfMode, int reserved, out UCOMIStorage storage);

			[DllImport("ole32.dll")]
			internal static extern int StgOpenStorage([MarshalAs(UnmanagedType.LPWStr)] string wcsName, IntPtr stgPriority, int grfMode, IntPtr snbExclude, int reserved, out UCOMIStorage storage);

			[DllImport("ole32.dll")]
			internal static extern int StgCreateDocfileOnILockBytes(UCOMILockBytes plkbyt, int grfMode, int reserved, out UCOMIStorage storage);

			[DllImport("ole32.dll")]
			internal static extern int CreateILockBytesOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease, out UCOMILockBytes lockBytes);

			[DllImport("ole32.dll")]
			internal static extern int StgCreateStorageEx([MarshalAs(UnmanagedType.LPWStr)] string wcsName, int grfMode, int stgfmt, int grfAttr, IntPtr StgOptions, IntPtr reserved2, ref Guid refiid, out UCOMIStorage storage);

			[DllImport("Ole32.dll")]
			internal static extern int StgCreatePropSetStg(UCOMIStorage storage, int reserverd, out IPropertySetStorage propSetStg);

			internal static bool Failed(int hr)
			{
				return hr < 0;
			}
		}

		private const int BUFFERSIZE = 1048576;

		internal static bool CreateMultiStreamFile(Stream[] sources, string[] streamNames, string clsId, string author, string title, string comments, Stream output, bool forceInMemory)
		{
			OLEStructuredStorage.UCOMIStorage storage = null;
			OLEStructuredStorage.UCOMILockBytes uCOMILockBytes = null;
			IStream stream = null;
			string text = null;
			Guid clsid = new Guid(clsId);
			long num = 0L;
			long num2 = 0L;
			for (int i = 0; i < sources.Length; i++)
			{
				num2 = Math.Max(sources[i].Length, num2);
				num += sources[i].Length;
			}
			int num3 = Math.Min(1048576, (int)num2 + 512);
			try
			{
				int grfMode = 134221842;
				int grfMode2 = 18;
				text = Path.GetTempPath() + Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture) + ".doc.tmp";
				if (OLEStructuredStorage.StgCreateDocfile(text, grfMode, 0, out storage) != 0)
				{
					return false;
				}
				byte[] array = new byte[num3];
				for (int j = 0; j < streamNames.Length; j++)
				{
					sources[j].Seek(0L, SeekOrigin.Begin);
					storage.CreateStream(streamNames[j], grfMode2, 0, 0, out stream);
					if (stream != null)
					{
						IntPtr pcbWritten = default(IntPtr);
						int num4 = 0;
						while ((num4 = sources[j].Read(array, 0, num3)) > 0)
						{
							stream.Write(array, num4, pcbWritten);
						}
						sources[j] = null;
					}
					Marshal.ReleaseComObject(stream);
					stream = null;
				}
				OLEStructuredStorage.IPropertySetStorage propSetStg = null;
				OLEStructuredStorage.IPropertyStorage propertyStorage = null;
				OLEStructuredStorage.StgCreatePropSetStg(storage, 0, out propSetStg);
				Guid rfmtid = new Guid("{F29F85E0-4FF9-1068-AB91-08002B27B3D9}");
				propSetStg.Create(ref rfmtid, IntPtr.Zero, 0, grfMode2, out propertyStorage);
				WriteProperty(propertyStorage, 2, title);
				WriteProperty(propertyStorage, 4, author);
				WriteProperty(propertyStorage, 6, comments);
				Marshal.ReleaseComObject(propertyStorage);
				Marshal.ReleaseComObject(propSetStg);
				storage.SetClass(ref clsid);
				storage.Commit(0);
				Marshal.ReleaseComObject(storage);
				storage = null;
				using (Stream stream2 = File.OpenRead(text))
				{
					int num5 = 0;
					while ((num5 = stream2.Read(array, 0, array.Length)) > 0)
					{
						output.Write(array, 0, num5);
					}
				}
				array = null;
				return true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (!string.IsNullOrEmpty(text) && File.Exists(text))
				{
					File.Delete(text);
				}
				if (stream != null)
				{
					Marshal.ReleaseComObject(stream);
					stream = null;
				}
				if (storage != null)
				{
					Marshal.ReleaseComObject(storage);
					storage = null;
				}
				if (uCOMILockBytes != null)
				{
					Marshal.ReleaseComObject(uCOMILockBytes);
					uCOMILockBytes = null;
				}
			}
		}

		private static void WriteProperty(OLEStructuredStorage.IPropertyStorage propertyStorage, int propid, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				OLEStructuredStorage.PropVariant propertyValues = default(OLEStructuredStorage.PropVariant);
				OLEStructuredStorage.PropSpec propertySpecification = default(OLEStructuredStorage.PropSpec);
				propertySpecification.ulKind = 1;
				propertySpecification.strPtr = new IntPtr(propid);
				try
				{
					propertyValues.FromString(value);
					propertyStorage.WriteMultiple(1u, ref propertySpecification, ref propertyValues, 2);
					propertyStorage.Commit(0);
				}
				finally
				{
					propertyValues.Dispose();
				}
			}
		}
	}
}
