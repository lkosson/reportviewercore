namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class DefineSharedStyle : StyleState
	{
		private StyleProperties m_styleProps;

		private string m_id;

		public override ExcelBorderStyle BorderLeftStyle
		{
			set
			{
				m_styleProps.BorderLeftStyle = value;
			}
		}

		public override ExcelBorderStyle BorderRightStyle
		{
			set
			{
				m_styleProps.BorderRightStyle = value;
			}
		}

		public override ExcelBorderStyle BorderTopStyle
		{
			set
			{
				m_styleProps.BorderTopStyle = value;
			}
		}

		public override ExcelBorderStyle BorderBottomStyle
		{
			set
			{
				m_styleProps.BorderBottomStyle = value;
			}
		}

		public override ExcelBorderStyle BorderDiagStyle
		{
			set
			{
				m_styleProps.BorderDiagStyle = value;
			}
		}

		public override IColor BorderLeftColor
		{
			set
			{
				m_styleProps.BorderLeftColor = value;
			}
		}

		public override IColor BorderRightColor
		{
			set
			{
				m_styleProps.BorderRightColor = value;
			}
		}

		public override IColor BorderTopColor
		{
			set
			{
				m_styleProps.BorderTopColor = value;
			}
		}

		public override IColor BorderBottomColor
		{
			set
			{
				m_styleProps.BorderBottomColor = value;
			}
		}

		public override IColor BorderDiagColor
		{
			set
			{
				m_styleProps.BorderDiagColor = value;
			}
		}

		public override ExcelBorderPart BorderDiagPart
		{
			set
			{
				m_styleProps.BorderDiagPart = value;
			}
		}

		public override IColor BackgroundColor
		{
			set
			{
				m_styleProps.BackgroundColor = value;
			}
		}

		public override int IndentLevel
		{
			set
			{
				m_styleProps.IndentLevel = value;
			}
		}

		public override bool WrapText
		{
			set
			{
				m_styleProps.WrapText = value;
			}
		}

		public override int Orientation
		{
			set
			{
				m_styleProps.Orientation = value;
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
				m_styleProps.NumberFormat = value;
			}
		}

		public override HorizontalAlignment HorizontalAlignment
		{
			set
			{
				m_styleProps.HorizontalAlignment = value;
			}
		}

		public override VerticalAlignment VerticalAlignment
		{
			set
			{
				m_styleProps.VerticalAlignment = value;
			}
		}

		public override TextDirection TextDirection
		{
			set
			{
				m_styleProps.TextDirection = value;
			}
		}

		public override int Bold
		{
			set
			{
				m_styleProps.Bold = value;
			}
		}

		public override bool Italic
		{
			set
			{
				m_styleProps.Italic = value;
			}
		}

		public override bool Strikethrough
		{
			set
			{
				m_styleProps.Strikethrough = value;
			}
		}

		public override ScriptStyle ScriptStyle
		{
			set
			{
				m_styleProps.ScriptStyle = value;
			}
		}

		public override IColor Color
		{
			set
			{
				m_styleProps.Color = value;
			}
		}

		public override Underline Underline
		{
			set
			{
				m_styleProps.Underline = value;
			}
		}

		public override string Name
		{
			set
			{
				m_styleProps.Name = value;
			}
		}

		public override double Size
		{
			set
			{
				m_styleProps.Size = value;
			}
		}

		internal DefineSharedStyle(StyleContainer parent, string id)
			: base(parent)
		{
			m_styleProps = new StyleProperties();
			m_id = id;
		}

		internal override void Finished()
		{
			m_context.AddSharedStyle(m_id, m_styleProps);
		}
	}
}
