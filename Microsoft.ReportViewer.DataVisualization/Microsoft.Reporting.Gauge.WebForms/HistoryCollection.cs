using System;
using System.Collections;
using System.Data;
using System.Globalization;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[Serializable]
	internal class HistoryCollection : CollectionBase, ICloneable
	{
		private long truncatedTicks;

		private double accumulatedValue;

		private ValueBase parent;

		public HistoryEntry this[int index]
		{
			get
			{
				return (HistoryEntry)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		internal HistoryEntry Top => this[base.Count - 1];

		internal double AccumulatedValue
		{
			get
			{
				if (base.Count > 0 && truncatedTicks > 0)
				{
					return accumulatedValue + this[0].Value * (double)(this[0].Timestamp.Ticks - truncatedTicks);
				}
				return accumulatedValue;
			}
		}

		public HistoryCollection(ValueBase parent)
		{
			this.parent = parent;
		}

		protected override void OnClear()
		{
			while (base.Count > 0)
			{
				RemoveAt(0);
			}
			base.OnClear();
		}

		protected override void OnRemove(int index, object value)
		{
			if (index == 0)
			{
				accumulatedValue += ((HistoryEntry)value).Value * (double)(((HistoryEntry)value).Timestamp.Ticks - truncatedTicks);
				truncatedTicks = ((HistoryEntry)value).Timestamp.Ticks;
			}
		}

		public void Add(DateTime timestamp, double value)
		{
			base.List.Add(new HistoryEntry(timestamp, value));
			if (base.Count > 1 && this[base.Count - 1].Timestamp < this[base.Count - 2].Timestamp)
			{
				lock (this)
				{
					base.InnerList.Sort();
				}
			}
		}

		public void LoadEntries(HistoryCollection sourceHistory)
		{
			if (sourceHistory == null)
			{
				throw new ApplicationException(Utils.SRGetStr("ExceptionHistoryCannotNull"));
			}
			if (parent != null)
			{
				parent.IntState = ValueState.DataLoading;
				parent.Reset();
				foreach (HistoryEntry item in sourceHistory)
				{
					parent.SetValueInternal(item.Value, item.Timestamp);
				}
				parent.IntState = ValueState.Interactive;
				parent.Invalidate();
				return;
			}
			foreach (HistoryEntry item2 in sourceHistory)
			{
				base.InnerList.Add(item2.Clone());
			}
		}

		internal void Truncate(GaugeDuration d)
		{
			if (d.IsInfinity)
			{
				return;
			}
			if (d.IsEmpty)
			{
				lock (this)
				{
					Clear();
				}
				return;
			}
			if (d.IsCountBased)
			{
				if (!((double)base.Count > d.Count))
				{
					return;
				}
				lock (this)
				{
					while ((double)base.Count > d.Count)
					{
						RemoveAt(0);
					}
				}
				return;
			}
			DateTime t = Top.Timestamp - d.ToTimeSpan();
			if (!(this[0].Timestamp < t))
			{
				return;
			}
			lock (this)
			{
				while (this[0].Timestamp < t)
				{
					RemoveAt(0);
				}
			}
		}

		internal int Locate(DateTime timestamp)
		{
			return SearchInternal(timestamp, exact: false);
		}

		internal int SearchInternal(DateTime timestamp, bool exact)
		{
			int num = base.InnerList.BinarySearch(new HistoryEntry(timestamp, 0.0));
			if (num < 0 && !exact)
			{
				num = ~num;
			}
			return Math.Max(-1, num);
		}

		internal HistoryEntry[] Select()
		{
			return Select(0);
		}

		internal HistoryEntry[] Select(DateTime fromDate, DateTime toDate)
		{
			return Select(Locate(fromDate), Locate(toDate));
		}

		internal HistoryEntry[] Select(int fromPoint)
		{
			return Select(fromPoint, base.Count);
		}

		internal HistoryEntry[] Select(int fromPoint, int toPoint)
		{
			if (base.Count == 0 || fromPoint < 0 || toPoint < fromPoint)
			{
				return new HistoryEntry[0];
			}
			lock (this)
			{
				HistoryEntry[] array = new HistoryEntry[Math.Min(base.Count, toPoint + 1) - fromPoint];
				base.InnerList.CopyTo(fromPoint, array, 0, array.Length);
				return array;
			}
		}

		internal HistoryEntry[] Select(GaugeDuration duration, DateTime currentDate)
		{
			if (!duration.IsInfinity)
			{
				if (duration.IsTimeBased)
				{
					DateTime fromDate = currentDate - duration.ToTimeSpan();
					return Select(fromDate, currentDate);
				}
				return Select((int)duration.Count);
			}
			return Select();
		}

		internal DataTable ToDataTable()
		{
			return ToDataTable(base.Count);
		}

		internal DataTable ToDataTable(DateTime toPoint)
		{
			return ToDataTable(base.Count - Locate(toPoint));
		}

		internal DataTable ToDataTable(int toPoint)
		{
			DataTable dataTable = new DataTable();
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add(new DataColumn("DateStamp", typeof(DateTime)));
			dataTable.Columns.Add(new DataColumn("Value", typeof(double)));
			lock (this)
			{
				toPoint = Math.Min(base.Count, toPoint);
				for (int i = base.Count - toPoint; i < base.Count; i++)
				{
					dataTable.Rows.Add(this[i].Timestamp, this[i].Value);
				}
				return dataTable;
			}
		}

		public object Clone()
		{
			HistoryCollection historyCollection = new HistoryCollection(parent);
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					HistoryEntry historyEntry = (HistoryEntry)enumerator.Current;
					historyCollection.InnerList.Add(historyEntry.Clone());
				}
				return historyCollection;
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

		public int Add(HistoryEntry value)
		{
			return base.List.Add(value);
		}

		public void Remove(HistoryEntry value)
		{
			base.List.Remove(value);
		}

		public bool Contains(HistoryEntry value)
		{
			return base.List.Contains(value);
		}

		public void Insert(int index, HistoryEntry value)
		{
			base.List.Insert(index, value);
		}

		public int IndexOf(HistoryEntry value)
		{
			return base.List.IndexOf(value);
		}
	}
}
