using System.Collections;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLContext
	{
		private Hashtable m_sharedProps;

		private Hashtable m_sharedImages;

		private BinaryReader m_binaryReader;

		private RPLVersionEnum m_versionPicker;

		internal BinaryReader BinaryReader => m_binaryReader;

		internal Hashtable SharedProps
		{
			get
			{
				return m_sharedProps;
			}
			set
			{
				m_sharedProps = value;
			}
		}

		internal Hashtable SharedImages
		{
			get
			{
				return m_sharedImages;
			}
			set
			{
				m_sharedImages = value;
			}
		}

		internal RPLVersionEnum VersionPicker
		{
			get
			{
				return m_versionPicker;
			}
			set
			{
				m_versionPicker = value;
			}
		}

		internal RPLContext(BinaryReader reader)
		{
			m_binaryReader = reader;
		}

		public void Release()
		{
			if (m_binaryReader != null)
			{
				m_binaryReader.Close();
				m_binaryReader = null;
			}
		}
	}
}
