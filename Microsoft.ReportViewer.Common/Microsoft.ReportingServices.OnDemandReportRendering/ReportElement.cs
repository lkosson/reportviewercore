using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class ReportElement : IDefinitionPath, IROMStyleDefinitionContainer
	{
		internal enum CriGenerationPhases
		{
			None,
			Definition,
			Instance
		}

		protected bool m_isOldSnapshot;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem m_reportItemDef;

		internal RenderingContext m_renderingContext;

		internal Microsoft.ReportingServices.ReportRendering.ReportItem m_renderReportItem;

		protected IDefinitionPath m_parentDefinitionPath;

		protected Style m_style;

		private IReportScope m_reportScope;

		private CustomReportItem __criOwner;

		private CriGenerationPhases __criGenerationPhase;

		public abstract string DefinitionPath
		{
			get;
		}

		public IDefinitionPath ParentDefinitionPath => m_parentDefinitionPath;

		internal abstract string InstanceUniqueName
		{
			get;
		}

		public ReportElementInstance Instance => ReportElementInstance;

		internal abstract ReportElementInstance ReportElementInstance
		{
			get;
		}

		public virtual Style Style
		{
			get
			{
				if (m_style == null)
				{
					if (m_isOldSnapshot)
					{
						m_style = new Style(RenderReportItem, m_renderingContext, UseRenderStyle);
					}
					else
					{
						m_style = new Style(this, ReportScope, StyleContainer, m_renderingContext);
					}
				}
				return m_style;
			}
		}

		public abstract string ID
		{
			get;
		}

		internal virtual bool UseRenderStyle => true;

		internal virtual IStyleContainer StyleContainer => m_reportItemDef;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem ReportItemDef => m_reportItemDef;

		internal RenderingContext RenderingContext => m_renderingContext;

		internal bool IsOldSnapshot => m_isOldSnapshot;

		internal virtual Microsoft.ReportingServices.ReportRendering.ReportItem RenderReportItem
		{
			get
			{
				if (!m_isOldSnapshot)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return m_renderReportItem;
			}
		}

		internal virtual IReportScope ReportScope => m_reportScope;

		internal CustomReportItem CriOwner
		{
			get
			{
				Global.Tracer.Assert(!m_isOldSnapshot || __criOwner == null, "(!m_isOldSnapshot || __criOwner == null)");
				return __criOwner;
			}
			set
			{
				Global.Tracer.Assert(!m_isOldSnapshot, "(!m_isOldSnapshot)");
				__criOwner = value;
			}
		}

		internal CriGenerationPhases CriGenerationPhase
		{
			get
			{
				Global.Tracer.Assert(!m_isOldSnapshot || __criGenerationPhase == CriGenerationPhases.None, "(!m_isOldSnapshot || __criGenerationPhase == CriGenerationPhases.None)");
				return __criGenerationPhase;
			}
			set
			{
				Global.Tracer.Assert(!m_isOldSnapshot, "(!m_isOldSnapshot)");
				__criGenerationPhase = value;
			}
		}

		internal ReportElement(IDefinitionPath parentDefinitionPath)
		{
			m_parentDefinitionPath = parentDefinitionPath;
		}

		internal ReportElement(IReportScope reportScope, IDefinitionPath parentDefinitionPath, Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItemDef, RenderingContext renderingContext)
		{
			m_reportScope = reportScope;
			m_parentDefinitionPath = parentDefinitionPath;
			m_reportItemDef = reportItemDef;
			m_renderingContext = renderingContext;
			m_isOldSnapshot = false;
		}

		internal ReportElement(IDefinitionPath parentDefinitionPath, Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem, RenderingContext renderingContext)
		{
			m_parentDefinitionPath = parentDefinitionPath;
			m_renderReportItem = renderReportItem;
			m_renderingContext = renderingContext;
			m_isOldSnapshot = true;
		}

		internal ReportElement(IDefinitionPath parentDefinitionPath, RenderingContext renderingContext)
		{
			m_parentDefinitionPath = parentDefinitionPath;
			m_renderingContext = renderingContext;
			m_isOldSnapshot = true;
		}

		internal virtual void SetNewContext()
		{
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			SetNewContextChildren();
		}

		internal abstract void SetNewContextChildren();

		internal void ConstructReportElementDefinitionImpl()
		{
			Global.Tracer.Assert(CriGenerationPhase == CriGenerationPhases.Definition, "(CriGenerationPhase == CriGenerationPhases.Definition)");
			Style.ConstructStyleDefinition();
		}
	}
}
