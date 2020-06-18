using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;
using System.Timers;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[Bindable(true)]
	[TypeConverter(typeof(InputValueConverter))]
	internal class InputValue : ValueBase
	{
		private CalculatedValueCollection calculatedValues;

		private string dataMember = string.Empty;

		private string dateFieldMember = string.Empty;

		private string valueFieldMember = string.Empty;

		private InputValue playBackValue;

		private Timer playBackTimer;

		private int playBackMarker;

		private float speedMultiplier = 1f;

		private bool pointersInitialized;

		private bool paused;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CalculatedValueCollection CalculatedValues => calculatedValues;

		[SRCategory("CategoryData")]
		[Bindable(true)]
		[Browsable(true)]
		[DefaultValue(double.NaN)]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[SRDescription("DescriptionAttributeInputValue_Value")]
		public virtual double Value
		{
			get
			{
				return base.GetValue();
			}
			set
			{
				base.SetValueInternal(value);
			}
		}

		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[DefaultValue(0L)]
		[SRDescription("DescriptionAttributeInputValue_HistoryDepth")]
		public long HistoryDepth
		{
			get
			{
				return (long)HistoryDeptInternal.Count;
			}
			set
			{
				HistoryDeptInternal.Count = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[DefaultValue(DurationType.Count)]
		[SRDescription("DescriptionAttributeInputValue_HistoryDepthType")]
		public DurationType HistoryDepthType
		{
			get
			{
				return HistoryDeptInternal.DurationType;
			}
			set
			{
				HistoryDeptInternal.DurationType = value;
			}
		}

		[SRCategory("CategoryData")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeInputValue_DataMember")]
		[DefaultValue("")]
		[TypeConverter(typeof(DataMemberConverter))]
		public string DataMember
		{
			get
			{
				return dataMember;
			}
			set
			{
				if (dataMember != value)
				{
					dataMember = value;
					if (Common != null)
					{
						Common.GaugeCore.boundToDataSource = false;
					}
				}
			}
		}

		[SRCategory("CategoryData")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeDataSource")]
		[DefaultValue(null)]
		[TypeConverter(typeof(DataSourceConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal object DataSource
		{
			get
			{
				if (Common != null)
				{
					return Common.GaugeCore.DataSource;
				}
				return null;
			}
		}

		[SRCategory("CategoryData")]
		[Bindable(true)]
		[TypeConverter(typeof(DataFieldMemberConverter))]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeInputValue_DateFieldMember")]
		public string DateFieldMember
		{
			get
			{
				return dateFieldMember;
			}
			set
			{
				if (dateFieldMember != value)
				{
					dateFieldMember = value;
					if (Common != null)
					{
						Common.GaugeCore.boundToDataSource = false;
					}
				}
			}
		}

		[SRCategory("CategoryData")]
		[Bindable(true)]
		[TypeConverter(typeof(DataFieldMemberConverter))]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeInputValue_ValueFieldMember")]
		public string ValueFieldMember
		{
			get
			{
				return valueFieldMember;
			}
			set
			{
				if (valueFieldMember != value)
				{
					valueFieldMember = value;
					if (Common != null)
					{
						Common.GaugeCore.boundToDataSource = false;
					}
				}
			}
		}

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				base.Common = value;
				calculatedValues.Common = value;
			}
		}

		internal float SpeedMultiplier => speedMultiplier;

		public InputValue()
		{
			calculatedValues = new CalculatedValueCollection(this, null);
		}

		public void IncValue()
		{
			IncValue(1.0);
		}

		public void IncValue(double increment)
		{
			base.SetValueInternal(Value + increment);
		}

		public void SetValue(double value)
		{
			base.SetValueInternal(value, DateTime.Now);
		}

		public void SetValue(double value, DateTime timestamp)
		{
			base.SetValueInternal(value, timestamp);
		}

		private void InitTimer()
		{
			playBackTimer = new Timer();
			playBackTimer.Elapsed += OnPlayBackRefresh;
			playBackTimer.AutoReset = false;
			playBackTimer.Enabled = false;
		}

		private void OnPlayBackRefreshInternal()
		{
			lock (this)
			{
				if (IntState == ValueState.Interactive)
				{
					return;
				}
				if (playBackMarker >= playBackValue.History.Count)
				{
					goto IL_016e;
				}
				DateTime timestamp = playBackValue.History[playBackMarker].Timestamp;
				double value = playBackValue.History[playBackMarker].Value;
				playBackValue.outputValue = value;
				playBackValue.outputDate = timestamp;
				playBackValue.inputValue = value;
				playBackValue.inputDate = timestamp;
				if (!pointersInitialized)
				{
					playBackValue.RefreshConsumers();
					pointersInitialized = true;
				}
				playBackValue.OnValueChanged(new ValueChangedEventArgs(value, timestamp, playBackValue.Name, playbackMode: true));
				if (paused)
				{
					return;
				}
				while (++playBackMarker < playBackValue.History.Count)
				{
					TimeSpan timeSpan = playBackValue.History[playBackMarker].Timestamp - timestamp;
					if (timeSpan.TotalMilliseconds != 0.0)
					{
						playBackTimer.Interval = (int)(timeSpan.TotalMilliseconds / (double)playBackValue.speedMultiplier);
						playBackTimer.Start();
						return;
					}
				}
				goto IL_016e;
				IL_016e:
				PlaybackComplete();
			}
		}

		private void OnPlayBackRefresh(object sender, ElapsedEventArgs e)
		{
			playBackTimer.Stop();
			OnPlayBackRefreshInternal();
		}

		private void PlaybackReverse(float speedMultiplier, int numberOfRecords)
		{
			lock (this)
			{
				if (provderState == ValueState.Suspended && playBackValue != null)
				{
					Stop();
				}
				playBackMarker = numberOfRecords;
				IntState = ValueState.Suspended;
				paused = false;
				playBackValue = (InputValue)Clone();
				pointersInitialized = false;
				playBackValue.IntState = ValueState.Playback;
				playBackValue.speedMultiplier = speedMultiplier;
				foreach (IValueConsumer consumer in playBackValue.consumers)
				{
					consumer.Reset();
				}
				if (playBackTimer == null)
				{
					InitTimer();
				}
				if (Common != null && Common.GaugeContainer != null)
				{
					Common.GaugeContainer.OnPlaybackStateChanged(this, new PlaybackStateChangedEventArgs(PlaybackState.Started, playBackValue.inputValue, playBackValue.inputDate, Name));
				}
				OnPlayBackRefreshInternal();
			}
		}

		public void Playback(float speedMultiplier)
		{
			if (base.History.Count == 0)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionPlaybackDataEmpty"));
			}
			if (!HistoryDeptInternal.IsTimeBased)
			{
				PlaybackReverse(speedMultiplier, 0);
			}
			else
			{
				PlaybackReverse(speedMultiplier, base.History.Locate(base.History.Top.Timestamp - HistoryDeptInternal.ToTimeSpan()));
			}
		}

		public void Playback(float speedMultiplier, DateTime startTime)
		{
			if (base.History.Count == 0)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionPlaybackDataEmpty"));
			}
			PlaybackReverse(speedMultiplier, base.History.Locate(startTime));
		}

		public void Playback(float speedMultiplier, int numberOfRecords)
		{
			if (base.History.Count == 0)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionPlaybackDataEmpty"));
			}
			PlaybackReverse(speedMultiplier, Math.Max(0, base.History.Count - numberOfRecords));
		}

		public void Stop()
		{
			lock (this)
			{
				if (provderState != 0)
				{
					return;
				}
				IntState = ValueState.Interactive;
				if (playBackValue != null)
				{
					if (Common != null && Common.GaugeContainer != null)
					{
						Common.GaugeContainer.OnPlaybackStateChanged(this, new PlaybackStateChangedEventArgs(PlaybackState.Stopped, playBackValue.inputValue, playBackValue.inputDate, Name));
					}
					playBackValue.Dispose();
					playBackValue = null;
					RefreshConsumers();
				}
			}
		}

		public void Pause()
		{
			if (IntState == ValueState.Suspended)
			{
				paused = true;
				if (playBackValue != null && Common != null && Common.GaugeContainer != null)
				{
					Common.GaugeContainer.OnPlaybackStateChanged(this, new PlaybackStateChangedEventArgs(PlaybackState.Paused, playBackValue.inputValue, playBackValue.inputDate, Name));
				}
			}
		}

		public void Resume()
		{
			if (IntState == ValueState.Suspended)
			{
				paused = false;
				if (playBackValue != null && Common != null && Common.GaugeContainer != null)
				{
					Common.GaugeContainer.OnPlaybackStateChanged(this, new PlaybackStateChangedEventArgs(PlaybackState.Resumed, playBackValue.inputValue, playBackValue.inputDate, Name));
				}
				OnPlayBackRefreshInternal();
			}
		}

		public double GetPlaybackValue()
		{
			if (playBackValue == null)
			{
				return double.NaN;
			}
			return playBackValue.Value;
		}

		public DateTime GetPlaybackTimestamp()
		{
			if (playBackValue == null)
			{
				return DateTime.MaxValue;
			}
			return playBackValue.Date;
		}

		public bool IsPlaybackMode()
		{
			if (playBackValue == null)
			{
				return false;
			}
			return true;
		}

		public DataTable GetData()
		{
			return base.History.ToDataTable();
		}

		public DataTable GetData(int toPoint)
		{
			return base.History.ToDataTable(toPoint);
		}

		public DataTable GetData(DateTime toPoint)
		{
			return base.History.ToDataTable(toPoint);
		}

		public void DataBind()
		{
			if (DataSource != null)
			{
				DataBind(DataSource, ValueFieldMember, DateFieldMember, DataMember);
			}
		}

		public void DataBind(object dataSource, string valueFieldName, string dateFieldName)
		{
			DataBind(dataSource, valueFieldName, dateFieldName, string.Empty);
		}

		public void DataBind(object dataSource, string valueFieldName, string dateFieldName, string dataMember)
		{
			if (IntState != ValueState.Interactive)
			{
				throw new ApplicationException(Utils.SRGetStr("ExceptionDatabindState"));
			}
			bool closeDataReader;
			IDbConnection connection;
			object dataSourceAsIEnumerable = GetDataSourceAsIEnumerable(dataSource, dataMember, out closeDataReader, out connection);
			try
			{
				if (dataSource == null || valueFieldName == string.Empty)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionBadDatasource_fields"));
				}
				object obj = null;
				object obj2 = null;
				double num = 0.0;
				DateTime now = DateTime.Now;
				HistoryCollection historyCollection = new HistoryCollection(this);
				try
				{
					IntState = ValueState.DataLoading;
					Reset();
					foreach (object item in (IEnumerable)dataSourceAsIEnumerable)
					{
						obj = ConvertEnumerationItem(item, valueFieldName);
						if (dateFieldName != string.Empty)
						{
							obj2 = ConvertEnumerationItem(item, dateFieldName);
							if (obj != null && obj2 != null && obj2 != Convert.DBNull)
							{
								num = ((obj != Convert.DBNull) ? Convert.ToDouble(obj, CultureInfo.InvariantCulture) : double.NaN);
								DateTime timestamp = Convert.ToDateTime(obj2, CultureInfo.InvariantCulture);
								historyCollection.Add(timestamp, num);
							}
						}
						else if (obj != null)
						{
							num = ((obj != Convert.DBNull) ? Convert.ToDouble(obj, CultureInfo.InvariantCulture) : double.NaN);
							historyCollection.Add(now, num);
						}
					}
					foreach (HistoryEntry item2 in historyCollection)
					{
						SetValueInternal(item2.Value, item2.Timestamp);
					}
				}
				finally
				{
					IntState = ValueState.Interactive;
					historyCollection.Clear();
				}
				Invalidate();
			}
			finally
			{
				if (closeDataReader)
				{
					((IDataReader)dataSourceAsIEnumerable).Close();
				}
				connection?.Close();
			}
			if (Common != null)
			{
				Common.GaugeCore.boundToDataSource = true;
			}
		}

		internal void AutoDataBind()
		{
			if (DataSource != null && valueFieldMember != string.Empty)
			{
				DataBind();
			}
		}

		internal void PerformDataBinding(IEnumerable data)
		{
			DataBind(data, ValueFieldMember, DateFieldMember, DataMember);
		}

		private IEnumerable GetDataSourceAsIEnumerable(object data, string dataMember, out bool closeDataReader, out IDbConnection connection)
		{
			object obj = data;
			closeDataReader = false;
			connection = null;
			if (obj != null)
			{
				try
				{
					if (obj is OleDbDataAdapter)
					{
						obj = ((OleDbDataAdapter)obj).SelectCommand;
					}
					else if (obj is SqlDataAdapter)
					{
						obj = ((SqlDataAdapter)obj).SelectCommand;
					}
					else if (obj is OleDbDataAdapter)
					{
						obj = ((OleDbDataAdapter)obj).SelectCommand;
					}
					if (obj is DataSet && ((DataSet)obj).Tables.Count > 0)
					{
						obj = ((!(dataMember == string.Empty) && ((DataSet)obj).Tables.Count != 1) ? ((DataSet)obj).Tables[dataMember] : ((DataSet)obj).Tables[0]);
					}
					if (obj is DataTable)
					{
						obj = new DataView((DataTable)obj);
					}
					else if (obj is IDbCommand)
					{
						if (((IDbCommand)obj).Connection.State != ConnectionState.Open)
						{
							connection = ((IDbCommand)obj).Connection;
							connection.Open();
						}
						obj = ((IDbCommand)obj).ExecuteReader();
						closeDataReader = true;
					}
					else
					{
						obj = (obj as IEnumerable);
					}
				}
				catch (Exception innerException)
				{
					throw new ApplicationException(Utils.SRGetStr("ExceptionBadDatasource"), innerException);
				}
				return obj as IEnumerable;
			}
			return null;
		}

		private object ConvertEnumerationItem(object item, string fieldName)
		{
			object result = item;
			bool flag = true;
			if (item is DataRow)
			{
				if (fieldName != null && fieldName.Length > 0)
				{
					if (((DataRow)item).Table.Columns.Contains(fieldName))
					{
						result = ((DataRow)item)[fieldName];
						flag = false;
					}
					else
					{
						try
						{
							int num = int.Parse(fieldName, CultureInfo.InvariantCulture);
							if (((DataRow)item).Table.Columns.Count < num && num >= 0)
							{
								result = ((DataRow)item)[num];
								flag = false;
							}
						}
						catch
						{
						}
					}
				}
			}
			else if (item is DataRowView)
			{
				if (fieldName != null && fieldName.Length > 0)
				{
					if (((DataRowView)item).DataView.Table.Columns.Contains(fieldName))
					{
						result = ((DataRowView)item)[fieldName];
						flag = false;
					}
					else
					{
						try
						{
							int num2 = int.Parse(fieldName, CultureInfo.InvariantCulture);
							if (((DataRowView)item).DataView.Table.Columns.Count < num2 && num2 >= 0)
							{
								result = ((DataRowView)item)[num2];
								flag = false;
							}
						}
						catch
						{
						}
					}
				}
			}
			else if (item is DbDataRecord)
			{
				if (fieldName != null && fieldName.Length > 0)
				{
					if (!char.IsNumber(fieldName, 0))
					{
						try
						{
							result = ((DbDataRecord)item)[fieldName];
							flag = false;
						}
						catch (Exception)
						{
						}
					}
					if (flag)
					{
						try
						{
							int i = int.Parse(fieldName, CultureInfo.InvariantCulture);
							result = ((DbDataRecord)item)[i];
							flag = false;
						}
						catch
						{
						}
					}
				}
			}
			else if (item != null)
			{
				try
				{
					PropertyInfo property = item.GetType().GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public);
					if (property != null)
					{
						result = property.GetValue(item, new object[0]);
						flag = false;
					}
					else
					{
						FieldInfo field = item.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
						if (field != null)
						{
							result = field.GetValue(item);
							flag = false;
						}
					}
				}
				catch
				{
				}
			}
			if (flag)
			{
				throw new ArgumentException(Utils.SRGetStr("ExceptionColumnNotFound", fieldName));
			}
			return result;
		}

		private void PlaybackComplete()
		{
			lock (this)
			{
				IntState = ValueState.Interactive;
				if (playBackValue != null)
				{
					if (Common != null && Common.GaugeContainer != null)
					{
						Common.GaugeContainer.OnPlaybackStateChanged(this, new PlaybackStateChangedEventArgs(PlaybackState.Complete, playBackValue.inputValue, playBackValue.inputDate, Name));
					}
					playBackValue.Dispose();
					playBackValue = null;
					RefreshConsumers();
				}
			}
		}

		internal override void BeginInit()
		{
			base.BeginInit();
			calculatedValues.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
			calculatedValues.EndInit();
		}

		protected override void OnDispose()
		{
			calculatedValues.Dispose();
			base.OnDispose();
		}

		public override void Reset()
		{
			Stop();
			base.Reset();
		}

		internal override object CloneInternals(object copy)
		{
			InputValue inputValue = (InputValue)base.CloneInternals(copy);
			inputValue.calculatedValues = new CalculatedValueCollection(inputValue, null);
			foreach (CalculatedValue calculatedValue in CalculatedValues)
			{
				inputValue.calculatedValues.Add((CalculatedValue)calculatedValue.Clone());
			}
			inputValue.calculatedValues.EndInit();
			return inputValue;
		}
	}
}
