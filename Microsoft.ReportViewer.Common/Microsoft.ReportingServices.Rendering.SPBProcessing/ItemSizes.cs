using Microsoft.ReportingServices.OnDemandReportRendering;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal class ItemSizes
	{
		protected double m_deltaX;

		protected double m_deltaY;

		protected double m_left;

		protected double m_top;

		protected double m_width;

		protected double m_height;

		protected string m_id;

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

		internal double Left
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

		internal double Top
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

		internal double Width
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

		internal double Height
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

		internal virtual double PadWidth => m_width;

		internal virtual double PadHeight => m_height;

		internal virtual double PaddingRight
		{
			get
			{
				return 0.0;
			}
			set
			{
			}
		}

		internal virtual double PaddingBottom
		{
			get
			{
				return 0.0;
			}
			set
			{
			}
		}

		internal string ID
		{
			get
			{
				return m_id;
			}
			set
			{
				m_id = value;
			}
		}

		internal ItemSizes()
		{
		}

		internal ItemSizes(ReportSize width, ReportSize height, string id)
		{
			m_width = width.ToMillimeters();
			m_height = height.ToMillimeters();
			m_id = id;
		}

		internal ItemSizes(ReportItem reportItem)
		{
			m_top = reportItem.Top.ToMillimeters();
			m_left = reportItem.Left.ToMillimeters();
			m_width = reportItem.Width.ToMillimeters();
			m_height = reportItem.Height.ToMillimeters();
			m_id = reportItem.ID;
		}

		internal ItemSizes(ItemSizes itemSizes)
		{
			m_top = itemSizes.Top;
			m_left = itemSizes.Left;
			m_width = itemSizes.Width;
			m_height = itemSizes.Height;
			m_deltaX = itemSizes.DeltaX;
			m_id = itemSizes.ID;
		}

		internal ItemSizes(double top, double left, string id)
		{
			m_top = top;
			m_left = left;
			m_id = id;
		}

		internal virtual ItemSizes GetNewItem()
		{
			return new ItemSizes(this)
			{
				DeltaY = m_deltaY
			};
		}

		internal virtual void Update(ReportSize width, ReportSize height)
		{
			Clean();
			m_width = width.ToMillimeters();
			m_height = height.ToMillimeters();
		}

		internal virtual void Update(ReportItem reportItem)
		{
			Clean();
			m_top = reportItem.Top.ToMillimeters();
			m_left = reportItem.Left.ToMillimeters();
			m_width = reportItem.Width.ToMillimeters();
			m_height = reportItem.Height.ToMillimeters();
		}

		internal virtual void Update(ItemSizes itemSizes, bool returnPaddings)
		{
			if (this != itemSizes)
			{
				Clean();
				m_top = itemSizes.Top;
				m_left = itemSizes.Left;
				m_width = itemSizes.Width;
				m_height = itemSizes.Height;
				m_deltaX = itemSizes.DeltaX;
			}
		}

		internal virtual void Update(double top, double left)
		{
			Clean();
			m_top = top;
			m_left = left;
		}

		internal virtual void Clean()
		{
			m_top = 0.0;
			m_left = 0.0;
			m_width = 0.0;
			m_height = 0.0;
			m_deltaX = 0.0;
			m_deltaY = 0.0;
		}

		internal void AdjustHeightTo(double amount)
		{
			m_deltaY += amount - m_height;
			m_height = amount;
		}

		internal void AdjustWidthTo(double amount)
		{
			m_deltaX += amount - m_width;
			m_width = amount;
		}

		internal void MoveVertical(double delta)
		{
			m_top += delta;
			m_deltaY += delta;
		}

		internal void MoveHorizontal(double delta)
		{
			m_left += delta;
			m_deltaX += delta;
		}

		internal void UpdateSizes(double topDelta, PageItem owner, PageItem[] siblings, RepeatWithItem[] repeatWithItems)
		{
			_ = owner.Source;
			m_left = owner.DefLeftValue;
			m_width = owner.SourceWidthInMM;
			m_deltaY = 0.0;
			m_deltaX = 0.0;
			m_top -= topDelta;
			if (m_top < 0.0)
			{
				if (owner.ItemState == PageItem.State.TopNextPage || owner.ItemState == PageItem.State.SpanPages)
				{
					m_deltaY = 0.0 - m_top;
					m_top = 0.0;
				}
				else if (owner.ItemState == PageItem.State.Below && !owner.HasItemsAbove(siblings, repeatWithItems))
				{
					m_deltaY = 0.0 - m_top;
					m_top = 0.0;
				}
			}
		}

		internal virtual void SetPaddings(double right, double bottom)
		{
		}

		internal virtual int ReadPaginationInfo(BinaryReader reader, long offsetEndPage)
		{
			if (reader == null || offsetEndPage <= 0)
			{
				return -1;
			}
			m_deltaX = reader.ReadDouble();
			m_deltaY = reader.ReadDouble();
			m_top = reader.ReadDouble();
			m_left = reader.ReadDouble();
			m_height = reader.ReadDouble();
			m_width = reader.ReadDouble();
			if (reader.BaseStream.Position > offsetEndPage)
			{
				throw new InvalidDataException(SPBRes.InvalidPaginationStream);
			}
			return 0;
		}

		internal virtual void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)1);
				reportPageInfo.Write(m_deltaX);
				reportPageInfo.Write(m_deltaY);
				reportPageInfo.Write(m_top);
				reportPageInfo.Write(m_left);
				reportPageInfo.Write(m_height);
				reportPageInfo.Write(m_width);
			}
		}

		internal virtual ItemSizes WritePaginationInfo()
		{
			return new ItemSizes(this)
			{
				DeltaY = m_deltaY,
				ID = null
			};
		}
	}
}
