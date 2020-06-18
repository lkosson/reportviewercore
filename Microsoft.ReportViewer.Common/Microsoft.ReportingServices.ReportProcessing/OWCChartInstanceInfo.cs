using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class OWCChartInstanceInfo : ReportItemInstanceInfo
	{
		private VariantList[] m_chartData;

		private string m_noRows;

		internal VariantList this[int index]
		{
			get
			{
				if (0 <= index && index < m_chartData.Length)
				{
					return m_chartData[index];
				}
				throw new InvalidOperationException();
			}
		}

		internal VariantList[] ChartData
		{
			get
			{
				return m_chartData;
			}
			set
			{
				m_chartData = value;
			}
		}

		internal int Size
		{
			get
			{
				Global.Tracer.Assert(m_chartData.Length != 0);
				return m_chartData[0].Count;
			}
		}

		internal string NoRows
		{
			get
			{
				return m_noRows;
			}
			set
			{
				m_noRows = value;
			}
		}

		internal OWCChartInstanceInfo(ReportProcessing.ProcessingContext pc, OWCChart reportItemDef, OWCChartInstance owner)
			: base(pc, reportItemDef, owner, addToChunk: false)
		{
			m_chartData = new VariantList[reportItemDef.ChartData.Count];
			for (int i = 0; i < reportItemDef.ChartData.Count; i++)
			{
				m_chartData[i] = new VariantList();
			}
			m_noRows = pc.ReportRuntime.EvaluateDataRegionNoRowsExpression(reportItemDef, reportItemDef.ObjectType, reportItemDef.Name, "NoRows");
		}

		internal OWCChartInstanceInfo(ReportProcessing.ProcessingContext pc, OWCChart reportItemDef, OWCChartInstance owner, VariantList[] chartData)
			: base(pc, reportItemDef, owner, addToChunk: false)
		{
			m_chartData = chartData;
			m_noRows = pc.ReportRuntime.EvaluateDataRegionNoRowsExpression(reportItemDef, reportItemDef.ObjectType, reportItemDef.Name, "NoRows");
		}

		internal OWCChartInstanceInfo(OWCChart reportItemDef)
			: base(reportItemDef)
		{
		}

		internal void ChartDataXML(IChartStream chartStream)
		{
			OWCChart oWCChart = (OWCChart)m_reportItemDef;
			int count = m_chartData[0].Count;
			int num = 0;
			int num2 = 0;
			string value = string.Empty;
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlTextWriter.WriteStartElement("xml");
			xmlTextWriter.WriteAttributeString("xmlns", "s", null, "uuid:BDC6E3F0-6DA3-11d1-A2A3-00AA00C14882");
			xmlTextWriter.WriteAttributeString("xmlns", "dt", null, "uuid:C2F41010-65B3-11d1-A29F-00AA00C14882");
			xmlTextWriter.WriteAttributeString("xmlns", "rs", null, "urn:schemas-microsoft-com:rowset");
			xmlTextWriter.WriteAttributeString("xmlns", "z", null, "#RowsetSchema");
			xmlTextWriter.WriteStartElement("s", "Schema", null);
			xmlTextWriter.WriteAttributeString("id", "RowsetSchema");
			xmlTextWriter.WriteStartElement("s", "ElementType", null);
			xmlTextWriter.WriteAttributeString("name", "row");
			xmlTextWriter.WriteAttributeString("content", "eltOnly");
			for (num = 0; num < oWCChart.ChartData.Count; num++)
			{
				xmlTextWriter.WriteStartElement("s", "AttributeType", null);
				xmlTextWriter.WriteAttributeString("name", "c" + num.ToString(CultureInfo.InvariantCulture));
				xmlTextWriter.WriteAttributeString("rs", "name", null, oWCChart.ChartData[num].Name);
				xmlTextWriter.WriteAttributeString("rs", "nullable", null, "true");
				xmlTextWriter.WriteAttributeString("rs", "writeunknown", null, "true");
				xmlTextWriter.WriteStartElement("s", "datatype", null);
				for (num2 = 0; num2 < m_chartData[num].Count && m_chartData[num][num2] == null; num2++)
				{
				}
				if (num2 < m_chartData[num].Count)
				{
					switch (Type.GetTypeCode(m_chartData[num][num2].GetType()))
					{
					case TypeCode.Boolean:
						value = "boolean";
						break;
					case TypeCode.Byte:
						value = "ui1";
						break;
					case TypeCode.Char:
						value = "char";
						break;
					case TypeCode.DateTime:
						value = "dateTime";
						break;
					case TypeCode.Single:
						value = "r4";
						break;
					case TypeCode.Double:
						value = "float";
						break;
					case TypeCode.Decimal:
						value = "r8";
						break;
					case TypeCode.Int16:
						value = "i2";
						break;
					case TypeCode.Int32:
						value = "i4";
						break;
					case TypeCode.Int64:
						value = "i8";
						break;
					case TypeCode.Object:
						if (m_chartData[num][num2] is TimeSpan)
						{
							value = "time";
						}
						else if (m_chartData[num][num2] is byte[])
						{
							value = "bin.hex";
						}
						break;
					case TypeCode.SByte:
						value = "i1";
						break;
					case TypeCode.UInt16:
						value = "ui2";
						break;
					case TypeCode.UInt32:
						value = "ui4";
						break;
					case TypeCode.UInt64:
						value = "ui8";
						break;
					default:
						value = "string";
						break;
					}
				}
				else
				{
					value = "string";
				}
				xmlTextWriter.WriteAttributeString("dt", "type", null, value);
				xmlTextWriter.WriteAttributeString("rs", "fixedlength", null, "true");
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteEndElement();
			}
			xmlTextWriter.WriteStartElement("s", "extends", null);
			xmlTextWriter.WriteAttributeString("type", "rs:rowbase");
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.WriteStartElement("rs", "data", null);
			bool flag = true;
			object obj = null;
			for (int i = 0; i < count; i++)
			{
				for (num = 0; num < oWCChart.ChartData.Count; num++)
				{
					if (m_chartData[num][i] != null)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
				xmlTextWriter.WriteStartElement("z", "row", null);
				for (num = 0; num < oWCChart.ChartData.Count; num++)
				{
					obj = m_chartData[num][i];
					if (obj != null)
					{
						string value2 = (obj is IFormattable) ? ((IFormattable)obj).ToString(null, CultureInfo.InvariantCulture) : obj.ToString();
						xmlTextWriter.WriteAttributeString("c" + num.ToString(CultureInfo.InvariantCulture), value2);
					}
				}
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteWhitespace("\r\n");
				flag = true;
			}
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.WriteEndElement();
			chartStream.Write(stringWriter.ToString());
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ChartData, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.VariantList));
			memberInfoList.Add(new MemberInfo(MemberName.NoRows, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstanceInfo, memberInfoList);
		}
	}
}
