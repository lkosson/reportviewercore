using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal abstract class DataRegion : ReportItem
	{
		internal new class Definition : DefinitionStore<DataRegion, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				Name,
				ActionInfo,
				Top,
				Left,
				Height,
				Width,
				ZIndex,
				Visibility,
				ToolTip,
				ToolTipLocID,
				DocumentMapLabel,
				DocumentMapLabelLocID,
				Bookmark,
				RepeatWith,
				CustomProperties,
				DataElementName,
				DataElementOutput,
				KeepTogether,
				NoRowsMessage,
				DataSetName,
				PageBreak,
				PageName,
				Filters,
				SortExpressions
			}

			private Definition()
			{
			}
		}

		[DefaultValue(false)]
		public bool KeepTogether
		{
			get
			{
				return base.PropertyStore.GetBoolean(18);
			}
			set
			{
				base.PropertyStore.SetBoolean(18, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression NoRowsMessage
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(19);
			}
			set
			{
				base.PropertyStore.SetObject(19, value);
			}
		}

		[DefaultValue("")]
		public string DataSetName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(20);
			}
			set
			{
				base.PropertyStore.SetObject(20, value);
			}
		}

		public PageBreak PageBreak
		{
			get
			{
				return (PageBreak)base.PropertyStore.GetObject(21);
			}
			set
			{
				base.PropertyStore.SetObject(21, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression PageName
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(22);
			}
			set
			{
				base.PropertyStore.SetObject(22, value);
			}
		}

		[XmlElement(typeof(RdlCollection<Filter>))]
		public IList<Filter> Filters
		{
			get
			{
				return (IList<Filter>)base.PropertyStore.GetObject(23);
			}
			set
			{
				base.PropertyStore.SetObject(23, value);
			}
		}

		[XmlElement(typeof(RdlCollection<SortExpression>))]
		public IList<SortExpression> SortExpressions
		{
			get
			{
				return (IList<SortExpression>)base.PropertyStore.GetObject(24);
			}
			set
			{
				base.PropertyStore.SetObject(24, value);
			}
		}

		public DataRegion()
		{
		}

		internal DataRegion(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			SortExpressions = new RdlCollection<SortExpression>();
			Filters = new RdlCollection<Filter>();
		}

		protected override void GetDependenciesCore(IList<ReportObject> dependencies)
		{
			base.GetDependenciesCore(dependencies);
			Report ancestor = GetAncestor<Report>();
			if (ancestor != null)
			{
				DataSet dataSetByName = ancestor.GetDataSetByName(DataSetName);
				if (dataSetByName != null && !dependencies.Contains(dataSetByName))
				{
					dependencies.Add(dataSetByName);
				}
			}
		}
	}
}
