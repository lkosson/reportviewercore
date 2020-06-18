using Microsoft.ReportingServices.OnDemandReportRendering;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class PaddItemSizes : ItemSizes
	{
		private double m_paddingRight;

		private double m_paddingBottom;

		internal override double PadWidth => m_width - m_paddingRight;

		internal override double PadHeight => m_height - m_paddingBottom;

		internal override double PaddingRight
		{
			get
			{
				return m_paddingRight;
			}
			set
			{
				m_paddingRight = value;
			}
		}

		internal override double PaddingBottom
		{
			get
			{
				return m_paddingBottom;
			}
			set
			{
				m_paddingBottom = value;
			}
		}

		internal PaddItemSizes()
		{
		}

		internal PaddItemSizes(ReportItem reportItem)
			: base(reportItem)
		{
		}

		internal PaddItemSizes(PaddItemSizes paddItemSizes)
			: base(paddItemSizes)
		{
			m_paddingRight = paddItemSizes.PaddingRight;
			m_paddingBottom = paddItemSizes.PaddingBottom;
		}

		internal PaddItemSizes(ItemSizes paddItemSizes)
			: base(paddItemSizes)
		{
		}

		internal PaddItemSizes(ReportSize width, ReportSize height, string id)
			: base(width, height, id)
		{
		}

		internal override ItemSizes GetNewItem()
		{
			return new PaddItemSizes(this)
			{
				DeltaY = m_deltaY
			};
		}

		internal override void Update(ReportItem reportItem)
		{
			Clean();
			base.Update(reportItem);
		}

		internal override void Update(ItemSizes paddItemSizes, bool returnPaddings)
		{
			Clean();
			base.Update(paddItemSizes, returnPaddings);
			if (returnPaddings)
			{
				PaddItemSizes paddItemSizes2 = paddItemSizes as PaddItemSizes;
				if (paddItemSizes2 != null)
				{
					m_paddingRight = paddItemSizes.PaddingRight;
					m_paddingBottom = paddItemSizes.PaddingBottom;
				}
			}
		}

		internal override void Update(ReportSize width, ReportSize height)
		{
			Clean();
			base.Update(width, height);
		}

		internal override void Clean()
		{
			base.Clean();
			m_paddingRight = 0.0;
			m_paddingBottom = 0.0;
		}

		internal override void SetPaddings(double right, double bottom)
		{
			m_paddingRight = right;
			m_paddingBottom = bottom;
			if (m_paddingRight < 0.0)
			{
				m_paddingRight = 0.0;
			}
			if (m_paddingBottom < 0.0)
			{
				m_paddingBottom = 0.0;
			}
		}

		internal override int ReadPaginationInfo(BinaryReader reader, long offsetEndPage)
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
			m_paddingBottom = reader.ReadDouble();
			m_paddingRight = reader.ReadDouble();
			if (reader.BaseStream.Position > offsetEndPage)
			{
				throw new InvalidDataException(SPBRes.InvalidPaginationStream);
			}
			return 0;
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)2);
				reportPageInfo.Write(m_deltaX);
				reportPageInfo.Write(m_deltaY);
				reportPageInfo.Write(m_top);
				reportPageInfo.Write(m_left);
				reportPageInfo.Write(m_height);
				reportPageInfo.Write(m_width);
				reportPageInfo.Write(m_paddingBottom);
				reportPageInfo.Write(m_paddingRight);
			}
		}

		internal override ItemSizes WritePaginationInfo()
		{
			return new PaddItemSizes(this)
			{
				DeltaY = m_deltaY,
				ID = null
			};
		}
	}
}
