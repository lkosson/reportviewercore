using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.docPropsVTypes
{
	internal class CT_Variant : OoxmlComplexType
	{
		public enum ChoiceBucket_0
		{
			variant,
			vector,
			array,
			blob,
			oblob,
			empty,
			_null,
			i1,
			i2,
			i4,
			i8,
			_int,
			ui1,
			ui2,
			ui4,
			ui8,
			_uint,
			r4,
			r8,
			_decimal,
			lpstr,
			lpwstr,
			bstr,
			date,
			filetime,
			_bool,
			cy,
			error,
			stream,
			ostream,
			storage,
			ostorage,
			vstream,
			clsid,
			cf
		}

		private CT_Variant _variant;

		private CT_Vector _vector;

		private string _blob;

		private string _oblob;

		private sbyte _i1;

		private short _i2;

		private int _i4;

		private long _i8;

		private int _int;

		private byte _ui1;

		private ushort _ui2;

		private uint _ui4;

		private ulong _ui8;

		private uint _uint;

		private double _r4;

		private double _r8;

		private double _decimal;

		private string _lpstr;

		private string _lpwstr;

		private string _bstr;

		private DateTime _date;

		private DateTime _filetime;

		private OoxmlBool _bool;

		private string _cy;

		private string _error;

		private string _stream;

		private string _ostream;

		private string _storage;

		private string _ostorage;

		private string _clsid;

		private ChoiceBucket_0 _choice_0;

		public CT_Variant Variant
		{
			get
			{
				return _variant;
			}
			set
			{
				_variant = value;
			}
		}

		public CT_Vector Vector
		{
			get
			{
				return _vector;
			}
			set
			{
				_vector = value;
			}
		}

		public string Blob
		{
			get
			{
				return _blob;
			}
			set
			{
				_blob = value;
			}
		}

		public string Oblob
		{
			get
			{
				return _oblob;
			}
			set
			{
				_oblob = value;
			}
		}

		public sbyte I1
		{
			get
			{
				return _i1;
			}
			set
			{
				_i1 = value;
			}
		}

		public short I2
		{
			get
			{
				return _i2;
			}
			set
			{
				_i2 = value;
			}
		}

		public int I4
		{
			get
			{
				return _i4;
			}
			set
			{
				_i4 = value;
			}
		}

		public long I8
		{
			get
			{
				return _i8;
			}
			set
			{
				_i8 = value;
			}
		}

		public int Int
		{
			get
			{
				return _int;
			}
			set
			{
				_int = value;
			}
		}

		public byte Ui1
		{
			get
			{
				return _ui1;
			}
			set
			{
				_ui1 = value;
			}
		}

		public ushort Ui2
		{
			get
			{
				return _ui2;
			}
			set
			{
				_ui2 = value;
			}
		}

		public uint Ui4
		{
			get
			{
				return _ui4;
			}
			set
			{
				_ui4 = value;
			}
		}

		public ulong Ui8
		{
			get
			{
				return _ui8;
			}
			set
			{
				_ui8 = value;
			}
		}

		public uint Uint
		{
			get
			{
				return _uint;
			}
			set
			{
				_uint = value;
			}
		}

		public double R4
		{
			get
			{
				return _r4;
			}
			set
			{
				_r4 = value;
			}
		}

		public double R8
		{
			get
			{
				return _r8;
			}
			set
			{
				_r8 = value;
			}
		}

		public double Decimal
		{
			get
			{
				return _decimal;
			}
			set
			{
				_decimal = value;
			}
		}

		public string Lpstr
		{
			get
			{
				return _lpstr;
			}
			set
			{
				_lpstr = value;
			}
		}

		public string Lpwstr
		{
			get
			{
				return _lpwstr;
			}
			set
			{
				_lpwstr = value;
			}
		}

		public string Bstr
		{
			get
			{
				return _bstr;
			}
			set
			{
				_bstr = value;
			}
		}

		public DateTime Date
		{
			get
			{
				return _date;
			}
			set
			{
				_date = value;
			}
		}

		public DateTime Filetime
		{
			get
			{
				return _filetime;
			}
			set
			{
				_filetime = value;
			}
		}

		public OoxmlBool Bool
		{
			get
			{
				return _bool;
			}
			set
			{
				_bool = value;
			}
		}

		public string Cy
		{
			get
			{
				return _cy;
			}
			set
			{
				_cy = value;
			}
		}

		public string Error
		{
			get
			{
				return _error;
			}
			set
			{
				_error = value;
			}
		}

		public string Stream
		{
			get
			{
				return _stream;
			}
			set
			{
				_stream = value;
			}
		}

		public string Ostream
		{
			get
			{
				return _ostream;
			}
			set
			{
				_ostream = value;
			}
		}

		public string Storage
		{
			get
			{
				return _storage;
			}
			set
			{
				_storage = value;
			}
		}

		public string Ostorage
		{
			get
			{
				return _ostorage;
			}
			set
			{
				_ostorage = value;
			}
		}

		public string Clsid
		{
			get
			{
				return _clsid;
			}
			set
			{
				_clsid = value;
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

		public static string VariantElementName => "variant";

		public static string VectorElementName => "vector";

		public static string I1ElementName => "i1";

		public static string I2ElementName => "i2";

		public static string I4ElementName => "i4";

		public static string I8ElementName => "i8";

		public static string IntElementName => "int";

		public static string Ui1ElementName => "ui1";

		public static string Ui2ElementName => "ui2";

		public static string Ui4ElementName => "ui4";

		public static string Ui8ElementName => "ui8";

		public static string UintElementName => "uint";

		public static string R4ElementName => "r4";

		public static string R8ElementName => "r8";

		public static string DecimalElementName => "decimal";

		public static string DateElementName => "date";

		public static string FiletimeElementName => "filetime";

		public static string BoolElementName => "bool";

		public static string BlobElementName => "blob";

		public static string OblobElementName => "oblob";

		public static string LpstrElementName => "lpstr";

		public static string LpwstrElementName => "lpwstr";

		public static string BstrElementName => "bstr";

		public static string CyElementName => "cy";

		public static string ErrorElementName => "error";

		public static string StreamElementName => "stream";

		public static string OstreamElementName => "ostream";

		public static string StorageElementName => "storage";

		public static string OstorageElementName => "ostorage";

		public static string ClsidElementName => "clsid";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_variant(s, depth, namespaces);
			Write_vector(s, depth, namespaces);
			Write_blob(s, depth, namespaces);
			Write_oblob(s, depth, namespaces);
			Write_i1(s, depth, namespaces);
			Write_i2(s, depth, namespaces);
			Write_i4(s, depth, namespaces);
			Write_i8(s, depth, namespaces);
			Write_int(s, depth, namespaces);
			Write_ui1(s, depth, namespaces);
			Write_ui2(s, depth, namespaces);
			Write_ui4(s, depth, namespaces);
			Write_ui8(s, depth, namespaces);
			Write_uint(s, depth, namespaces);
			Write_r4(s, depth, namespaces);
			Write_r8(s, depth, namespaces);
			Write_decimal(s, depth, namespaces);
			Write_lpstr(s, depth, namespaces);
			Write_lpwstr(s, depth, namespaces);
			Write_bstr(s, depth, namespaces);
			Write_date(s, depth, namespaces);
			Write_filetime(s, depth, namespaces);
			Write_bool(s, depth, namespaces);
			Write_cy(s, depth, namespaces);
			Write_error(s, depth, namespaces);
			Write_stream(s, depth, namespaces);
			Write_ostream(s, depth, namespaces);
			Write_storage(s, depth, namespaces);
			Write_ostorage(s, depth, namespaces);
			Write_clsid(s, depth, namespaces);
		}

		public void Write_variant(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.variant && _variant != null)
			{
				_variant.Write(s, "variant", depth + 1, namespaces);
			}
		}

		public void Write_vector(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.vector && _vector != null)
			{
				_vector.Write(s, "vector", depth + 1, namespaces);
			}
		}

		public void Write_i1(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.i1)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "i1", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _i1);
			}
		}

		public void Write_i2(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.i2)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "i2", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _i2);
			}
		}

		public void Write_i4(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.i4)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "i4", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _i4);
			}
		}

		public void Write_i8(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.i8)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "i8", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _i8);
			}
		}

		public void Write_int(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0._int)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "int", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _int);
			}
		}

		public void Write_ui1(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.ui1)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "ui1", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _ui1);
			}
		}

		public void Write_ui2(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.ui2)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "ui2", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _ui2);
			}
		}

		public void Write_ui4(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.ui4)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "ui4", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _ui4);
			}
		}

		public void Write_ui8(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.ui8)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "ui8", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _ui8);
			}
		}

		public void Write_uint(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0._uint)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "uint", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _uint);
			}
		}

		public void Write_r4(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.r4)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "r4", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _r4);
			}
		}

		public void Write_r8(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.r8)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "r8", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _r8);
			}
		}

		public void Write_decimal(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0._decimal)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "decimal", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _decimal);
			}
		}

		public void Write_date(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.date)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "date", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _date);
			}
		}

		public void Write_filetime(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.filetime)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "filetime", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _filetime);
			}
		}

		public void Write_bool(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0._bool)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "bool", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _bool);
			}
		}

		public void Write_blob(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.blob && _blob != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "blob", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _blob);
			}
		}

		public void Write_oblob(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.oblob && _oblob != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "oblob", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _oblob);
			}
		}

		public void Write_lpstr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.lpstr && _lpstr != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "lpstr", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _lpstr);
			}
		}

		public void Write_lpwstr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.lpwstr && _lpwstr != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "lpwstr", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _lpwstr);
			}
		}

		public void Write_bstr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.bstr && _bstr != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "bstr", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _bstr);
			}
		}

		public void Write_cy(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.cy && _cy != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "cy", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _cy);
			}
		}

		public void Write_error(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.error && _error != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "error", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _error);
			}
		}

		public void Write_stream(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.stream && _stream != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "stream", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _stream);
			}
		}

		public void Write_ostream(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.ostream && _ostream != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "ostream", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _ostream);
			}
		}

		public void Write_storage(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.storage && _storage != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "storage", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _storage);
			}
		}

		public void Write_ostorage(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.ostorage && _ostorage != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "ostorage", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _ostorage);
			}
		}

		public void Write_clsid(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.clsid && _clsid != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "clsid", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", _clsid);
			}
		}
	}
}
