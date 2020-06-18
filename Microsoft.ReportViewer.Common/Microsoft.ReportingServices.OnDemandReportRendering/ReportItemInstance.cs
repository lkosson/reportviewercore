using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class ReportItemInstance : ReportElementInstance
	{
		[NonSerialized]
		protected string m_uniqueName;

		private string m_toolTip;

		private string m_bookmark;

		private string m_documentMapLabel;

		[NonSerialized]
		private bool m_toolTipEvaluated;

		[NonSerialized]
		private bool m_bookmarkEvaluated;

		[NonSerialized]
		private bool m_documentMapLabelEvaluated;

		[NonSerialized]
		protected VisibilityInstance m_visibility;

		private static readonly Declaration m_Declaration = GetDeclaration();

		public virtual string UniqueName
		{
			get
			{
				if (m_reportElementDef.IsOldSnapshot)
				{
					return m_reportElementDef.RenderReportItem.UniqueName;
				}
				if (m_uniqueName == null)
				{
					m_uniqueName = m_reportElementDef.ReportItemDef.UniqueName;
				}
				return m_uniqueName;
			}
		}

		public string ToolTip
		{
			get
			{
				if (!m_toolTipEvaluated)
				{
					m_toolTipEvaluated = true;
					if (m_reportElementDef.IsOldSnapshot)
					{
						m_toolTip = m_reportElementDef.RenderReportItem.ToolTip;
					}
					else
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItemDef = m_reportElementDef.ReportItemDef;
						if (reportItemDef.ToolTip != null)
						{
							if (!reportItemDef.ToolTip.IsExpression)
							{
								m_toolTip = reportItemDef.ToolTip.StringValue;
							}
							else if (m_reportElementDef.CriOwner == null)
							{
								m_toolTip = reportItemDef.EvaluateToolTip(ReportScopeInstance, RenderingContext.OdpContext);
							}
						}
					}
				}
				return m_toolTip;
			}
			set
			{
				if (m_reportElementDef.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_reportElementDef.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !((ReportItem)m_reportElementDef).ToolTip.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				m_toolTipEvaluated = true;
				m_toolTip = value;
			}
		}

		public string Bookmark
		{
			get
			{
				if (!m_bookmarkEvaluated)
				{
					m_bookmarkEvaluated = true;
					if (m_reportElementDef.IsOldSnapshot)
					{
						m_bookmark = m_reportElementDef.RenderReportItem.Bookmark;
					}
					else
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItemDef = m_reportElementDef.ReportItemDef;
						if (reportItemDef.Bookmark != null)
						{
							if (!reportItemDef.Bookmark.IsExpression)
							{
								m_bookmark = reportItemDef.Bookmark.StringValue;
							}
							else if (m_reportElementDef.CriOwner == null)
							{
								m_bookmark = reportItemDef.EvaluateBookmark(ReportScopeInstance, RenderingContext.OdpContext);
							}
						}
					}
				}
				return m_bookmark;
			}
			set
			{
				if (m_reportElementDef.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_reportElementDef.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !((ReportItem)m_reportElementDef).Bookmark.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				m_bookmarkEvaluated = true;
				m_bookmark = value;
			}
		}

		public string DocumentMapLabel
		{
			get
			{
				if (!m_documentMapLabelEvaluated)
				{
					m_documentMapLabelEvaluated = true;
					if (m_reportElementDef.IsOldSnapshot)
					{
						m_documentMapLabel = m_reportElementDef.RenderReportItem.Label;
					}
					else
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItemDef = m_reportElementDef.ReportItemDef;
						if (reportItemDef.DocumentMapLabel != null)
						{
							if (!reportItemDef.DocumentMapLabel.IsExpression)
							{
								m_documentMapLabel = reportItemDef.DocumentMapLabel.StringValue;
							}
							else if (m_reportElementDef.CriOwner == null)
							{
								m_documentMapLabel = reportItemDef.EvaluateDocumentMapLabel(ReportScopeInstance, RenderingContext.OdpContext);
							}
						}
					}
				}
				return m_documentMapLabel;
			}
			set
			{
				if (m_reportElementDef.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (m_reportElementDef.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !((ReportItem)m_reportElementDef).DocumentMapLabel.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				m_documentMapLabelEvaluated = true;
				m_documentMapLabel = value;
			}
		}

		public virtual VisibilityInstance Visibility
		{
			get
			{
				if (m_visibility == null && ((ReportItem)m_reportElementDef).Visibility != null)
				{
					m_visibility = new ReportItemVisibilityInstance(m_reportElementDef as ReportItem);
				}
				return m_visibility;
			}
		}

		internal RenderingContext RenderingContext => m_reportElementDef.RenderingContext;

		internal ReportItemInstance(ReportItem reportItemDef)
			: base(reportItemDef)
		{
		}

		protected string GetDefaultFontFamily()
		{
			if (RenderingContext.OdpContext == null)
			{
				return null;
			}
			return RenderingContext.OdpContext.ReportDefinition.DefaultFontFamily;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_uniqueName = null;
			m_toolTipEvaluated = false;
			m_toolTip = null;
			m_bookmarkEvaluated = false;
			m_bookmark = null;
			m_documentMapLabelEvaluated = false;
			m_documentMapLabel = null;
			if (m_visibility != null)
			{
				m_visibility.SetNewContext();
			}
		}

		internal override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			ReportItem reportItem = (ReportItem)base.ReportElementDef;
			reportItem.CustomProperties.GetDynamicValues(out List<string> customPropertyNames, out List<object> customPropertyValues);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ToolTip:
				{
					string value2 = null;
					if (reportItem.ToolTip.IsExpression)
					{
						value2 = m_toolTip;
					}
					writer.Write(value2);
					break;
				}
				case MemberName.Bookmark:
				{
					string value3 = null;
					if (reportItem.Bookmark.IsExpression)
					{
						value3 = m_bookmark;
					}
					writer.Write(value3);
					break;
				}
				case MemberName.Label:
				{
					string value = null;
					if (reportItem.DocumentMapLabel.IsExpression)
					{
						value = m_documentMapLabel;
					}
					writer.Write(value);
					break;
				}
				case MemberName.CustomPropertyNames:
					writer.WriteListOfPrimitives(customPropertyNames);
					break;
				case MemberName.CustomPropertyValues:
					writer.WriteListOfPrimitives(customPropertyValues);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		internal override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			ReportItem reportItem = (ReportItem)base.ReportElementDef;
			List<string> customPropertyNames = null;
			List<object> customPropertyValues = null;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ToolTip:
				{
					string text3 = reader.ReadString();
					if (reportItem.ToolTip.IsExpression)
					{
						m_toolTip = text3;
					}
					else
					{
						Global.Tracer.Assert(text3 == null, "(toolTip == null)");
					}
					break;
				}
				case MemberName.Bookmark:
				{
					string text2 = reader.ReadString();
					if (reportItem.Bookmark.IsExpression)
					{
						m_bookmark = text2;
					}
					else
					{
						Global.Tracer.Assert(text2 == null, "(bookmark == null)");
					}
					break;
				}
				case MemberName.Label:
				{
					string text = reader.ReadString();
					if (reportItem.DocumentMapLabel.IsExpression)
					{
						m_documentMapLabel = text;
					}
					else
					{
						Global.Tracer.Assert(text == null, "(documentMapLabel == null)");
					}
					break;
				}
				case MemberName.CustomPropertyNames:
					customPropertyNames = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.CustomPropertyValues:
					customPropertyValues = reader.ReadListOfPrimitives<object>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
			reportItem.CustomProperties.SetDynamicValues(customPropertyNames, customPropertyValues);
		}

		internal override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemInstance;
		}

		[SkipMemberStaticValidation(MemberName.CustomPropertyNames)]
		[SkipMemberStaticValidation(MemberName.CustomPropertyValues)]
		private static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ToolTip, Token.String));
			list.Add(new MemberInfo(MemberName.Bookmark, Token.String));
			list.Add(new MemberInfo(MemberName.Label, Token.String));
			list.Add(new MemberInfo(MemberName.CustomPropertyNames, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
			list.Add(new MemberInfo(MemberName.CustomPropertyValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Object));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportElementInstance, list);
		}
	}
}
