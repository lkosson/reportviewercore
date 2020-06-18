using System;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class DataBindingRulesCollection : NamedCollection
	{
		private DataBindingRuleBase this[int index]
		{
			get
			{
				return (DataBindingRuleBase)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private DataBindingRuleBase this[string name]
		{
			get
			{
				return (DataBindingRuleBase)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public DataBindingRuleBase this[object obj]
		{
			get
			{
				if (obj is string)
				{
					return this[(string)obj];
				}
				if (obj is int)
				{
					return this[(int)obj];
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgument);
			}
			set
			{
				if (obj is string)
				{
					this[(string)obj] = value;
					return;
				}
				if (obj is int)
				{
					this[(int)obj] = value;
					return;
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgument);
			}
		}

		internal DataBindingRulesCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(DataBindingRuleBase);
		}

		public int Add(DataBindingRuleBase value)
		{
			return base.List.Add(value);
		}

		public void Remove(DataBindingRuleBase value)
		{
			base.List.Remove(value);
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return base.GetElementNameFormat(el).Replace("Data", "");
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return string.Format(CultureInfo.InvariantCulture, GetElementNameFormat(el), 1);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			DataBindingRuleBase dataBindingRuleBase = value as DataBindingRuleBase;
			if (dataBindingRuleBase != null && string.IsNullOrEmpty(dataBindingRuleBase.DataMember))
			{
				dataBindingRuleBase.DataMember = DataBindingHelper.GetDataSourceDefaultDataMember(dataBindingRuleBase.DataSource);
			}
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			base.OnSetComplete(index, oldValue, newValue);
			DataBindingRuleBase dataBindingRuleBase = newValue as DataBindingRuleBase;
			if (dataBindingRuleBase != null && string.IsNullOrEmpty(dataBindingRuleBase.DataMember))
			{
				dataBindingRuleBase.DataMember = DataBindingHelper.GetDataSourceDefaultDataMember(dataBindingRuleBase.DataSource);
			}
		}

		internal override void Invalidate()
		{
			if (base.Common != null)
			{
				base.Common.MapCore.InvalidateDataBinding();
			}
			base.Invalidate();
		}
	}
}
