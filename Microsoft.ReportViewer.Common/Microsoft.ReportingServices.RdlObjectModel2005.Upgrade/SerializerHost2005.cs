using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using Microsoft.ReportingServices.RdlObjectModel2008.Upgrade;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.RdlObjectModel2005.Upgrade
{
	internal class SerializerHost2005 : SerializerHost2008
	{
		private List<IUpgradeable> m_upgradeable;

		private List<DataSource2005> m_dataSources;

		private Hashtable m_nameTable;

		private static Type[,] m_substituteTypes = new Type[20, 2]
		{
			{
				typeof(Report),
				typeof(Report2005)
			},
			{
				typeof(Body),
				typeof(Body2005)
			},
			{
				typeof(Rectangle),
				typeof(Rectangle2005)
			},
			{
				typeof(Textbox),
				typeof(Textbox2005)
			},
			{
				typeof(Image),
				typeof(Image2005)
			},
			{
				typeof(Line),
				typeof(Line2005)
			},
			{
				typeof(Chart),
				typeof(Chart2005)
			},
			{
				typeof(CustomReportItem),
				typeof(CustomReportItem2005)
			},
			{
				typeof(Style),
				typeof(Style2005)
			},
			{
				typeof(BackgroundImage),
				typeof(BackgroundImage2005)
			},
			{
				typeof(Group),
				typeof(Grouping2005)
			},
			{
				typeof(SortExpression),
				typeof(SortBy2005)
			},
			{
				typeof(ChartDataPoint),
				typeof(DataPoint2005)
			},
			{
				typeof(CustomData),
				typeof(CustomData2005)
			},
			{
				typeof(DataHierarchy),
				typeof(DataGroupings2005)
			},
			{
				typeof(DataMember),
				typeof(DataGrouping2005)
			},
			{
				typeof(Subreport),
				typeof(Subreport2005)
			},
			{
				typeof(DataSource),
				typeof(DataSource2005)
			},
			{
				typeof(Query),
				typeof(Query2005)
			},
			{
				typeof(ReportParameter),
				typeof(ReportParameter2005)
			}
		};

		public List<IUpgradeable> Upgradeable
		{
			get
			{
				return m_upgradeable;
			}
			set
			{
				m_upgradeable = value;
			}
		}

		public List<DataSource2005> DataSources
		{
			get
			{
				return m_dataSources;
			}
			set
			{
				m_dataSources = value;
			}
		}

		public Hashtable NameTable
		{
			get
			{
				return m_nameTable;
			}
			set
			{
				m_nameTable = value;
			}
		}

		public override Type GetSubstituteType(Type type)
		{
			return GetSubstituteType(type, m_serializing);
		}

		public static Type GetSubstituteType(Type type, bool serializing)
		{
			if (!serializing)
			{
				if (type == typeof(Field))
				{
					return typeof(FieldEx);
				}
				for (int i = 0; i < m_substituteTypes.GetLength(0); i++)
				{
					if (type == m_substituteTypes[i, 0])
					{
						return m_substituteTypes[i, 1];
					}
				}
			}
			else
			{
				if (type.BaseType == typeof(Tablix))
				{
					return typeof(Tablix);
				}
				for (int j = 0; j < m_substituteTypes.GetLength(0); j++)
				{
					if (type == m_substituteTypes[j, 1])
					{
						return m_substituteTypes[j, 0];
					}
				}
			}
			return type;
		}

		public override void OnDeserialization(object value)
		{
			if (m_extraStringData != null)
			{
				if (value is IExpression)
				{
					((IExpression)value).Expression += m_extraStringData;
				}
				m_extraStringData = null;
			}
			if (m_nameTable != null && value is IGlobalNamedObject)
			{
				m_nameTable[((IGlobalNamedObject)value).Name] = value;
			}
			if (m_upgradeable != null)
			{
				if (m_dataSources != null && value is DataSource2005)
				{
					m_dataSources.Add((DataSource2005)value);
				}
				else if (value is IUpgradeable)
				{
					m_upgradeable.Add((IUpgradeable)value);
				}
			}
			base.OnDeserialization(value);
		}

		public override IEnumerable<ExtensionNamespace> GetExtensionNamespaces()
		{
			return new ExtensionNamespace[1]
			{
				new ExtensionNamespace("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")
			};
		}

		public SerializerHost2005(bool serializing)
			: base(serializing)
		{
		}
	}
}
