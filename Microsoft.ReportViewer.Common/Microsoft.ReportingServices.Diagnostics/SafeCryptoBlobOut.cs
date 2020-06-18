using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal sealed class SafeCryptoBlobOut : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeLocalFree m_pBlob;

		private bool m_managedBlobInitialized;

		private NativeMethods.DATA_BLOB m_blob;

		internal NativeMethods.DATA_BLOB Blob
		{
			get
			{
				if (!m_managedBlobInitialized)
				{
					m_blob = (NativeMethods.DATA_BLOB)Marshal.PtrToStructure(handle, typeof(NativeMethods.DATA_BLOB));
					m_managedBlobInitialized = true;
				}
				return m_blob;
			}
		}

		internal SafeCryptoBlobOut()
			: base(ownsHandle: true)
		{
			m_pBlob = SafeLocalFree.LocalAlloc(Marshal.SizeOf(typeof(NativeMethods.DATA_BLOB)));
			handle = m_pBlob.DangerousGetHandle();
		}

		internal void ZeroBuffer()
		{
			NativeMethods.DATA_BLOB blob = Blob;
			byte[] destination = new byte[blob.cbData];
			Marshal.Copy(blob.pbData, destination, 0, blob.cbData);
		}

		protected override bool ReleaseHandle()
		{
			if (m_pBlob != null)
			{
				NativeMemoryMethods.LocalFree(Blob.pbData);
				m_pBlob.Close();
			}
			return true;
		}
	}
}
