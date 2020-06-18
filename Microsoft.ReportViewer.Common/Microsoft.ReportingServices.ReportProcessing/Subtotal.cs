using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Subtotal : IDOwner
	{
		internal enum PositionType
		{
			After,
			Before
		}

		private bool m_autoDerived;

		private ReportItemCollection m_reportItems;

		private Style m_styleClass;

		private PositionType m_position;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput = DataElementOutputTypes.NoOutput;

		[NonSerialized]
		private bool m_firstInstance = true;

		[NonSerialized]
		private string m_renderingModelID;

		[NonSerialized]
		private bool m_computed;

		internal bool AutoDerived
		{
			get
			{
				return m_autoDerived;
			}
			set
			{
				m_autoDerived = value;
			}
		}

		internal ReportItemCollection ReportItems
		{
			get
			{
				return m_reportItems;
			}
			set
			{
				m_reportItems = value;
			}
		}

		internal ReportItem ReportItem
		{
			get
			{
				if (m_reportItems != null && 0 < m_reportItems.Count)
				{
					return m_reportItems[0];
				}
				return null;
			}
		}

		internal Style StyleClass
		{
			get
			{
				return m_styleClass;
			}
			set
			{
				m_styleClass = value;
			}
		}

		internal PositionType Position
		{
			get
			{
				return m_position;
			}
			set
			{
				m_position = value;
			}
		}

		internal bool FirstInstance
		{
			get
			{
				return m_firstInstance;
			}
			set
			{
				m_firstInstance = value;
			}
		}

		internal string RenderingModelID
		{
			get
			{
				return m_renderingModelID;
			}
			set
			{
				m_renderingModelID = value;
			}
		}

		internal bool Computed
		{
			get
			{
				return m_computed;
			}
			set
			{
				m_computed = value;
			}
		}

		internal string DataElementName
		{
			get
			{
				return m_dataElementName;
			}
			set
			{
				m_dataElementName = value;
			}
		}

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return m_dataElementOutput;
			}
			set
			{
				m_dataElementOutput = value;
			}
		}

		internal Subtotal()
		{
		}

		internal Subtotal(int id, int idForReportItems, bool autoDerived)
			: base(id)
		{
			m_autoDerived = autoDerived;
			m_reportItems = new ReportItemCollection(idForReportItems, normal: false);
		}

		internal void RegisterReportItems(InitializationContext context)
		{
			context.RegisterReportItems(m_reportItems);
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.SubtotalStart();
			DataRendererInitialize(context);
			context.RegisterRunningValues(m_reportItems.RunningValues);
			if (m_styleClass != null)
			{
				m_styleClass.Initialize(context);
			}
			m_reportItems.Initialize(context, registerRunningValues: false);
			context.UnRegisterRunningValues(m_reportItems.RunningValues);
			context.ExprHostBuilder.SubtotalEnd();
		}

		internal void UnregisterReportItems(InitializationContext context)
		{
			context.UnRegisterReportItems(m_reportItems);
		}

		internal void RegisterReceiver(InitializationContext context)
		{
			context.RegisterReportItems(m_reportItems);
			m_reportItems.RegisterReceiver(context);
			context.UnRegisterReportItems(m_reportItems);
		}

		private void DataRendererInitialize(InitializationContext context)
		{
			CLSNameValidator.ValidateDataElementName(ref m_dataElementName, "Total", context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
		}

		internal void SetExprHost(StyleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null && m_styleClass != null);
			exprHost.SetReportObjectModel(reportObjectModel);
			m_styleClass.SetStyleExprHost(exprHost);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.AutoDerived, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.ReportItems, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			memberInfoList.Add(new MemberInfo(MemberName.StyleClass, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			memberInfoList.Add(new MemberInfo(MemberName.Position, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.IDOwner, memberInfoList);
		}
	}
}
