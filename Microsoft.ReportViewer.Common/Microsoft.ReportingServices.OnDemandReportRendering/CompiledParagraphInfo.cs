namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal class CompiledParagraphInfo
	{
		internal class FlattenedPropertyStore
		{
			private ListStyle m_listStyle;

			private int m_listLevel;

			private ReportSize m_spaceBefore;

			private ReportSize m_marginTop;

			private ReportSize m_pendingMarginBottom;

			private ICompiledParagraphInstance m_lastPopulatedParagraph;

			internal int ListLevel
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

			internal ListStyle ListStyle
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

			internal ReportSize SpaceBefore
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

			internal ReportSize MarginTop => m_marginTop;

			internal ReportSize PendingMarginBottom => m_pendingMarginBottom;

			internal ICompiledParagraphInstance LastPopulatedParagraph
			{
				get
				{
					return m_lastPopulatedParagraph;
				}
				set
				{
					m_lastPopulatedParagraph = value;
				}
			}

			internal void ClearMarginTop()
			{
				m_marginTop = null;
			}

			internal void UpdateMarginTop(ReportSize marginTop)
			{
				m_marginTop = GetLargest(m_marginTop, marginTop);
			}

			internal void AddMarginTop(ReportSize margin)
			{
				m_marginTop = ReportSize.SumSizes(m_marginTop, margin);
			}

			internal void ClearPendingMarginBottom()
			{
				m_pendingMarginBottom = null;
			}

			internal void UpdatePendingMarginBottom(ReportSize marginBottom)
			{
				m_pendingMarginBottom = GetLargest(m_pendingMarginBottom, marginBottom);
			}

			private ReportSize GetLargest(ReportSize size1, ReportSize size2)
			{
				if (size1 == null)
				{
					return size2;
				}
				if (size2 == null)
				{
					return size1;
				}
				if (size1.ToMillimeters() > size2.ToMillimeters())
				{
					return size1;
				}
				return size2;
			}
		}

		private HtmlElement.HtmlElementType m_elementType;

		private CompiledParagraphInfo m_parentParagraph;

		private CompiledParagraphInfo m_childParagraph;

		private ReportSize m_leftIndent;

		private ReportSize m_rightIndent;

		private ReportSize m_hangingIndent;

		private ReportSize m_spaceAfter;

		private ReportSize m_marginBottom;

		private bool m_hasSpaceBefore;

		private bool m_marginBottomSet;

		private bool m_leftIndentSet;

		private bool m_rightIndentSet;

		private bool m_hangingIndentSet;

		private bool m_spaceAfterSet;

		private FlattenedPropertyStore m_flatStore;

		private ICompiledParagraphInstance m_lastParagraph;

		internal HtmlElement.HtmlElementType ElementType
		{
			get
			{
				return m_elementType;
			}
			set
			{
				m_elementType = value;
			}
		}

		internal int ListLevel
		{
			get
			{
				return m_flatStore.ListLevel;
			}
			set
			{
				m_flatStore.ListLevel = value;
			}
		}

		internal ListStyle ListStyle
		{
			get
			{
				return m_flatStore.ListStyle;
			}
			set
			{
				m_flatStore.ListStyle = value;
			}
		}

		internal ReportSize LeftIndent
		{
			get
			{
				if (m_leftIndentSet)
				{
					return m_leftIndent;
				}
				if (m_parentParagraph != null)
				{
					return m_parentParagraph.LeftIndent;
				}
				return null;
			}
		}

		internal ReportSize RightIndent
		{
			get
			{
				if (m_rightIndentSet)
				{
					return m_rightIndent;
				}
				if (m_parentParagraph != null)
				{
					return m_parentParagraph.RightIndent;
				}
				return null;
			}
		}

		internal ReportSize HangingIndent
		{
			get
			{
				if (m_hangingIndentSet)
				{
					return m_hangingIndent;
				}
				if (m_parentParagraph != null)
				{
					return m_parentParagraph.HangingIndent;
				}
				return null;
			}
			set
			{
				m_hangingIndent = value;
				m_hangingIndentSet = true;
			}
		}

		internal ReportSize MarginTop => m_flatStore.MarginTop;

		internal ReportSize MarginBottom
		{
			get
			{
				if (m_marginBottomSet)
				{
					return m_marginBottom;
				}
				return null;
			}
		}

		internal ReportSize SpaceBefore => m_flatStore.SpaceBefore;

		internal ReportSize SpaceAfter
		{
			get
			{
				if (m_spaceAfterSet)
				{
					return m_spaceAfter;
				}
				return null;
			}
		}

		internal CompiledParagraphInfo()
		{
			m_flatStore = new FlattenedPropertyStore();
		}

		internal void AddLeftIndent(ReportSize size)
		{
			m_leftIndent = ReportSize.SumSizes(LeftIndent, size);
			m_leftIndentSet = true;
		}

		internal void AddRightIndent(ReportSize size)
		{
			m_rightIndent = ReportSize.SumSizes(RightIndent, size);
			m_rightIndentSet = true;
		}

		internal void UpdateMarginTop(ReportSize value)
		{
			m_flatStore.UpdateMarginTop(value);
		}

		internal void AddMarginBottom(ReportSize size)
		{
			m_marginBottom = size;
			m_marginBottomSet = true;
		}

		internal void AddSpaceBefore(ReportSize size)
		{
			ReportSize spaceBefore = m_flatStore.SpaceBefore;
			ReportSize reportSize = ReportSize.SumSizes(spaceBefore, size);
			m_hasSpaceBefore = (reportSize != null && reportSize.ToMillimeters() > 0.0 && (spaceBefore == null || reportSize.ToMillimeters() != spaceBefore.ToMillimeters()));
			m_flatStore.SpaceBefore = reportSize;
		}

		internal void AddSpaceAfter(ReportSize size)
		{
			if (m_spaceAfterSet)
			{
				m_spaceAfter = ReportSize.SumSizes(m_spaceAfter, size);
				return;
			}
			m_spaceAfter = size;
			m_spaceAfterSet = true;
		}

		internal CompiledParagraphInfo CreateChildParagraph(HtmlElement.HtmlElementType elementType)
		{
			CompiledParagraphInfo compiledParagraphInfo = new CompiledParagraphInfo();
			compiledParagraphInfo.ElementType = elementType;
			compiledParagraphInfo.m_parentParagraph = this;
			compiledParagraphInfo.m_flatStore = m_flatStore;
			m_childParagraph = compiledParagraphInfo;
			return compiledParagraphInfo;
		}

		internal CompiledParagraphInfo RemoveAll()
		{
			CompiledParagraphInfo compiledParagraphInfo = this;
			while (compiledParagraphInfo.m_parentParagraph != null)
			{
				compiledParagraphInfo = compiledParagraphInfo.RemoveParagraph(compiledParagraphInfo.ElementType);
			}
			ApplyPendingMargins();
			compiledParagraphInfo.ResetParagraph();
			return compiledParagraphInfo;
		}

		internal CompiledParagraphInfo RemoveParagraph(HtmlElement.HtmlElementType elementType)
		{
			if (m_elementType == elementType)
			{
				ApplySpaceAfter();
				if (m_parentParagraph != null)
				{
					m_parentParagraph.m_childParagraph = null;
					return m_parentParagraph;
				}
				ResetParagraph();
			}
			else if (m_parentParagraph != null)
			{
				m_parentParagraph.InternalRemoveParagraph(elementType);
			}
			return this;
		}

		internal void InternalRemoveParagraph(HtmlElement.HtmlElementType elementType)
		{
			if (m_elementType == elementType)
			{
				ApplySpaceAfter();
				if (m_parentParagraph != null)
				{
					m_parentParagraph.m_childParagraph = m_childParagraph;
					m_childParagraph.m_parentParagraph = m_parentParagraph;
				}
				else if (m_parentParagraph == null)
				{
					m_childParagraph.m_parentParagraph = null;
				}
			}
			else if (m_parentParagraph != null)
			{
				m_parentParagraph.InternalRemoveParagraph(elementType);
			}
		}

		private void ApplySpaceAfter()
		{
			ReportSize spaceAfter = SpaceAfter;
			if (m_lastParagraph == null)
			{
				if (IsNonEmptySize(spaceAfter) || m_hasSpaceBefore)
				{
					ApplyPendingMargins();
					AddSpaceBefore(MarginTop);
					m_flatStore.ClearMarginTop();
					AddSpaceBefore(spaceAfter);
					m_flatStore.UpdatePendingMarginBottom(MarginBottom);
				}
				else
				{
					m_flatStore.UpdateMarginTop(MarginBottom);
				}
			}
			else
			{
				m_flatStore.UpdatePendingMarginBottom(MarginBottom);
				AddToParagraphSpaceAfter(m_lastParagraph, spaceAfter);
			}
		}

		private void ApplyPendingMargins()
		{
			ICompiledParagraphInstance lastPopulatedParagraph = m_flatStore.LastPopulatedParagraph;
			ReportSize pendingMarginBottom = m_flatStore.PendingMarginBottom;
			if (IsNonEmptySize(pendingMarginBottom))
			{
				if (lastPopulatedParagraph != null)
				{
					ReportSize marginTop = m_flatStore.MarginTop;
					if (marginTop == null)
					{
						AddToParagraphSpaceAfter(lastPopulatedParagraph, pendingMarginBottom);
					}
					else if (pendingMarginBottom.ToMillimeters() >= marginTop.ToMillimeters())
					{
						AddToParagraphSpaceAfter(lastPopulatedParagraph, pendingMarginBottom);
						m_flatStore.ClearMarginTop();
					}
				}
				else
				{
					m_flatStore.UpdateMarginTop(pendingMarginBottom);
				}
				m_flatStore.ClearPendingMarginBottom();
			}
			m_flatStore.LastPopulatedParagraph = null;
		}

		private void AddToParagraphSpaceAfter(ICompiledParagraphInstance paragraphInstance, ReportSize additionalSpace)
		{
			paragraphInstance.SpaceAfter = ReportSize.SumSizes(paragraphInstance.SpaceAfter, additionalSpace);
		}

		private bool IsNonEmptySize(ReportSize size)
		{
			if (size != null)
			{
				return size.ToMillimeters() > 0.0;
			}
			return false;
		}

		private void ResetParagraph()
		{
			m_leftIndentSet = false;
			m_rightIndentSet = false;
			m_hangingIndentSet = false;
			m_spaceAfterSet = false;
			m_marginBottomSet = false;
			m_hasSpaceBefore = false;
			m_lastParagraph = null;
		}

		internal void PopulateParagraph(ICompiledParagraphInstance paragraphInstance)
		{
			ApplyPendingMargins();
			m_flatStore.LastPopulatedParagraph = paragraphInstance;
			paragraphInstance.ListStyle = m_flatStore.ListStyle;
			m_flatStore.ListStyle = ListStyle.None;
			paragraphInstance.ListLevel = m_flatStore.ListLevel;
			ReportSize leftIndent = LeftIndent;
			if (leftIndent != null)
			{
				paragraphInstance.LeftIndent = leftIndent;
			}
			ReportSize rightIndent = RightIndent;
			if (rightIndent != null)
			{
				paragraphInstance.RightIndent = rightIndent;
			}
			ReportSize hangingIndent = HangingIndent;
			if (hangingIndent != null)
			{
				paragraphInstance.HangingIndent = hangingIndent;
			}
			ReportSize marginTop = MarginTop;
			ReportSize spaceBefore = SpaceBefore;
			if (spaceBefore != null || marginTop != null)
			{
				paragraphInstance.SpaceBefore = ReportSize.SumSizes(marginTop, spaceBefore);
				m_flatStore.SpaceBefore = null;
				m_flatStore.ClearMarginTop();
			}
			StoreLastParagraph(paragraphInstance);
		}

		private void StoreLastParagraph(ICompiledParagraphInstance paragraphInstance)
		{
			m_lastParagraph = paragraphInstance;
			if (m_parentParagraph != null)
			{
				m_parentParagraph.StoreLastParagraph(paragraphInstance);
			}
		}
	}
}
