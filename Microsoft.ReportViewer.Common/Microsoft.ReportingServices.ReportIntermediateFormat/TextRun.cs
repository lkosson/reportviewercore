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
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class TextRun : IDOwner, IPersistable, IStyleContainer, IActionOwner
	{
		private ExpressionInfo m_value;

		private ExpressionInfo m_toolTip;

		private Style m_styleClass;

		private Action m_action;

		private string m_label;

		private ExpressionInfo m_markupType;

		private DataType m_constantDataType = DataType.String;

		private int m_indexInCollection;

		private int m_exprHostID = -1;

		private bool m_valueReferenced;

		[Reference]
		private Paragraph m_paragraph;

		[NonSerialized]
		private string m_idString;

		[NonSerialized]
		private string m_name;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private TextRunImpl m_textRunImpl;

		[NonSerialized]
		private TextRunExprHost m_exprHost;

		[NonSerialized]
		private TypeCode m_valueType = TypeCode.String;

		[NonSerialized]
		private bool m_valueTypeSet;

		[NonSerialized]
		private Formatter m_formatter;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		internal string IDString
		{
			get
			{
				if (m_idString == null)
				{
					m_idString = m_paragraph.IDString + "x" + m_indexInCollection.ToString(CultureInfo.InvariantCulture);
				}
				return m_idString;
			}
		}

		internal ExpressionInfo Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}

		internal string Label
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

		internal ExpressionInfo MarkupType
		{
			get
			{
				return m_markupType;
			}
			set
			{
				m_markupType = value;
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

		internal Paragraph Paragraph
		{
			get
			{
				return m_paragraph;
			}
			set
			{
				m_paragraph = value;
			}
		}

		internal int IndexInCollection
		{
			get
			{
				return m_indexInCollection;
			}
			set
			{
				m_indexInCollection = value;
			}
		}

		internal DataType DataType
		{
			get
			{
				return m_constantDataType;
			}
			set
			{
				m_constantDataType = value;
			}
		}

		internal bool ValueReferenced
		{
			get
			{
				return m_valueReferenced;
			}
			set
			{
				m_valueReferenced = value;
			}
		}

		IInstancePath IStyleContainer.InstancePath => m_paragraph.TextBox;

		Style IStyleContainer.StyleClass => m_styleClass;

		public Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.TextRun;

		public string Name
		{
			get
			{
				if (m_name == null)
				{
					m_name = m_paragraph.Name + ".TextRuns[" + m_indexInCollection.ToString(CultureInfo.InvariantCulture) + "]";
				}
				return m_name;
			}
		}

		internal TypeCode ValueTypeCode
		{
			get
			{
				if (!m_valueTypeSet)
				{
					m_valueTypeSet = true;
					if (m_value == null)
					{
						m_valueType = TypeCode.String;
					}
					else if (!m_value.IsExpression)
					{
						m_valueType = m_value.ConstantTypeCode;
					}
					else
					{
						m_valueType = TypeCode.Object;
					}
				}
				return m_valueType;
			}
		}

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

		internal TextRunExprHost ExprHost => m_exprHost;

		internal TextRun(Paragraph paragraph, int index, int id)
			: base(id)
		{
			m_indexInCollection = index;
			m_paragraph = paragraph;
		}

		internal TextRun()
		{
		}

		internal bool Initialize(InitializationContext context, out bool hasExpressionBasedValue)
		{
			bool result = false;
			hasExpressionBasedValue = false;
			context.ExprHostBuilder.TextRunStart(m_indexInCollection);
			if (m_value != null)
			{
				result = true;
				hasExpressionBasedValue = m_value.IsExpression;
				m_value.Initialize("Value", context);
				context.ExprHostBuilder.TextRunValue(m_value);
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.TextRunToolTip(m_toolTip);
			}
			if (m_markupType != null)
			{
				m_markupType.Initialize("MarkupType", context);
				context.ExprHostBuilder.TextRunMarkupType(m_markupType);
			}
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_styleClass != null)
			{
				m_styleClass.Initialize(context);
			}
			m_exprHostID = context.ExprHostBuilder.TextRunEnd();
			return result;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			TextRun textRun = (TextRun)base.PublishClone(context);
			if (m_value != null)
			{
				textRun.m_value = (ExpressionInfo)m_value.PublishClone(context);
			}
			if (m_toolTip != null)
			{
				textRun.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			if (m_styleClass != null)
			{
				textRun.m_styleClass = (Style)m_styleClass.PublishClone(context);
			}
			if (m_markupType != null)
			{
				textRun.m_markupType = (ExpressionInfo)m_markupType.PublishClone(context);
			}
			if (m_action != null)
			{
				textRun.m_action = (Action)m_action.PublishClone(context);
			}
			return textRun;
		}

		internal bool DetermineSimplicity()
		{
			if (m_markupType == null || (!m_markupType.IsExpression && string.Equals(m_markupType.StringValue, "None", StringComparison.Ordinal)))
			{
				TextBox textBox = m_paragraph.TextBox;
				if ((m_action == null || textBox.Action == null) && (m_toolTip == null || textBox.ToolTip == null))
				{
					if (m_action != null)
					{
						textBox.Action = m_action;
						m_action = null;
					}
					if (m_toolTip != null)
					{
						textBox.ToolTip = m_toolTip;
						m_toolTip = null;
					}
					return true;
				}
			}
			return false;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Label, Token.String));
			list.Add(new MemberInfo(MemberName.MarkupType, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Style, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.DataType, Token.Enum));
			list.Add(new MemberInfo(MemberName.IndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.Paragraph, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Paragraph, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ValueReferenced, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextRun, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Value:
					writer.Write(m_value);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
					break;
				case MemberName.Style:
					writer.Write(m_styleClass);
					break;
				case MemberName.Label:
					writer.Write(m_label);
					break;
				case MemberName.MarkupType:
					writer.Write(m_markupType);
					break;
				case MemberName.Action:
					writer.Write(m_action);
					break;
				case MemberName.DataType:
					writer.WriteEnum((int)m_constantDataType);
					break;
				case MemberName.IndexInCollection:
					writer.Write(m_indexInCollection);
					break;
				case MemberName.Paragraph:
					writer.WriteReference(m_paragraph);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.ValueReferenced:
					writer.Write(m_valueReferenced);
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
				case MemberName.Value:
					m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Style:
					m_styleClass = (Style)reader.ReadRIFObject();
					break;
				case MemberName.Label:
					m_label = reader.ReadString();
					break;
				case MemberName.MarkupType:
					m_markupType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Action:
					m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.DataType:
					m_constantDataType = (DataType)reader.ReadEnum();
					break;
				case MemberName.IndexInCollection:
					m_indexInCollection = reader.ReadInt32();
					break;
				case MemberName.Paragraph:
					m_paragraph = reader.ReadReference<Paragraph>(this);
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.ValueReferenced:
					m_valueReferenced = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.Paragraph)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					Global.Tracer.Assert(referenceableItems[item.RefID] is Paragraph);
					m_paragraph = (Paragraph)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextRun;
		}

		internal void SetExprHost(TextRunExprHost textRunExprHost)
		{
			m_exprHost = textRunExprHost;
		}

		internal void SetExprHost(ParagraphExprHost paragraphExprHost, ObjectModelImpl reportObjectModel)
		{
			if (m_exprHostID >= 0)
			{
				m_exprHost = paragraphExprHost.TextRunHostsRemotable[m_exprHostID];
				Global.Tracer.Assert(m_exprHost != null && reportObjectModel != null);
				m_exprHost.SetReportObjectModel(reportObjectModel);
				if (m_action != null && m_exprHost.ActionInfoHost != null)
				{
					m_action.SetExprHost(m_exprHost.ActionInfoHost, reportObjectModel);
				}
				if (m_styleClass != null)
				{
					m_styleClass.SetStyleExprHost(m_exprHost);
				}
			}
		}

		internal string EvaluateMarkupType(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_paragraph.TextBox, instance);
			return context.ReportRuntime.EvaluateTextRunMarkupTypeExpression(this);
		}

		internal string EvaluateToolTip(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_paragraph.TextBox, instance);
			return context.ReportRuntime.EvaluateTextRunToolTipExpression(this);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateValue(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			return GetTextRunImpl(context).GetResult(instance);
		}

		internal List<string> GetFieldsUsedInValueExpression(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			return GetTextRunImpl(context).GetFieldsUsedInValueExpression(romInstance);
		}

		private TextRunImpl GetTextRunImpl(OnDemandProcessingContext context)
		{
			if (m_textRunImpl == null)
			{
				m_textRunImpl = (TextRunImpl)m_paragraph.GetParagraphImpl(context).TextRuns[m_indexInCollection];
			}
			return m_textRunImpl;
		}

		internal string FormatTextRunValue(Microsoft.ReportingServices.RdlExpressions.VariantResult textRunResult, OnDemandProcessingContext context)
		{
			string result = null;
			if (textRunResult.ErrorOccurred)
			{
				result = RPRes.rsExpressionErrorValue;
			}
			else if (textRunResult.Value != null)
			{
				result = FormatTextRunValue(textRunResult.Value, textRunResult.TypeCode, null, context);
			}
			return result;
		}

		internal string FormatTextRunValue(object textRunValue, TypeCode typeCode, OnDemandProcessingContext context)
		{
			return FormatTextRunValue(textRunValue, typeCode, string.Empty, context);
		}

		private string FormatTextRunValue(object textRunValue, TypeCode typeCode, string formatCode, OnDemandProcessingContext context)
		{
			if (m_formatter == null)
			{
				m_formatter = new Formatter(m_styleClass, context, Microsoft.ReportingServices.ReportProcessing.ObjectType.TextRun, Name);
			}
			return m_formatter.FormatValue(textRunValue, formatCode, typeCode);
		}
	}
}
