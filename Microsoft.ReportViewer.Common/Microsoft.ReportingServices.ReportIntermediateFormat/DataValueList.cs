using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class DataValueList : ArrayList, IList<DataValue>, ICollection<DataValue>, IEnumerable<DataValue>, IEnumerable
	{
		internal new DataValue this[int index] => (DataValue)base[index];

		DataValue IList<DataValue>.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				base[index] = value;
			}
		}

		int ICollection<DataValue>.Count => Count;

		bool ICollection<DataValue>.IsReadOnly => IsReadOnly;

		public DataValueList()
		{
		}

		internal DataValueList(int capacity)
			: base(capacity)
		{
		}

		internal static string CreatePropertyNameString(string prefix, int rowIndex, int cellIndex, int valueIndex)
		{
			if (rowIndex > 0)
			{
				return prefix + "DataValue(Row:" + rowIndex + ")(Cell:" + cellIndex + ")(Index:" + valueIndex + ")";
			}
			return prefix + "CustomProperty(Index:" + valueIndex + ")";
		}

		internal void Initialize(string prefix, InitializationContext context)
		{
			Initialize(prefix, -1, -1, isCustomProperty: true, context);
		}

		internal void Initialize(string prefix, int rowIndex, int cellIndex, bool isCustomProperty, InitializationContext context)
		{
			int count = Count;
			Microsoft.ReportingServices.ReportPublishing.CustomPropertyUniqueNameValidator validator = new Microsoft.ReportingServices.ReportPublishing.CustomPropertyUniqueNameValidator();
			for (int i = 0; i < count; i++)
			{
				Global.Tracer.Assert(this[i] != null);
				this[i].Initialize(CreatePropertyNameString(prefix, rowIndex + 1, cellIndex + 1, i + 1), isCustomProperty, validator, context);
			}
		}

		internal void SetExprHost(IList<DataValueExprHost> dataValueHosts, ObjectModelImpl reportObjectModel)
		{
			if (dataValueHosts != null)
			{
				int count = Count;
				for (int i = 0; i < count; i++)
				{
					Global.Tracer.Assert(this[i] != null);
					this[i].SetExprHost(dataValueHosts, reportObjectModel);
				}
			}
		}

		int IList<DataValue>.IndexOf(DataValue item)
		{
			return IndexOf(item);
		}

		void IList<DataValue>.Insert(int index, DataValue item)
		{
			Insert(index, item);
		}

		void IList<DataValue>.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		void ICollection<DataValue>.Add(DataValue item)
		{
			Add(item);
		}

		void ICollection<DataValue>.Clear()
		{
			Clear();
		}

		bool ICollection<DataValue>.Contains(DataValue item)
		{
			return Contains(item);
		}

		void ICollection<DataValue>.CopyTo(DataValue[] array, int arrayIndex)
		{
			CopyTo(array, arrayIndex);
		}

		bool ICollection<DataValue>.Remove(DataValue item)
		{
			if (!Contains(item))
			{
				return false;
			}
			Remove(item);
			return true;
		}

		IEnumerator<DataValue> IEnumerable<DataValue>.GetEnumerator()
		{
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					yield return (DataValue)enumerator.Current;
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
