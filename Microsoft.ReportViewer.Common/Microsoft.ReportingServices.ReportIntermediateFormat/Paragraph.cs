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
	internal sealed class Paragraph : IDOwner, IPersistable, IStyleContainer
	{
		private List<TextRun> m_textRuns;

		private Style m_styleClass;

		private ExpressionInfo m_leftIndent;

		private ExpressionInfo m_rightIndent;

		private ExpressionInfo m_hangingIndent;

		private ExpressionInfo m_spaceBefore;

		private ExpressionInfo m_spaceAfter;

		private ExpressionInfo m_listLevel;

		private ExpressionInfo m_listStyle;

		private int m_indexInCollection;

		private int m_exprHostID = -1;

		private bool m_textRunValueReferenced;

		[Reference]
		private TextBox m_textBox;

		[NonSerialized]
		private string m_idString;

		[NonSerialized]
		private ParagraphImpl m_paragraphImpl;

		[NonSerialized]
		private string m_name;

		[NonSerialized]
		private ParagraphExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal string IDString
		{
			get
			{
				if (m_idString == null)
				{
					m_idString = m_textBox.GlobalID.ToString(CultureInfo.InvariantCulture) + "x" + m_indexInCollection.ToString(CultureInfo.InvariantCulture);
				}
				return m_idString;
			}
		}

		internal List<TextRun> TextRuns
		{
			get
			{
				return m_textRuns;
			}
			set
			{
				m_textRuns = value;
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

		internal ExpressionInfo LeftIndent
		{
			get
			{
				return m_leftIndent;
			}
			set
			{
				m_leftIndent = value;
			}
		}

		internal ExpressionInfo RightIndent
		{
			get
			{
				return m_rightIndent;
			}
			set
			{
				m_rightIndent = value;
			}
		}

		internal ExpressionInfo HangingIndent
		{
			get
			{
				return m_hangingIndent;
			}
			set
			{
				m_hangingIndent = value;
			}
		}

		internal ExpressionInfo SpaceBefore
		{
			get
			{
				return m_spaceBefore;
			}
			set
			{
				m_spaceBefore = value;
			}
		}

		internal ExpressionInfo SpaceAfter
		{
			get
			{
				return m_spaceAfter;
			}
			set
			{
				m_spaceAfter = value;
			}
		}

		internal ExpressionInfo ListStyle
		{
			get
			{
				return m_listStyle;
			}
			set
			{
				m_listStyle = value;
			}
		}

		internal ExpressionInfo ListLevel
		{
			get
			{
				return m_listLevel;
			}
			set
			{
				m_listLevel = value;
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

		internal TextBox TextBox
		{
			get
			{
				return m_textBox;
			}
			set
			{
				m_textBox = value;
			}
		}

		internal bool TextRunValueReferenced
		{
			get
			{
				return m_textRunValueReferenced;
			}
			set
			{
				m_textRunValueReferenced = value;
			}
		}

		IInstancePath IStyleContainer.InstancePath => m_textBox;

		Style IStyleContainer.StyleClass => m_styleClass;

		public Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Paragraph;

		public string Name
		{
			get
			{
				if (m_name == null)
				{
					m_name = m_textBox.Name + ".Paragraphs[" + m_indexInCollection.ToString(CultureInfo.InvariantCulture) + "]";
				}
				return m_name;
			}
		}

		internal ParagraphExprHost ExprHost => m_exprHost;

		internal Paragraph(TextBox textbox, int index, int id)
			: base(id)
		{
			m_indexInCollection = index;
			m_textBox = textbox;
			m_textRuns = new List<TextRun>();
		}

		internal Paragraph()
		{
		}

		internal bool Initialize(InitializationContext context, out bool aHasExpressionBasedValue)
		{
			bool flag = false;
			bool hasExpressionBasedValue = false;
			aHasExpressionBasedValue = false;
			context.ExprHostBuilder.ParagraphStart(m_indexInCollection);
			if (m_textRuns != null)
			{
				foreach (TextRun textRun in m_textRuns)
				{
					flag |= textRun.Initialize(context, out hasExpressionBasedValue);
					aHasExpressionBasedValue |= hasExpressionBasedValue;
				}
			}
			if (m_styleClass != null)
			{
				m_styleClass.Initialize(context);
			}
			if (m_leftIndent != null)
			{
				m_leftIndent.Initialize("LeftIndent", context);
				context.ExprHostBuilder.ParagraphLeftIndent(m_leftIndent);
			}
			if (m_rightIndent != null)
			{
				m_rightIndent.Initialize("RightIndent", context);
				context.ExprHostBuilder.ParagraphRightIndent(m_rightIndent);
			}
			if (m_hangingIndent != null)
			{
				m_hangingIndent.Initialize("HangingIndent", context);
				context.ExprHostBuilder.ParagraphHangingIndent(m_hangingIndent);
			}
			if (m_spaceBefore != null)
			{
				m_spaceBefore.Initialize("SpaceBefore", context);
				context.ExprHostBuilder.ParagraphSpaceBefore(m_spaceBefore);
			}
			if (m_spaceAfter != null)
			{
				m_spaceAfter.Initialize("SpaceAfter", context);
				context.ExprHostBuilder.ParagraphSpaceAfter(m_spaceAfter);
			}
			if (m_listStyle != null)
			{
				m_listStyle.Initialize("ListStyle", context);
				context.ExprHostBuilder.ParagraphListStyle(m_listStyle);
			}
			if (m_listLevel != null)
			{
				m_listLevel.Initialize("ListLevel", context);
				context.ExprHostBuilder.ParagraphListLevel(m_listLevel);
			}
			m_exprHostID = context.ExprHostBuilder.ParagraphEnd();
			return flag;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			Paragraph paragraph = (Paragraph)base.PublishClone(context);
			if (m_textRuns != null)
			{
				paragraph.m_textRuns = new List<TextRun>(m_textRuns.Count);
				foreach (TextRun textRun2 in m_textRuns)
				{
					TextRun textRun = (TextRun)textRun2.PublishClone(context);
					textRun.Paragraph = paragraph;
					paragraph.m_textRuns.Add(textRun);
				}
			}
			if (m_styleClass != null)
			{
				paragraph.m_styleClass = (Style)m_styleClass.PublishClone(context);
			}
			if (m_leftIndent != null)
			{
				paragraph.m_leftIndent = (ExpressionInfo)m_leftIndent.PublishClone(context);
			}
			if (m_rightIndent != null)
			{
				paragraph.m_rightIndent = (ExpressionInfo)m_rightIndent.PublishClone(context);
			}
			if (m_hangingIndent != null)
			{
				paragraph.m_hangingIndent = (ExpressionInfo)m_hangingIndent.PublishClone(context);
			}
			if (m_spaceBefore != null)
			{
				paragraph.m_spaceBefore = (ExpressionInfo)m_spaceBefore.PublishClone(context);
			}
			if (m_spaceAfter != null)
			{
				paragraph.m_spaceAfter = (ExpressionInfo)m_spaceAfter.PublishClone(context);
			}
			if (m_listStyle != null)
			{
				paragraph.m_listStyle = (ExpressionInfo)m_listStyle.PublishClone(context);
			}
			if (m_listLevel != null)
			{
				paragraph.m_listLevel = (ExpressionInfo)m_listLevel.PublishClone(context);
			}
			return paragraph;
		}

		internal bool DetermineSimplicity()
		{
			if (m_textRuns.Count == 1 && m_listLevel == null && m_listStyle == null && m_leftIndent == null && m_rightIndent == null && m_hangingIndent == null && m_spaceBefore == null && m_spaceAfter == null)
			{
				return m_textRuns[0].DetermineSimplicity();
			}
			return false;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Style, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			list.Add(new MemberInfo(MemberName.TextRuns, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextRun));
			list.Add(new MemberInfo(MemberName.LeftIndent, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RightIndent, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HangingIndent, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SpaceBefore, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SpaceAfter, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ListStyle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ListLevel, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.TextBox, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.TextRunValueReferenced, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Paragraph, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.TextRuns:
					writer.Write(m_textRuns);
					break;
				case MemberName.Style:
					writer.Write(m_styleClass);
					break;
				case MemberName.LeftIndent:
					writer.Write(m_leftIndent);
					break;
				case MemberName.RightIndent:
					writer.Write(m_rightIndent);
					break;
				case MemberName.HangingIndent:
					writer.Write(m_hangingIndent);
					break;
				case MemberName.SpaceBefore:
					writer.Write(m_spaceBefore);
					break;
				case MemberName.SpaceAfter:
					writer.Write(m_spaceAfter);
					break;
				case MemberName.ListStyle:
					writer.Write(m_listStyle);
					break;
				case MemberName.ListLevel:
					writer.Write(m_listLevel);
					break;
				case MemberName.IndexInCollection:
					writer.Write(m_indexInCollection);
					break;
				case MemberName.TextBox:
					writer.WriteReference(m_textBox);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.TextRunValueReferenced:
					writer.Write(m_textRunValueReferenced);
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
				case MemberName.TextRuns:
					m_textRuns = reader.ReadGenericListOfRIFObjects<TextRun>();
					break;
				case MemberName.Style:
					m_styleClass = (Style)reader.ReadRIFObject();
					break;
				case MemberName.LeftIndent:
					m_leftIndent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.RightIndent:
					m_rightIndent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HangingIndent:
					m_hangingIndent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SpaceBefore:
					m_spaceBefore = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SpaceAfter:
					m_spaceAfter = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ListStyle:
					m_listStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ListLevel:
					m_listLevel = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IndexInCollection:
					m_indexInCollection = reader.ReadInt32();
					break;
				case MemberName.TextBox:
					m_textBox = reader.ReadReference<TextBox>(this);
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.TextRunValueReferenced:
					m_textRunValueReferenced = reader.ReadBoolean();
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
				if (memberName == MemberName.TextBox)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					Global.Tracer.Assert(referenceableItems[item.RefID] is TextBox);
					m_textBox = (TextBox)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Paragraph;
		}

		internal void SetExprHost(TextBoxExprHost textBoxExprHost, ObjectModelImpl reportObjectModel)
		{
			if (m_exprHostID >= 0)
			{
				m_exprHost = textBoxExprHost.ParagraphHostsRemotable[m_exprHostID];
				Global.Tracer.Assert(m_exprHost != null && reportObjectModel != null);
				m_exprHost.SetReportObjectModel(reportObjectModel);
				if (m_styleClass != null)
				{
					m_styleClass.SetStyleExprHost(m_exprHost);
				}
				if (m_textRuns == null)
				{
					return;
				}
				foreach (TextRun textRun in m_textRuns)
				{
					textRun.SetExprHost(m_exprHost, reportObjectModel);
				}
			}
			else if (m_ID == -1)
			{
				if (m_styleClass != null)
				{
					m_styleClass.SetStyleExprHost(textBoxExprHost);
					m_textRuns[0].StyleClass.SetStyleExprHost(textBoxExprHost);
				}
				m_textRuns[0].SetExprHost(new Microsoft.ReportingServices.RdlExpressions.ReportRuntime.TextRunExprHostWrapper(textBoxExprHost));
			}
		}

		internal string EvaluateSpaceAfter(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_textBox, instance);
			return context.ReportRuntime.EvaluateParagraphSpaceAfterExpression(this);
		}

		internal string EvaluateSpaceBefore(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_textBox, instance);
			return context.ReportRuntime.EvaluateParagraphSpaceBeforeExpression(this);
		}

		internal string EvaluateListStyle(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_textBox, instance);
			return context.ReportRuntime.EvaluateParagraphListStyleExpression(this);
		}

		internal int? EvaluateListLevel(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_textBox, instance);
			return context.ReportRuntime.EvaluateParagraphListLevelExpression(this);
		}

		internal string EvaluateLeftIndent(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_textBox, instance);
			return context.ReportRuntime.EvaluateParagraphLeftIndentExpression(this);
		}

		internal string EvaluateRightIndent(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_textBox, instance);
			return context.ReportRuntime.EvaluateParagraphRightIndentExpression(this);
		}

		internal string EvaluateHangingIndent(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_textBox, instance);
			return context.ReportRuntime.EvaluateParagraphHangingIndentExpression(this);
		}

		internal ParagraphImpl GetParagraphImpl(OnDemandProcessingContext context)
		{
			if (m_paragraphImpl == null)
			{
				m_paragraphImpl = (ParagraphImpl)m_textBox.GetTextBoxImpl(context).Paragraphs[m_indexInCollection];
			}
			return m_paragraphImpl;
		}
	}
}
