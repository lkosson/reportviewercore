using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Image : PageItem, IStorable, IPersistable
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

		private bool m_invalidImage;

		[StaticReference]
		private GDIImageProps m_imageProps;

		private string m_streamName;

		private double m_padHorizontal;

		private double m_padVertical;

		private static Declaration m_declaration = GetDeclaration();

		public override int Size => base.Size + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(m_streamName) + 1 + 16;

		internal Image()
		{
		}

		internal Image(Microsoft.ReportingServices.OnDemandReportRendering.Image source)
			: base(source)
		{
			m_itemPageSizes = new ItemSizes(source);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.InvalidImage:
					writer.Write(m_invalidImage);
					break;
				case MemberName.ImageProps:
				{
					int value = scalabilityCache.StoreStaticReference(m_imageProps);
					writer.Write(value);
					break;
				}
				case MemberName.StreamName:
					writer.Write(m_streamName);
					break;
				case MemberName.HorizontalPadding:
					writer.Write(m_padHorizontal);
					break;
				case MemberName.VerticalPadding:
					writer.Write(m_padVertical);
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.InvalidImage:
					m_invalidImage = reader.ReadBoolean();
					break;
				case MemberName.ImageProps:
				{
					int id = reader.ReadInt32();
					m_imageProps = (GDIImageProps)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.StreamName:
					m_streamName = reader.ReadString();
					break;
				case MemberName.HorizontalPadding:
					m_padHorizontal = reader.ReadDouble();
					break;
				case MemberName.VerticalPadding:
					m_padVertical = reader.ReadDouble();
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.Image;
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.InvalidImage, Token.Boolean));
				list.Add(new MemberInfo(MemberName.ImageProps, Token.Int32));
				list.Add(new MemberInfo(MemberName.StreamName, Token.String));
				list.Add(new MemberInfo(MemberName.HorizontalPadding, Token.Double));
				list.Add(new MemberInfo(MemberName.VerticalPadding, Token.Double));
				return new Declaration(ObjectType.Image, ObjectType.PageItem, list);
			}
			return m_declaration;
		}

		protected override void DetermineHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			DetermineSize(pageContext);
		}

		protected override void DetermineVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			DetermineSize(pageContext);
		}

		private void DetermineSize(PageContext pageContext)
		{
			if (!m_invalidImage && m_streamName == null && m_imageProps == null)
			{
				Microsoft.ReportingServices.OnDemandReportRendering.Image image = (Microsoft.ReportingServices.OnDemandReportRendering.Image)m_source;
				CheckAutoSize(image, pageContext);
			}
			if (m_imageProps != null)
			{
				ResizeImage(pageContext, m_imageProps.Width, m_imageProps.Height);
			}
			else
			{
				if (m_streamName == null)
				{
					return;
				}
				Hashtable autoSizeSharedImages = pageContext.AutoSizeSharedImages;
				if (autoSizeSharedImages != null)
				{
					AutosizeImageProps autosizeImageProps = (AutosizeImageProps)autoSizeSharedImages[m_streamName];
					if (autosizeImageProps != null)
					{
						ResizeImage(pageContext, autosizeImageProps.Width, autosizeImageProps.Height);
					}
				}
			}
		}

		internal override void CacheNonSharedProperties(PageContext pageContext)
		{
			if (pageContext.CacheNonSharedProps)
			{
				Microsoft.ReportingServices.OnDemandReportRendering.Image image = (Microsoft.ReportingServices.OnDemandReportRendering.Image)m_source;
				CheckAutoSize(image, pageContext);
				base.CacheNonSharedProperties(pageContext);
			}
		}

		private void CheckAutoSize(Microsoft.ReportingServices.OnDemandReportRendering.Image image, PageContext pageContext)
		{
			if (image.Sizing == Microsoft.ReportingServices.OnDemandReportRendering.Image.Sizings.AutoSize)
			{
				ImageInstance imageInstance = (ImageInstance)image.Instance;
				System.Drawing.Image gdiImage = null;
				m_invalidImage = AutoSizeImage(pageContext, imageInstance, out gdiImage);
				if (gdiImage != null)
				{
					m_imageProps = new GDIImageProps(gdiImage);
					gdiImage.Dispose();
				}
			}
		}

		private bool AutoSizeImage(PageContext pageContext, ImageInstance imageInstance, out System.Drawing.Image gdiImage)
		{
			gdiImage = null;
			bool result = false;
			AutosizeImageProps autosizeImageProps = null;
			m_streamName = imageInstance.StreamName;
			if (m_streamName != null)
			{
				Hashtable hashtable = pageContext.AutoSizeSharedImages;
				if (hashtable != null)
				{
					autosizeImageProps = (AutosizeImageProps)hashtable[m_streamName];
					if (autosizeImageProps != null)
					{
						GetPaddings(pageContext);
						return autosizeImageProps.InvalidImage;
					}
				}
				autosizeImageProps = new AutosizeImageProps();
				if (hashtable == null)
				{
					hashtable = (pageContext.AutoSizeSharedImages = new Hashtable());
				}
				hashtable.Add(m_streamName, autosizeImageProps);
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
				GetPaddings(pageContext);
				autosizeImageProps.Width = gdiImage.Width;
				autosizeImageProps.Height = gdiImage.Height;
			}
			return result;
		}

		private void GetPaddings(PageContext pageContext)
		{
			PaddingsStyle paddingsStyle = null;
			if (pageContext.ItemPaddingsStyle != null)
			{
				paddingsStyle = (PaddingsStyle)pageContext.ItemPaddingsStyle[m_source.ID];
			}
			double padTop = 0.0;
			if (paddingsStyle != null)
			{
				paddingsStyle.GetPaddingValues(m_source, out m_padVertical, out m_padHorizontal, out padTop);
			}
			else
			{
				PaddingsStyle.CreatePaddingsStyle(pageContext, m_source, out m_padVertical, out m_padHorizontal, out padTop);
			}
		}

		private void ResizeImage(PageContext pageContext, int width, int height)
		{
			double num = pageContext.ConvertToMillimeters(height, pageContext.Common.Pagination.MeasureImageDpiY);
			m_itemPageSizes.AdjustHeightTo(num + m_padVertical);
			double num2 = pageContext.ConvertToMillimeters(width, pageContext.Common.Pagination.MeasureImageDpiX);
			m_itemPageSizes.AdjustWidthTo(num2 + m_padHorizontal);
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
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
				else if (m_rplElement == null)
				{
					m_rplElement = new RPLImage();
					WriteElementProps(m_rplElement.ElementProps, pageContext);
				}
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

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, PageContext pageContext)
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

		internal override void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Image image = (Microsoft.ReportingServices.OnDemandReportRendering.Image)m_source;
			ImageInstance imageInstance = (ImageInstance)image.Instance;
			if (m_invalidImage)
			{
				if (m_nonSharedOffset < 0)
				{
					WriteInvalidImage(spbifWriter, pageContext, m_imageProps);
				}
			}
			else
			{
				WriteImage(imageInstance, null, spbifWriter, pageContext, m_imageProps, writeShared: false);
			}
			WriteActionInfo(image.ActionInfo, spbifWriter);
		}

		internal override void WriteCustomNonSharedItemProps(RPLElementProps nonSharedProps, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Image image = (Microsoft.ReportingServices.OnDemandReportRendering.Image)m_source;
			ImageInstance imageInstance = (ImageInstance)image.Instance;
			RPLImageProps rPLImageProps = (RPLImageProps)nonSharedProps;
			if (m_invalidImage)
			{
				WriteInvalidImage(rPLImageProps, pageContext, m_imageProps);
			}
			else
			{
				WriteImage(imageInstance, null, rPLImageProps, pageContext, m_imageProps);
			}
			rPLImageProps.ActionInfo = PageItem.WriteActionInfo(image.ActionInfo);
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
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingBottom, 18);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingLeft, 15);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingRight, 16);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingTop, 17);
		}

		internal override void WriteItemNonSharedStyleProps(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
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

		internal override void WriteItemNonSharedStyleProps(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
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
	}
}
