using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class MapSpatialElementTemplate : MapStyleContainer, IPersistable, IActionOwner
	{
		[NonSerialized]
		protected MapSpatialElementTemplateExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private Action m_action;

		private int m_id;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[Reference]
		protected MapVectorLayer m_mapVectorLayer;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_offsetX;

		private ExpressionInfo m_offsetY;

		private ExpressionInfo m_label;

		private ExpressionInfo m_toolTip;

		private ExpressionInfo m_dataElementLabel;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput;

		internal Action Action
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

		Action IActionOwner.Action => m_action;

		List<string> IActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				return m_fieldsUsedInValueExpression;
			}
			set
			{
				m_fieldsUsedInValueExpression = value;
			}
		}

		internal int ID => m_id;

		internal ExpressionInfo Hidden
		{
			get
			{
				return m_hidden;
			}
			set
			{
				m_hidden = value;
			}
		}

		internal ExpressionInfo OffsetX
		{
			get
			{
				return m_offsetX;
			}
			set
			{
				m_offsetX = value;
			}
		}

		internal ExpressionInfo OffsetY
		{
			get
			{
				return m_offsetY;
			}
			set
			{
				m_offsetY = value;
			}
		}

		internal ExpressionInfo Label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
			}
		}

		internal ExpressionInfo ToolTip
		{
			get
			{
				return m_toolTip;
			}
			set
			{
				m_toolTip = value;
			}
		}

		internal ExpressionInfo DataElementLabel
		{
			get
			{
				return m_dataElementLabel;
			}
			set
			{
				m_dataElementLabel = value;
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

		internal string OwnerName => m_map.Name;

		internal MapSpatialElementTemplateExprHost ExprHost => m_exprHost;

		protected IInstancePath InstancePath => m_mapVectorLayer.InstancePath;

		internal MapSpatialElementTemplate()
		{
		}

		internal MapSpatialElementTemplate(MapVectorLayer mapVectorLayer, Map map, int id)
			: base(map)
		{
			m_id = id;
			m_mapVectorLayer = mapVectorLayer;
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.MapSpatialElementTemplateHidden(m_hidden);
			}
			if (m_offsetX != null)
			{
				m_offsetX.Initialize("OffsetX", context);
				context.ExprHostBuilder.MapSpatialElementTemplateOffsetX(m_offsetX);
			}
			if (m_offsetY != null)
			{
				m_offsetY.Initialize("OffsetY", context);
				context.ExprHostBuilder.MapSpatialElementTemplateOffsetY(m_offsetY);
			}
			if (m_label != null)
			{
				m_label.Initialize("Label", context);
				context.ExprHostBuilder.MapSpatialElementTemplateLabel(m_label);
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.MapSpatialElementTemplateToolTip(m_toolTip);
			}
			if (m_dataElementLabel != null)
			{
				m_dataElementLabel.Initialize("DataElementLabel", context);
				context.ExprHostBuilder.MapSpatialElementTemplateDataElementLabel(m_dataElementLabel);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapSpatialElementTemplate mapSpatialElementTemplate = (MapSpatialElementTemplate)base.PublishClone(context);
			mapSpatialElementTemplate.m_mapVectorLayer = context.CurrentMapVectorLayerClone;
			if (m_action != null)
			{
				mapSpatialElementTemplate.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_hidden != null)
			{
				mapSpatialElementTemplate.m_hidden = (ExpressionInfo)m_hidden.PublishClone(context);
			}
			if (m_offsetX != null)
			{
				mapSpatialElementTemplate.m_offsetX = (ExpressionInfo)m_offsetX.PublishClone(context);
			}
			if (m_offsetY != null)
			{
				mapSpatialElementTemplate.m_offsetY = (ExpressionInfo)m_offsetY.PublishClone(context);
			}
			if (m_label != null)
			{
				mapSpatialElementTemplate.m_label = (ExpressionInfo)m_label.PublishClone(context);
			}
			if (m_toolTip != null)
			{
				mapSpatialElementTemplate.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			if (m_dataElementLabel != null)
			{
				mapSpatialElementTemplate.m_dataElementLabel = (ExpressionInfo)m_dataElementLabel.PublishClone(context);
			}
			return mapSpatialElementTemplate;
		}

		internal void SetExprHost(MapSpatialElementTemplateExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			base.SetExprHost(exprHost, reportObjectModel);
			if (m_action != null && ExprHost.ActionInfoHost != null)
			{
				m_action.SetExprHost(ExprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Hidden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.OffsetX, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.OffsetY, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Label, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DataElementLabel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			list.Add(new MemberInfo(MemberName.MapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer, Token.Reference));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElementTemplate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Action:
					writer.Write(m_action);
					break;
				case MemberName.ID:
					writer.Write(m_id);
					break;
				case MemberName.Hidden:
					writer.Write(m_hidden);
					break;
				case MemberName.OffsetX:
					writer.Write(m_offsetX);
					break;
				case MemberName.OffsetY:
					writer.Write(m_offsetY);
					break;
				case MemberName.Label:
					writer.Write(m_label);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
					break;
				case MemberName.MapVectorLayer:
					writer.WriteReference(m_mapVectorLayer);
					break;
				case MemberName.DataElementLabel:
					writer.Write(m_dataElementLabel);
					break;
				case MemberName.DataElementName:
					writer.Write(m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)m_dataElementOutput);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Action:
					m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.ID:
					m_id = reader.ReadInt32();
					break;
				case MemberName.Hidden:
					m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.OffsetX:
					m_offsetX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.OffsetY:
					m_offsetY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Label:
					m_label = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapVectorLayer:
					m_mapVectorLayer = reader.ReadReference<MapVectorLayer>(this);
					break;
				case MemberName.DataElementLabel:
					m_dataElementLabel = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DataElementName:
					m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.MapVectorLayer)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_mapVectorLayer = (MapVectorLayer)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElementTemplate;
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSpatialElementTemplateHiddenExpression(this, m_map.Name);
		}

		internal double EvaluateOffsetX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSpatialElementTemplateOffsetXExpression(this, m_map.Name);
		}

		internal double EvaluateOffsetY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSpatialElementTemplateOffsetYExpression(this, m_map.Name);
		}

		internal string EvaluateLabel(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			Microsoft.ReportingServices.RdlExpressions.VariantResult result = context.ReportRuntime.EvaluateMapSpatialElementTemplateLabelExpression(this, m_map.Name);
			return m_map.GetFormattedStringFromValue(ref result, context);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			Microsoft.ReportingServices.RdlExpressions.VariantResult result = context.ReportRuntime.EvaluateMapSpatialElementTemplateToolTipExpression(this, m_map.Name);
			return m_map.GetFormattedStringFromValue(ref result, context);
		}

		internal string EvaluateDataElementLabel(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			Microsoft.ReportingServices.RdlExpressions.VariantResult result = context.ReportRuntime.EvaluateMapSpatialElementTemplateDataElementLabelExpression(this, m_map.Name);
			return m_map.GetFormattedStringFromValue(ref result, context);
		}
	}
}
