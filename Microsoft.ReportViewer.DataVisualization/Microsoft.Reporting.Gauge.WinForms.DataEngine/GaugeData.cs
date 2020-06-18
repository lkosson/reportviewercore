using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Reporting.Gauge.WinForms.DataEngine
{
	[Serializable]
	[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "2.0.0.0")]
	[DesignerCategory("code")]
	[ToolboxItem(false)]
	[XmlSchemaProvider("GetTypedDataSetSchema")]
	[XmlRoot("GaugeData")]
	[HelpKeyword("vs.data.DataSet")]
	internal class GaugeData : DataSet
	{
		public delegate void ValuesRowChangeEventHandler(object sender, ValuesRowChangeEvent e);

		[Serializable]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "2.0.0.0")]
		[XmlSchemaProvider("GetTypedTableSchema")]
		public class ValuesDataTable : DataTable, IEnumerable
		{
			private DataColumn columnDateStamp;

			private DataColumn columnValue;

			[DebuggerNonUserCode]
			public DataColumn DateStampColumn => columnDateStamp;

			[DebuggerNonUserCode]
			public DataColumn ValueColumn => columnValue;

			[DebuggerNonUserCode]
			[Browsable(false)]
			public int Count => base.Rows.Count;

			[DebuggerNonUserCode]
			public ValuesRow this[int index] => (ValuesRow)base.Rows[index];

			public event ValuesRowChangeEventHandler ValuesRowChanging;

			public event ValuesRowChangeEventHandler ValuesRowChanged;

			public event ValuesRowChangeEventHandler ValuesRowDeleting;

			public event ValuesRowChangeEventHandler ValuesRowDeleted;

			[DebuggerNonUserCode]
			public ValuesDataTable()
			{
				base.TableName = "Values";
				BeginInit();
				InitClass();
				EndInit();
			}

			[DebuggerNonUserCode]
			internal ValuesDataTable(DataTable table)
			{
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (table.Locale.ToString() != table.DataSet.Locale.ToString())
				{
					base.Locale = table.Locale;
				}
				if (table.Namespace != table.DataSet.Namespace)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[DebuggerNonUserCode]
			protected ValuesDataTable(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
				InitVars();
			}

			[DebuggerNonUserCode]
			public void AddValuesRow(ValuesRow row)
			{
				base.Rows.Add(row);
			}

			[DebuggerNonUserCode]
			public ValuesRow AddValuesRow(DateTime DateStamp, double Value)
			{
				ValuesRow valuesRow = (ValuesRow)NewRow();
				valuesRow.ItemArray = new object[2]
				{
					DateStamp,
					Value
				};
				base.Rows.Add(valuesRow);
				return valuesRow;
			}

			[DebuggerNonUserCode]
			public ValuesRow FindByDateStamp(DateTime DateStamp)
			{
				return (ValuesRow)base.Rows.Find(new object[1]
				{
					DateStamp
				});
			}

			[DebuggerNonUserCode]
			public virtual IEnumerator GetEnumerator()
			{
				return base.Rows.GetEnumerator();
			}

			[DebuggerNonUserCode]
			public override DataTable Clone()
			{
				ValuesDataTable obj = (ValuesDataTable)base.Clone();
				obj.InitVars();
				return obj;
			}

			[DebuggerNonUserCode]
			protected override DataTable CreateInstance()
			{
				return new ValuesDataTable();
			}

			[DebuggerNonUserCode]
			internal void InitVars()
			{
				columnDateStamp = base.Columns["DateStamp"];
				columnValue = base.Columns["Value"];
			}

			[DebuggerNonUserCode]
			private void InitClass()
			{
				columnDateStamp = new DataColumn("DateStamp", typeof(DateTime), null, MappingType.Element);
				base.Columns.Add(columnDateStamp);
				columnValue = new DataColumn("Value", typeof(double), null, MappingType.Element);
				base.Columns.Add(columnValue);
				base.Constraints.Add(new UniqueConstraint("GaugeDataKey", new DataColumn[1]
				{
					columnDateStamp
				}, isPrimaryKey: true));
				columnDateStamp.AllowDBNull = false;
				columnDateStamp.Unique = true;
			}

			[DebuggerNonUserCode]
			public ValuesRow NewValuesRow()
			{
				return (ValuesRow)NewRow();
			}

			[DebuggerNonUserCode]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new ValuesRow(builder);
			}

			[DebuggerNonUserCode]
			protected override Type GetRowType()
			{
				return typeof(ValuesRow);
			}

			[DebuggerNonUserCode]
			protected override void OnRowChanged(DataRowChangeEventArgs e)
			{
				base.OnRowChanged(e);
				if (this.ValuesRowChanged != null)
				{
					this.ValuesRowChanged(this, new ValuesRowChangeEvent((ValuesRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			protected override void OnRowChanging(DataRowChangeEventArgs e)
			{
				base.OnRowChanging(e);
				if (this.ValuesRowChanging != null)
				{
					this.ValuesRowChanging(this, new ValuesRowChangeEvent((ValuesRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			protected override void OnRowDeleted(DataRowChangeEventArgs e)
			{
				base.OnRowDeleted(e);
				if (this.ValuesRowDeleted != null)
				{
					this.ValuesRowDeleted(this, new ValuesRowChangeEvent((ValuesRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			protected override void OnRowDeleting(DataRowChangeEventArgs e)
			{
				base.OnRowDeleting(e);
				if (this.ValuesRowDeleting != null)
				{
					this.ValuesRowDeleting(this, new ValuesRowChangeEvent((ValuesRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			public void RemoveValuesRow(ValuesRow row)
			{
				base.Rows.Remove(row);
			}

			[DebuggerNonUserCode]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				GaugeData gaugeData = new GaugeData();
				xs.Add(gaugeData.GetSchemaSerializable());
				XmlSchemaAny item = new XmlSchemaAny
				{
					Namespace = "http://www.w3.org/2001/XMLSchema",
					MinOccurs = 0m,
					MaxOccurs = decimal.MaxValue,
					ProcessContents = XmlSchemaContentProcessing.Lax
				};
				xmlSchemaSequence.Items.Add(item);
				XmlSchemaAny item2 = new XmlSchemaAny
				{
					Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1",
					MinOccurs = 1m,
					ProcessContents = XmlSchemaContentProcessing.Lax
				};
				xmlSchemaSequence.Items.Add(item2);
				XmlSchemaAttribute item3 = new XmlSchemaAttribute
				{
					Name = "namespace",
					FixedValue = gaugeData.Namespace
				};
				xmlSchemaComplexType.Attributes.Add(item3);
				XmlSchemaAttribute item4 = new XmlSchemaAttribute
				{
					Name = "tableTypeName",
					FixedValue = "ValuesDataTable"
				};
				xmlSchemaComplexType.Attributes.Add(item4);
				xmlSchemaComplexType.Particle = xmlSchemaSequence;
				return xmlSchemaComplexType;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "2.0.0.0")]
		public class ValuesRow : DataRow
		{
			private ValuesDataTable tableValues;

			[DebuggerNonUserCode]
			public DateTime DateStamp
			{
				get
				{
					return (DateTime)base[tableValues.DateStampColumn];
				}
				set
				{
					base[tableValues.DateStampColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			public double Value
			{
				get
				{
					try
					{
						return (double)base[tableValues.ValueColumn];
					}
					catch (InvalidCastException innerException)
					{
						throw new StrongTypingException("The value for column 'Value' in table 'Values' is DBNull.", innerException);
					}
				}
				set
				{
					base[tableValues.ValueColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			internal ValuesRow(DataRowBuilder rb)
				: base(rb)
			{
				tableValues = (ValuesDataTable)base.Table;
			}

			[DebuggerNonUserCode]
			public bool IsValueNull()
			{
				return IsNull(tableValues.ValueColumn);
			}

			[DebuggerNonUserCode]
			public void SetValueNull()
			{
				base[tableValues.ValueColumn] = Convert.DBNull;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "2.0.0.0")]
		public class ValuesRowChangeEvent : EventArgs
		{
			private ValuesRow eventRow;

			private DataRowAction eventAction;

			[DebuggerNonUserCode]
			public ValuesRow Row => eventRow;

			[DebuggerNonUserCode]
			public DataRowAction Action => eventAction;

			[DebuggerNonUserCode]
			public ValuesRowChangeEvent(ValuesRow row, DataRowAction action)
			{
				eventRow = row;
				eventAction = action;
			}
		}

		private ValuesDataTable tableValues;

		private SchemaSerializationMode _schemaSerializationMode = SchemaSerializationMode.IncludeSchema;

		[DebuggerNonUserCode]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ValuesDataTable Values => tableValues;

		[DebuggerNonUserCode]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override SchemaSerializationMode SchemaSerializationMode
		{
			get
			{
				return _schemaSerializationMode;
			}
			set
			{
				_schemaSerializationMode = value;
			}
		}

		[DebuggerNonUserCode]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DataTableCollection Tables => base.Tables;

		[DebuggerNonUserCode]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DataRelationCollection Relations => base.Relations;

		[DebuggerNonUserCode]
		public GaugeData()
		{
			BeginInit();
			InitClass();
			CollectionChangeEventHandler value = SchemaChanged;
			base.Tables.CollectionChanged += value;
			base.Relations.CollectionChanged += value;
			EndInit();
		}

		[DebuggerNonUserCode]
		protected GaugeData(SerializationInfo info, StreamingContext context)
			: base(info, context, ConstructSchema: false)
		{
			if (IsBinarySerialized(info, context))
			{
				InitVars(initTable: false);
				CollectionChangeEventHandler value = SchemaChanged;
				Tables.CollectionChanged += value;
				Relations.CollectionChanged += value;
				return;
			}
			string s = (string)info.GetValue("XmlSchema", typeof(string));
			if (DetermineSchemaSerializationMode(info, context) == SchemaSerializationMode.IncludeSchema)
			{
				DataSet dataSet = new DataSet();
				dataSet.ReadXmlSchema(new XmlTextReader(new StringReader(s)));
				if (dataSet.Tables["Values"] != null)
				{
					base.Tables.Add(new ValuesDataTable(dataSet.Tables["Values"]));
				}
				base.DataSetName = dataSet.DataSetName;
				base.Prefix = dataSet.Prefix;
				base.Namespace = dataSet.Namespace;
				base.Locale = dataSet.Locale;
				base.CaseSensitive = dataSet.CaseSensitive;
				base.EnforceConstraints = dataSet.EnforceConstraints;
				Merge(dataSet, preserveChanges: false, MissingSchemaAction.Add);
				InitVars();
			}
			else
			{
				ReadXmlSchema(new XmlTextReader(new StringReader(s)));
			}
			GetSerializationData(info, context);
			CollectionChangeEventHandler value2 = SchemaChanged;
			base.Tables.CollectionChanged += value2;
			Relations.CollectionChanged += value2;
		}

		[DebuggerNonUserCode]
		protected override void InitializeDerivedDataSet()
		{
			BeginInit();
			InitClass();
			EndInit();
		}

		[DebuggerNonUserCode]
		public override DataSet Clone()
		{
			GaugeData obj = (GaugeData)base.Clone();
			obj.InitVars();
			obj.SchemaSerializationMode = SchemaSerializationMode;
			return obj;
		}

		[DebuggerNonUserCode]
		protected override bool ShouldSerializeTables()
		{
			return false;
		}

		[DebuggerNonUserCode]
		protected override bool ShouldSerializeRelations()
		{
			return false;
		}

		[DebuggerNonUserCode]
		protected override void ReadXmlSerializable(XmlReader reader)
		{
			if (DetermineSchemaSerializationMode(reader) == SchemaSerializationMode.IncludeSchema)
			{
				Reset();
				DataSet dataSet = new DataSet();
				dataSet.ReadXml(reader);
				if (dataSet.Tables["Values"] != null)
				{
					base.Tables.Add(new ValuesDataTable(dataSet.Tables["Values"]));
				}
				base.DataSetName = dataSet.DataSetName;
				base.Prefix = dataSet.Prefix;
				base.Namespace = dataSet.Namespace;
				base.Locale = dataSet.Locale;
				base.CaseSensitive = dataSet.CaseSensitive;
				base.EnforceConstraints = dataSet.EnforceConstraints;
				Merge(dataSet, preserveChanges: false, MissingSchemaAction.Add);
				InitVars();
			}
			else
			{
				ReadXml(reader);
				InitVars();
			}
		}

		[DebuggerNonUserCode]
		protected override XmlSchema GetSchemaSerializable()
		{
			MemoryStream memoryStream = new MemoryStream();
			WriteXmlSchema(new XmlTextWriter(memoryStream, null));
			memoryStream.Position = 0L;
			return XmlSchema.Read(new XmlTextReader(memoryStream), null);
		}

		[DebuggerNonUserCode]
		internal void InitVars()
		{
			InitVars(initTable: true);
		}

		[DebuggerNonUserCode]
		internal void InitVars(bool initTable)
		{
			tableValues = (ValuesDataTable)base.Tables["Values"];
			if (initTable && tableValues != null)
			{
				tableValues.InitVars();
			}
		}

		[DebuggerNonUserCode]
		private void InitClass()
		{
			base.DataSetName = "GaugeData";
			base.Prefix = "";
			base.Namespace = "http://tempuri.org/GaugeData.xsd";
			base.EnforceConstraints = true;
			SchemaSerializationMode = SchemaSerializationMode.IncludeSchema;
			tableValues = new ValuesDataTable();
			base.Tables.Add(tableValues);
		}

		[DebuggerNonUserCode]
		private bool ShouldSerializeValues()
		{
			return false;
		}

		[DebuggerNonUserCode]
		private void SchemaChanged(object sender, CollectionChangeEventArgs e)
		{
			if (e.Action == CollectionChangeAction.Remove)
			{
				InitVars();
			}
		}

		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedDataSetSchema(XmlSchemaSet xs)
		{
			GaugeData gaugeData = new GaugeData();
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			xs.Add(gaugeData.GetSchemaSerializable());
			XmlSchemaAny item = new XmlSchemaAny
			{
				Namespace = gaugeData.Namespace
			};
			xmlSchemaSequence.Items.Add(item);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			return xmlSchemaComplexType;
		}
	}
}
