using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal abstract class GaugePanelStyleContainer : IStyleContainer, IPersistable
	{
		[Reference]
		protected GaugePanel m_gaugePanel;

		protected Style m_styleClass;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		public Style StyleClass
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

		IInstancePath IStyleContainer.InstancePath => m_gaugePanel;

		Microsoft.ReportingServices.ReportProcessing.ObjectType IStyleContainer.ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugePanel;

		string IStyleContainer.Name => m_gaugePanel.Name;

		internal GaugePanelStyleContainer()
		{
		}

		internal GaugePanelStyleContainer(GaugePanel gaugePanel)
		{
			m_gaugePanel = gaugePanel;
		}

		internal virtual void Initialize(InitializationContext context)
		{
			if (m_styleClass != null)
			{
				m_styleClass.Initialize(context);
			}
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context)
		{
			GaugePanelStyleContainer gaugePanelStyleContainer = (GaugePanelStyleContainer)MemberwiseClone();
			gaugePanelStyleContainer.m_gaugePanel = (GaugePanel)context.CurrentDataRegionClone;
			if (m_styleClass != null)
			{
				gaugePanelStyleContainer.m_styleClass = (Style)m_styleClass.PublishClone(context);
			}
			return gaugePanelStyleContainer;
		}

		internal virtual void SetExprHost(StyleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			exprHost.SetReportObjectModel(reportObjectModel);
			if (m_styleClass != null)
			{
				m_styleClass.SetStyleExprHost(exprHost);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.GaugePanel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanel, Token.Reference));
			list.Add(new MemberInfo(MemberName.StyleClass, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugePanel:
					writer.WriteReference(m_gaugePanel);
					break;
				case MemberName.StyleClass:
					writer.Write(m_styleClass);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugePanel:
					m_gaugePanel = reader.ReadReference<GaugePanel>(this);
					break;
				case MemberName.StyleClass:
					m_styleClass = (Style)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.GaugePanel)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_gaugePanel = (GaugePanel)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer;
		}
	}
}
