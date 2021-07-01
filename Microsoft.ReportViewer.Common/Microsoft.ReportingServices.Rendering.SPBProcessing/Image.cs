using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Drawing;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class Image : PageItem
	{
		internal sealed class AutosizeImageProps
		{
			private bool m_invalidImage;

			private int m_width;

			private int m_height;

			internal bool InvalidImage
			{
				get
				{
					return m_invalidImage;
				}
				set
				{
					m_invalidImage = value;
				}
			}

			internal int Width
			{
				get
				{
					return m_width;
				}
				set
				{
					m_width = value;
				}
			}

			internal int Height
			{
				get
				{
					return m_height;
				}
				set
				{
					m_height = value;
				}
			}
		}

		internal Image(Microsoft.ReportingServices.OnDemandReportRendering.Image source, PageContext pageContext, bool createForRepeat)
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
			if (source.Sizing == Microsoft.ReportingServices.OnDemandReportRendering.Image.Sizings.AutoSize)
			{
				if (m_itemPageSizes.Width == 0.0)
				{
					m_itemPageSizes.Width = 3.8;
				}
				if (m_itemPageSizes.Height == 0.0)
				{
					m_itemPageSizes.Height = 4.0;
				}
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

		internal override void CalculateRepeatWithPage(RPLWriter rplWriter, PageContext pageContext, PageItem[] siblings)
		{
			AdjustOriginFromItemsAbove(siblings, null);
			ItemSizes contentSize = null;
			ResolveItemHiddenState(rplWriter, null, pageContext, createForRepeat: true, ref contentSize);
			if (m_itemRenderSizes == null)
			{
				CreateItemRenderSizes(contentSize, pageContext, createForRepeat: true);
			}
		}

		internal override int WriteRepeatWithToPage(RPLWriter rplWriter, PageContext pageContext)
		{
			if (base.ItemState == State.OnPageHidden)
			{
				return 0;
			}
			WriteItemToStream(rplWriter, pageContext);
			return 1;
		}

		private ReportSize GetStyleValue(StyleAttributeNames styleName)
		{
			object obj = null;
			ReportProperty reportProperty = m_source.Style[styleName];
			if (reportProperty != null)
			{
				if (reportProperty.IsExpression)
				{
					obj = GetStyleInstance(m_source, null)[styleName];
				}
				if (obj == null)
				{
					obj = ((ReportSizeProperty)reportProperty).Value;
				}
			}
			return obj as ReportSize;
		}

		private double GetPaddings(StyleAttributeNames padd1, StyleAttributeNames padd2)
		{
			double num = 0.0;
			ReportSize styleValue = GetStyleValue(padd1);
			if (styleValue != null)
			{
				num += styleValue.ToMillimeters();
			}
			styleValue = GetStyleValue(padd2);
			if (styleValue != null)
			{
				num += styleValue.ToMillimeters();
			}
			return num;
		}

		private bool AutoSizeImage(PageContext pageContext, ImageInstance imageInstance, out System.Drawing.Image gdiImage)
		{
			gdiImage = null;
			if (!pageContext.MeasureItems)
			{
				return false;
			}
			if (((Microsoft.ReportingServices.OnDemandReportRendering.Image)m_source).Sizing != 0)
			{
				return false;
			}
			bool result = false;
			AutosizeImageProps autosizeImageProps = null;
			string streamName = imageInstance.StreamName;
			if (streamName != null)
			{
				Hashtable hashtable = pageContext.AutoSizeSharedImages;
				if (hashtable != null)
				{
					autosizeImageProps = (AutosizeImageProps)hashtable[streamName];
					if (autosizeImageProps != null)
					{
						ResizeImage(pageContext, autosizeImageProps.Width, autosizeImageProps.Height);
						return autosizeImageProps.InvalidImage;
					}
				}
				autosizeImageProps = new AutosizeImageProps();
				if (hashtable == null)
				{
					hashtable = (pageContext.AutoSizeSharedImages = new Hashtable());
				}
				hashtable.Add(streamName, autosizeImageProps);
			}
			else
			{
				autosizeImageProps = new AutosizeImageProps();
			}
			byte[] array = imageInstance.ImageData;
			if (array != null)
			{
				try
				{
					MemoryStream stream = new MemoryStream(array, writable: false);
					gdiImage = System.Drawing.Image.FromStream(stream);
					if (gdiImage != null)
					{
						((Bitmap)gdiImage).SetResolution(pageContext.DpiX, pageContext.DpiY);
					}
				}
				catch
				{
					array = null;
					if (gdiImage != null)
					{
						gdiImage.Dispose();
						gdiImage = null;
					}
				}
			}
			if (array == null)
			{
				gdiImage = Microsoft.ReportingServices.InvalidImage.Image;
				result = true;
				autosizeImageProps.InvalidImage = true;
			}
			if (gdiImage != null)
			{
				ResizeImage(pageContext, gdiImage.Width, gdiImage.Height);
				autosizeImageProps.Width = gdiImage.Width;
				autosizeImageProps.Height = gdiImage.Height;
			}
			return result;
		}

		private void ResizeImage(PageContext pageContext, int width, int height)
		{
			double paddings = GetPaddings(StyleAttributeNames.PaddingLeft, StyleAttributeNames.PaddingRight);
			double paddings2 = GetPaddings(StyleAttributeNames.PaddingTop, StyleAttributeNames.PaddingBottom);
			double num = pageContext.ConvertToMillimeters(height, pageContext.DpiY);
			m_itemRenderSizes.AdjustHeightTo(num + paddings2);
			double num2 = pageContext.ConvertToMillimeters(width, pageContext.DpiX);
			m_itemRenderSizes.AdjustWidthTo(num2 + paddings);
		}

		internal void WriteItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				Stream baseStream = binaryWriter.BaseStream;
				long position = baseStream.Position;
				binaryWriter.Write((byte)9);
				WriteElementProps(binaryWriter, rplWriter, pageContext, position + 1);
				m_offset = baseStream.Position;
				binaryWriter.Write((byte)254);
				binaryWriter.Write(position);
				binaryWriter.Write(byte.MaxValue);
			}
			else
			{
				m_rplElement = new RPLImage();
				WriteElementProps(m_rplElement.ElementProps, rplWriter, pageContext);
			}
		}

		internal override void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			switch (((Microsoft.ReportingServices.OnDemandReportRendering.Image)m_source).Sizing)
			{
			case Microsoft.ReportingServices.OnDemandReportRendering.Image.Sizings.Clip:
				spbifWriter.Write((byte)41);
				spbifWriter.Write((byte)3);
				break;
			case Microsoft.ReportingServices.OnDemandReportRendering.Image.Sizings.Fit:
				spbifWriter.Write((byte)41);
				spbifWriter.Write((byte)1);
				break;
			case Microsoft.ReportingServices.OnDemandReportRendering.Image.Sizings.FitProportional:
				spbifWriter.Write((byte)41);
				spbifWriter.Write((byte)2);
				break;
			}
		}

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Image obj = (Microsoft.ReportingServices.OnDemandReportRendering.Image)m_source;
			RPLImagePropsDef rPLImagePropsDef = (RPLImagePropsDef)sharedProps;
			switch (obj.Sizing)
			{
			case Microsoft.ReportingServices.OnDemandReportRendering.Image.Sizings.AutoSize:
				rPLImagePropsDef.Sizing = RPLFormat.Sizings.AutoSize;
				break;
			case Microsoft.ReportingServices.OnDemandReportRendering.Image.Sizings.Clip:
				rPLImagePropsDef.Sizing = RPLFormat.Sizings.Clip;
				break;
			case Microsoft.ReportingServices.OnDemandReportRendering.Image.Sizings.Fit:
				rPLImagePropsDef.Sizing = RPLFormat.Sizings.Fit;
				break;
			case Microsoft.ReportingServices.OnDemandReportRendering.Image.Sizings.FitProportional:
				rPLImagePropsDef.Sizing = RPLFormat.Sizings.FitProportional;
				break;
			}
		}

		internal override void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Image image = (Microsoft.ReportingServices.OnDemandReportRendering.Image)m_source;
			ImageInstance imageInstance = (ImageInstance)image.Instance;
			System.Drawing.Image gdiImage = null;
			bool flag = AutoSizeImage(pageContext, imageInstance, out gdiImage);
			try
			{
				if (flag)
				{
					WriteImage(null, "InvalidImage", spbifWriter, pageContext, gdiImage);
				}
				else
				{
					WriteImage(imageInstance, null, spbifWriter, pageContext, gdiImage);
				}
			}
			finally
			{
				if (gdiImage != null)
				{
					gdiImage.Dispose();
					gdiImage = null;
				}
			}
			WriteActionInfo(image.ActionInfo, spbifWriter, pageContext, 7);
			WriteImageMapAreaInstanceCollection(imageInstance.ActionInfoWithDynamicImageMapAreas, spbifWriter, pageContext);
		}

		internal static string SharedImageStreamName(Microsoft.ReportingServices.OnDemandReportRendering.Image image, PageContext pageContext)
		{
			ImageInstance imageInstance = (ImageInstance)image.Instance;
			System.Drawing.Image gdiImage = null;
			bool num = new Image(image, pageContext, createForRepeat: false).AutoSizeImage(pageContext, imageInstance, out gdiImage);
			if (gdiImage != null)
			{
				gdiImage.Dispose();
				gdiImage = null;
			}
			if (num)
			{
				return "InvalidImage";
			}
			return imageInstance?.StreamName;
		}

		internal override void WriteCustomNonSharedItemProps(RPLElementProps nonSharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Image image = (Microsoft.ReportingServices.OnDemandReportRendering.Image)m_source;
			ImageInstance imageInstance = (ImageInstance)image.Instance;
			System.Drawing.Image gdiImage = null;
			RPLImageProps rPLImageProps = (RPLImageProps)nonSharedProps;
			if (AutoSizeImage(pageContext, imageInstance, out gdiImage))
			{
				WriteImage(null, "InvalidImage", rPLImageProps, pageContext, gdiImage);
			}
			else
			{
				WriteImage(imageInstance, null, rPLImageProps, pageContext, gdiImage);
			}
			rPLImageProps.ActionInfo = WriteActionInfo(image.ActionInfo, pageContext);
			rPLImageProps.ActionImageMapAreas = WriteImageMapAreaInstanceCollection(imageInstance.ActionInfoWithDynamicImageMapAreas, pageContext);
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingBottom, 18);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingLeft, 15);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingRight, 16);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingTop, 17);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingBottom, 18);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingLeft, 15);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingRight, 16);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingTop, 17);
		}

		internal override void WriteItemNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.PaddingBottom:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingBottom, 18);
				break;
			case StyleAttributeNames.PaddingLeft:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingLeft, 15);
				break;
			case StyleAttributeNames.PaddingRight:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingRight, 16);
				break;
			case StyleAttributeNames.PaddingTop:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingTop, 17);
				break;
			}
		}

		internal override void WriteItemNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.PaddingBottom:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingBottom, 18);
				break;
			case StyleAttributeNames.PaddingLeft:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingLeft, 15);
				break;
			case StyleAttributeNames.PaddingRight:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingRight, 16);
				break;
			case StyleAttributeNames.PaddingTop:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingTop, 17);
				break;
			}
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)9);
				base.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write(byte.MaxValue);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemHelper(9);
			base.WritePaginationInfoProperties(pageItemHelper);
			return pageItemHelper;
		}
	}
}
