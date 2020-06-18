using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal class ItemSizes : IStorable, IPersistable
	{
		private double m_deltaX;

		private double m_deltaY;

		protected double m_left;

		protected double m_top;

		protected double m_width;

		protected double m_height;

		private static Declaration m_declaration = GetDeclaration();

		internal double DeltaX
		{
			get
			{
				return m_deltaX;
			}
			set
			{
				m_deltaX = value;
			}
		}

		internal double DeltaY
		{
			get
			{
				return m_deltaY;
			}
			set
			{
				m_deltaY = value;
			}
		}

		internal virtual double Left
		{
			get
			{
				return m_left;
			}
			set
			{
				m_left = value;
			}
		}

		internal virtual double Top
		{
			get
			{
				return m_top;
			}
			set
			{
				m_top = value;
			}
		}

		internal double Bottom => m_top + m_height;

		internal double Right => m_left + m_width;

		internal virtual double Width
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

		internal virtual double Height
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

		public int Size => 48;

		internal ItemSizes()
		{
		}

		internal ItemSizes(ReportItem reportItem)
		{
			m_top = reportItem.Top.ToMillimeters();
			m_left = reportItem.Left.ToMillimeters();
			m_width = reportItem.Width.ToMillimeters();
			m_height = reportItem.Height.ToMillimeters();
		}

		internal ItemSizes(double left, double top, double width, double height)
		{
			m_top = top;
			m_left = left;
			m_width = width;
			m_height = height;
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DeltaX:
					writer.Write(m_deltaX);
					break;
				case MemberName.DeltaY:
					writer.Write(m_deltaY);
					break;
				case MemberName.Left:
					writer.Write(m_left);
					break;
				case MemberName.Top:
					writer.Write(m_top);
					break;
				case MemberName.Width:
					writer.Write(m_width);
					break;
				case MemberName.Height:
					writer.Write(m_height);
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DeltaX:
					m_deltaX = reader.ReadDouble();
					break;
				case MemberName.DeltaY:
					m_deltaY = reader.ReadDouble();
					break;
				case MemberName.Left:
					m_left = reader.ReadDouble();
					break;
				case MemberName.Top:
					m_top = reader.ReadDouble();
					break;
				case MemberName.Width:
					m_width = reader.ReadDouble();
					break;
				case MemberName.Height:
					m_height = reader.ReadDouble();
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public ObjectType GetObjectType()
		{
			return ObjectType.ItemSizes;
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.DeltaX, Token.Double));
				list.Add(new MemberInfo(MemberName.DeltaY, Token.Double));
				list.Add(new MemberInfo(MemberName.Left, Token.Double));
				list.Add(new MemberInfo(MemberName.Top, Token.Double));
				list.Add(new MemberInfo(MemberName.Width, Token.Double));
				list.Add(new MemberInfo(MemberName.Height, Token.Double));
				return new Declaration(ObjectType.ItemSizes, ObjectType.None, list);
			}
			return m_declaration;
		}

		internal virtual void AdjustHeightTo(double amount)
		{
			m_deltaY += amount - m_height;
			m_height = amount;
		}

		internal virtual void AdjustWidthTo(double amount)
		{
			m_deltaX += amount - m_width;
			m_width = amount;
		}

		internal virtual void MoveVertical(double delta)
		{
			m_top += delta;
			m_deltaY += delta;
		}

		internal virtual void MoveHorizontal(double delta)
		{
			m_left += delta;
			m_deltaX += delta;
		}
	}
}
