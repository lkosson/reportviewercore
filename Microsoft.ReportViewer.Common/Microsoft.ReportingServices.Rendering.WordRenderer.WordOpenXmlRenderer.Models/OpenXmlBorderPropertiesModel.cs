using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal class OpenXmlBorderPropertiesModel : IStorable, IPersistable
	{
		internal enum BorderStyle : byte
		{
			Dashed,
			Dotted,
			Double,
			None,
			Solid
		}

		private int _widthInEighthPoints;

		private static Declaration _declaration;

		internal string Color
		{
			get;
			set;
		}

		internal BorderStyle Style
		{
			get;
			set;
		}

		internal int WidthInEighthPoints
		{
			get
			{
				return _widthInEighthPoints;
			}
			set
			{
				if (value < 0)
				{
					_widthInEighthPoints = 0;
				}
				else if (value > 255)
				{
					_widthInEighthPoints = 255;
				}
				else
				{
					_widthInEighthPoints = value;
				}
			}
		}

		public int Size => ItemSizes.SizeOf(Color) + 4 + 1;

		public OpenXmlBorderPropertiesModel()
		{
			Style = BorderStyle.None;
			_widthInEighthPoints = -1;
		}

		static OpenXmlBorderPropertiesModel()
		{
			_declaration = new Declaration(ObjectType.WordOpenXmlBorderProperties, ObjectType.None, new List<MemberInfo>
			{
				new MemberInfo(MemberName.Color, Token.String),
				new MemberInfo(MemberName.Width, Token.Int32),
				new MemberInfo(MemberName.Style, Token.Byte)
			});
		}

		private ulong ActualBorderSize()
		{
			if (Style == BorderStyle.Double)
			{
				return (ulong)WidthInEighthPoints / 2uL;
			}
			return (ulong)WidthInEighthPoints;
		}

		internal void Write(TextWriter writer, string tagName)
		{
			writer.Write("<w:");
			writer.Write(tagName);
			switch (Style)
			{
			case BorderStyle.Dashed:
				writer.Write(" w:val=\"dashSmallGap\"");
				break;
			case BorderStyle.Dotted:
				writer.Write(" w:val=\"dotted\"");
				break;
			case BorderStyle.Double:
				writer.Write(" w:val=\"double\"");
				break;
			case BorderStyle.None:
				writer.Write(" w:val=\"nil\"");
				break;
			case BorderStyle.Solid:
				writer.Write(" w:val=\"single\"");
				break;
			default:
				writer.Write(" w:val=\"none\"");
				break;
			}
			if (Color != null)
			{
				writer.Write(" w:color=\"");
				writer.Write(Color);
				writer.Write("\"");
			}
			if (WidthInEighthPoints != -1)
			{
				writer.Write(" w:sz=\"");
				writer.Write(ActualBorderSize().ToString(CultureInfo.InvariantCulture));
				writer.Write("\"");
			}
			writer.Write("/>");
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(GetDeclaration());
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Color:
					writer.Write(Color);
					break;
				case MemberName.Width:
					writer.Write(_widthInEighthPoints);
					break;
				case MemberName.Style:
					writer.Write((byte)Style);
					break;
				default:
					WordOpenXmlUtils.FailSerializable();
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(GetDeclaration());
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Color:
					Color = reader.ReadString();
					break;
				case MemberName.Width:
					_widthInEighthPoints = reader.ReadInt32();
					break;
				case MemberName.Style:
					Style = (BorderStyle)reader.ReadByte();
					break;
				default:
					WordOpenXmlUtils.FailSerializable();
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public ObjectType GetObjectType()
		{
			return ObjectType.WordOpenXmlBorderProperties;
		}

		internal static Declaration GetDeclaration()
		{
			return _declaration;
		}
	}
}
