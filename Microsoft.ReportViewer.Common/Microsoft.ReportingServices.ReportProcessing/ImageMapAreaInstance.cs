using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ImageMapAreaInstance
	{
		private string m_id;

		private ImageMapArea.ImageMapAreaShape m_shape;

		private float[] m_coordinates;

		private Action m_actionDef;

		private ActionInstance m_actionInstance;

		private int m_uniqueName;

		public string ID
		{
			get
			{
				return m_id;
			}
			set
			{
				m_id = value;
			}
		}

		public ImageMapArea.ImageMapAreaShape Shape
		{
			get
			{
				return m_shape;
			}
			set
			{
				m_shape = value;
			}
		}

		public float[] Coordinates
		{
			get
			{
				return m_coordinates;
			}
			set
			{
				m_coordinates = value;
			}
		}

		public Action Action
		{
			get
			{
				return m_actionDef;
			}
			set
			{
				m_actionDef = value;
			}
		}

		public ActionInstance ActionInstance
		{
			get
			{
				return m_actionInstance;
			}
			set
			{
				m_actionInstance = value;
			}
		}

		public int UniqueName
		{
			get
			{
				return m_uniqueName;
			}
			set
			{
				m_uniqueName = value;
			}
		}

		internal ImageMapAreaInstance()
		{
		}

		internal ImageMapAreaInstance(ReportProcessing.ProcessingContext processingContext)
		{
			m_uniqueName = processingContext.CreateUniqueName();
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Id, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Shape, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Coordinates, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Single));
			memberInfoList.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Action));
			memberInfoList.Add(new MemberInfo(MemberName.ActionInstance, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ActionInstance));
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
