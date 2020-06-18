using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing
{
	internal class CT_TwoCellAnchor : OoxmlComplexType
	{
		public enum ChoiceBucket_0
		{
			sp,
			grpSp,
			graphicFrame,
			cxnSp,
			pic,
			contentPart
		}

		private ST_EditAs _editAs_attr;

		private CT_Marker _from;

		private CT_Marker _to;

		private CT_Picture _pic;

		private CT_AnchorClientData _clientData;

		private ChoiceBucket_0 _choice_0;

		public ST_EditAs EditAs_Attr
		{
			get
			{
				return _editAs_attr;
			}
			set
			{
				_editAs_attr = value;
			}
		}

		public CT_Marker From
		{
			get
			{
				return _from;
			}
			set
			{
				_from = value;
			}
		}

		public CT_Marker To
		{
			get
			{
				return _to;
			}
			set
			{
				_to = value;
			}
		}

		public CT_Picture Pic
		{
			get
			{
				return _pic;
			}
			set
			{
				_pic = value;
			}
		}

		public CT_AnchorClientData ClientData
		{
			get
			{
				return _clientData;
			}
			set
			{
				_clientData = value;
			}
		}

		public ChoiceBucket_0 Choice_0
		{
			get
			{
				return _choice_0;
			}
			set
			{
				_choice_0 = value;
			}
		}

		public static string FromElementName => "from";

		public static string ToElementName => "to";

		public static string PicElementName => "pic";

		public static string ClientDataElementName => "clientData";

		protected override void InitAttributes()
		{
			_editAs_attr = ST_EditAs.twoCell;
		}

		protected override void InitElements()
		{
			_from = new CT_Marker();
			_to = new CT_Marker();
			_clientData = new CT_AnchorClientData();
		}

		protected override void InitCollections()
		{
		}

		public override void WriteAsRoot(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, depth, namespaces, root: true);
			WriteElements(s, depth, namespaces);
			WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void Write(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, depth, namespaces, root: false);
			WriteElements(s, depth, namespaces);
			WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces, bool root)
		{
			s.Write("<");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
			s.Write(tagName);
			WriteAttributes(s);
			if (root)
			{
				foreach (string key in namespaces.Keys)
				{
					s.Write(" xmlns");
					if (namespaces[key] != "")
					{
						s.Write(":");
						s.Write(namespaces[key]);
					}
					s.Write("=\"");
					s.Write(key);
					s.Write("\"");
				}
			}
			s.Write(">");
		}

		public override void WriteCloseTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			s.Write("</");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			if (_editAs_attr != ST_EditAs.twoCell)
			{
				s.Write(" editAs=\"");
				OoxmlComplexType.WriteData(s, _editAs_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_from(s, depth, namespaces);
			Write_to(s, depth, namespaces);
			Write_pic(s, depth, namespaces);
			Write_clientData(s, depth, namespaces);
		}

		public void Write_from(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_from != null)
			{
				_from.Write(s, "from", depth + 1, namespaces);
			}
		}

		public void Write_to(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_to != null)
			{
				_to.Write(s, "to", depth + 1, namespaces);
			}
		}

		public void Write_pic(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.pic && _pic != null)
			{
				_pic.Write(s, "pic", depth + 1, namespaces);
			}
		}

		public void Write_clientData(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_clientData != null)
			{
				_clientData.Write(s, "clientData", depth + 1, namespaces);
			}
		}
	}
}
