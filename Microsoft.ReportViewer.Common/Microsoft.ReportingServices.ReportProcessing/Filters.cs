using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class Filters
	{
		internal enum FilterTypes
		{
			DataSetFilter,
			DataRegionFilter,
			GroupFilter
		}

		private sealed class MyTopComparer : IComparer
		{
			private CompareInfo m_compareInfo;

			private CompareOptions m_compareOptions;

			internal MyTopComparer(CompareInfo compareInfo, CompareOptions compareOptions)
			{
				m_compareInfo = compareInfo;
				m_compareOptions = compareOptions;
			}

			int IComparer.Compare(object x, object y)
			{
				return ReportProcessing.CompareTo(y, x, m_compareInfo, m_compareOptions);
			}
		}

		private sealed class MyBottomComparer : IComparer
		{
			private CompareInfo m_compareInfo;

			private CompareOptions m_compareOptions;

			internal MyBottomComparer(CompareInfo compareInfo, CompareOptions compareOptions)
			{
				m_compareInfo = compareInfo;
				m_compareOptions = compareOptions;
			}

			int IComparer.Compare(object x, object y)
			{
				return ReportProcessing.CompareTo(x, y, m_compareInfo, m_compareOptions);
			}
		}

		private abstract class MySortedList
		{
			protected IComparer m_comparer;

			protected ArrayList m_keys;

			protected ArrayList m_values;

			protected Filters m_filters;

			internal int Count
			{
				get
				{
					if (m_keys == null)
					{
						return 0;
					}
					return m_keys.Count;
				}
			}

			internal MySortedList(IComparer comparer, Filters filters)
			{
				m_comparer = comparer;
				m_filters = filters;
			}

			internal abstract bool Add(object key, object value, FilterInfo owner);

			internal virtual void TrimInstanceSet(int maxSize, FilterInfo owner)
			{
			}

			protected int Search(object key)
			{
				Global.Tracer.Assert(m_keys != null, "(null != m_keys)");
				int num = m_keys.BinarySearch(key, m_comparer);
				if (num < 0)
				{
					num = ~num;
				}
				else
				{
					for (num++; num < m_keys.Count && m_comparer.Compare(m_keys[num - 1], m_keys[num]) == 0; num++)
					{
					}
				}
				return num;
			}
		}

		private sealed class MySortedListWithMaxSize : MySortedList
		{
			private int m_maxSize;

			internal MySortedListWithMaxSize(IComparer comparer, int maxSize, Filters filters)
				: base(comparer, filters)
			{
				if (0 > maxSize)
				{
					m_maxSize = 0;
				}
				else
				{
					m_maxSize = maxSize;
				}
			}

			internal override bool Add(object key, object value, FilterInfo owner)
			{
				if (m_keys == null)
				{
					m_keys = new ArrayList(Math.Min(1000, m_maxSize));
					m_values = new ArrayList(Math.Min(1000, m_maxSize));
				}
				int num;
				try
				{
					num = Search(key);
				}
				catch
				{
					throw new ReportProcessingException(m_filters.RegisterComparisonError());
				}
				int count = m_keys.Count;
				bool flag = false;
				if (count < m_maxSize)
				{
					flag = true;
				}
				else if (num < count)
				{
					flag = true;
					if (num < m_maxSize)
					{
						int num2 = m_maxSize - 1;
						object y = (num != m_maxSize - 1) ? m_keys[num2 - 1] : key;
						int num3;
						try
						{
							num3 = m_comparer.Compare(m_keys[num2], y);
						}
						catch
						{
							throw new ReportProcessingException(m_filters.RegisterComparisonError());
						}
						if (num3 != 0)
						{
							for (int i = num2; i < count; i++)
							{
								owner.Remove((DataInstanceInfo)m_values[i]);
							}
							m_keys.RemoveRange(num2, count - num2);
							m_values.RemoveRange(num2, count - num2);
						}
					}
				}
				else if (count > 0)
				{
					try
					{
						if (m_comparer.Compare(m_keys[count - 1], key) == 0)
						{
							flag = true;
						}
					}
					catch
					{
						throw new ReportProcessingException(m_filters.RegisterComparisonError());
					}
				}
				if (flag)
				{
					m_keys.Insert(num, key);
					m_values.Insert(num, value);
				}
				return flag;
			}
		}

		private sealed class MySortedListWithoutMaxSize : MySortedList
		{
			internal MySortedListWithoutMaxSize(IComparer comparer, Filters filters)
				: base(comparer, filters)
			{
			}

			internal override bool Add(object key, object value, FilterInfo owner)
			{
				if (m_keys == null)
				{
					m_keys = new ArrayList();
					m_values = new ArrayList();
				}
				int index;
				try
				{
					index = Search(key);
				}
				catch
				{
					throw new ReportProcessingException(m_filters.RegisterComparisonError());
				}
				m_keys.Insert(index, key);
				m_values.Insert(index, value);
				return true;
			}

			internal override void TrimInstanceSet(int maxSize, FilterInfo owner)
			{
				int count = base.Count;
				int num = count;
				if (count <= maxSize)
				{
					return;
				}
				if (0 < maxSize)
				{
					for (num = maxSize; num < count && m_comparer.Compare(m_keys[num - 1], m_keys[num]) == 0; num++)
					{
					}
					for (int i = num; i < count; i++)
					{
						owner.Remove((DataInstanceInfo)m_values[i]);
					}
					m_keys.RemoveRange(num, count - num);
					m_values.RemoveRange(num, count - num);
				}
				else
				{
					owner.RemoveAll();
					m_keys = null;
					m_values = null;
				}
			}
		}

		private sealed class FilterInfo
		{
			private double m_percentage = -1.0;

			private MySortedList m_dataInstances;

			private DataInstanceInfo m_firstInstance;

			private DataInstanceInfo m_currentInstance;

			internal double Percentage
			{
				get
				{
					return m_percentage;
				}
				set
				{
					m_percentage = value;
				}
			}

			internal int InstanceCount => m_dataInstances.Count;

			internal DataInstanceInfo FirstInstance => m_firstInstance;

			internal FilterInfo(MySortedList dataInstances)
			{
				Global.Tracer.Assert(dataInstances != null, "(null != dataInstances)");
				m_dataInstances = dataInstances;
			}

			internal bool Add(object key, object dataInstance)
			{
				DataInstanceInfo dataInstanceInfo = new DataInstanceInfo();
				dataInstanceInfo.DataInstance = dataInstance;
				bool num = m_dataInstances.Add(key, dataInstanceInfo, this);
				if (num)
				{
					if (m_firstInstance == null)
					{
						m_firstInstance = dataInstanceInfo;
					}
					if (m_currentInstance != null)
					{
						m_currentInstance.NextInstance = dataInstanceInfo;
					}
					dataInstanceInfo.PrevInstance = m_currentInstance;
					dataInstanceInfo.NextInstance = null;
					m_currentInstance = dataInstanceInfo;
				}
				return num;
			}

			internal void Remove(DataInstanceInfo instance)
			{
				if (instance.NextInstance != null)
				{
					instance.NextInstance.PrevInstance = instance.PrevInstance;
				}
				else
				{
					Global.Tracer.Assert(instance == m_currentInstance, "(instance == m_currentInstance)");
					m_currentInstance = instance.PrevInstance;
				}
				if (instance.PrevInstance != null)
				{
					instance.PrevInstance.NextInstance = instance.NextInstance;
					return;
				}
				Global.Tracer.Assert(instance == m_firstInstance, "(instance == m_firstInstance)");
				m_firstInstance = instance.NextInstance;
			}

			internal void RemoveAll()
			{
				m_firstInstance = null;
				m_currentInstance = null;
			}

			internal void TrimInstanceSet(int maxSize)
			{
				m_dataInstances.TrimInstanceSet(maxSize, this);
			}
		}

		private sealed class DataInstanceInfo
		{
			internal object DataInstance;

			internal DataInstanceInfo PrevInstance;

			internal DataInstanceInfo NextInstance;
		}

		private FilterTypes m_filterType;

		private ReportProcessing.IFilterOwner m_owner;

		private FilterList m_filters;

		private ObjectType m_objectType;

		private string m_objectName;

		private ReportProcessing.ProcessingContext m_processingContext;

		private int m_startFilterIndex;

		private int m_currentSpecialFilterIndex = -1;

		private FilterInfo m_filterInfo;

		private bool m_failFilters;

		internal bool FailFilters
		{
			set
			{
				m_failFilters = value;
			}
		}

		internal Filters(FilterTypes filterType, ReportProcessing.IFilterOwner owner, FilterList filters, ObjectType objectType, string objectName, ReportProcessing.ProcessingContext processingContext)
		{
			m_filterType = filterType;
			m_owner = owner;
			m_filters = filters;
			m_objectType = objectType;
			m_objectName = objectName;
			m_processingContext = processingContext;
		}

		internal bool PassFilters(object dataInstance)
		{
			bool specialFilter;
			return PassFilters(dataInstance, out specialFilter);
		}

		private void ThrowIfErrorOccurred(string propertyName, bool errorOccurred, DataFieldStatus fieldStatus)
		{
			if (errorOccurred)
			{
				if (fieldStatus != 0)
				{
					throw new ReportProcessingException(ErrorCode.rsFilterFieldError, m_objectType, m_objectName, propertyName, ReportRuntime.GetErrorName(fieldStatus, null));
				}
				throw new ReportProcessingException(ErrorCode.rsFilterEvaluationError, m_objectType, m_objectName, propertyName);
			}
		}

		internal bool PassFilters(object dataInstance, out bool specialFilter)
		{
			bool flag = true;
			specialFilter = false;
			if (m_failFilters)
			{
				return false;
			}
			if (m_filters != null)
			{
				for (int num = m_startFilterIndex; num < m_filters.Count; num++)
				{
					Filter filter = m_filters[num];
					if (Filter.Operators.Like == filter.Operator)
					{
						StringResult stringResult = m_processingContext.ReportRuntime.EvaluateFilterStringExpression(filter, m_objectType, m_objectName);
						ThrowIfErrorOccurred("FilterExpression", stringResult.ErrorOccurred, stringResult.FieldStatus);
						Global.Tracer.Assert(filter.Values != null);
						Global.Tracer.Assert(1 <= filter.Values.Count);
						StringResult stringResult2 = m_processingContext.ReportRuntime.EvaluateFilterStringValue(filter, 0, m_objectType, m_objectName);
						ThrowIfErrorOccurred("FilterValue", stringResult2.ErrorOccurred, stringResult2.FieldStatus);
						if (stringResult.Value != null && stringResult2.Value != null)
						{
							if (!StringType.StrLikeText(stringResult.Value, stringResult2.Value))
							{
								flag = false;
							}
						}
						else if (stringResult.Value != null || stringResult2.Value != null)
						{
							flag = false;
						}
					}
					else
					{
						VariantResult variantResult = m_processingContext.ReportRuntime.EvaluateFilterVariantExpression(filter, m_objectType, m_objectName);
						ThrowIfErrorOccurred("FilterExpression", variantResult.ErrorOccurred, variantResult.FieldStatus);
						object value = variantResult.Value;
						if (filter.Operator == Filter.Operators.Equal || Filter.Operators.NotEqual == filter.Operator || Filter.Operators.GreaterThan == filter.Operator || Filter.Operators.GreaterThanOrEqual == filter.Operator || Filter.Operators.LessThan == filter.Operator || Filter.Operators.LessThanOrEqual == filter.Operator)
						{
							object y = EvaluateFilterValue(filter);
							int num2 = 0;
							try
							{
								num2 = ReportProcessing.CompareTo(value, y, m_processingContext.CompareInfo, m_processingContext.ClrCompareOptions);
							}
							catch (ReportProcessingException_ComparisonError e)
							{
								throw new ReportProcessingException(RegisterComparisonError(e));
							}
							catch
							{
								throw new ReportProcessingException(RegisterComparisonError());
							}
							if (flag)
							{
								switch (filter.Operator)
								{
								case Filter.Operators.Equal:
									if (num2 != 0)
									{
										flag = false;
									}
									break;
								case Filter.Operators.NotEqual:
									if (num2 == 0)
									{
										flag = false;
									}
									break;
								case Filter.Operators.GreaterThan:
									if (0 >= num2)
									{
										flag = false;
									}
									break;
								case Filter.Operators.GreaterThanOrEqual:
									if (0 > num2)
									{
										flag = false;
									}
									break;
								case Filter.Operators.LessThan:
									if (0 <= num2)
									{
										flag = false;
									}
									break;
								case Filter.Operators.LessThanOrEqual:
									if (0 < num2)
									{
										flag = false;
									}
									break;
								}
							}
						}
						else if (Filter.Operators.In == filter.Operator)
						{
							object[] array = EvaluateFilterValues(filter);
							flag = false;
							if (array != null)
							{
								for (int i = 0; i < array.Length; i++)
								{
									try
									{
										if (array[i] is ICollection)
										{
											foreach (object item in (ICollection)array[i])
											{
												if (ReportProcessing.CompareTo(value, item, m_processingContext.CompareInfo, m_processingContext.ClrCompareOptions) == 0)
												{
													flag = true;
													break;
												}
											}
										}
										else if (ReportProcessing.CompareTo(value, array[i], m_processingContext.CompareInfo, m_processingContext.ClrCompareOptions) == 0)
										{
											flag = true;
										}
										if (flag)
										{
											goto IL_05f2;
										}
									}
									catch (ReportProcessingException_ComparisonError e2)
									{
										throw new ReportProcessingException(RegisterComparisonError(e2));
									}
									catch
									{
										throw new ReportProcessingException(RegisterComparisonError());
									}
								}
							}
						}
						else if (Filter.Operators.Between == filter.Operator)
						{
							object[] array2 = EvaluateFilterValues(filter);
							flag = false;
							Global.Tracer.Assert(array2 != null && 2 == array2.Length);
							try
							{
								if (0 <= ReportProcessing.CompareTo(value, array2[0], m_processingContext.CompareInfo, m_processingContext.ClrCompareOptions) && 0 >= ReportProcessing.CompareTo(value, array2[1], m_processingContext.CompareInfo, m_processingContext.ClrCompareOptions))
								{
									flag = true;
								}
							}
							catch (ReportProcessingException_ComparisonError e3)
							{
								throw new ReportProcessingException(RegisterComparisonError(e3));
							}
							catch
							{
								throw new ReportProcessingException(RegisterComparisonError());
							}
						}
						else if (Filter.Operators.TopN == filter.Operator || Filter.Operators.BottomN == filter.Operator)
						{
							if (m_filterInfo == null)
							{
								Global.Tracer.Assert(filter.Values != null && 1 == filter.Values.Count);
								IntegerResult integerResult = m_processingContext.ReportRuntime.EvaluateFilterIntegerValue(filter, 0, m_objectType, m_objectName);
								ThrowIfErrorOccurred("FilterValue", integerResult.ErrorOccurred, integerResult.FieldStatus);
								int value2 = integerResult.Value;
								IComparer comparer = (Filter.Operators.TopN != filter.Operator) ? ((IComparer)new MyBottomComparer(m_processingContext.CompareInfo, m_processingContext.ClrCompareOptions)) : ((IComparer)new MyTopComparer(m_processingContext.CompareInfo, m_processingContext.ClrCompareOptions));
								InitFilterInfos(new MySortedListWithMaxSize(comparer, value2, this), num);
							}
							SortAndSave(value, dataInstance);
							flag = false;
							specialFilter = true;
						}
						else if (Filter.Operators.TopPercent == filter.Operator || Filter.Operators.BottomPercent == filter.Operator)
						{
							if (m_filterInfo == null)
							{
								Global.Tracer.Assert(filter.Values != null && 1 == filter.Values.Count);
								FloatResult floatResult = m_processingContext.ReportRuntime.EvaluateFilterIntegerOrFloatValue(filter, 0, m_objectType, m_objectName);
								ThrowIfErrorOccurred("FilterValue", floatResult.ErrorOccurred, floatResult.FieldStatus);
								double value3 = floatResult.Value;
								IComparer comparer2 = (Filter.Operators.TopPercent != filter.Operator) ? ((IComparer)new MyBottomComparer(m_processingContext.CompareInfo, m_processingContext.ClrCompareOptions)) : ((IComparer)new MyTopComparer(m_processingContext.CompareInfo, m_processingContext.ClrCompareOptions));
								InitFilterInfos(new MySortedListWithoutMaxSize(comparer2, this), num);
								m_filterInfo.Percentage = value3;
							}
							SortAndSave(value, dataInstance);
							flag = false;
							specialFilter = true;
						}
					}
					goto IL_05f2;
					IL_05f2:
					if (!flag)
					{
						return false;
					}
				}
			}
			return flag;
		}

		internal void FinishReadingRows()
		{
			if (m_filterInfo == null)
			{
				return;
			}
			FilterInfo filterInfo = m_filterInfo;
			m_filterInfo = null;
			m_startFilterIndex = m_currentSpecialFilterIndex + 1;
			bool flag = m_startFilterIndex >= m_filters.Count;
			TrimInstanceSet(filterInfo);
			for (DataInstanceInfo dataInstanceInfo = filterInfo.FirstInstance; dataInstanceInfo != null; dataInstanceInfo = dataInstanceInfo.NextInstance)
			{
				object dataInstance = dataInstanceInfo.DataInstance;
				if (FilterTypes.GroupFilter == m_filterType)
				{
					ReportProcessing.RuntimeGroupLeafObj runtimeGroupLeafObj = (ReportProcessing.RuntimeGroupLeafObj)dataInstance;
					runtimeGroupLeafObj.SetupEnvironment();
					if (flag || PassFilters(dataInstance))
					{
						runtimeGroupLeafObj.PostFilterNextRow();
					}
					else
					{
						runtimeGroupLeafObj.FailFilter();
					}
				}
				else
				{
					FieldImpl[] fields = (FieldImpl[])dataInstance;
					m_processingContext.ReportObjectModel.FieldsImpl.SetFields(fields);
					if (flag || PassFilters(dataInstance))
					{
						m_owner.PostFilterNextRow();
					}
				}
			}
			filterInfo = null;
			FinishReadingRows();
		}

		private ProcessingMessageList RegisterComparisonError()
		{
			return RegisterComparisonError(null);
		}

		private ProcessingMessageList RegisterComparisonError(ReportProcessingException_ComparisonError e)
		{
			if (e == null)
			{
				m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsComparisonError, Severity.Error, m_objectType, m_objectName, "FilterExpression");
			}
			else
			{
				m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsComparisonTypeError, Severity.Error, m_objectType, m_objectName, "FilterExpression", e.TypeX, e.TypeY);
			}
			return m_processingContext.ErrorContext.Messages;
		}

		private void TrimInstanceSet(FilterInfo filterInfo)
		{
			if (-1.0 == filterInfo.Percentage)
			{
				return;
			}
			int instanceCount = filterInfo.InstanceCount;
			double num = (double)instanceCount * filterInfo.Percentage / 100.0;
			if (num <= 0.0)
			{
				instanceCount = 0;
			}
			else
			{
				try
				{
					instanceCount = Convert.ToInt32(num);
				}
				catch
				{
					throw new ReportProcessingException(ErrorCode.rsFilterEvaluationError, "FilterValues");
				}
			}
			filterInfo.TrimInstanceSet(instanceCount);
		}

		private object EvaluateFilterValue(Filter filterDef)
		{
			Global.Tracer.Assert(filterDef.Values != null, "(filterDef.Values != null)");
			Global.Tracer.Assert(filterDef.Values.Count > 0, "(filterDef.Values.Count > 0)");
			VariantResult variantResult = m_processingContext.ReportRuntime.EvaluateFilterVariantValue(filterDef, 0, m_objectType, m_objectName);
			ThrowIfErrorOccurred("FilterValue", variantResult.ErrorOccurred, variantResult.FieldStatus);
			return variantResult.Value;
		}

		private object[] EvaluateFilterValues(Filter filterDef)
		{
			if (filterDef.Values != null)
			{
				object[] array = new object[filterDef.Values.Count];
				for (int num = filterDef.Values.Count - 1; num >= 0; num--)
				{
					VariantResult variantResult = m_processingContext.ReportRuntime.EvaluateFilterVariantValue(filterDef, num, m_objectType, m_objectName);
					ThrowIfErrorOccurred("FilterValues", variantResult.ErrorOccurred, variantResult.FieldStatus);
					array[num] = variantResult.Value;
				}
				return array;
			}
			return null;
		}

		private void SortAndSave(object key, object dataInstance)
		{
			if (m_filterInfo.Add(key, dataInstance) && FilterTypes.GroupFilter != m_filterType)
			{
				m_processingContext.ReportObjectModel.FieldsImpl.GetAndSaveFields();
			}
		}

		private void InitFilterInfos(MySortedList dataInstanceList, int currentFilterIndex)
		{
			m_filterInfo = new FilterInfo(dataInstanceList);
			if (-1 == m_currentSpecialFilterIndex && FilterTypes.DataRegionFilter == m_filterType)
			{
				m_processingContext.AddSpecialDataRegionFilters(this);
			}
			m_currentSpecialFilterIndex = currentFilterIndex;
		}
	}
}
