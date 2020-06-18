using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8
{
	internal static class StructuredStorage
	{
		internal sealed class OLEStructuredStorage
		{
			[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			[Guid("0000000a-0000-0000-C000-000000000046")]
			public interface UCOMILockBytes
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

			internal const long MEMORY_THRESHOLD = 11534336L;

			internal const int STGM_SIMPLE = 134217728;

			internal const int STGM_READ = 0;

			internal const int STGM_READWRITE = 2;

			internal const int STGM_SHARE_DENY_NONE = 64;

			internal const int STGM_SHARE_EXCLUSIVE = 16;

			internal const int STGM_CREATE = 4096;

			internal const int STGM_DELETEONRELEASE = 67108864;

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
		}

		private const int BUFFERSIZE = 1048576;

		public static bool CreateSingleStreamFile(Stream source, string streamName, string clsId, Stream output, bool forceInMemory)
		{
			OLEStructuredStorage.UCOMIStorage storage = null;
			OLEStructuredStorage.UCOMILockBytes uCOMILockBytes = null;
			IStream stream = null;
			string text = null;
			Guid clsid = new Guid(clsId);
			int num = Math.Min(1048576, (int)source.Length + 512);
			try
			{
				int grfMode = 134221842;
				int grfMode2 = 18;
				text = Path.GetTempPath() + Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture) + ".xls.tmp";
				if (OLEStructuredStorage.StgCreateDocfile(text, grfMode, 0, out storage) != 0)
				{
					return false;
				}
				byte[] array = new byte[num];
				storage.CreateStream(streamName, grfMode2, 0, 0, out stream);
				if (stream != null)
				{
					IntPtr pcbWritten = default(IntPtr);
					int num2 = 0;
					while ((num2 = source.Read(array, 0, array.Length)) > 0)
					{
						stream.Write(array, num2, pcbWritten);
					}
					source = null;
				}
				Marshal.ReleaseComObject(stream);
				stream = null;
				storage.SetClass(ref clsid);
				storage.Commit(0);
				Marshal.ReleaseComObject(storage);
				storage = null;
				using (Stream stream2 = File.OpenRead(text))
				{
					int num3 = 0;
					while ((num3 = stream2.Read(array, 0, array.Length)) > 0)
					{
						output.Write(array, 0, num3);
					}
				}
				array = null;
				return true;
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
	}
}
