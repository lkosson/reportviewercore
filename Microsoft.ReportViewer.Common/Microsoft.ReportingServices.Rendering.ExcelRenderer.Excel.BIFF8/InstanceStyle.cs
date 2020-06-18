namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class InstanceStyle : StyleState
	{
		private BIFF8Style m_xf;

		private BIFF8Font m_font;

		private string m_format;

		private bool m_fontModified;

		public override ExcelBorderStyle BorderLeftStyle
		{
			set
			{
				m_xf.BorderLeftStyle = value;
			}
		}

		public override ExcelBorderStyle BorderRightStyle
		{
			set
			{
				m_xf.BorderRightStyle = value;
			}
		}

		public override ExcelBorderStyle BorderTopStyle
		{
			set
			{
				m_xf.BorderTopStyle = value;
			}
		}

		public override ExcelBorderStyle BorderBottomStyle
		{
			set
			{
				m_xf.BorderBottomStyle = value;
			}
		}

		public override ExcelBorderStyle BorderDiagStyle
		{
			set
			{
				m_xf.BorderDiagStyle = value;
			}
		}

		public override IColor BorderLeftColor
		{
			set
			{
				m_xf.BorderLeftColor = value;
			}
		}

		public override IColor BorderRightColor
		{
			set
			{
				m_xf.BorderRightColor = value;
			}
		}

		public override IColor BorderTopColor
		{
			set
			{
				m_xf.BorderTopColor = value;
			}
		}

		public override IColor BorderBottomColor
		{
			set
			{
				m_xf.BorderBottomColor = value;
			}
		}

		public override IColor BorderDiagColor
		{
			set
			{
				m_xf.BorderDiagColor = value;
			}
		}

		public override ExcelBorderPart BorderDiagPart
		{
			set
			{
				m_xf.BorderDiagPart = value;
			}
		}

		public override IColor BackgroundColor
		{
			set
			{
				m_xf.BackgroundColor = value;
			}
		}

		public override int IndentLevel
		{
			set
			{
				m_xf.IndentLevel = value;
			}
		}

		public override bool WrapText
		{
			set
			{
				m_xf.WrapText = value;
			}
		}

		public override int Orientation
		{
			set
			{
				m_xf.Orientation = value;
			}
		}

		public override string NumberFormat
		{
			get
			{
				InitFormat();
				return m_format;
			}
			set
			{
				InitFormat();
				if (!m_format.Equals(value))
				{
					m_format = value;
				}
			}
		}

		public override HorizontalAlignment HorizontalAlignment
		{
			set
			{
				m_xf.HorizontalAlignment = value;
			}
		}

		public override VerticalAlignment VerticalAlignment
		{
			set
			{
				m_xf.VerticalAlignment = value;
			}
		}

		public override TextDirection TextDirection
		{
			set
			{
				m_xf.TextDirection = value;
			}
		}

		public override int Bold
		{
			set
			{
				if (m_fontModified)
				{
					GetFont().Bold = value;
				}
				else if (GetFont().Bold != value)
				{
					CloneFont();
					m_font.Bold = value;
				}
			}
		}

		public override bool Italic
		{
			set
			{
				if (m_fontModified)
				{
					GetFont().Italic = value;
				}
				else if (GetFont().Italic != value)
				{
					CloneFont();
					m_font.Italic = value;
				}
			}
		}

		public override bool Strikethrough
		{
			set
			{
				if (m_fontModified)
				{
					GetFont().Strikethrough = value;
				}
				else if (GetFont().Strikethrough != value)
				{
					CloneFont();
					m_font.Strikethrough = value;
				}
			}
		}

		public override ScriptStyle ScriptStyle
		{
			set
			{
				if (m_fontModified)
				{
					GetFont().ScriptStyle = value;
				}
				else if (GetFont().ScriptStyle != value)
				{
					CloneFont();
					m_font.ScriptStyle = value;
				}
			}
		}

		public override IColor Color
		{
			set
			{
				int paletteIndex = ((BIFF8Color)value).PaletteIndex;
				if (m_fontModified)
				{
					GetFont().Color = paletteIndex;
				}
				else if (GetFont().Color != paletteIndex)
				{
					CloneFont();
					m_font.Color = paletteIndex;
				}
			}
		}

		public override Underline Underline
		{
			set
			{
				if (m_fontModified)
				{
					GetFont().Underline = value;
				}
				else if (GetFont().Underline != value)
				{
					CloneFont();
					m_font.Underline = value;
				}
			}
		}

		public override string Name
		{
			set
			{
				if (m_fontModified)
				{
					GetFont().Name = value;
				}
				else if (!GetFont().Name.Equals(value))
				{
					CloneFont();
					m_font.Name = value;
				}
			}
		}

		public override double Size
		{
			set
			{
				if (m_fontModified)
				{
					GetFont().Size = value;
				}
				else if (GetFont().Size != value)
				{
					CloneFont();
					m_font.Size = value;
				}
			}
		}

		internal InstanceStyle(StyleContainer parent, StyleProperties props)
			: base(parent)
		{
			if (props.Ixfe != 0)
			{
				m_xf = (BIFF8Style)m_context.GetStyle(props.Ixfe).Clone();
				return;
			}
			m_xf = new BIFF8Style(props);
			m_font = new BIFF8Font(props);
			m_fontModified = true;
			m_format = props.NumberFormat;
		}

		internal InstanceStyle(StyleContainer aParent)
			: base(aParent)
		{
			m_xf = new BIFF8Style();
		}

		internal override void Finished()
		{
			if (m_font != null)
			{
				m_xf.Ifnt = m_context.AddFont(m_font);
				m_font = null;
			}
			if (m_format != null)
			{
				m_xf.Ifmt = m_context.AddFormat(m_format);
				m_format = null;
			}
			m_context.CellIxfe = m_context.AddStyle(m_xf);
		}

		private BIFF8Font GetFont()
		{
			if (m_font == null)
			{
				if (m_xf.Ifnt != 0)
				{
					m_font = m_context.GetFont(m_xf.Ifnt);
				}
				else
				{
					m_font = new BIFF8Font();
				}
			}
			return m_font;
		}

		private void CloneFont()
		{
			m_font = (BIFF8Font)m_font.Clone();
			m_fontModified = true;
		}

		private void InitFormat()
		{
			if (m_format == null)
			{
				if (m_xf.Ifmt != 0)
				{
					m_format = m_context.GetFormat(m_xf.Ifmt);
				}
				else
				{
					m_format = string.Empty;
				}
			}
		}
	}
}
