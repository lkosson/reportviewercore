using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Drawing;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal abstract class DynamicImage : PageItem
	{
		protected abstract PaginationInfoItems PaginationInfoEnum
		{
			get;
		}

		protected abstract bool SpecialBorderHandling
		{
			get;
		}

		internal DynamicImage(ReportItem source, PageContext pageContext, bool createForRepeat)
			: base(source)
		{
			if (pageContext != null)
			{
				if (createForRepeat)
				{
					m_itemPageSizes = pageContext.GetSharedFromRepeatItemSizesElement(source, isPadded: false);
				}
				else
				{
					m_itemPageSizes = pageContext.GetSharedItemSizesElement(source, isPadded: false);
				}
			}
			else
			{
				m_itemPageSizes = new ItemSizes(source);
			}
		}

		internal override bool CalculatePage(RPLWriter rplWriter, PageItemHelper lastPageInfo, PageContext pageContext, PageItem[] siblings, RepeatWithItem[] repeatWithItems, double parentTopInPage, ref double parentPageHeight, Interactivity interactivity)
		{
			AdjustOriginFromItemsAbove(siblings, repeatWithItems);
			if (!HitsCurrentPage(pageContext, parentTopInPage))
			{
				return false;
			}
			ItemSizes contentSize = null;
			bool flag = ResolveItemHiddenState(rplWriter, interactivity, pageContext, createForRepeat: false, ref contentSize);
			parentPageHeight = Math.Max(parentPageHeight, m_itemPageSizes.Bottom);
			if (!pageContext.IgnorePageBreaks)
			{
				if (base.PageBreakAtEnd)
				{
					m_itemState = State.OnPagePBEnd;
					pageContext.RegisterPageBreak(new PageBreakInfo(PageBreak, base.ItemName));
				}
				if (!flag)
				{
					pageContext.RegisterPageName(PageName);
				}
			}
			if (pageContext.TracingEnabled && pageContext.IgnorePageBreaks)
			{
				TracePageBreakAtEndIgnored(pageContext);
			}
			if (rplWriter != null)
			{
				if (m_itemRenderSizes == null)
				{
					CreateItemRenderSizes(contentSize, pageContext, createForRepeat: false);
				}
				if (!flag)
				{
					WriteItemToStream(rplWriter, pageContext);
				}
			}
			return true;
		}

		internal Stream LoadDynamicImage(PageContext pageContext, ref string streamName, out ActionInfoWithDynamicImageMapCollection actionImageMaps, out System.Drawing.Rectangle offsets)
		{
			IDynamicImageInstance dynamicImageInstance = (IDynamicImageInstance)m_source.Instance;
			Stream stream = null;
			if (pageContext.EmfDynamicImage)
			{
				dynamicImageInstance.SetDpi(96, 96);
				stream = dynamicImageInstance.GetImage(DynamicImageInstance.ImageType.EMF, out actionImageMaps);
				Register(ref stream, ref streamName, "emf", "image/emf", pageContext, out offsets);
			}
			else
			{
				stream = dynamicImageInstance.GetImage(DynamicImageInstance.ImageType.PNG, out actionImageMaps);
				Register(ref stream, ref streamName, "png", PageContext.PNG_MIME_TYPE, pageContext, out offsets);
			}
			return stream;
		}

		private void Register(ref Stream dynamicImageStream, ref string streamName, string extension, string mimeType, PageContext pageContext, out System.Drawing.Rectangle offsets)
		{
			offsets = System.Drawing.Rectangle.Empty;
			if (dynamicImageStream == null || dynamicImageStream.Length == 0L)
			{
				return;
			}
			if (pageContext.AddSecondaryStreamNames)
			{
				streamName = GenerateStreamName(pageContext);
			}
			if (pageContext.SecondaryStreams != 0)
			{
				string streamName2 = streamName;
				if (streamName2 == null)
				{
					streamName2 = GenerateStreamName(pageContext);
				}
				RegisterDynamicImage(ref dynamicImageStream, ref streamName2, extension, mimeType, pageContext, out offsets);
				streamName = streamName2;
			}
		}

		internal void WriteItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				Stream baseStream = binaryWriter.BaseStream;
				long position = baseStream.Position;
				binaryWriter.Write(GetElementToken(pageContext));
				WriteElementProps(binaryWriter, rplWriter, pageContext, position + 1);
				m_offset = baseStream.Position;
				binaryWriter.Write((byte)254);
				binaryWriter.Write(position);
				binaryWriter.Write(byte.MaxValue);
			}
			else
			{
				m_rplElement = CreateRPLItem();
				WriteElementProps(m_rplElement.ElementProps, rplWriter, pageContext);
			}
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)PaginationInfoEnum);
				base.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write(byte.MaxValue);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemHelper((byte)PaginationInfoEnum);
			base.WritePaginationInfoProperties(pageItemHelper);
			return pageItemHelper;
		}

		protected void RegisterDynamicImage(ref Stream dynamicImageStream, ref string streamName, string extension, string mimeType, PageContext pageContext, out System.Drawing.Rectangle offsets)
		{
			offsets = System.Drawing.Rectangle.Empty;
			if (dynamicImageStream == null || dynamicImageStream.Length == 0L)
			{
				return;
			}
			if (pageContext.ImageConsolidation != null)
			{
				ImageConsolidation imageConsolidation = pageContext.ImageConsolidation;
				offsets = imageConsolidation.AppendImage(dynamicImageStream);
				if (offsets != System.Drawing.Rectangle.Empty)
				{
					dynamicImageStream = null;
					streamName = imageConsolidation.GetStreamName();
				}
			}
			else
			{
				if (pageContext.RegisteredStreamNames.Contains(streamName))
				{
					return;
				}
				pageContext.RegisteredStreamNames.Add(streamName, null);
				Stream stream = pageContext.CreateAndRegisterStream(streamName, extension, null, mimeType, willSeek: false, StreamOper.CreateAndRegister);
				if (stream != null)
				{
					dynamicImageStream.Position = 0L;
					int num = (int)dynamicImageStream.Length;
					byte[] array = new byte[4096];
					while (num > 0)
					{
						int num2 = dynamicImageStream.Read(array, 0, Math.Min(array.Length, num));
						stream.Write(array, 0, num2);
						num -= num2;
					}
					dynamicImageStream = null;
				}
			}
		}

		internal override void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			ActionInfoWithDynamicImageMapCollection actionImageMaps = null;
			string streamName = null;
			System.Drawing.Rectangle offsets = System.Drawing.Rectangle.Empty;
			Stream stream = LoadDynamicImage(pageContext, ref streamName, out actionImageMaps, out offsets);
			if (streamName != null)
			{
				spbifWriter.Write((byte)40);
				spbifWriter.Write(streamName);
			}
			if (!offsets.IsEmpty)
			{
				if (pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
				{
					spbifWriter.Write((byte)47);
				}
				else
				{
					spbifWriter.Write((byte)49);
				}
				spbifWriter.Write(offsets.Left);
				spbifWriter.Write(offsets.Top);
				spbifWriter.Write(offsets.Width);
				spbifWriter.Write(offsets.Height);
			}
			if (stream != null)
			{
				spbifWriter.Write((byte)39);
				spbifWriter.Write((int)stream.Length);
				byte[] array = new byte[4096];
				stream.Position = 0L;
				for (int num = stream.Read(array, 0, array.Length); num != 0; num = stream.Read(array, 0, array.Length))
				{
					spbifWriter.Write(array, 0, num);
				}
			}
			if (actionImageMaps != null)
			{
				WriteImageMapAreaInstanceCollection(actionImageMaps, spbifWriter, pageContext);
			}
		}

		internal override void WriteCustomNonSharedItemProps(RPLElementProps nonSharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
			ActionInfoWithDynamicImageMapCollection actionImageMaps = null;
			RPLDynamicImageProps rPLDynamicImageProps = (RPLDynamicImageProps)nonSharedProps;
			string streamName = null;
			System.Drawing.Rectangle offsets = System.Drawing.Rectangle.Empty;
			rPLDynamicImageProps.DynamicImageContent = LoadDynamicImage(pageContext, ref streamName, out actionImageMaps, out offsets);
			rPLDynamicImageProps.ImageConsolidationOffsets = offsets;
			rPLDynamicImageProps.StreamName = streamName;
			if (actionImageMaps != null)
			{
				rPLDynamicImageProps.ActionImageMapAreas = WriteImageMapAreaInstanceCollection(actionImageMaps, pageContext);
			}
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps styleProps, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, styleProps, StyleAttributeNames.BackgroundColor, 34);
		}

		internal override void WriteNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			if (!SpecialBorderHandling)
			{
				base.WriteNonSharedStyleProp(spbifWriter, styleDef, style, styleAttribute, pageContext);
			}
			else
			{
				WriteItemNonSharedStyleProp(spbifWriter, styleDef, style, styleAttribute, pageContext);
			}
		}

		internal override void WriteItemNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			if (styleAttribute == StyleAttributeNames.BackgroundColor)
			{
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			}
		}

		internal override void WriteNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			if (!SpecialBorderHandling)
			{
				base.WriteNonSharedStyleProp(rplStyleProps, styleDef, style, styleAttribute, pageContext);
			}
			else
			{
				WriteItemNonSharedStyleProp(rplStyleProps, styleDef, style, styleAttribute, pageContext);
			}
		}

		internal override void WriteItemNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			if (styleAttribute == StyleAttributeNames.BackgroundColor)
			{
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			}
		}

		internal override void WriteBorderProps(BinaryWriter spbifWriter, Style style)
		{
			if (!SpecialBorderHandling)
			{
				base.WriteBorderProps(spbifWriter, style);
			}
		}

		internal override void WriteBorderProps(RPLStyleProps rplStyleProps, Style style)
		{
			if (!SpecialBorderHandling)
			{
				base.WriteBorderProps(rplStyleProps, style);
			}
		}

		protected abstract string GenerateStreamName(PageContext pageContext);

		protected abstract RPLItem CreateRPLItem();

		protected abstract byte GetElementToken(PageContext pageContext);
	}
}
