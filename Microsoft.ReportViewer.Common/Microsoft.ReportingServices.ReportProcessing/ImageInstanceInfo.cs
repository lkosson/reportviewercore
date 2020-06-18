using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class ImageInstanceInfo : ReportItemInstanceInfo
	{
		private object m_data;

		private ActionInstance m_action;

		private bool m_brokenImage;

		private ImageMapAreaInstanceList m_imageMapAreas;

		internal string ImageValue
		{
			get
			{
				return m_data as string;
			}
			set
			{
				m_data = value;
			}
		}

		internal ImageData Data
		{
			get
			{
				return m_data as ImageData;
			}
			set
			{
				m_data = value;
			}
		}

		internal object ValueObject => m_data;

		internal ActionInstance Action
		{
			get
			{
				return m_action;
			}
			set
			{
				m_action = value;
			}
		}

		internal bool BrokenImage
		{
			get
			{
				return m_brokenImage;
			}
			set
			{
				m_brokenImage = value;
			}
		}

		internal ImageMapAreaInstanceList ImageMapAreas
		{
			get
			{
				return m_imageMapAreas;
			}
			set
			{
				m_imageMapAreas = value;
			}
		}

		internal ImageInstanceInfo(ReportProcessing.ProcessingContext pc, Image reportItemDef, ReportItemInstance owner, int index, bool customCreated)
			: base(pc, reportItemDef, owner, index, customCreated)
		{
		}

		internal ImageInstanceInfo(Image reportItemDef)
			: base(reportItemDef)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ImageValue, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ActionInstance));
			memberInfoList.Add(new MemberInfo(MemberName.BrokenImage, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.ImageMapAreas, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ImageMapAreaInstanceList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstanceInfo, memberInfoList);
		}
	}
}
