using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Diagnostics;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CustomReportItem : ReportItem, IDataRegion, IReportScope
	{
		private const Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes ChunkType = Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.GeneratedReportItems;

		private int m_indexIntoParentCollectionDef = -1;

		private int m_memberCellDefinitionIndex;

		private CustomData m_data;

		private ReportItem m_altReportItem;

		private ReportItem m_generatedReportItem;

		private ReportItem m_exposeAs;

		private ReportSize m_dynamicWidth;

		private ReportSize m_dynamicHeight;

		public ReportSize DynamicWidth
		{
			set
			{
				m_dynamicWidth = value;
			}
		}

		public ReportSize DynamicHeight
		{
			set
			{
				m_dynamicHeight = value;
			}
		}

		public override ReportSize Width => m_dynamicWidth ?? base.Width;

		public override ReportSize Height => m_dynamicHeight ?? base.Height;

		public string Type
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return RenderCri.Type;
				}
				return CriDef.Type;
			}
		}

		internal bool HasCustomData => m_data != null;

		public CustomData CustomData
		{
			get
			{
				if (m_data == null)
				{
					m_data = new CustomData(this);
				}
				return m_data;
			}
		}

		public ReportItem AltReportItem
		{
			get
			{
				if (m_altReportItem == null)
				{
					if (m_isOldSnapshot)
					{
						m_altReportItem = ReportItem.CreateShim(this, 0, m_inSubtotal, RenderCri.AltReportItem, m_renderingContext);
					}
					else
					{
						m_altReportItem = ReportItem.CreateItem(ParentScope, base.ParentDefinitionPath, CriDef.AltReportItemIndexInParentCollectionDef, CriDef.AltReportItem, m_renderingContext);
					}
				}
				return m_altReportItem;
			}
		}

		public ReportItem GeneratedReportItem => m_generatedReportItem;

		internal Microsoft.ReportingServices.ReportRendering.CustomReportItem RenderCri
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return (Microsoft.ReportingServices.ReportRendering.CustomReportItem)m_renderReportItem;
				}
				return null;
			}
		}

		IReportScopeInstance IReportScope.ReportScopeInstance
		{
			get
			{
				if (CriDef.IsDataRegion)
				{
					return m_data;
				}
				return ParentScope.ReportScopeInstance;
			}
		}

		IRIFReportScope IReportScope.RIFReportScope
		{
			get
			{
				if (CriDef.IsDataRegion)
				{
					return CriDef;
				}
				return ParentScope.RIFReportScope;
			}
		}

		private IReportScope ParentScope => base.ReportScope;

		bool IDataRegion.HasDataCells
		{
			get
			{
				if (m_data != null)
				{
					return m_data.HasDataRowCollection;
				}
				return false;
			}
		}

		IDataRegionRowCollection IDataRegion.RowCollection
		{
			get
			{
				if (m_data != null)
				{
					return m_data.RowCollection;
				}
				return null;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem CriDef => (Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem)m_reportItemDef;

		internal CustomReportItem(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem reportItemDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
			m_indexIntoParentCollectionDef = indexIntoParentCollectionDef;
		}

		internal CustomReportItem(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, Microsoft.ReportingServices.ReportRendering.CustomReportItem renderCri, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderCri, renderingContext)
		{
		}

		internal bool Initialize(RenderingContext renderingContext)
		{
			m_exposeAs = null;
			if (renderingContext.IsRenderAsNativeCri(CriDef))
			{
				m_exposeAs = this;
			}
			else
			{
				if (!LoadGeneratedReportItemDefinition())
				{
					GenerateReportItemDefinition();
				}
				m_exposeAs = m_generatedReportItem;
			}
			return m_exposeAs != null;
		}

		internal override ReportItem ExposeAs(RenderingContext renderingContext)
		{
			Global.Tracer.Assert(m_exposeAs != null, "m_exposeAs != null");
			return m_exposeAs;
		}

		private bool LoadGeneratedReportItemDefinition()
		{
			if (!base.RenderingContext.OdpContext.OdpMetadata.ReportSnapshot.TryGetGeneratedReportItemChunkName(GetGeneratedDefinitionChunkKey(), out string name))
			{
				return false;
			}
			string mimeType;
			Stream chunk = base.RenderingContext.OdpContext.ChunkFactory.GetChunk(name, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.GeneratedReportItems, ChunkMode.Open, out mimeType);
			if (chunk == null)
			{
				return false;
			}
			using (chunk)
			{
				IntermediateFormatReader intermediateFormatReader = new IntermediateFormatReader(chunk, new ProcessingRIFObjectCreator((Microsoft.ReportingServices.ReportIntermediateFormat.IDOwner)m_reportItemDef.ParentInstancePath, m_reportItemDef.Parent));
				Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItem = (Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem)intermediateFormatReader.ReadRIFObject();
				Global.Tracer.Assert(!intermediateFormatReader.HasReferences, "!reader.HasReferences");
				reportItem.GlobalID = -CriDef.GlobalID;
				if (reportItem.StyleClass != null)
				{
					reportItem.StyleClass.InitializeForCRIGeneratedReportItem();
				}
				reportItem.Visibility = m_reportItemDef.Visibility;
				Microsoft.ReportingServices.ReportProcessing.ObjectType objectType = reportItem.ObjectType;
				if (objectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Image)
				{
					Image image = new Image(ParentScope, base.ParentDefinitionPath, m_indexIntoParentCollectionDef, (Microsoft.ReportingServices.ReportIntermediateFormat.Image)reportItem, base.RenderingContext);
					image.CriOwner = this;
					image.CriGenerationPhase = CriGenerationPhases.None;
					m_generatedReportItem = image;
				}
				else
				{
					Global.Tracer.Assert(condition: false, "Unexpected CRI generated report item type: " + reportItem.ObjectType);
				}
			}
			return true;
		}

		private static string CreateChunkName()
		{
			return Guid.NewGuid().ToString("N");
		}

		private void GenerateReportItemDefinition()
		{
			m_generatedReportItem = null;
			ICustomReportItem controlInstance = base.RenderingContext.OdpContext.CriProcessingControls.GetControlInstance(CriDef.Type, base.RenderingContext.OdpContext.ExtFactory);
			if (controlInstance == null)
			{
				return;
			}
			try
			{
				controlInstance.GenerateReportItemDefinition(this);
			}
			catch (Exception ex)
			{
				base.RenderingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIProcessingError, Severity.Warning, Name, Type);
				Global.Tracer.TraceException(TraceLevel.Error, RPRes.rsCRIProcessingError(Name, Type) + " " + ex.ToString());
				return;
			}
			if (m_generatedReportItem == null)
			{
				base.RenderingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemNull, Severity.Warning, CriDef.ObjectType, Name, Type);
				return;
			}
			m_generatedReportItem.ConstructReportItemDefinition();
			m_generatedReportItem.CriGenerationPhase = CriGenerationPhases.None;
			string text = CreateChunkName();
			OnDemandProcessingContext odpContext = base.RenderingContext.OdpContext;
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot = odpContext.OdpMetadata.ReportSnapshot;
			using (Stream stream = odpContext.ChunkFactory.CreateChunk(text, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.GeneratedReportItems, null))
			{
				IntermediateFormatWriter intermediateFormatWriter = new IntermediateFormatWriter(stream, odpContext.GetActiveCompatibilityVersion());
				Microsoft.ReportingServices.ReportIntermediateFormat.Visibility visibility = m_generatedReportItem.ReportItemDef.Visibility;
				m_generatedReportItem.ReportItemDef.Visibility = null;
				intermediateFormatWriter.Write(m_generatedReportItem.ReportItemDef);
				m_generatedReportItem.ReportItemDef.Visibility = visibility;
				stream.Flush();
			}
			reportSnapshot.AddGeneratedReportItemChunkName(GetGeneratedDefinitionChunkKey(), text);
		}

		internal void EvaluateGeneratedReportItemInstance()
		{
			Global.Tracer.Assert(m_generatedReportItem.CriGenerationPhase == CriGenerationPhases.None);
			m_generatedReportItem.CriGenerationPhase = CriGenerationPhases.Instance;
			try
			{
				if (LoadGeneratedReportItemInstance())
				{
					return;
				}
				try
				{
					ICustomReportItem controlInstance = base.RenderingContext.OdpContext.CriProcessingControls.GetControlInstance(CriDef.Type, base.RenderingContext.OdpContext.ExtFactory);
					Global.Tracer.Assert(controlInstance != null, "(null != control)");
					controlInstance.EvaluateReportItemInstance(this);
					m_generatedReportItem.CompleteCriGeneratedInstanceEvaluation();
				}
				catch (Exception innerException)
				{
					throw new RenderingObjectModelException(ErrorCode.rsCRIProcessingError, innerException, Name, Type);
				}
			}
			finally
			{
				m_generatedReportItem.CriGenerationPhase = CriGenerationPhases.None;
			}
			SaveGeneratedReportItemInstance();
		}

		private string GetGeneratedDefinitionChunkKey()
		{
			return DefinitionPath;
		}

		private string GetGeneratedInstanceChunkKey()
		{
			return GetGeneratedDefinitionChunkKey() + "_II_" + base.Instance.UniqueName;
		}

		private bool LoadGeneratedReportItemInstance()
		{
			Global.Tracer.Assert(m_generatedReportItem != null && m_generatedReportItem.Instance != null, "m_generatedReportItem != null && m_generatedReportItem.Instance != null");
			if (m_dynamicWidth != null || m_dynamicHeight != null)
			{
				return false;
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot = base.RenderingContext.OdpContext.OdpMetadata.ReportSnapshot;
			string name;
			if (CriDef.RepeatWith != null)
			{
				if (!reportSnapshot.TryGetImageChunkName(GetGeneratedInstanceChunkKey(), out name))
				{
					return false;
				}
				((ImageInstance)m_generatedReportItem.Instance).StreamName = name;
				return true;
			}
			if (!reportSnapshot.TryGetGeneratedReportItemChunkName(GetGeneratedInstanceChunkKey(), out name))
			{
				return false;
			}
			string mimeType;
			Stream chunk = base.RenderingContext.OdpContext.ChunkFactory.GetChunk(name, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.GeneratedReportItems, ChunkMode.Open, out mimeType);
			if (chunk == null)
			{
				return false;
			}
			using (chunk)
			{
				ROMInstanceObjectCreator rOMInstanceObjectCreator = new ROMInstanceObjectCreator(m_generatedReportItem.Instance);
				IntermediateFormatReader intermediateFormatReader = new IntermediateFormatReader(chunk, rOMInstanceObjectCreator, rOMInstanceObjectCreator);
				IPersistable persistable = intermediateFormatReader.ReadRIFObject();
				Global.Tracer.Assert(persistable is ReportItemInstance, "reportItemInstance is ReportItemInstance");
				Global.Tracer.Assert(!intermediateFormatReader.HasReferences, "!reader.HasReferences");
			}
			return true;
		}

		private void SaveGeneratedReportItemInstance()
		{
			if (m_dynamicWidth != null || m_dynamicHeight != null)
			{
				return;
			}
			OnDemandProcessingContext odpContext = base.RenderingContext.OdpContext;
			Microsoft.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot = odpContext.OdpMetadata.ReportSnapshot;
			IChunkFactory chunkFactory = odpContext.ChunkFactory;
			if (CriDef.RepeatWith == null)
			{
				string text = CreateChunkName();
				using (Stream stream = chunkFactory.CreateChunk(text, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.GeneratedReportItems, null))
				{
					new IntermediateFormatWriter(stream, odpContext.GetActiveCompatibilityVersion()).Write(m_generatedReportItem.Instance);
					stream.Flush();
				}
				reportSnapshot.AddGeneratedReportItemChunkName(GetGeneratedInstanceChunkKey(), text);
			}
			else
			{
				ImageInstance imageInstance = (ImageInstance)m_generatedReportItem.Instance;
				reportSnapshot.AddImageChunkName(name: imageInstance.StreamName = ImageHelper.StoreImageDataInChunk(Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image, imageInstance.ImageData, imageInstance.MIMEType, base.RenderingContext.OdpContext.OdpMetadata, base.RenderingContext.OdpContext.ChunkFactory), definitionKey: GetGeneratedInstanceChunkKey());
			}
		}

		public void CreateCriImageDefinition()
		{
			if (m_generatedReportItem != null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			Microsoft.ReportingServices.ReportIntermediateFormat.Image image = new Microsoft.ReportingServices.ReportIntermediateFormat.Image(-m_reportItemDef.ID, m_reportItemDef.Parent);
			image.ParentInstancePath = (Microsoft.ReportingServices.ReportIntermediateFormat.IDOwner)m_reportItemDef.ParentInstancePath;
			image.GlobalID = -CriDef.GlobalID;
			image.Name = "Image";
			m_reportItemDef.SetupCriRenderItemDef(image);
			image.Source = Image.SourceType.Database;
			image.Action = new Microsoft.ReportingServices.ReportIntermediateFormat.Action();
			Image image2 = new Image(ParentScope, base.ParentDefinitionPath, m_indexIntoParentCollectionDef, image, base.RenderingContext);
			image2.CriOwner = this;
			image2.CriGenerationPhase = CriGenerationPhases.Definition;
			m_generatedReportItem = image2;
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (m_instance == null)
			{
				m_instance = new CustomReportItemInstance(this);
			}
			return m_instance;
		}

		internal override void SetNewContextChildren()
		{
			if (m_data != null)
			{
				m_data.SetNewContext();
			}
			if (m_altReportItem != null && m_isOldSnapshot)
			{
				m_altReportItem.SetNewContext();
			}
			if (m_generatedReportItem != null)
			{
				m_generatedReportItem.SetNewContext();
			}
		}

		internal override void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			base.UpdateRenderReportItem(renderReportItem);
			if (renderReportItem != null)
			{
				m_altReportItem = null;
				m_data = null;
				return;
			}
			if (m_data != null && m_data.DataColumnHierarchy != null)
			{
				m_data.DataColumnHierarchy.ResetContext();
			}
			if (m_data != null && m_data.DataRowHierarchy != null)
			{
				m_data.DataRowHierarchy.ResetContext();
			}
		}

		internal int GetCurrentMemberCellDefinitionIndex()
		{
			return m_memberCellDefinitionIndex;
		}

		internal int GetAndIncrementMemberCellDefinitionIndex()
		{
			return m_memberCellDefinitionIndex++;
		}

		internal void ResetMemberCellDefinitionIndex(int startIndex)
		{
			m_memberCellDefinitionIndex = startIndex;
		}
	}
}
