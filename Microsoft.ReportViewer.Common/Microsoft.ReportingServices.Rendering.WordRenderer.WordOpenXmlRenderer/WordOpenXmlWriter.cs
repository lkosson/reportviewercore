using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.Rendering.Utilities;
using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models;
using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models.Relationships;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer
{
	internal sealed class WordOpenXmlWriter : IWordWriter, IDisposable
	{
		public delegate Stream CreateXmlStream(string name);

		private CreateAndRegisterStream _createAndRegisterStream;

		private AutoFit _autofit;

		private OpenXmlDocumentModel _document;

		private OpenXmlParagraphModel _currentParagraph;

		private OpenXmlRunPropertiesModel _currentTextStyle;

		private ScalabilityCache _scalabilityCache;

		private int _numberedListId;

		private int _bookmarkId;

		private uint _nextPictureId;

		private bool _forceEmptyLayoutCell;

		public bool CanBand => _document.TableContext.Depth > 1;

		public AutoFit AutoFit
		{
			get
			{
				return _autofit;
			}
			set
			{
				_autofit = value;
			}
		}

		public bool HasTitlePage
		{
			set
			{
				_document.SectionHasTitlePage = value;
			}
		}

		public int SectionCount => 1;

		private uint NextPictureId()
		{
			return _nextPictureId++;
		}

		private OpenXmlParagraphModel GetCurrentParagraph()
		{
			if (_currentParagraph == null)
			{
				_currentParagraph = new OpenXmlParagraphModel();
			}
			return _currentParagraph;
		}

		private void EndAndWriteCurrentParagraph(bool forceEmptyParagraph)
		{
			if (_currentParagraph != null)
			{
				if (_document.TableContext.Location == TableContext.State.InCell)
				{
					_document.TableContext.CurrentCell.AddContent(GetCurrentParagraph());
				}
				else
				{
					_document.WriteParagraph(GetCurrentParagraph());
				}
				_currentParagraph = null;
			}
			else if (forceEmptyParagraph)
			{
				if (_document.TableContext.Location == TableContext.State.InCell)
				{
					_document.TableContext.CurrentCell.AddContent(new OpenXmlParagraphModel.EmptyParagraph());
				}
				else
				{
					_document.WriteEmptyParagraph();
				}
			}
		}

		private OpenXmlRunPropertiesModel GetCurrentTextStyle()
		{
			if (_currentTextStyle == null)
			{
				_currentTextStyle = new OpenXmlRunPropertiesModel();
			}
			return _currentTextStyle;
		}

		private void SetTextDirection(RPLFormat.Directions direction)
		{
			GetCurrentTextStyle().RightToLeft = (direction == RPLFormat.Directions.RTL);
		}

		private void SetTextDecoration(RPLFormat.TextDecorations decoration)
		{
			switch (decoration)
			{
			case RPLFormat.TextDecorations.Overline:
				break;
			case RPLFormat.TextDecorations.LineThrough:
				GetCurrentTextStyle().Strikethrough = true;
				break;
			case RPLFormat.TextDecorations.Underline:
				GetCurrentTextStyle().Underline = true;
				break;
			}
		}

		private void SetTextColor(string color)
		{
			SetTextColor(new RPLReportColor(color).ToColor());
		}

		private void SetTextColor(Color color)
		{
			GetCurrentTextStyle().Color = color;
		}

		private void SetLineHeight(string height)
		{
			double lineHeight = new RPLReportSize(height).ToPoints();
			GetCurrentParagraph().Properties.LineHeight = lineHeight;
		}

		private void SetUnicodeBiDi(RPLFormat.UnicodeBiDiTypes biDiType)
		{
		}

		private void SetLanguage(string language)
		{
			GetCurrentTextStyle().Language = language;
		}

		private void SetParagraphDirection(RPLFormat.Directions direction)
		{
			GetCurrentParagraph().Properties.RightToLeft = (direction == RPLFormat.Directions.RTL);
		}

		private void SetBorderColor(IHaveABorderAndShading borderHolder, string color)
		{
			SetBorderColor(borderHolder, color, TableData.Positions.Bottom);
			SetBorderColor(borderHolder, color, TableData.Positions.Left);
			SetBorderColor(borderHolder, color, TableData.Positions.Right);
			SetBorderColor(borderHolder, color, TableData.Positions.Top);
		}

		private void SetBorderColor(IHaveABorderAndShading borderHolder, string color, TableData.Positions side)
		{
			switch (side)
			{
			case TableData.Positions.Top:
				borderHolder.BorderTop.Color = WordOpenXmlUtils.RgbColor(new RPLReportColor(color).ToColor());
				break;
			case TableData.Positions.Bottom:
				borderHolder.BorderBottom.Color = WordOpenXmlUtils.RgbColor(new RPLReportColor(color).ToColor());
				break;
			case TableData.Positions.Left:
				borderHolder.BorderLeft.Color = WordOpenXmlUtils.RgbColor(new RPLReportColor(color).ToColor());
				break;
			case TableData.Positions.Right:
				borderHolder.BorderRight.Color = WordOpenXmlUtils.RgbColor(new RPLReportColor(color).ToColor());
				break;
			}
		}

		private void SetBorderStyle(IHaveABorderAndShading borderHolder, RPLFormat.BorderStyles style)
		{
			SetBorderStyle(borderHolder, style, TableData.Positions.Top);
			SetBorderStyle(borderHolder, style, TableData.Positions.Bottom);
			SetBorderStyle(borderHolder, style, TableData.Positions.Left);
			SetBorderStyle(borderHolder, style, TableData.Positions.Right);
		}

		private OpenXmlBorderPropertiesModel.BorderStyle RPLFormatToBorderStyle(RPLFormat.BorderStyles style)
		{
			OpenXmlBorderPropertiesModel.BorderStyle result = OpenXmlBorderPropertiesModel.BorderStyle.None;
			switch (style)
			{
			case RPLFormat.BorderStyles.Dashed:
				result = OpenXmlBorderPropertiesModel.BorderStyle.Dashed;
				break;
			case RPLFormat.BorderStyles.Dotted:
				result = OpenXmlBorderPropertiesModel.BorderStyle.Dotted;
				break;
			case RPLFormat.BorderStyles.Double:
				result = OpenXmlBorderPropertiesModel.BorderStyle.Double;
				break;
			case RPLFormat.BorderStyles.None:
				result = OpenXmlBorderPropertiesModel.BorderStyle.None;
				break;
			case RPLFormat.BorderStyles.Solid:
				result = OpenXmlBorderPropertiesModel.BorderStyle.Solid;
				break;
			}
			return result;
		}

		private void SetBorderStyle(IHaveABorderAndShading borderHolder, RPLFormat.BorderStyles style, TableData.Positions side)
		{
			OpenXmlBorderPropertiesModel.BorderStyle style2 = RPLFormatToBorderStyle(style);
			switch (side)
			{
			case TableData.Positions.Top:
				borderHolder.BorderTop.Style = style2;
				break;
			case TableData.Positions.Bottom:
				borderHolder.BorderBottom.Style = style2;
				break;
			case TableData.Positions.Left:
				borderHolder.BorderLeft.Style = style2;
				break;
			case TableData.Positions.Right:
				borderHolder.BorderRight.Style = style2;
				break;
			}
		}

		private void SetBorderWidth(IHaveABorderAndShading borderHolder, string width)
		{
			SetBorderWidth(borderHolder, width, TableData.Positions.Top);
			SetBorderWidth(borderHolder, width, TableData.Positions.Bottom);
			SetBorderWidth(borderHolder, width, TableData.Positions.Left);
			SetBorderWidth(borderHolder, width, TableData.Positions.Right);
		}

		private void SetBorderWidth(IHaveABorderAndShading borderHolder, string width, TableData.Positions side)
		{
			int widthInEighthPoints = (int)Math.Floor(new RPLReportSize(width).ToPoints() * 8.0);
			switch (side)
			{
			case TableData.Positions.Top:
				borderHolder.BorderTop.WidthInEighthPoints = widthInEighthPoints;
				break;
			case TableData.Positions.Bottom:
				borderHolder.BorderBottom.WidthInEighthPoints = widthInEighthPoints;
				break;
			case TableData.Positions.Left:
				borderHolder.BorderLeft.WidthInEighthPoints = widthInEighthPoints;
				break;
			case TableData.Positions.Right:
				borderHolder.BorderRight.WidthInEighthPoints = widthInEighthPoints;
				break;
			}
		}

		private void SetVerticalAlign(RPLFormat.VerticalAlignments alignment)
		{
			switch (alignment)
			{
			case RPLFormat.VerticalAlignments.Bottom:
				_document.TableContext.CurrentCell.CellProperties.VerticalAlignment = OpenXmlTableCellPropertiesModel.VerticalAlign.Bottom;
				break;
			case RPLFormat.VerticalAlignments.Middle:
				_document.TableContext.CurrentCell.CellProperties.VerticalAlignment = OpenXmlTableCellPropertiesModel.VerticalAlign.Middle;
				break;
			case RPLFormat.VerticalAlignments.Top:
				_document.TableContext.CurrentCell.CellProperties.VerticalAlignment = OpenXmlTableCellPropertiesModel.VerticalAlign.Top;
				break;
			}
		}

		private void SetWritingMode(RPLFormat.WritingModes mode)
		{
			switch (mode)
			{
			case RPLFormat.WritingModes.Rotate270:
				_document.TableContext.CurrentCell.CellProperties.TextOrientation = OpenXmlTableCellPropertiesModel.TextOrientationEnum.Rotate270;
				break;
			case RPLFormat.WritingModes.Vertical:
				_document.TableContext.CurrentCell.CellProperties.TextOrientation = OpenXmlTableCellPropertiesModel.TextOrientationEnum.Rotate90;
				break;
			}
		}

		private void SetShading(IHaveABorderAndShading shadingHolder, string shading)
		{
			if (!shading.Equals("Transparent"))
			{
				string text2 = shadingHolder.BackgroundColor = WordOpenXmlUtils.RgbColor(new RPLReportColor(shading).ToColor());
			}
		}

		private Stream CreateXmlStreamImplementation(string name)
		{
			return _createAndRegisterStream(name, "xml", null, "application/xml", willSeek: false, StreamOper.CreateOnly);
		}

		public void Init(CreateAndRegisterStream createAndRegisterStream, AutoFit autoFit, string reportName)
		{
			_createAndRegisterStream = createAndRegisterStream;
			_autofit = autoFit;
			InitCache(_createAndRegisterStream);
			_document = new OpenXmlDocumentModel(createAndRegisterStream(reportName, "docx", null, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", willSeek: true, StreamOper.CreateAndRegister), CreateXmlStreamImplementation, _scalabilityCache);
			_nextPictureId = 0u;
			_forceEmptyLayoutCell = false;
		}

		public void SetPageDimensions(float pageHeight, float pageWidth, float leftMargin, float rightMargin, float topMargin, float bottomMargin)
		{
			_document.SectionProperties.Height = pageHeight;
			_document.SectionProperties.Width = pageWidth;
			_document.SectionProperties.IsLandscape = (pageWidth > pageHeight);
			_document.SectionProperties.LeftMargin = leftMargin;
			_document.SectionProperties.RightMargin = rightMargin;
			_document.SectionProperties.TopMargin = topMargin;
			_document.SectionProperties.BottomMargin = bottomMargin;
		}

		public void AddImage(byte[] imgBuf, float height, float width, RPLFormat.Sizings sizing)
		{
			bool flag = imgBuf == null || imgBuf.Length == 0;
			Size image = default(Size);
			string extension = null;
			if (!flag)
			{
				try
				{
					using (Image image2 = Image.FromStream(new MemoryStream(imgBuf)))
					{
						image.Height = WordOpenXmlUtils.PixelsToEmus(image2.Height, image2.VerticalResolution, 0, 20116800);
						image.Width = WordOpenXmlUtils.PixelsToEmus(image2.Width, image2.HorizontalResolution, 0, 20116800);
						extension = ((image2.RawFormat.Guid == ImageFormat.Png.Guid) ? "png" : ((image2.RawFormat.Guid == ImageFormat.Jpeg.Guid) ? "jpg" : ((!(image2.RawFormat.Guid == ImageFormat.Gif.Guid)) ? "bmp" : "gif")));
					}
				}
				catch (ArgumentException)
				{
					flag = true;
				}
			}
			if (flag)
			{
				AddImage(PictureDescriptor.INVALIDIMAGEDATA, height, width, RPLFormat.Sizings.Clip);
				return;
			}
			Size size = default(Size);
			size.Height = WordOpenXmlUtils.ToEmus(height, 0, 20116800);
			size.Width = WordOpenXmlUtils.ToEmus(width, 0, 20116800);
			Size size2 = size;
			ImageHash hash = new SizingIndependentImageHash(new OfficeImageHasher(imgBuf).Hash);
			Relationship relationship = _document.WriteImageData(imgBuf, hash, extension);
			Size.Strategy strategy = Size.Strategy.AutoSize;
			switch (sizing)
			{
			case RPLFormat.Sizings.AutoSize:
				strategy = Size.Strategy.AutoSize;
				break;
			case RPLFormat.Sizings.Fit:
				strategy = Size.Strategy.Fit;
				break;
			case RPLFormat.Sizings.FitProportional:
				strategy = Size.Strategy.FitProportional;
				break;
			case RPLFormat.Sizings.Clip:
				strategy = Size.Strategy.Clip;
				break;
			}
			Size size3 = WordOpenXmlUtils.SizeImage(image, size2, strategy);
			Size desiredSize = (strategy == Size.Strategy.FitProportional || strategy == Size.Strategy.AutoSize) ? size3 : size2;
			GetCurrentParagraph().AddImage(new OpenXmlPictureModel(size3, desiredSize, sizing == RPLFormat.Sizings.Clip, NextPictureId(), NextPictureId(), relationship.RelationshipId, Path.GetFileName(relationship.RelatedPart)));
		}

		public void WriteText(string text)
		{
			GetCurrentParagraph().AddText(text, GetCurrentTextStyle());
			_currentTextStyle = null;
		}

		public void WriteHyperlinkBegin(string target, bool bookmarkLink)
		{
			GetCurrentParagraph().StartHyperlink(target, bookmarkLink, GetCurrentTextStyle());
		}

		public void WriteHyperlinkEnd()
		{
			GetCurrentParagraph().EndHyperlink(GetCurrentTextStyle());
		}

		public void AddTableStyleProp(byte code, object value)
		{
			if (value != null)
			{
				IHaveABorderAndShading tableProperties = _document.TableContext.CurrentTable.TableProperties;
				switch (code)
				{
				case 15:
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
				case 24:
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
				case 32:
				case 33:
				case 35:
				case 36:
				case 37:
					break;
				case 0:
					SetBorderColor(tableProperties, (string)value);
					break;
				case 1:
					SetBorderColor(tableProperties, (string)value, TableData.Positions.Left);
					break;
				case 2:
					SetBorderColor(tableProperties, (string)value, TableData.Positions.Right);
					break;
				case 3:
					SetBorderColor(tableProperties, (string)value, TableData.Positions.Top);
					break;
				case 4:
					SetBorderColor(tableProperties, (string)value, TableData.Positions.Bottom);
					break;
				case 5:
					SetBorderStyle(tableProperties, (RPLFormat.BorderStyles)value);
					break;
				case 6:
					SetBorderStyle(tableProperties, (RPLFormat.BorderStyles)value, TableData.Positions.Left);
					break;
				case 7:
					SetBorderStyle(tableProperties, (RPLFormat.BorderStyles)value, TableData.Positions.Right);
					break;
				case 8:
					SetBorderStyle(tableProperties, (RPLFormat.BorderStyles)value, TableData.Positions.Top);
					break;
				case 9:
					SetBorderStyle(tableProperties, (RPLFormat.BorderStyles)value, TableData.Positions.Bottom);
					break;
				case 10:
					SetBorderWidth(tableProperties, (string)value);
					break;
				case 11:
					SetBorderWidth(tableProperties, (string)value, TableData.Positions.Left);
					break;
				case 12:
					SetBorderWidth(tableProperties, (string)value, TableData.Positions.Right);
					break;
				case 13:
					SetBorderWidth(tableProperties, (string)value, TableData.Positions.Top);
					break;
				case 14:
					SetBorderWidth(tableProperties, (string)value, TableData.Positions.Bottom);
					break;
				case 34:
					SetShading(tableProperties, (string)value);
					break;
				}
			}
		}

		public void SetTableContext(BorderContext borderContext)
		{
			if (borderContext.Top)
			{
				SetBorderStyle(_document.TableContext.CurrentTable.TableProperties, RPLFormat.BorderStyles.None, TableData.Positions.Top);
			}
			if (borderContext.Bottom)
			{
				SetBorderStyle(_document.TableContext.CurrentTable.TableProperties, RPLFormat.BorderStyles.None, TableData.Positions.Bottom);
			}
			if (borderContext.Left)
			{
				SetBorderStyle(_document.TableContext.CurrentTable.TableProperties, RPLFormat.BorderStyles.None, TableData.Positions.Left);
			}
			if (borderContext.Right)
			{
				SetBorderStyle(_document.TableContext.CurrentTable.TableProperties, RPLFormat.BorderStyles.None, TableData.Positions.Right);
			}
		}

		public void AddBodyStyleProp(byte code, object value)
		{
			if ((uint)code > 18u)
			{
				_ = code - 33;
				_ = 4;
			}
		}

		public void AddCellStyleProp(int cellIndex, byte code, object value)
		{
			if (value != null)
			{
				IHaveABorderAndShading cellProperties = _document.TableContext.CurrentCell.CellProperties;
				switch (code)
				{
				case 15:
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
				case 24:
				case 25:
				case 27:
				case 28:
				case 29:
				case 31:
				case 32:
				case 33:
				case 35:
					break;
				case 0:
					SetBorderColor(cellProperties, (string)value);
					break;
				case 1:
					SetBorderColor(cellProperties, (string)value, TableData.Positions.Left);
					break;
				case 2:
					SetBorderColor(cellProperties, (string)value, TableData.Positions.Right);
					break;
				case 3:
					SetBorderColor(cellProperties, (string)value, TableData.Positions.Top);
					break;
				case 4:
					SetBorderColor(cellProperties, (string)value, TableData.Positions.Bottom);
					break;
				case 5:
					SetBorderStyle(cellProperties, (RPLFormat.BorderStyles)value);
					break;
				case 6:
					SetBorderStyle(cellProperties, (RPLFormat.BorderStyles)value, TableData.Positions.Left);
					break;
				case 7:
					SetBorderStyle(cellProperties, (RPLFormat.BorderStyles)value, TableData.Positions.Right);
					break;
				case 8:
					SetBorderStyle(cellProperties, (RPLFormat.BorderStyles)value, TableData.Positions.Top);
					break;
				case 9:
					SetBorderStyle(cellProperties, (RPLFormat.BorderStyles)value, TableData.Positions.Bottom);
					break;
				case 10:
					SetBorderWidth(cellProperties, (string)value);
					break;
				case 11:
					SetBorderWidth(cellProperties, (string)value, TableData.Positions.Left);
					break;
				case 12:
					SetBorderWidth(cellProperties, (string)value, TableData.Positions.Right);
					break;
				case 13:
					SetBorderWidth(cellProperties, (string)value, TableData.Positions.Top);
					break;
				case 14:
					SetBorderWidth(cellProperties, (string)value, TableData.Positions.Bottom);
					break;
				case 26:
					SetVerticalAlign((RPLFormat.VerticalAlignments)value);
					break;
				case 30:
					SetWritingMode((RPLFormat.WritingModes)value);
					break;
				case 34:
					SetShading(cellProperties, (string)value);
					break;
				}
			}
		}

		public void AddPadding(int cellIndex, byte code, object value, int defaultValue)
		{
			double num = new RPLReportSize((string)value).ToPoints();
			switch (code)
			{
			case 15:
				_document.TableContext.CurrentCell.CellProperties.PaddingLeft = num;
				break;
			case 16:
				_document.TableContext.CurrentCell.CellProperties.PaddingRight = num;
				break;
			case 17:
				_document.TableContext.CurrentCell.CellProperties.PaddingTop = num;
				_document.TableContext.CurrentRow.RowProperties.SetCellPaddingTop(num);
				break;
			case 18:
				_document.TableContext.CurrentCell.CellProperties.PaddingBottom = num;
				_document.TableContext.CurrentRow.RowProperties.SetCellPaddingBottom(num);
				break;
			}
		}

		public void ApplyCellBorderContext(BorderContext borderContext)
		{
			OpenXmlTableCellModel currentCell = _document.TableContext.CurrentCell;
			if (borderContext.Top)
			{
				currentCell.UseTopTableBorder();
			}
			if (borderContext.Bottom)
			{
				currentCell.UseBottomTableBorder();
			}
			if (borderContext.Left)
			{
				currentCell.UseLeftTableBorder();
			}
			if (borderContext.Right)
			{
				currentCell.UseRightTableBorder();
			}
		}

		public void AddTextStyleProp(byte code, object value)
		{
			if (value == null)
			{
				return;
			}
			switch (code)
			{
			case 23:
			case 25:
			case 26:
			case 30:
			case 33:
			case 34:
			case 35:
			case 36:
			case 37:
				break;
			case 24:
				SetTextDecoration((RPLFormat.TextDecorations)value);
				break;
			case 27:
				if (value is string)
				{
					SetTextColor((string)value);
				}
				else if (value is Color)
				{
					SetTextColor((Color)value);
				}
				break;
			case 28:
				SetLineHeight(value as string);
				break;
			case 29:
				SetParagraphDirection((RPLFormat.Directions)value);
				break;
			case 31:
				SetUnicodeBiDi((RPLFormat.UnicodeBiDiTypes)value);
				break;
			case 32:
				SetLanguage(value as string);
				break;
			}
		}

		public void AddFirstLineIndent(float indent)
		{
			if (indent >= 0f)
			{
				GetCurrentParagraph().Properties.Indentation.First = indent;
			}
			else
			{
				GetCurrentParagraph().Properties.Indentation.Hanging = 0f - indent;
			}
		}

		public void AddLeftIndent(float margin)
		{
			GetCurrentParagraph().Properties.Indentation.Left = margin;
		}

		public void AddRightIndent(float margin)
		{
			GetCurrentParagraph().Properties.Indentation.Right = margin;
		}

		public void AddSpaceBefore(float space)
		{
			_currentParagraph.Properties.PointsBefore = space;
		}

		public void AddSpaceAfter(float space)
		{
			_currentParagraph.Properties.PointsAfter = space;
		}

		public void RenderTextRunDirection(RPLFormat.Directions direction)
		{
			SetTextDirection(direction);
		}

		public void RenderTextAlign(TypeCode type, RPLFormat.TextAlignments textAlignments, RPLFormat.Directions direction)
		{
			OpenXmlParagraphPropertiesModel.HorizontalAlignment horizontalAlign = OpenXmlParagraphPropertiesModel.HorizontalAlignment.Left;
			if (textAlignments == RPLFormat.TextAlignments.General)
			{
				textAlignments = ((!WordOpenXmlUtils.GetTextAlignForType(type)) ? RPLFormat.TextAlignments.Left : RPLFormat.TextAlignments.Right);
			}
			else if (direction == RPLFormat.Directions.RTL)
			{
				switch (textAlignments)
				{
				case RPLFormat.TextAlignments.Left:
					textAlignments = RPLFormat.TextAlignments.Right;
					break;
				case RPLFormat.TextAlignments.Right:
					textAlignments = RPLFormat.TextAlignments.Left;
					break;
				}
			}
			switch (textAlignments)
			{
			case RPLFormat.TextAlignments.Left:
				horizontalAlign = OpenXmlParagraphPropertiesModel.HorizontalAlignment.Left;
				break;
			case RPLFormat.TextAlignments.Center:
				horizontalAlign = OpenXmlParagraphPropertiesModel.HorizontalAlignment.Center;
				break;
			case RPLFormat.TextAlignments.Right:
				horizontalAlign = OpenXmlParagraphPropertiesModel.HorizontalAlignment.Right;
				break;
			}
			GetCurrentParagraph().Properties.HorizontalAlign = horizontalAlign;
		}

		public void RenderFontWeight(RPLFormat.FontWeights fontWeights, RPLFormat.Directions dir)
		{
			GetCurrentTextStyle().SetBold((int)fontWeights >= 5, dir == RPLFormat.Directions.RTL);
		}

		public void RenderFontWeight(RPLFormat.FontWeights? fontWeights, RPLFormat.Directions dir)
		{
			if (fontWeights.HasValue)
			{
				RenderFontWeight(fontWeights.Value, dir);
			}
		}

		public void RenderFontSize(string size, RPLFormat.Directions dir)
		{
			GetCurrentTextStyle().SetSize(new RPLReportSize(size).ToPoints(), dir == RPLFormat.Directions.RTL);
		}

		public void RenderFontFamily(string font, RPLFormat.Directions dir)
		{
			GetCurrentTextStyle().SetFont(font, dir == RPLFormat.Directions.RTL);
		}

		public void RenderFontStyle(RPLFormat.FontStyles value, RPLFormat.Directions dir)
		{
			GetCurrentTextStyle().SetItalic(value == RPLFormat.FontStyles.Italic, dir == RPLFormat.Directions.RTL);
		}

		public void RenderFontStyle(RPLFormat.FontStyles? value, RPLFormat.Directions dir)
		{
			if (value.HasValue)
			{
				RenderFontStyle(value.Value, dir);
			}
		}

		public void WriteParagraphEnd()
		{
			EndAndWriteCurrentParagraph(forceEmptyParagraph: true);
		}

		public void WriteListEnd(int level, RPLFormat.ListStyles listStyle, bool endParagraph)
		{
			if (listStyle != 0)
			{
				OpenXmlParagraphModel currentParagraph = GetCurrentParagraph();
				currentParagraph.Properties.ListLevel = level - 1;
				currentParagraph.Properties.ListStyleId = ((listStyle == RPLFormat.ListStyles.Bulleted) ? 1 : _numberedListId);
			}
			if (endParagraph)
			{
				WriteParagraphEnd();
			}
		}

		public void InitListLevels()
		{
		}

		public void ResetListlevels()
		{
			_numberedListId = _document.ListManager.RegisterNewNumberedList();
		}

		public void WriteTableCellEnd(int cellIndex, BorderContext borderContext, bool emptyLayoutCell)
		{
			EndAndWriteCurrentParagraph(forceEmptyParagraph: false);
			_document.TableContext.WriteTableCellEnd(cellIndex, borderContext, emptyLayoutCell || _forceEmptyLayoutCell);
			_forceEmptyLayoutCell = false;
		}

		public void WriteEmptyStyle()
		{
			_forceEmptyLayoutCell = true;
		}

		public void WriteTableBegin(float left, bool layoutTable)
		{
			_document.TableContext.WriteTableBegin(left, layoutTable, AutoFit);
		}

		public void WriteTableRowBegin(float left, float height, float[] columnWidths)
		{
			_document.TableContext.WriteTableRowBegin(left, height, columnWidths);
		}

		public void IgnoreRowHeight(bool canGrow)
		{
			_document.TableContext.CurrentRow.RowProperties.IgnoreRowHeight = canGrow;
		}

		public void SetWriteExactRowHeight(bool writeExactRowHeight)
		{
			_document.TableContext.CurrentRow.RowProperties.ExactRowHeight = writeExactRowHeight;
		}

		public void WriteTableCellBegin(int cellIndex, int numColumns, bool firstVertMerge, bool firstHorzMerge, bool vertMerge, bool horzMerge)
		{
			_document.TableContext.WriteTableCellBegin(cellIndex, numColumns, firstVertMerge, firstHorzMerge, vertMerge, horzMerge);
		}

		public void WriteTableRowEnd()
		{
			_document.TableContext.WriteTableRowEnd();
		}

		public void WriteTableEnd()
		{
			_document.TableContext.WriteTableEnd();
		}

		public void Finish(string title, string author, string comments)
		{
			_document.WriteDocumentProperties(title, author, comments);
			_document.Save();
		}

		public int WriteFont(string fontName)
		{
			return 0;
		}

		public void RenderBookmark(string name)
		{
			GetCurrentParagraph().AddBookmark(name, _bookmarkId++);
		}

		public void RenderLabel(string label, int level)
		{
			GetCurrentParagraph().AddLabel(label, level, GetCurrentTextStyle());
		}

		public void WritePageNumberField()
		{
			GetCurrentParagraph().AddPageNumberField(GetCurrentTextStyle());
		}

		public void WriteTotalPagesField()
		{
			GetCurrentParagraph().AddPageCountField(GetCurrentTextStyle());
		}

		public void AddListStyle(int level, bool bulleted)
		{
		}

		public void WriteCellDiagonal(int cellIndex, RPLFormat.BorderStyles style, string width, string color, bool slantUp)
		{
			OpenXmlTableCellModel currentCell = _document.TableContext.CurrentCell;
			OpenXmlBorderPropertiesModel obj = slantUp ? currentCell.CellProperties.BorderDiagonalUp : currentCell.CellProperties.BorderDiagonalDown;
			obj.Color = WordOpenXmlUtils.RgbColor(new RPLReportColor(color).ToColor());
			obj.Style = RPLFormatToBorderStyle(style);
			obj.WidthInEighthPoints = (int)Math.Floor(new RPLReportSize(width).ToPoints() * 8.0);
		}

		public void WritePageBreak()
		{
			EndAndWriteCurrentParagraph(forceEmptyParagraph: false);
			if (_document.TableContext.Location == TableContext.State.InCell)
			{
				_document.TableContext.CurrentCell.AddContent(new OpenXmlParagraphModel.PageBreakParagraph());
			}
			else
			{
				_document.WritePageBreak();
			}
		}

		public void WriteEndSection()
		{
			_document.WriteSectionBreak();
		}

		public void ClearCellBorder(TableData.Positions position)
		{
			SetBorderStyle(_document.TableContext.CurrentCell.CellProperties, RPLFormat.BorderStyles.None, position);
			_document.TableContext.CurrentCell.BlockBorderAt(position);
		}

		public void StartHeader()
		{
			_document.StartHeader();
		}

		public void StartHeader(bool firstPage)
		{
			if (firstPage)
			{
				_document.StartFirstPageHeader();
			}
			else
			{
				_document.StartHeader();
			}
		}

		public void FinishHeader()
		{
			_document.FinishHeader();
		}

		public void StartFooter()
		{
			_document.StartFooter();
		}

		public void StartFooter(bool firstPage)
		{
			if (firstPage)
			{
				_document.StartFirstPageFooter();
			}
			else
			{
				_document.StartFooter();
			}
		}

		public void FinishFooter()
		{
			_document.FinishFooter();
		}

		private void InitCache(CreateAndRegisterStream streamDelegate)
		{
			if (_scalabilityCache == null)
			{
				_scalabilityCache = (ScalabilityCache)ScalabilityUtils.CreateCacheForTransientAllocations(streamDelegate, "Word", Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models.StorageObjectCreator.Instance, WordRendererReferenceCreator.Instance, ComponentType.Rendering, 1);
			}
		}

		public void Dispose()
		{
			if (_scalabilityCache != null)
			{
				_scalabilityCache.Dispose();
			}
		}

		public void InitHeaderFooter()
		{
		}

		public void FinishHeader(int section)
		{
		}

		public void FinishFooter(int section)
		{
		}

		public void FinishHeader(int section, Word97Writer.HeaderFooterLocation location)
		{
		}

		public void FinishFooter(int section, Word97Writer.HeaderFooterLocation location)
		{
		}

		public void FinishHeaderFooterRegion(int section, int index)
		{
		}

		public void FinishHeadersFooters(bool hasTitlePage)
		{
		}
	}
}
