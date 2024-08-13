using System;
using System.Collections;
using System.Globalization;
using System.Xml;
using Microsoft.ReportingServices.OnDemandReportRendering;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal sealed class XmlSchemaVisitor : XmlVisitorBase, IXmlVisitor
{
	private enum Cardinality
	{
		Zero,
		One,
		Unbounded
	}

	private const string XmlXSDURI = "http://www.w3.org/2001/XMLSchema";

	private const string XmlXSD = "xsd";

	private const string XmlSchema = "schema";

	private const string XmlTargetNamespace = "targetNamespace";

	private const string XmlElementFormDefault = "elementFormDefault";

	private const string XmlQualified = "qualified";

	private const string XmlElement = "element";

	private const string XmlAttribute = "attribute";

	private const string XmlName = "name";

	private const string XmlRdlNameAttribute = "Name";

	private const string XmlMinOccurs = "minOccurs";

	private const string XmlMaxOccurs = "maxOccurs";

	private const string XmlUnbounded = "unbounded";

	private const string XmlComplexType = "complexType";

	private const string XmlSequence = "sequence";

	private const string XmlType = "type";

	private const string XmlString = "xsd:string";

	private const string XmlBoolean = "xsd:boolean";

	private const string XmlDecimal = "xsd:decimal";

	private const string XmlFloat = "xsd:float";

	private const string XmlDouble = "xsd:double";

	private const string XmlDateTime = "xsd:dateTime";

	private const string XmlInteger = "xsd:integer";

	private const string XmlReporTag = "Report";

	private const string XmlRectangleTag = "Rectangle";

	private const string XmlSectionTag = "ReportSection";

	private const string XmlTablixTag = "Tablix";

	private const string XmlTablixCollectionColumnTag = "Column{0}_Collection";

	private const string XmlTablixCollectionRowTag = "Row{0}_Collection";

	private const string XmlTablixGroupDetalTag = "Detail";

	private const string XmlTablixCellTag = "Cell{0}";

	private const string XmlTablixStaticColumnTag = "Column{0}";

	private const string XmlTablixStaticRowTag = "Row{0}";

	private const string XmlTextBoxTag = "Textbox";

	private const string XmlSubReportTag = "Subreport";

	private const string XmlChartTag = "Chart";

	private const string XmlChartMemberCategoryTag = "Category{0}";

	private const string XmlChartMemberSeriesTag = "Series{0}";

	private const string XmlChartDataPointTag = "DataPoint";

	private const string XmlChartDataValueTag = "DataValue{0}";

	private const string XmlChartCollectionCategoryTag = "ChartCategory{0}_Collection";

	private const string XmlChartCollectionSeriesTag = "ChartSeries{0}_Collection";

	private const string XmlChartMemberLabelTag = "Label";

	private const string XmlGaugePanelTag = "GaugePanel";

	private const string XmlGaugeTag = "Gauge{0}";

	private const string XmlGaugeScaleCollectionTag = "GaugeScale_Collection";

	private const string XmlGaugeScaleTag = "GaugeScale{0}";

	private const string XmlGaugePointerCollectionTag = "GaugePointer_Collection";

	private const string XmlGaugePointerTag = "GaugePointer{0}";

	private const string XmlGaugeScaleRangeCollectionTag = "ScaleRange_Collection";

	private const string XmlGaugeScaleRangeTag = "ScaleRange{0}";

	private const string XmlGaugeInputValueTag = "GaugeInputValue";

	private const string XmlGaugeStartValueTag = "StartValue";

	private const string XmlGaugeEndValueTag = "EndValue";

	private const string XmlGaugeMinimumValueTag = "MinimumValue";

	private const string XmlGaugeMaximumValueTag = "MaximumValue";

	private Hashtable m_TextBoxTypes;

	private int m_elementLevel;

	private int m_complexTypeLevel;

	private int m_sequenceLevel;

	internal XmlSchemaVisitor(XmlWriter xw, string schemaName, Hashtable textBoxTypes)
	{
		m_xw = xw;
		m_TextBoxTypes = textBoxTypes;
		m_xw.WriteStartElement("xsd", "schema", "http://www.w3.org/2001/XMLSchema");
		WriteAttributeString("targetNamespace", schemaName);
		WriteAttributeString("xmlns", schemaName);
		WriteAttributeString("elementFormDefault", "qualified");
	}

	public string getNameTag()
	{
		return "name";
	}

	public string getXmlRdlNameTag()
	{
		return "Name";
	}

	public string getDefaultRectangleTag()
	{
		return "Rectangle";
	}

	public string getDefaultSectionTag()
	{
		return "ReportSection";
	}

	public string getDefaultReportTag()
	{
		return "Report";
	}

	public string getDefaultTablixTag()
	{
		return "Tablix";
	}

	public string getDefaultGroupCollectionTag(bool isColumn, int aCellIndex)
	{
		return string.Format(CultureInfo.InvariantCulture, isColumn ? "Column{0}_Collection" : "Row{0}_Collection", aCellIndex);
	}

	public string getDefaultGroupDetailTag()
	{
		return "Detail";
	}

	public string getDefaultCellTag(string aCellID)
	{
		return string.Format(CultureInfo.InvariantCulture, "Cell{0}", aCellID);
	}

	public string getDefaultTablixStaticMemberTag(bool isColumn, int aCellIndex)
	{
		return string.Format(CultureInfo.InvariantCulture, isColumn ? "Column{0}" : "Row{0}", aCellIndex);
	}

	public string getDefaultTextboxTag()
	{
		return "Textbox";
	}

	public string getDefaultSubreportTag()
	{
		return "Subreport";
	}

	public string getDefaultChartTag()
	{
		return "Chart";
	}

	public string getDefaultChartCollectionTag(bool isCategory, int aIndex)
	{
		return string.Format(CultureInfo.InvariantCulture, isCategory ? "ChartCategory{0}_Collection" : "ChartSeries{0}_Collection", aIndex);
	}

	public string getDefaultChartMemberTag(bool isCategory, int aCellIndex)
	{
		return string.Format(CultureInfo.InvariantCulture, isCategory ? "Category{0}" : "Series{0}", aCellIndex);
	}

	public string getDefaultChartDataPointTag()
	{
		return "DataPoint";
	}

	public string getChartDataValueTag(int aIndex)
	{
		return string.Format(CultureInfo.InvariantCulture, "DataValue{0}", aIndex);
	}

	public string getChartMemberLabelTag()
	{
		return "Label";
	}

	public string getDefaultGaugePanelTag()
	{
		return "GaugePanel";
	}

	public string getGaugeTag(int aGaugeIndex)
	{
		return string.Format(CultureInfo.InvariantCulture, "Gauge{0}", aGaugeIndex);
	}

	public string getGaugeScaleCollectionTag()
	{
		return "GaugeScale_Collection";
	}

	public string getGaugeScaleTag(int aGaugeScaleIndex)
	{
		return string.Format(CultureInfo.InvariantCulture, "GaugeScale{0}", aGaugeScaleIndex);
	}

	public string getGaugePointerCollectionTag()
	{
		return "GaugePointer_Collection";
	}

	public string getGaugePointerTag(int aGaugePointerIndex)
	{
		return string.Format(CultureInfo.InvariantCulture, "GaugePointer{0}", aGaugePointerIndex);
	}

	public string getDefaultGaugeInputValueTag()
	{
		return "GaugeInputValue";
	}

	public string getGaugeScaleRangeCollectionTag()
	{
		return "ScaleRange_Collection";
	}

	public string getGaugeScaleRangeTag(int aGaugeScaleRangeIndex)
	{
		return string.Format(CultureInfo.InvariantCulture, "ScaleRange{0}", aGaugeScaleRangeIndex);
	}

	public string getDefaultGaugeStartValueTag()
	{
		return "StartValue";
	}

	public string getDefaultGaugeEndValueTag()
	{
		return "EndValue";
	}

	public string getDefaultGaugeMinimumValueTag()
	{
		return "MinimumValue";
	}

	public string getDefaultGaugeMaximumValueTag()
	{
		return "MaximumValue";
	}

	public string EncodeString(string aSource)
	{
		return XmlConvert.EncodeLocalName(aSource);
	}

	public void StartReport(string name, bool firstInstance)
	{
		StartElementDeclaration(name, Cardinality.Zero, Cardinality.One);
	}

	public void StartRootReport(string name)
	{
		StartElementDeclaration(name, Cardinality.One, Cardinality.One);
	}

	public void StartReportSection(string name, bool firstInstance)
	{
		StartElementDeclaration(name, Cardinality.One, Cardinality.One);
	}

	public void StartDataRegion(string name, bool firstInstance, bool optional)
	{
		StartElementDeclaration(name, (!optional) ? Cardinality.One : Cardinality.Zero, Cardinality.One);
	}

	public void StartGroup(string name, bool firstInstance)
	{
		StartElementDeclaration(name, Cardinality.One, Cardinality.Unbounded);
	}

	public void StartTablixMember(string name, bool isStatic, bool firstInstance)
	{
		if (isStatic)
		{
			StartElementDeclaration(name, Cardinality.One, Cardinality.One);
		}
		else
		{
			StartElementDeclaration(name, Cardinality.Zero, Cardinality.Unbounded);
		}
	}

	public void StartCollection(string name, bool firstInstance)
	{
		StartElementDeclaration(name, Cardinality.Zero, Cardinality.One);
	}

	public void StartCell(string name, bool firstInstance)
	{
		StartElementDeclaration(name, Cardinality.One, Cardinality.One);
	}

	public void StartRectangle(string name, bool firstInstance)
	{
		StartElementDeclaration(name, Cardinality.One, Cardinality.One);
	}

	public void StartChartMember(string name, bool isStatic, bool firstInstance)
	{
		if (isStatic)
		{
			StartElementDeclaration(name, Cardinality.One, Cardinality.One);
		}
		else
		{
			StartElementDeclaration(name, Cardinality.One, Cardinality.Unbounded);
		}
	}

	public void StartChartDataPoint(string name, bool firstInstance)
	{
		StartElementDeclaration(name, Cardinality.One, Cardinality.One);
	}

	public void StartMapLayer(string name, bool firstInstance)
	{
		StartElementDeclaration(name, Cardinality.One, Cardinality.One);
	}

	public void EndElement(bool firstInstance)
	{
		EndElementDeclaration();
	}

	public void ValueElement(string name, object val, TypeCode tc, bool firstInstance)
	{
		StartElementDeclaration(name, Cardinality.Zero, Cardinality.One);
		WriteXmlType(tc);
		EndElementDeclaration();
	}

	public void ValueAttribute(string name, object val, TypeCode tc, bool firstInstance)
	{
		DeclareAttribute(name, tc);
	}

	public void ValueElement(string name, object val, string ID, bool firstInstance)
	{
		object obj = m_TextBoxTypes[ID];
		TypeCode tc = TypeCode.String;
		if (obj != null)
		{
			tc = (TypeCode)obj;
		}
		ValueElement(name, val, tc, firstInstance);
	}

	public void ValueAttribute(string name, object val, string ID, bool firstInstance)
	{
		object obj = m_TextBoxTypes[ID];
		TypeCode tc = TypeCode.String;
		if (obj != null)
		{
			tc = (TypeCode)obj;
		}
		ValueAttribute(name, val, tc, firstInstance);
	}

	public RowCount Count(bool noRows)
	{
		if (!noRows)
		{
			return RowCount.One;
		}
		return RowCount.Zero;
	}

	public void Flush()
	{
		m_xw.WriteEndElement();
		m_xw.Flush();
	}

	private string Cardinality2String(Cardinality cardinality)
	{
		return cardinality switch
		{
			Cardinality.Zero => "0", 
			Cardinality.One => null, 
			Cardinality.Unbounded => "unbounded", 
			_ => throw new ReportRenderingException(StringResources.rrUnknownCardinality), 
		};
	}

	private void StartElementDeclaration(string name, Cardinality minOccurs, Cardinality maxOccurs)
	{
		StartComplex();
		StartSequence();
		m_elementLevel++;
		ValidateElement(ref name);
		m_xw.WriteStartElement("xsd", "element", "http://www.w3.org/2001/XMLSchema");
		WriteAttributeString("name", name);
		if (minOccurs != Cardinality.One)
		{
			WriteAttributeString("minOccurs", Cardinality2String(minOccurs));
		}
		if (maxOccurs != Cardinality.One)
		{
			WriteAttributeString("maxOccurs", Cardinality2String(maxOccurs));
		}
	}

	private void EndElementDeclaration()
	{
		EndSequence();
		EndComplex();
		m_elementLevel--;
		m_xw.WriteEndElement();
	}

	private void DeclareAttribute(string name, TypeCode tc)
	{
		StartComplex();
		ValidateAttribute(name, tc);
	}

	private void WriteAttributes()
	{
		if (m_currentScope.Attributes.Count <= 0)
		{
			return;
		}
		foreach (string key in m_currentScope.Attributes.Keys)
		{
			m_xw.WriteStartElement("xsd", "attribute", "http://www.w3.org/2001/XMLSchema");
			WriteAttributeString("name", key);
			WriteXmlType((TypeCode)m_currentScope.Attributes[key]);
			m_xw.WriteEndElement();
		}
	}

	private void StartComplex()
	{
		if (m_elementLevel != m_complexTypeLevel)
		{
			m_complexTypeLevel++;
			m_xw.WriteStartElement("xsd", "complexType", "http://www.w3.org/2001/XMLSchema");
			PushScope();
		}
	}

	private void EndComplex()
	{
		if (m_elementLevel == m_complexTypeLevel)
		{
			WriteAttributes();
			m_xw.WriteEndElement();
			m_complexTypeLevel--;
			PopScope();
		}
	}

	private void StartSequence()
	{
		if (m_elementLevel != m_sequenceLevel)
		{
			m_sequenceLevel++;
			m_xw.WriteStartElement("xsd", "sequence", "http://www.w3.org/2001/XMLSchema");
		}
	}

	private void EndSequence()
	{
		if (m_elementLevel == m_sequenceLevel)
		{
			m_xw.WriteEndElement();
			m_sequenceLevel--;
		}
	}

	private void WriteXmlType(TypeCode tc)
	{
		switch (tc)
		{
		case TypeCode.String:
			WriteAttributeString("type", "xsd:string");
			break;
		case TypeCode.Decimal:
			WriteAttributeString("type", "xsd:decimal");
			break;
		case TypeCode.Int32:
			WriteAttributeString("type", "xsd:integer");
			break;
		case TypeCode.DateTime:
			WriteAttributeString("type", "xsd:dateTime");
			break;
		case TypeCode.Double:
			WriteAttributeString("type", "xsd:double");
			break;
		case TypeCode.Single:
			WriteAttributeString("type", "xsd:float");
			break;
		case TypeCode.Int16:
			WriteAttributeString("type", "xsd:integer");
			break;
		case TypeCode.Boolean:
			WriteAttributeString("type", "xsd:boolean");
			break;
		case TypeCode.Byte:
			WriteAttributeString("type", "xsd:integer");
			break;
		case TypeCode.SByte:
			WriteAttributeString("type", "xsd:integer");
			break;
		case TypeCode.Int64:
			WriteAttributeString("type", "xsd:integer");
			break;
		case TypeCode.UInt16:
			WriteAttributeString("type", "xsd:integer");
			break;
		case TypeCode.UInt32:
			WriteAttributeString("type", "xsd:integer");
			break;
		case TypeCode.UInt64:
			WriteAttributeString("type", "xsd:integer");
			break;
		case TypeCode.Char:
			WriteAttributeString("type", "xsd:string");
			break;
		default:
			WriteAttributeString("type", "xsd:string");
			break;
		}
	}
}
