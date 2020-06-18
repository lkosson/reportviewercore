namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal abstract class StyleState : IStyle, IFont
	{
		protected StyleContainer m_context;

		public abstract ExcelBorderStyle BorderLeftStyle
		{
			set;
		}

		public abstract ExcelBorderStyle BorderRightStyle
		{
			set;
		}

		public abstract ExcelBorderStyle BorderTopStyle
		{
			set;
		}

		public abstract ExcelBorderStyle BorderBottomStyle
		{
			set;
		}

		public abstract ExcelBorderStyle BorderDiagStyle
		{
			set;
		}

		public abstract IColor BorderLeftColor
		{
			set;
		}

		public abstract IColor BorderRightColor
		{
			set;
		}

		public abstract IColor BorderTopColor
		{
			set;
		}

		public abstract IColor BorderBottomColor
		{
			set;
		}

		public abstract IColor BorderDiagColor
		{
			set;
		}

		public abstract ExcelBorderPart BorderDiagPart
		{
			set;
		}

		public abstract IColor BackgroundColor
		{
			set;
		}

		public abstract int IndentLevel
		{
			set;
		}

		public abstract bool WrapText
		{
			set;
		}

		public abstract int Orientation
		{
			set;
		}

		public abstract string NumberFormat
		{
			get;
			set;
		}

		public abstract HorizontalAlignment HorizontalAlignment
		{
			set;
		}

		public abstract VerticalAlignment VerticalAlignment
		{
			set;
		}

		public abstract TextDirection TextDirection
		{
			set;
		}

		public abstract int Bold
		{
			set;
		}

		public abstract bool Italic
		{
			set;
		}

		public abstract bool Strikethrough
		{
			set;
		}

		public abstract ScriptStyle ScriptStyle
		{
			set;
		}

		public abstract IColor Color
		{
			set;
		}

		public abstract Underline Underline
		{
			set;
		}

		public abstract string Name
		{
			set;
		}

		public abstract double Size
		{
			set;
		}

		protected StyleState(StyleContainer parent)
		{
			m_context = parent;
		}

		internal abstract void Finished();
	}
}
