using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalParagraphInstance : ParagraphInstance
	{
		private ReportSize m_leftIndent;

		private ReportSize m_rightIndent;

		private ReportSize m_hangingIndent;

		private ReportSize m_spaceBefore;

		private ReportSize m_spaceAfter;

		private ListStyle? m_listStyle;

		private int? m_listLevel;

		public override string UniqueName
		{
			get
			{
				if (m_uniqueName == null)
				{
					m_uniqueName = InstancePathItem.GenerateUniqueNameString(ParagraphDef.IDString, ParagraphDef.InstancePath);
				}
				return m_uniqueName;
			}
		}

		public override ReportSize LeftIndent
		{
			get
			{
				if (m_leftIndent == null)
				{
					m_leftIndent = GetLeftIndent(constantUsable: true);
				}
				return m_leftIndent;
			}
		}

		public override ReportSize RightIndent
		{
			get
			{
				if (m_rightIndent == null)
				{
					m_rightIndent = GetRightIndent(constantUsable: true);
				}
				return m_rightIndent;
			}
		}

		public override ReportSize HangingIndent
		{
			get
			{
				if (m_hangingIndent == null)
				{
					m_hangingIndent = GetHangingIndent(constantUsable: true);
				}
				return m_hangingIndent;
			}
		}

		public override ListStyle ListStyle
		{
			get
			{
				if (!m_listStyle.HasValue)
				{
					ExpressionInfo listStyle = ParagraphDef.ListStyle;
					if (listStyle != null)
					{
						if (listStyle.IsExpression)
						{
							m_listStyle = RichTextHelpers.TranslateListStyle(ParagraphDef.EvaluateListStyle(ReportScopeInstance, m_reportElementDef.RenderingContext.OdpContext));
						}
						else
						{
							m_listStyle = RichTextHelpers.TranslateListStyle(listStyle.StringValue);
						}
					}
					else
					{
						m_listStyle = ListStyle.None;
					}
				}
				return m_listStyle.Value;
			}
		}

		public override int ListLevel
		{
			get
			{
				if (!m_listLevel.HasValue)
				{
					ExpressionInfo listLevel = ParagraphDef.ListLevel;
					if (listLevel != null)
					{
						if (listLevel.IsExpression)
						{
							m_listLevel = ParagraphDef.EvaluateListLevel(ReportScopeInstance, m_reportElementDef.RenderingContext.OdpContext);
						}
						else
						{
							m_listLevel = listLevel.IntValue;
						}
					}
					if (!m_listLevel.HasValue)
					{
						m_listLevel = ((ListStyle != 0) ? 1 : 0);
					}
				}
				return m_listLevel.Value;
			}
		}

		public override ReportSize SpaceBefore
		{
			get
			{
				if (m_spaceBefore == null)
				{
					m_spaceBefore = GetSpaceBefore(constantUsable: true);
				}
				return m_spaceBefore;
			}
		}

		public override ReportSize SpaceAfter
		{
			get
			{
				if (m_spaceAfter == null)
				{
					m_spaceAfter = GetSpaceAfter(constantUsable: true);
				}
				return m_spaceAfter;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Paragraph ParagraphDef => ((InternalParagraph)m_reportElementDef).ParagraphDef;

		public override bool IsCompiled => false;

		internal InternalParagraphInstance(Paragraph paragraphDef)
			: base(paragraphDef)
		{
		}

		internal InternalParagraphInstance(ReportElement reportElementDef)
			: base(reportElementDef)
		{
		}

		internal ReportSize GetLeftIndent(bool constantUsable)
		{
			ExpressionInfo leftIndent = ParagraphDef.LeftIndent;
			if (leftIndent != null)
			{
				if (leftIndent.IsExpression)
				{
					return new ReportSize(ParagraphDef.EvaluateLeftIndent(ReportScopeInstance, m_reportElementDef.RenderingContext.OdpContext), validate: false, allowNegative: false);
				}
				if (constantUsable)
				{
					return new ReportSize(leftIndent.StringValue, validate: false, allowNegative: false);
				}
			}
			return null;
		}

		internal ReportSize GetRightIndent(bool constantUsable)
		{
			ExpressionInfo rightIndent = ParagraphDef.RightIndent;
			if (rightIndent != null)
			{
				if (rightIndent.IsExpression)
				{
					return new ReportSize(ParagraphDef.EvaluateRightIndent(ReportScopeInstance, m_reportElementDef.RenderingContext.OdpContext), validate: false, allowNegative: false);
				}
				if (constantUsable)
				{
					return new ReportSize(rightIndent.StringValue, validate: false, allowNegative: false);
				}
			}
			return null;
		}

		internal ReportSize GetHangingIndent(bool constantUsable)
		{
			ExpressionInfo hangingIndent = ParagraphDef.HangingIndent;
			if (hangingIndent != null)
			{
				if (hangingIndent.IsExpression)
				{
					return new ReportSize(ParagraphDef.EvaluateHangingIndent(ReportScopeInstance, m_reportElementDef.RenderingContext.OdpContext), validate: false, allowNegative: true);
				}
				if (constantUsable)
				{
					return new ReportSize(hangingIndent.StringValue, validate: false, allowNegative: true);
				}
			}
			return null;
		}

		internal ReportSize GetSpaceBefore(bool constantUsable)
		{
			ExpressionInfo spaceBefore = ParagraphDef.SpaceBefore;
			if (spaceBefore != null)
			{
				if (spaceBefore.IsExpression)
				{
					return new ReportSize(ParagraphDef.EvaluateSpaceBefore(ReportScopeInstance, m_reportElementDef.RenderingContext.OdpContext), validate: false, allowNegative: false);
				}
				if (constantUsable)
				{
					return new ReportSize(spaceBefore.StringValue, validate: false, allowNegative: false);
				}
			}
			return null;
		}

		internal ReportSize GetSpaceAfter(bool constantUsable)
		{
			ExpressionInfo spaceAfter = ParagraphDef.SpaceAfter;
			if (spaceAfter != null)
			{
				if (spaceAfter.IsExpression)
				{
					return new ReportSize(ParagraphDef.EvaluateSpaceAfter(ReportScopeInstance, m_reportElementDef.RenderingContext.OdpContext), validate: false, allowNegative: false);
				}
				if (constantUsable)
				{
					return new ReportSize(spaceAfter.StringValue, validate: false, allowNegative: false);
				}
			}
			return null;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_leftIndent = null;
			m_rightIndent = null;
			m_hangingIndent = null;
			m_spaceBefore = null;
			m_spaceAfter = null;
			m_listStyle = null;
			m_listLevel = null;
		}
	}
}
