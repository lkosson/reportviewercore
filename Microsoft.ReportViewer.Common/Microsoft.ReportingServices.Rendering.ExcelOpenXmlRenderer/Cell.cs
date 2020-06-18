using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Cell
	{
		internal sealed class CellValueType
		{
			private readonly int mValue;

			private readonly string mName;

			public static readonly CellValueType Blank;

			public static readonly CellValueType Boolean;

			public static readonly CellValueType Currency;

			public static readonly CellValueType Date;

			public static readonly CellValueType Double;

			public static readonly CellValueType Error;

			public static readonly CellValueType Integer;

			public static readonly CellValueType Text;

			public static readonly CellValueType Time;

			public int Value => mValue;

			static CellValueType()
			{
				Blank = new CellValueType("Blank", 4);
				Boolean = new CellValueType("Boolean", 5);
				Currency = new CellValueType("Currency", 7);
				Date = new CellValueType("Date", 3);
				Double = new CellValueType("Double", 2);
				Error = new CellValueType("Error", 6);
				Integer = new CellValueType("Integer", 1);
				Text = new CellValueType("Text", 0);
				Time = new CellValueType("Time", 8);
			}

			private CellValueType(string aName, int aValue)
			{
				mName = aName;
				mValue = aValue;
			}

			public override bool Equals(object aObject)
			{
				if (aObject is CellValueType)
				{
					CellValueType cellValueType = (CellValueType)aObject;
					return Value == cellValueType.Value;
				}
				return false;
			}

			public override int GetHashCode()
			{
				return Value;
			}

			public override string ToString()
			{
				return mName;
			}
		}

		private readonly ICellModel mModel;

		public string Name => mModel.Name;

		public Style Style
		{
			set
			{
				mModel.Style = value?.Model;
			}
		}

		public object Value
		{
			get
			{
				return mModel.Value;
			}
			set
			{
				mModel.Value = value;
			}
		}

		public CellValueType ValueType => mModel.ValueType;

		internal Cell(ICellModel model)
		{
			mModel = model;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Cell))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((Cell)obj).mModel.Equals(mModel);
		}

		public CharacterRun GetCharacters(int startIndex, int length)
		{
			return mModel.getCharacters(startIndex, length).Interface;
		}

		public override int GetHashCode()
		{
			return mModel.GetHashCode();
		}
	}
}
