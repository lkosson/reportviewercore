using System;
using System.Globalization;
using System.Xml;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

internal sealed class XmlDataVisitor : XmlVisitorBase, IXmlVisitor
{
	private const string XmlName = "Name";

	private const string XmlXmlnsXsi = "xsi";

	private const string XmlXSIURI = "http://www.w3.org/2001/XMLSchema-instance";

	private const string XmlXsiSchemaLocation = "schemaLocation";

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

	private string m_schemaName;

	private string m_schemaLocation;

	internal XmlDataVisitor(XmlWriter xw, string schemaName, string schemaLocation)
	{
		m_xw = xw;
		m_schemaName = schemaName;
		m_schemaLocation = schemaLocation;
	}

	public string getNameTag()
	{
		return "Name";
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

	public string getDefaultChartMemberTag(bool isCategory, int aCellIndex)
	{
		return string.Format(CultureInfo.InvariantCulture, isCategory ? "Category{0}" : "Series{0}", aCellIndex);
	}

	public string getDefaultChartCollectionTag(bool isCategory, int aIndex)
	{
		return string.Format(CultureInfo.InvariantCulture, isCategory ? "ChartCategory{0}_Collection" : "ChartSeries{0}_Collection", aIndex);
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

	public void StartReport(string name, bool firstInstance)
	{
		InternalStartElement(name, firstInstance);
	}

	public void StartRootReport(string name)
	{
		InternalStartElement(name, firstInstance: true);
		if (m_schemaName != null && m_schemaLocation != null)
		{
			WriteAttributeString("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance", string.Format(CultureInfo.CurrentCulture, "{0} {1}", m_schemaName, m_schemaLocation));
		}
	}

	public void StartReportSection(string name, bool firstInstance)
	{
		InternalStartElement(name, firstInstance);
	}

	public void StartDataRegion(string name, bool firstInstance, bool optional)
	{
		InternalStartElement(name, firstInstance);
	}

	public void StartGroup(string name, bool firstInstance)
	{
		InternalStartElement(name, firstInstance);
	}

	public void StartTablixMember(string name, bool isStatic, bool firstInstance)
	{
		InternalStartElement(name, firstInstance);
	}

	public void StartCollection(string name, bool firstInstance)
	{
		InternalStartElement(name, firstInstance);
	}

	public void StartCell(string name, bool firstInstance)
	{
		InternalStartElement(name, firstInstance);
	}

	public void StartRectangle(string name, bool firstInstance)
	{
		InternalStartElement(name, firstInstance);
	}

	public void StartChartMember(string name, bool isStatic, bool firstInstance)
	{
		InternalStartElement(name, firstInstance);
	}

	public void StartMapLayer(string name, bool firstInstance)
	{
		InternalStartElement(name, firstInstance);
	}

	public void StartChartDataPoint(string name, bool firstInstance)
	{
		InternalStartElement(name, firstInstance);
	}

	public void EndElement(bool firstInstance)
	{
		InternalEndElement(firstInstance);
	}

	public void ValueElement(string name, object val, TypeCode tc, bool firstInstance)
	{
		if (firstInstance)
		{
			ValidateElement(ref name);
		}
		if (val != null)
		{
			m_xw.WriteStartElement(name, m_schemaName);
			WriteString(XmlConvertOriginalValue(val));
			m_xw.WriteEndElement();
		}
	}

	public void ValueAttribute(string name, object val, TypeCode tc, bool firstInstance)
	{
		if (firstInstance)
		{
			ValidateAttribute(name);
		}
		if (val != null)
		{
			WriteAttributeString(name, XmlConvertOriginalValue(val));
		}
	}

	public void ValueElement(string name, object val, string ID, bool firstInstance)
	{
		ValueElement(name, val, TypeCode.Object, firstInstance);
	}

	public void ValueAttribute(string name, object val, string ID, bool firstInstance)
	{
		ValueAttribute(name, val, TypeCode.Object, firstInstance);
	}

	public RowCount Count(bool noRows)
	{
		if (!noRows)
		{
			return RowCount.More;
		}
		return RowCount.Zero;
	}

	public void Flush()
	{
		m_xw.Flush();
	}

	public string EncodeString(string aSource)
	{
		return XmlConvert.EncodeLocalName(aSource);
	}

	private void InternalStartElement(string name, bool firstInstance)
	{
		if (firstInstance)
		{
			ValidateElement(ref name);
			PushScope();
		}
		m_xw.WriteStartElement(name, m_schemaName);
	}

	private void InternalEndElement(bool firstInstance)
	{
		if (firstInstance)
		{
			PopScope();
		}
		m_xw.WriteEndElement();
	}

	private string XmlConvertOriginalValue(object val)
	{
		if (val is string)
		{
			return (string)val;
		}
		if (val is DateTime dateTime)
		{
			return XmlConvert.ToString(dateTimeOption: dateTime.Kind switch
			{
				DateTimeKind.Local => XmlDateTimeSerializationMode.Local, 
				DateTimeKind.Unspecified => XmlDateTimeSerializationMode.Unspecified, 
				DateTimeKind.Utc => XmlDateTimeSerializationMode.Utc, 
				_ => XmlDateTimeSerializationMode.Local, 
			}, value: dateTime);
		}
		if (val is TimeSpan)
		{
			return XmlConvert.ToString((TimeSpan)val);
		}
		if (val is IFormattable)
		{
			IFormattable formattable = (IFormattable)val;
			return formattable.ToString(null, CultureInfo.InvariantCulture);
		}
		return val.ToString();
	}
}
