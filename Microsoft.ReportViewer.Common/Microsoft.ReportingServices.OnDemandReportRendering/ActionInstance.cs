using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ActionInstance : BaseInstance, IPersistable, IActionInstance
	{
		[NonSerialized]
		private bool m_isOldSnapshot;

		[NonSerialized]
		private Microsoft.ReportingServices.ReportRendering.Action m_renderAction;

		[NonSerialized]
		private Action m_actionDef;

		[NonSerialized]
		private ReportUrl m_hyperlink;

		private string m_label;

		private string m_bookmark;

		private string m_hyperlinkText;

		private static readonly Declaration m_Declaration = GetDeclaration();

		public string Label
		{
			get
			{
				if (m_label == null)
				{
					if (m_isOldSnapshot)
					{
						if (m_renderAction != null)
						{
							m_label = m_renderAction.Label;
						}
					}
					else if (m_actionDef.Label != null)
					{
						if (!m_actionDef.Label.IsExpression)
						{
							m_label = m_actionDef.Label.Value;
						}
						else if (m_actionDef.Owner.ReportElementOwner == null || m_actionDef.Owner.ReportElementOwner.CriOwner == null)
						{
							ActionInfo owner = m_actionDef.Owner;
							m_label = m_actionDef.ActionItemDef.EvaluateLabel(ReportScopeInstance, owner.RenderingContext.OdpContext, owner.InstancePath, owner.ObjectType, owner.ObjectName);
						}
					}
				}
				return m_label;
			}
			set
			{
				ReportElement reportElementOwner = m_actionDef.Owner.ReportElementOwner;
				if (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && m_actionDef.Label != null && !m_actionDef.Label.IsExpression))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				m_label = value;
			}
		}

		public string BookmarkLink
		{
			get
			{
				if (m_bookmark == null)
				{
					if (m_isOldSnapshot)
					{
						if (m_renderAction != null)
						{
							m_bookmark = m_renderAction.BookmarkLink;
						}
					}
					else if (m_actionDef.BookmarkLink != null)
					{
						if (!m_actionDef.BookmarkLink.IsExpression)
						{
							m_bookmark = m_actionDef.BookmarkLink.Value;
						}
						else if (m_actionDef.Owner.ReportElementOwner == null || m_actionDef.Owner.ReportElementOwner.CriOwner == null)
						{
							ActionInfo owner = m_actionDef.Owner;
							m_bookmark = m_actionDef.ActionItemDef.EvaluateBookmarkLink(ReportScopeInstance, owner.RenderingContext.OdpContext, owner.InstancePath, owner.ObjectType, owner.ObjectName);
						}
					}
				}
				return m_bookmark;
			}
			set
			{
				ReportElement reportElementOwner = m_actionDef.Owner.ReportElementOwner;
				if (!m_actionDef.Owner.IsChartConstruction && (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Definition && m_actionDef.BookmarkLink == null) || (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && (m_actionDef.BookmarkLink == null || !m_actionDef.BookmarkLink.IsExpression))))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				m_bookmark = value;
			}
		}

		public ReportUrl Hyperlink
		{
			get
			{
				if (m_hyperlink == null)
				{
					if (m_isOldSnapshot)
					{
						if (m_renderAction != null && m_renderAction.HyperLinkURL != null)
						{
							m_hyperlink = new ReportUrl(m_renderAction.HyperLinkURL);
						}
					}
					else if (m_actionDef.Hyperlink != null)
					{
						if (!m_actionDef.Hyperlink.IsExpression)
						{
							m_hyperlink = m_actionDef.Hyperlink.Value;
						}
						else if (m_actionDef.Owner.ReportElementOwner == null || m_actionDef.Owner.ReportElementOwner.CriOwner == null)
						{
							ActionInfo owner = m_actionDef.Owner;
							string hyperlinkText = m_actionDef.ActionItemDef.EvaluateHyperLinkURL(ReportScopeInstance, owner.RenderingContext.OdpContext, owner.InstancePath, owner.ObjectType, owner.ObjectName);
							((IActionInstance)this).SetHyperlinkText(hyperlinkText);
						}
					}
				}
				return m_hyperlink;
			}
		}

		public string HyperlinkText
		{
			get
			{
				return m_hyperlinkText;
			}
			set
			{
				ReportElement reportElementOwner = m_actionDef.Owner.ReportElementOwner;
				if (!m_actionDef.Owner.IsChartConstruction && (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Definition && m_actionDef.Hyperlink == null) || (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && (m_actionDef.Hyperlink == null || !m_actionDef.Hyperlink.IsExpression))))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				((IActionInstance)this).SetHyperlinkText(value);
			}
		}

		internal ActionInstance(IReportScope reportScope, Action actionDef)
			: base(reportScope)
		{
			m_isOldSnapshot = false;
			m_actionDef = actionDef;
		}

		internal ActionInstance(Microsoft.ReportingServices.ReportRendering.Action renderAction)
			: base(null)
		{
			m_isOldSnapshot = true;
			m_renderAction = renderAction;
		}

		void IActionInstance.SetHyperlinkText(string hyperlinkText)
		{
			m_hyperlinkText = hyperlinkText;
			if (m_hyperlinkText != null)
			{
				ActionInfo owner = m_actionDef.Owner;
				m_hyperlink = ReportUrl.BuildHyperlinkUrl(owner.RenderingContext, owner.ObjectType, owner.ObjectName, "Hyperlink", owner.RenderingContext.OdpContext.ReportContext, m_hyperlinkText);
				if (m_hyperlink == null)
				{
					m_hyperlinkText = null;
				}
			}
			else
			{
				m_hyperlink = null;
			}
		}

		internal void Update(Microsoft.ReportingServices.ReportRendering.Action newAction)
		{
			m_renderAction = newAction;
			m_label = null;
			m_bookmark = null;
			m_hyperlink = null;
		}

		protected override void ResetInstanceCache()
		{
			m_label = null;
			m_bookmark = null;
			m_hyperlink = null;
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Label:
				{
					string value2 = null;
					if (m_actionDef.Label.IsExpression)
					{
						value2 = m_label;
					}
					writer.Write(value2);
					break;
				}
				case MemberName.BookmarkLink:
				{
					string value = null;
					if (m_actionDef.BookmarkLink != null && m_actionDef.BookmarkLink.IsExpression)
					{
						value = m_bookmark;
					}
					writer.Write(value);
					break;
				}
				case MemberName.HyperLinkURL:
				{
					string value3 = null;
					if (m_actionDef.Hyperlink != null && m_actionDef.Hyperlink.IsExpression)
					{
						value3 = m_hyperlinkText;
					}
					writer.Write(value3);
					break;
				}
				case MemberName.DrillthroughReportName:
				{
					string value4 = null;
					if (m_actionDef.Drillthrough != null && m_actionDef.Drillthrough.ReportName.IsExpression)
					{
						value4 = m_actionDef.Drillthrough.Instance.ReportName;
					}
					writer.Write(value4);
					break;
				}
				case MemberName.DrillthroughParameters:
				{
					ParameterInstance[] array = null;
					if (m_actionDef.Drillthrough != null && m_actionDef.Drillthrough.Parameters != null)
					{
						array = new ParameterInstance[m_actionDef.Drillthrough.Parameters.Count];
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = m_actionDef.Drillthrough.Parameters[i].Instance;
						}
					}
					writer.Write(array);
					break;
				}
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Label:
				{
					string text2 = reader.ReadString();
					if (m_actionDef.Label.IsExpression)
					{
						m_label = text2;
					}
					else
					{
						Global.Tracer.Assert(text2 == null, "(label == null)");
					}
					break;
				}
				case MemberName.BookmarkLink:
				{
					string text4 = reader.ReadString();
					if (m_actionDef.BookmarkLink != null && m_actionDef.BookmarkLink.IsExpression)
					{
						m_bookmark = text4;
					}
					else
					{
						Global.Tracer.Assert(text4 == null, "(bookmarkLink == null)");
					}
					break;
				}
				case MemberName.HyperLinkURL:
				{
					string text = reader.ReadString();
					if (m_actionDef.Hyperlink != null && m_actionDef.Hyperlink.IsExpression)
					{
						m_hyperlinkText = text;
					}
					else
					{
						Global.Tracer.Assert(text == null, "(hyperlink == null)");
					}
					break;
				}
				case MemberName.DrillthroughReportName:
				{
					string text3 = reader.ReadString();
					if (m_actionDef.Drillthrough != null && m_actionDef.Drillthrough.ReportName.IsExpression)
					{
						m_actionDef.Drillthrough.Instance.ReportName = text3;
					}
					else
					{
						Global.Tracer.Assert(text3 == null, "(reportName == null)");
					}
					break;
				}
				case MemberName.DrillthroughParameters:
				{
					ParameterCollection paramCollection = null;
					if (m_actionDef.Drillthrough != null)
					{
						paramCollection = m_actionDef.Drillthrough.Parameters;
					}
					((ROMInstanceObjectCreator)reader.PersistenceHelper).StartParameterInstancesDeserialization(paramCollection);
					reader.ReadArrayOfRIFObjects<ParameterInstance>();
					((ROMInstanceObjectCreator)reader.PersistenceHelper).CompleteParameterInstancesDeserialization();
					break;
				}
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInstance;
		}

		[SkipMemberStaticValidation(MemberName.DrillthroughReportName)]
		[SkipMemberStaticValidation(MemberName.DrillthroughParameters)]
		private static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Label, Token.String));
			list.Add(new MemberInfo(MemberName.BookmarkLink, Token.String));
			list.Add(new MemberInfo(MemberName.HyperLinkURL, Token.String));
			list.Add(new MemberInfo(MemberName.DrillthroughReportName, Token.String));
			list.Add(new MemberInfo(MemberName.DrillthroughParameters, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterInstance));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}
	}
}
