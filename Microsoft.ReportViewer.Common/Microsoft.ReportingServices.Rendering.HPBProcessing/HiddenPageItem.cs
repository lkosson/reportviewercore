using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal class HiddenPageItem : PageItem, IStorable, IPersistable
	{
		private Dictionary<string, List<object>> m_textBoxValues;

		private static Declaration m_declaration = GetDeclaration();

		internal override bool ContentOnPage => false;

		internal override double OriginalLeft => base.ItemPageSizes.Left;

		internal override double OriginalWidth => base.ItemPageSizes.Width;

		public override int Size => base.Size + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_textBoxValues);

		internal HiddenPageItem()
		{
		}

		internal HiddenPageItem(double top, double left)
			: base(null)
		{
			m_itemPageSizes = new ItemSizes(left, top, 0.0, 0.0);
			base.KeepTogetherHorizontal = false;
			base.KeepTogetherVertical = false;
			base.UnresolvedKTV = (base.UnresolvedKTH = false);
		}

		internal HiddenPageItem(ReportItem source, PageContext pageContext, bool checkHiddenState)
			: base(source)
		{
			m_itemPageSizes = new ItemSizes(source);
			base.KeepTogetherHorizontal = false;
			base.KeepTogetherVertical = false;
			base.UnresolvedKTV = (base.UnresolvedKTH = false);
			if (pageContext.EvaluatePageHeaderFooter && (m_source.Visibility == null || m_source.Visibility.ToggleItem != null || !m_source.Visibility.Hidden.IsExpression || (checkHiddenState && !m_source.Instance.Visibility.CurrentlyHidden)))
			{
				m_textBoxValues = new Dictionary<string, List<object>>();
				HeaderFooterEval.CollectTextBoxes(m_source, pageContext, useForPageHFEval: true, m_textBoxValues);
			}
		}

		internal void AddToCollection(HiddenPageItem hiddenItem)
		{
			if (hiddenItem == null || hiddenItem.m_textBoxValues == null)
			{
				return;
			}
			if (m_textBoxValues == null)
			{
				m_textBoxValues = hiddenItem.m_textBoxValues;
				return;
			}
			foreach (string key in hiddenItem.m_textBoxValues.Keys)
			{
				List<object> list = hiddenItem.m_textBoxValues[key];
				if (m_textBoxValues.ContainsKey(key))
				{
					m_textBoxValues[key].AddRange(list);
				}
				else
				{
					m_textBoxValues[key] = list;
				}
			}
		}

		protected override void DetermineVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			if (m_source != null && m_source.Visibility != null && m_source.Visibility.HiddenState == SharedHiddenState.Sometimes)
			{
				m_itemPageSizes.AdjustHeightTo(0.0);
			}
		}

		protected override void DetermineHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			if (m_source != null && m_source.Visibility != null && m_source.Visibility.HiddenState == SharedHiddenState.Sometimes)
			{
				m_itemPageSizes.AdjustWidthTo(0.0);
			}
		}

		internal override void RegisterTextBoxes(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter == null || m_textBoxValues == null || pageContext.Common.InSubReport)
			{
				return;
			}
			if (rplWriter.DelayedTBLevels == 0)
			{
				foreach (string key in m_textBoxValues.Keys)
				{
					foreach (object item in m_textBoxValues[key])
					{
						pageContext.AddTextBox(key, item);
					}
				}
			}
			else
			{
				rplWriter.AddTextBoxes(m_textBoxValues);
			}
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.TextBoxValues)
				{
					writer.WriteStringVariantListDictionary(m_textBoxValues);
				}
				else
				{
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.TextBoxValues)
				{
					m_textBoxValues = reader.ReadStringVariantListDictionary();
				}
				else
				{
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.HiddenPageItem;
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.TextBoxValues, ObjectType.StringVariantListDictionary));
				return new Declaration(ObjectType.HiddenPageItem, ObjectType.PageItem, list);
			}
			return m_declaration;
		}
	}
}
