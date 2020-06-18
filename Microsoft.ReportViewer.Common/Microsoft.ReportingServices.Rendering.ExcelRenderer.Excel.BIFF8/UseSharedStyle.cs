namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class UseSharedStyle : StyleState
	{
		private StyleProperties m_styleProps;

		public override ExcelBorderStyle BorderLeftStyle
		{
			set
			{
				if (m_styleProps.BorderLeftStyle != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.BorderLeftStyle = value;
				}
			}
		}

		public override ExcelBorderStyle BorderRightStyle
		{
			set
			{
				if (m_styleProps.BorderRightStyle != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.BorderRightStyle = value;
				}
			}
		}

		public override ExcelBorderStyle BorderTopStyle
		{
			set
			{
				if (m_styleProps.BorderTopStyle != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.BorderTopStyle = value;
				}
			}
		}

		public override ExcelBorderStyle BorderBottomStyle
		{
			set
			{
				if (m_styleProps.BorderBottomStyle != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.BorderBottomStyle = value;
				}
			}
		}

		public override ExcelBorderStyle BorderDiagStyle
		{
			set
			{
				if (m_styleProps.BorderDiagStyle != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.BorderDiagStyle = value;
				}
			}
		}

		public override IColor BorderLeftColor
		{
			set
			{
				if (m_styleProps.BorderLeftColor != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.BorderLeftColor = value;
				}
			}
		}

		public override IColor BorderRightColor
		{
			set
			{
				if (m_styleProps.BorderRightColor != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.BorderRightColor = value;
				}
			}
		}

		public override IColor BorderTopColor
		{
			set
			{
				if (m_styleProps.BorderTopColor != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.BorderTopColor = value;
				}
			}
		}

		public override IColor BorderBottomColor
		{
			set
			{
				if (m_styleProps.BorderBottomColor != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.BorderBottomColor = value;
				}
			}
		}

		public override IColor BorderDiagColor
		{
			set
			{
				if (m_styleProps.BorderDiagColor != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.BorderDiagColor = value;
				}
			}
		}

		public override ExcelBorderPart BorderDiagPart
		{
			set
			{
				if (m_styleProps.BorderDiagPart != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.BorderDiagPart = value;
				}
			}
		}

		public override IColor BackgroundColor
		{
			set
			{
				if (m_styleProps.BackgroundColor != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.BackgroundColor = value;
				}
			}
		}

		public override int IndentLevel
		{
			set
			{
				if (m_styleProps.IndentLevel != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.IndentLevel = value;
				}
			}
		}

		public override bool WrapText
		{
			set
			{
				if (m_styleProps.WrapText != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.WrapText = value;
				}
			}
		}

		public override int Orientation
		{
			set
			{
				if (m_styleProps.Orientation != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.Orientation = value;
				}
			}
		}

		public override string NumberFormat
		{
			get
			{
				return m_styleProps.NumberFormat;
			}
			set
			{
				if (m_styleProps.NumberFormat != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.NumberFormat = value;
				}
			}
		}

		public override HorizontalAlignment HorizontalAlignment
		{
			set
			{
				if (m_styleProps.HorizontalAlignment != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.HorizontalAlignment = value;
				}
			}
		}

		public override VerticalAlignment VerticalAlignment
		{
			set
			{
				if (m_styleProps.VerticalAlignment != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.VerticalAlignment = value;
				}
			}
		}

		public override TextDirection TextDirection
		{
			set
			{
				if (m_styleProps.TextDirection != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.TextDirection = value;
				}
			}
		}

		public override int Bold
		{
			set
			{
				if (m_styleProps.Bold != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.Bold = value;
				}
			}
		}

		public override bool Italic
		{
			set
			{
				if (m_styleProps.Italic != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.Italic = value;
				}
			}
		}

		public override bool Strikethrough
		{
			set
			{
				if (m_styleProps.Strikethrough != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.Strikethrough = value;
				}
			}
		}

		public override ScriptStyle ScriptStyle
		{
			set
			{
				if (m_styleProps.ScriptStyle != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.ScriptStyle = value;
				}
			}
		}

		public override IColor Color
		{
			set
			{
				if (m_styleProps.Color != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.Color = value;
				}
			}
		}

		public override Underline Underline
		{
			set
			{
				if (m_styleProps.Underline != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.Underline = value;
				}
			}
		}

		public override string Name
		{
			set
			{
				if (m_styleProps.Name != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.Name = value;
				}
			}
		}

		public override double Size
		{
			set
			{
				if (m_styleProps.Size != value)
				{
					m_context.SetContext(new InstanceStyle(m_context, m_styleProps));
					m_context.Size = value;
				}
			}
		}

		internal UseSharedStyle(StyleContainer parent, StyleProperties props)
			: base(parent)
		{
			m_styleProps = props;
		}

		internal override void Finished()
		{
			if (m_styleProps.Ixfe != 0)
			{
				m_context.CellIxfe = m_styleProps.Ixfe;
				return;
			}
			m_styleProps.Ixfe = m_context.AddStyle(m_styleProps);
			m_context.CellIxfe = m_styleProps.Ixfe;
		}
	}
}
