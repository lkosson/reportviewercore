using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal sealed class SafeCryptoBlobIn : SafeHandleZeroOrMinusOneIsInvalid
	{
		private int m_bufferSize;

		private SafeLocalFree m_blob;

		private SafeLocalFree m_pbData;

		internal SafeCryptoBlobIn(byte[] data)
			: base(ownsHandle: true)
		{
			m_bufferSize = data.Length;
			m_blob = SafeLocalFree.LocalAlloc(Marshal.SizeOf(typeof(NativeMethods.DATA_BLOB)));
			handle = m_blob.DangerousGetHandle();
			m_pbData = SafeLocalFree.LocalAlloc(data.Length);
			Marshal.Copy(data, 0, m_pbData.DangerousGetHandle(), data.Length);
			Marshal.StructureToPtr(new NativeMethods.DATA_BLOB
			{
				cbData = data.Length,
				pbData = m_pbData.DangerousGetHandle()
			}, handle, fDeleteOld: true);
		}

		internal void ZeroBuffer()
		{
			if (!IsInvalid && m_bufferSize > 0 && m_pbData != null)
			{
				Marshal.Copy(new byte[m_bufferSize], 0, m_pbData.DangerousGetHandle(), m_bufferSize);
			}
		}

		protected override bool ReleaseHandle()
		{
			if (m_pbData != null)
			{
				m_pbData.Close();
			}
			if (m_blob != null)
			{
				m_blob.Close();
			}
			return true;
		}
	}
}
