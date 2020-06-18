using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlSectionPropertiesModel
	{
		internal interface IHeaderFooterReferences
		{
			string Header
			{
				get;
			}

			string Footer
			{
				get;
			}

			string FirstPageHeader
			{
				get;
			}

			string FirstPageFooter
			{
				get;
			}
		}

		private CT_SectPr _sectionPr;

		internal float Height
		{
			set
			{
				PageSize().H_Attr = WordOpenXmlUtils.ToTwips(value, 144f, 31680f);
			}
		}

		internal float Width
		{
			set
			{
				PageSize().W_Attr = WordOpenXmlUtils.ToTwips(value, 144f, 31680f);
			}
		}

		internal bool IsLandscape
		{
			set
			{
				if (value)
				{
					PageSize().Orient_Attr = ST_Orientation.landscape;
				}
			}
		}

		internal float BottomMargin
		{
			set
			{
				PageMargins().Bottom_Attr = WordOpenXmlUtils.ToTwips(value, 0f, 31680f);
			}
		}

		internal float LeftMargin
		{
			set
			{
				PageMargins().Left_Attr = WordOpenXmlUtils.ToTwips(value, 0f, 31680f);
			}
		}

		internal float RightMargin
		{
			set
			{
				PageMargins().Right_Attr = WordOpenXmlUtils.ToTwips(value, 0f, 31680f);
			}
		}

		internal float TopMargin
		{
			set
			{
				PageMargins().Top_Attr = WordOpenXmlUtils.ToTwips(value, 0f, 31680f);
			}
		}

		public bool Continuous
		{
			set
			{
				if (value)
				{
					_sectionPr.Type = new CT_SectType
					{
						Val_Attr = ST_SectionMark.continuous
					};
				}
				else
				{
					_sectionPr.Type = null;
				}
			}
		}

		public bool HasTitlePage
		{
			set
			{
				if (value)
				{
					_sectionPr.TitlePg = new CT_OnOff();
				}
				else
				{
					_sectionPr.TitlePg = null;
				}
			}
		}

		public CT_SectPr CtSectPr => _sectionPr;

		internal OpenXmlSectionPropertiesModel()
		{
			_sectionPr = new CT_SectPr();
		}

		private CT_PageSz PageSize()
		{
			if (_sectionPr.PgSz == null)
			{
				_sectionPr.PgSz = new CT_PageSz();
			}
			return _sectionPr.PgSz;
		}

		private CT_PageMar PageMargins()
		{
			if (_sectionPr.PgMar == null)
			{
				_sectionPr.PgMar = new CT_PageMar();
			}
			return _sectionPr.PgMar;
		}

		internal void AddHeaderId(string value)
		{
			if (value != null)
			{
				_sectionPr.EG_HdrFtrReferencess.Add(new CT_HdrRef
				{
					Id_Attr = value,
					Type_Attr = ST_HdrFtr._default
				});
			}
		}

		internal void AddFirstPageHeaderId(string value)
		{
			if (value != null)
			{
				_sectionPr.EG_HdrFtrReferencess.Add(new CT_HdrRef
				{
					Id_Attr = value,
					Type_Attr = ST_HdrFtr.first
				});
			}
		}

		internal void AddFooterId(string value)
		{
			if (value != null)
			{
				_sectionPr.EG_HdrFtrReferencess.Add(new CT_FtrRef
				{
					Id_Attr = value,
					Type_Attr = ST_HdrFtr._default
				});
			}
		}

		internal void AddFirstPageFooterId(string value)
		{
			if (value != null)
			{
				_sectionPr.EG_HdrFtrReferencess.Add(new CT_FtrRef
				{
					Id_Attr = value,
					Type_Attr = ST_HdrFtr.first
				});
			}
		}

		public void SetHeaderFooterReferences(IHeaderFooterReferences headerFooterReferences)
		{
			AddHeaderId(headerFooterReferences.Header);
			AddFooterId(headerFooterReferences.Footer);
			AddFirstPageHeaderId(headerFooterReferences.FirstPageHeader);
			AddFirstPageFooterId(headerFooterReferences.FirstPageFooter);
		}

		public void ResetHeadersAndFooters()
		{
			_sectionPr.EG_HdrFtrReferencess.Clear();
		}

		public void WriteToBody(TextWriter writer, IHeaderFooterReferences headerFooterReferences)
		{
			SetHeaderFooterReferences(headerFooterReferences);
			_sectionPr.Write(writer, CT_Body.SectPrElementName);
			ResetHeadersAndFooters();
		}
	}
}
