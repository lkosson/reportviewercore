using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.Rendering.DataRenderer;

public class AtomDataTypeVisitor : AtomDataFeedVisitor
{
	private List<TypeCode> m_currentTypeCodes;

	private List<string> m_columnEdmTypes;

	public List<string> ColumnEdmTypes => m_columnEdmTypes;

	public AtomDataTypeVisitor()
		: base(null, null, null, null)
	{
	}

	protected override void WriteDataRegionFeed(string feedName)
	{
		List<TypeCode> list = null;
		m_currentTypeCodes = new List<TypeCode>();
		IEnumerator enumerator = m_syndicationFeed.Items.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (list == null)
			{
				list = new List<TypeCode>(m_currentTypeCodes);
				continue;
			}
			for (int i = 0; i < m_currentTypeCodes.Count; i++)
			{
				TypeCode typeCode = m_currentTypeCodes[i];
				if (typeCode != 0)
				{
					if (list[i] == TypeCode.Empty)
					{
						list[i] = typeCode;
					}
					else
					{
						list[i] = ResolveTypeCode(list[i], typeCode);
					}
				}
			}
		}
		m_columnEdmTypes = null;
		if (list == null)
		{
			return;
		}
		m_columnEdmTypes = new List<string>(list.Count);
		foreach (TypeCode item in list)
		{
			m_typeCodeToEdmMapping.TryGetValue(item, out var value);
			m_columnEdmTypes.Add(value);
		}
	}

	private static TypeCode ResolveTypeCode(TypeCode typeCodeA, TypeCode typeCodeB)
	{
		if (typeCodeA == typeCodeB)
		{
			return typeCodeA;
		}
		if (typeCodeA == TypeCode.Object || typeCodeB == TypeCode.Object)
		{
			return TypeCode.Object;
		}
		if (typeCodeA == TypeCode.String || typeCodeB == TypeCode.String || typeCodeA == TypeCode.DateTime || typeCodeB == TypeCode.DateTime || typeCodeA == TypeCode.Boolean || typeCodeB == TypeCode.Boolean || typeCodeA == TypeCode.Char || typeCodeB == TypeCode.Char)
		{
			return TypeCode.String;
		}
		DataAggregate.DataTypeCode x = ConvertToDataTypeCode(typeCodeA);
		DataAggregate.DataTypeCode y = ConvertToDataTypeCode(typeCodeB);
		DataAggregate.DataTypeCode dataTypeCode = DataTypeUtility.CommonNumericDenominator(x, y);
		if (dataTypeCode == DataAggregate.DataTypeCode.Null)
		{
			return TypeCode.String;
		}
		return ConvertToTypeCode(dataTypeCode);
	}

	private static DataAggregate.DataTypeCode ConvertToDataTypeCode(TypeCode typeCode)
	{
		return typeCode switch
		{
			TypeCode.Int32 => DataAggregate.DataTypeCode.Int32, 
			TypeCode.Double => DataAggregate.DataTypeCode.Double, 
			TypeCode.UInt16 => DataAggregate.DataTypeCode.UInt16, 
			TypeCode.Int16 => DataAggregate.DataTypeCode.Int16, 
			TypeCode.Int64 => DataAggregate.DataTypeCode.Int64, 
			TypeCode.Decimal => DataAggregate.DataTypeCode.Decimal, 
			TypeCode.UInt32 => DataAggregate.DataTypeCode.UInt32, 
			TypeCode.UInt64 => DataAggregate.DataTypeCode.UInt64, 
			TypeCode.Byte => DataAggregate.DataTypeCode.Byte, 
			TypeCode.SByte => DataAggregate.DataTypeCode.SByte, 
			TypeCode.Single => DataAggregate.DataTypeCode.Single, 
			TypeCode.String => DataAggregate.DataTypeCode.String, 
			TypeCode.DateTime => DataAggregate.DataTypeCode.DateTime, 
			TypeCode.Char => DataAggregate.DataTypeCode.Char, 
			TypeCode.Boolean => DataAggregate.DataTypeCode.Boolean, 
			_ => DataAggregate.DataTypeCode.Null, 
		};
	}

	private static TypeCode ConvertToTypeCode(DataAggregate.DataTypeCode dataTypeCode)
	{
		return dataTypeCode switch
		{
			DataAggregate.DataTypeCode.Int32 => TypeCode.Int32, 
			DataAggregate.DataTypeCode.Double => TypeCode.Double, 
			DataAggregate.DataTypeCode.UInt16 => TypeCode.UInt16, 
			DataAggregate.DataTypeCode.Int16 => TypeCode.Int16, 
			DataAggregate.DataTypeCode.Int64 => TypeCode.Int64, 
			DataAggregate.DataTypeCode.Decimal => TypeCode.Decimal, 
			DataAggregate.DataTypeCode.UInt32 => TypeCode.UInt32, 
			DataAggregate.DataTypeCode.UInt64 => TypeCode.UInt64, 
			DataAggregate.DataTypeCode.Byte => TypeCode.Byte, 
			DataAggregate.DataTypeCode.SByte => TypeCode.SByte, 
			DataAggregate.DataTypeCode.Single => TypeCode.Single, 
			DataAggregate.DataTypeCode.String => TypeCode.String, 
			DataAggregate.DataTypeCode.DateTime => TypeCode.DateTime, 
			DataAggregate.DataTypeCode.Char => TypeCode.Char, 
			DataAggregate.DataTypeCode.Boolean => TypeCode.Boolean, 
			_ => TypeCode.Empty, 
		};
	}

	internal override void WriteValue(string value, TypeCode typeCode)
	{
		if (m_syndicationItem == null)
		{
			m_columnCount = 0;
			m_syndicationItem = new SyndicationItem();
		}
		m_columnCount++;
		if (value == null)
		{
			m_currentTypeCodes.Add(TypeCode.Empty);
		}
		else
		{
			m_currentTypeCodes.Add(typeCode);
		}
	}

	protected override void ResetRowState()
	{
		base.ResetRowState();
		m_currentTypeCodes.Clear();
	}
}
