using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ImageInfo : IPersistable
	{
		private string m_streamName;

		private string m_mimeType;

		private bool m_errorOccurred;

		[NonSerialized]
		private WeakReference m_imageDataRef;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

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

		internal bool ErrorOccurred
		{
			get
			{
				return m_errorOccurred;
			}
			set
			{
				m_errorOccurred = value;
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

		internal byte[] GetCachedImageData()
		{
			if (m_imageDataRef != null && m_imageDataRef.IsAlive)
			{
				return (byte[])m_imageDataRef.Target;
			}
			return null;
		}

		internal void SetCachedImageData(byte[] imageData)
		{
			if (m_imageDataRef == null)
			{
				m_imageDataRef = new WeakReference(imageData);
			}
			else
			{
				m_imageDataRef.Target = imageData;
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.StreamName, Token.String));
			list.Add(new MemberInfo(MemberName.MIMEType, Token.String));
			list.Add(new MemberInfo(MemberName.ErrorOccurred, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.StreamName:
					writer.Write(m_streamName);
					break;
				case MemberName.MIMEType:
					writer.Write(m_mimeType);
					break;
				case MemberName.ErrorOccurred:
					writer.Write(m_errorOccurred);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.StreamName:
					m_streamName = reader.ReadString();
					break;
				case MemberName.MIMEType:
					m_mimeType = reader.ReadString();
					break;
				case MemberName.ErrorOccurred:
					m_errorOccurred = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ImageInfo;
		}
	}
}
