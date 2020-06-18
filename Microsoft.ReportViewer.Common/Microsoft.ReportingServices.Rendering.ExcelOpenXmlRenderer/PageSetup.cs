using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System.Collections;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal class PageSetup
	{
		internal sealed class PageOrientation
		{
			private readonly string mName;

			private readonly int mValue;

			public static readonly PageOrientation Landscape;

			public static readonly PageOrientation Portrait;

			public int Value => mValue;

			static PageOrientation()
			{
				Landscape = new PageOrientation("Landscape", 1);
				Portrait = new PageOrientation("Portrait", 0);
			}

			private PageOrientation(string aName, int aValue)
			{
				mName = aName;
				mValue = aValue;
			}

			public override bool Equals(object aObject)
			{
				if (aObject is PageOrientation)
				{
					PageOrientation pageOrientation = (PageOrientation)aObject;
					return Value == pageOrientation.Value;
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

		internal sealed class PagePaperSize
		{
			private static readonly Hashtable mCollection;

			private readonly string mName;

			private readonly int mValue;

			public static readonly PagePaperSize A3;

			public static readonly PagePaperSize A4;

			public static readonly PagePaperSize A4Small;

			public static readonly PagePaperSize A5;

			public static readonly PagePaperSize B4;

			public static readonly PagePaperSize B5;

			public static readonly PagePaperSize CSheet;

			public static readonly PagePaperSize Default;

			public static readonly PagePaperSize DSheet;

			public static readonly PagePaperSize Envelope10;

			public static readonly PagePaperSize Envelope11;

			public static readonly PagePaperSize Envelope12;

			public static readonly PagePaperSize Envelope14;

			public static readonly PagePaperSize Envelope9;

			public static readonly PagePaperSize EnvelopeB4;

			public static readonly PagePaperSize EnvelopeB5;

			public static readonly PagePaperSize EnvelopeB6;

			public static readonly PagePaperSize EnvelopeC3;

			public static readonly PagePaperSize EnvelopeC4;

			public static readonly PagePaperSize EnvelopeC5;

			public static readonly PagePaperSize EnvelopeC6;

			public static readonly PagePaperSize EnvelopeC65;

			public static readonly PagePaperSize EnvelopeDL;

			public static readonly PagePaperSize EnvelopeItaly;

			public static readonly PagePaperSize EnvelopeMonarch;

			public static readonly PagePaperSize EnvelopePersonal;

			public static readonly PagePaperSize ESheet;

			public static readonly PagePaperSize Executive;

			public static readonly PagePaperSize FanfoldLegalGerman;

			public static readonly PagePaperSize FanfoldStdGerman;

			public static readonly PagePaperSize FanfoldUS;

			public static readonly PagePaperSize Folio;

			public static readonly PagePaperSize Ledger;

			public static readonly PagePaperSize Legal;

			public static readonly PagePaperSize Letter;

			public static readonly PagePaperSize LetterSmall;

			public static readonly PagePaperSize Note;

			public static readonly PagePaperSize Paper10x14;

			public static readonly PagePaperSize Paper11x17;

			public static readonly PagePaperSize Quarto;

			public static readonly PagePaperSize Statement;

			public static readonly PagePaperSize Tabloid;

			public static readonly PagePaperSize User;

			public int Value => mValue;

			static PagePaperSize()
			{
				mCollection = Hashtable.Synchronized(new Hashtable());
				A3 = new PagePaperSize("A3", 8);
				A4 = new PagePaperSize("A4", 9);
				A4Small = new PagePaperSize("A4 Small", 10);
				A5 = new PagePaperSize("A5", 11);
				B4 = new PagePaperSize("B4", 12);
				B5 = new PagePaperSize("B5", 13);
				CSheet = new PagePaperSize("C Sheet", 24);
				Default = new PagePaperSize("Default", 0);
				DSheet = new PagePaperSize("D Sheet", 25);
				Envelope10 = new PagePaperSize("Envelope 10", 20);
				Envelope11 = new PagePaperSize("Envelope 11", 21);
				Envelope12 = new PagePaperSize("Envelope 12", 22);
				Envelope14 = new PagePaperSize("Envelope 14", 23);
				Envelope9 = new PagePaperSize("Envelope 9", 19);
				EnvelopeB4 = new PagePaperSize("Envelope B4", 33);
				EnvelopeB5 = new PagePaperSize("Envelope B5", 34);
				EnvelopeB6 = new PagePaperSize("Envelope B6", 35);
				EnvelopeC3 = new PagePaperSize("Envelope C3", 29);
				EnvelopeC4 = new PagePaperSize("Envelope C4", 30);
				EnvelopeC5 = new PagePaperSize("Envelope C5", 28);
				EnvelopeC6 = new PagePaperSize("Envelope C6", 31);
				EnvelopeC65 = new PagePaperSize("Envelope C65", 32);
				EnvelopeDL = new PagePaperSize("Envelope DL", 27);
				EnvelopeItaly = new PagePaperSize("Envelope Italy", 36);
				EnvelopeMonarch = new PagePaperSize("Envelope Monarch", 37);
				EnvelopePersonal = new PagePaperSize("Envelope Personal", 38);
				ESheet = new PagePaperSize("E Sheet", 26);
				Executive = new PagePaperSize("Executive", 7);
				FanfoldLegalGerman = new PagePaperSize("Fanfold Legal German", 41);
				FanfoldStdGerman = new PagePaperSize("Fanfold Standard German", 40);
				FanfoldUS = new PagePaperSize("Fanfold US", 39);
				Folio = new PagePaperSize("Folio", 14);
				Ledger = new PagePaperSize("Ledger", 4);
				Legal = new PagePaperSize("Legal", 5);
				Letter = new PagePaperSize("Letter", 1);
				LetterSmall = new PagePaperSize("Letter Small", 2);
				Note = new PagePaperSize("Note", 18);
				Paper10x14 = new PagePaperSize("10x14", 16);
				Paper11x17 = new PagePaperSize("11x17", 17);
				Quarto = new PagePaperSize("Quarto", 15);
				Statement = new PagePaperSize("Statement", 6);
				Tabloid = new PagePaperSize("Tabloid", 3);
				User = new PagePaperSize("User", 256);
			}

			private PagePaperSize(string aName, int aValue)
			{
				mName = aName;
				mValue = aValue;
				mCollection[GetType().FullName + mValue] = this;
			}

			public override bool Equals(object aObject)
			{
				if (aObject is PagePaperSize)
				{
					PagePaperSize pagePaperSize = (PagePaperSize)aObject;
					return Value == pagePaperSize.Value;
				}
				return false;
			}

			public static PagePaperSize findByValue(int aValue)
			{
				switch (aValue)
				{
				case 0:
					return Default;
				case 1:
					return Letter;
				case 2:
					return LetterSmall;
				case 3:
					return Tabloid;
				case 4:
					return Ledger;
				case 5:
					return Legal;
				case 6:
					return Statement;
				case 7:
					return Executive;
				case 8:
					return A3;
				case 9:
					return A4;
				case 10:
					return A4Small;
				case 11:
					return A5;
				case 12:
					return B4;
				case 13:
					return B5;
				case 14:
					return Folio;
				case 15:
					return Quarto;
				case 16:
					return Paper10x14;
				case 17:
					return Paper11x17;
				case 18:
					return Note;
				case 19:
					return Envelope9;
				case 20:
					return Envelope10;
				case 21:
					return Envelope11;
				case 22:
					return Envelope12;
				case 23:
					return Envelope14;
				case 24:
					return CSheet;
				case 25:
					return DSheet;
				case 26:
					return ESheet;
				case 27:
					return EnvelopeDL;
				case 28:
					return EnvelopeC5;
				case 29:
					return EnvelopeC3;
				case 30:
					return EnvelopeC4;
				case 31:
					return EnvelopeC6;
				case 32:
					return EnvelopeC65;
				case 33:
					return EnvelopeB4;
				case 34:
					return EnvelopeB5;
				case 35:
					return EnvelopeB6;
				case 36:
					return EnvelopeItaly;
				case 37:
					return EnvelopeMonarch;
				case 38:
					return EnvelopePersonal;
				case 39:
					return FanfoldUS;
				case 40:
					return FanfoldStdGerman;
				case 41:
					return FanfoldLegalGerman;
				case 256:
					return User;
				default:
					throw new FatalException();
				}
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

		private readonly IPageSetupModel mModel;

		public double BottomMargin
		{
			set
			{
				mModel.BottomMargin = value;
			}
		}

		public string CenterFooter
		{
			set
			{
				mModel.CenterFooter = value;
			}
		}

		public string CenterHeader
		{
			set
			{
				mModel.CenterHeader = value;
			}
		}

		public double FooterMargin
		{
			set
			{
				mModel.FooterMargin = value;
			}
		}

		public double HeaderMargin
		{
			set
			{
				mModel.HeaderMargin = value;
			}
		}

		public string LeftFooter
		{
			set
			{
				mModel.LeftFooter = value;
			}
		}

		public string LeftHeader
		{
			set
			{
				mModel.LeftHeader = value;
			}
		}

		public double LeftMargin
		{
			set
			{
				mModel.LeftMargin = value;
			}
		}

		public PageOrientation Orientation
		{
			set
			{
				mModel.Orientation = value;
			}
		}

		public PagePaperSize PaperSize
		{
			set
			{
				mModel.PaperSize = value;
			}
		}

		public string RightFooter
		{
			set
			{
				mModel.RightFooter = value;
			}
		}

		public string RightHeader
		{
			set
			{
				mModel.RightHeader = value;
			}
		}

		public double RightMargin
		{
			set
			{
				mModel.RightMargin = value;
			}
		}

		public double TopMargin
		{
			set
			{
				mModel.TopMargin = value;
			}
		}

		public bool SummaryRowsBelow
		{
			set
			{
				mModel.SummaryRowsBelow = value;
			}
		}

		public bool SummaryColumnsRight
		{
			set
			{
				mModel.SummaryColumnsRight = value;
			}
		}

		internal PageSetup(IPageSetupModel model)
		{
			mModel = model;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is PageSetup))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((PageSetup)obj).mModel.Equals(mModel);
		}

		public override int GetHashCode()
		{
			return mModel.GetHashCode();
		}

		public void SetPrintTitleToRows(int firstRow, int lastRow)
		{
			mModel.SetPrintTitleToRows(firstRow, lastRow);
		}
	}
}
