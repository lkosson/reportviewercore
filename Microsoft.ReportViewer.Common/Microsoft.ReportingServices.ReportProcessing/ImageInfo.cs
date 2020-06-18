using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ImageInfo
	{
		private string m_streamName;

		private string m_mimeType;

		[NonSerialized]
		private WeakReference m_imageDataRef;

		internal string StreamName
		{
			get
			{
				return m_streamName;
			}
			set
			{
				m_streamName = value;
			}
		}

		internal string MimeType
		{
			get
			{
				return m_mimeType;
			}
			set
			{
				m_mimeType = value;
			}
		}

		internal WeakReference ImageDataRef
		{
			get
			{
				return m_imageDataRef;
			}
			set
			{
				m_imageDataRef = value;
			}
		}

		internal ImageInfo()
		{
		}

		internal ImageInfo(string streamName, string mimeType)
		{
			m_streamName = streamName;
			m_mimeType = mimeType;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.StreamName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.MIMEType, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
