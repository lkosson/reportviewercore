using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlTableRowPropertiesModel : BaseInterleaver, IHaveABorderAndShading
	{
		private float _height;

		private float _rowIndent;

		private bool _exactRowHeight;

		private bool _ignoreRowHeight;

		private float _maxPaddingBottom;

		private float _maxPaddingTop;

		private OpenXmlBorderPropertiesModel _borderTop;

		private OpenXmlBorderPropertiesModel _borderBottom;

		private OpenXmlBorderPropertiesModel _borderLeft;

		private OpenXmlBorderPropertiesModel _borderRight;

		private string _bgColor;

		[NonSerialized]
		private static Declaration _declaration;

		public float Height
		{
			get
			{
				return _height;
			}
			set
			{
				_height = value;
			}
		}

		public float RowIndent
		{
			set
			{
				_rowIndent = value;
			}
		}

		public bool ExactRowHeight
		{
			set
			{
				_exactRowHeight = value;
			}
		}

		public bool IgnoreRowHeight
		{
			set
			{
				_ignoreRowHeight = value;
			}
		}

		public string BackgroundColor
		{
			set
			{
				_bgColor = value;
			}
		}

		public OpenXmlBorderPropertiesModel BorderTop
		{
			get
			{
				if (_borderTop == null)
				{
					_borderTop = new OpenXmlBorderPropertiesModel();
				}
				return _borderTop;
			}
		}

		public OpenXmlBorderPropertiesModel BorderBottom
		{
			get
			{
				if (_borderBottom == null)
				{
					_borderBottom = new OpenXmlBorderPropertiesModel();
				}
				return _borderBottom;
			}
		}

		public OpenXmlBorderPropertiesModel BorderLeft
		{
			get
			{
				if (_borderLeft == null)
				{
					_borderLeft = new OpenXmlBorderPropertiesModel();
				}
				return _borderLeft;
			}
		}

		public OpenXmlBorderPropertiesModel BorderRight
		{
			get
			{
				if (_borderRight == null)
				{
					_borderRight = new OpenXmlBorderPropertiesModel();
				}
				return _borderRight;
			}
		}

		public override int Size => base.Size + 8 + 2 + 8 + ItemSizes.SizeOf(_borderTop) + ItemSizes.SizeOf(_borderBottom) + ItemSizes.SizeOf(_borderLeft) + ItemSizes.SizeOf(_borderRight) + ItemSizes.SizeOf(_bgColor);

		public OpenXmlTableRowPropertiesModel(int index, long location)
			: base(index, location)
		{
		}

		static OpenXmlTableRowPropertiesModel()
		{
			_declaration = new Declaration(ObjectType.WordOpenXmlTableRowProperties, ObjectType.WordOpenXmlBaseInterleaver, new List<MemberInfo>
			{
				new MemberInfo(MemberName.RowHeight, Token.Single),
				new MemberInfo(MemberName.LeftIndent, Token.Single),
				new MemberInfo(MemberName.ExactRowHeight, Token.Boolean),
				new MemberInfo(MemberName.IgnoreRowHeight, Token.Boolean),
				new MemberInfo(MemberName.BottomPadding, Token.Single),
				new MemberInfo(MemberName.TopPadding, Token.Single),
				new MemberInfo(MemberName.TopBorder, ObjectType.WordOpenXmlBorderProperties),
				new MemberInfo(MemberName.BottomBorder, ObjectType.WordOpenXmlBorderProperties),
				new MemberInfo(MemberName.LeftBorder, ObjectType.WordOpenXmlBorderProperties),
				new MemberInfo(MemberName.RightBorder, ObjectType.WordOpenXmlBorderProperties),
				new MemberInfo(MemberName.Color, Token.String)
			});
		}

		public OpenXmlTableRowPropertiesModel()
		{
		}

		public void SetCellPaddingTop(double padding)
		{
			_maxPaddingTop = Math.Max(_maxPaddingTop, (float)padding);
		}

		public void SetCellPaddingBottom(double padding)
		{
			_maxPaddingBottom = Math.Max(_maxPaddingBottom, (float)padding);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(GetDeclaration());
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.RowHeight:
					writer.Write(_height);
					break;
				case MemberName.LeftIndent:
					writer.Write(_rowIndent);
					break;
				case MemberName.ExactRowHeight:
					writer.Write(_exactRowHeight);
					break;
				case MemberName.IgnoreRowHeight:
					writer.Write(_ignoreRowHeight);
					break;
				case MemberName.TopPadding:
					writer.Write(_maxPaddingTop);
					break;
				case MemberName.BottomPadding:
					writer.Write(_maxPaddingBottom);
					break;
				case MemberName.TopBorder:
					writer.Write(_borderTop);
					break;
				case MemberName.BottomBorder:
					writer.Write(_borderBottom);
					break;
				case MemberName.LeftBorder:
					writer.Write(_borderLeft);
					break;
				case MemberName.RightBorder:
					writer.Write(_borderRight);
					break;
				case MemberName.Color:
					writer.Write(_bgColor);
					break;
				default:
					WordOpenXmlUtils.FailSerializable();
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(GetDeclaration());
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.RowHeight:
					_height = reader.ReadSingle();
					break;
				case MemberName.LeftIndent:
					_rowIndent = reader.ReadSingle();
					break;
				case MemberName.ExactRowHeight:
					_exactRowHeight = reader.ReadBoolean();
					break;
				case MemberName.IgnoreRowHeight:
					_ignoreRowHeight = reader.ReadBoolean();
					break;
				case MemberName.TopPadding:
					_maxPaddingTop = reader.ReadSingle();
					break;
				case MemberName.BottomPadding:
					_maxPaddingBottom = reader.ReadSingle();
					break;
				case MemberName.TopBorder:
					_borderTop = (OpenXmlBorderPropertiesModel)reader.ReadRIFObject();
					break;
				case MemberName.BottomBorder:
					_borderBottom = (OpenXmlBorderPropertiesModel)reader.ReadRIFObject();
					break;
				case MemberName.LeftBorder:
					_borderLeft = (OpenXmlBorderPropertiesModel)reader.ReadRIFObject();
					break;
				case MemberName.RightBorder:
					_borderRight = (OpenXmlBorderPropertiesModel)reader.ReadRIFObject();
					break;
				case MemberName.Color:
					_bgColor = reader.ReadString();
					break;
				default:
					WordOpenXmlUtils.FailSerializable();
					break;
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.WordOpenXmlTableRowProperties;
		}

		internal new static Declaration GetDeclaration()
		{
			return _declaration;
		}

		public override void Write(TextWriter output)
		{
			output.Write("<w:trPr>");
			if (_rowIndent > 0f)
			{
				output.Write("<w:wBefore w:w=\"");
				output.Write(WordOpenXmlUtils.ToTwips(_rowIndent, 0f, 31680f));
				output.Write("\" w:type=\"dxa\"/>");
			}
			long num = (long)WordOpenXmlUtils.ToTwips(_height) - (long)WordOpenXmlUtils.PointsToTwips(_maxPaddingTop, 0.0, 31680.0) - WordOpenXmlUtils.PointsToTwips(_maxPaddingBottom, 0.0, 31680.0);
			if (!_ignoreRowHeight && num > 0)
			{
				output.Write("<w:trHeight w:val=\"");
				output.Write(WordOpenXmlUtils.TwipsToString(num, 0, 31680));
				output.Write(_exactRowHeight ? "\" w:hRule=\"exact\"/>" : "\" w:hRule=\"atLeast\"/>");
			}
			output.Write("</w:trPr>");
			bool num2 = _borderTop != null || _borderBottom != null || _borderLeft != null || _borderRight != null;
			if (num2 || _bgColor != null)
			{
				output.Write("<w:tblPrEx>");
			}
			if (num2)
			{
				output.Write("<w:tblBorders>");
				if (_borderTop != null)
				{
					_borderTop.Write(output, "top");
				}
				if (_borderLeft != null)
				{
					_borderLeft.Write(output, "left");
				}
				if (_borderBottom != null)
				{
					_borderBottom.Write(output, "bottom");
				}
				if (_borderRight != null)
				{
					_borderRight.Write(output, "right");
				}
				output.Write("</w:tblBorders>");
			}
			if (_bgColor != null)
			{
				output.Write("<w:shd w:val=\"clear\" w:fill=\"");
				output.Write(_bgColor);
				output.Write("\"/>");
			}
			if (num2 || _bgColor != null)
			{
				output.Write("</w:tblPrEx>");
			}
		}
	}
}
