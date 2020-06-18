using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class Filters : IStaticReferenceable
	{
		internal enum FilterTypes
		{
			DataSetFilter,
			DataRegionFilter,
			GroupFilter
		}

		private sealed class MyTopComparer : IComparer
		{
			private readonly IDataComparer m_comparer;

			internal MyTopComparer(IDataComparer comparer)
			{
				m_comparer = comparer;
			}

			int IComparer.Compare(object x, object y)
			{
				object key = ((FilterKey)x).Key;
				object key2 = ((FilterKey)y).Key;
				return m_comparer.Compare(key2, key);
			}
		}

		private sealed class MyBottomComparer : IComparer
		{
			private readonly IDataComparer m_comparer;

			internal MyBottomComparer(IDataComparer comparer)
			{
				m_comparer = comparer;
			}

			int IComparer.Compare(object x, object y)
			{
				object key = ((FilterKey)x).Key;
				object key2 = ((FilterKey)y).Key;
				return m_comparer.Compare(key, key2);
			}
		}

		private abstract class MySortedList
		{
			protected IComparer m_comparer;

			protected ScalableList<FilterKey> m_keys;

			protected ScalableHybridList<object> m_values;

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

			internal IEnumerator<object> Instances
			{
				get
				{
					if (m_keys == null)
					{
						return null;
					}
					return m_values.GetEnumerator();
				}
			}

			internal MySortedList(IComparer comparer, Filters filters)
			{
				m_comparer = comparer;
				m_filters = filters;
			}

			internal abstract bool Add(FilterKey key, object value, FilterInfo owner);

			internal virtual void TrimInstanceSet(int maxSize, FilterInfo owner)
			{
			}

			protected int Search(FilterKey key)
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

			internal void Clear()
			{
				if (m_values != null)
				{
					m_values.Clear();
				}
				if (m_keys != null)
				{
					m_keys.Clear();
				}
			}

			protected void InitLists(int bucketSize, int initialCapacity)
			{
				OnDemandProcessingContext processingContext = m_filters.m_processingContext;
				processingContext.EnsureScalabilitySetup();
				m_keys = new ScalableList<FilterKey>(m_filters.m_scalabilityPriority, processingContext.TablixProcessingScalabilityCache, bucketSize, initialCapacity);
				m_values = new ScalableHybridList<object>(m_filters.m_scalabilityPriority, processingContext.TablixProcessingScalabilityCache, bucketSize, initialCapacity);
			}
		}

		internal sealed class FilterKey : IStorable, IPersistable
		{
			internal object Key;

			internal int ValueIndex;

			private static readonly Declaration m_declaration = GetDeclaration();

			public int Size => ItemSizes.SizeOf(Key) + 4;

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.Key:
						writer.WriteVariantOrPersistable(Key);
						break;
					case MemberName.Value:
						writer.Write(ValueIndex);
						break;
					default:
						Global.Tracer.Assert(condition: false);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.Key:
						Key = reader.ReadVariant();
						break;
					case MemberName.Value:
						ValueIndex = reader.ReadInt32();
						break;
					default:
						Global.Tracer.Assert(condition: false);
						break;
					}
				}
			}

			public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
				Global.Tracer.Assert(condition: false, "No references to resolve");
			}

			public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
			{
				return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FilterKey;
			}

			public static Declaration GetDeclaration()
			{
				if (m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.Key, Token.Object));
					list.Add(new MemberInfo(MemberName.Value, Token.Int32));
					return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FilterKey, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
				}
				return m_declaration;
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

			internal override bool Add(FilterKey key, object value, FilterInfo owner)
			{
				if (m_keys == null)
				{
					int num = Math.Min(500, m_maxSize);
					InitLists(num, num);
				}
				int num2;
				try
				{
					num2 = Search(key);
				}
				catch (Exception e)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e))
					{
						throw;
					}
					throw new ReportProcessingException(m_filters.RegisterComparisonError());
				}
				int count = m_keys.Count;
				bool flag = false;
				if (count < m_maxSize)
				{
					flag = true;
				}
				else if (num2 < count)
				{
					flag = true;
					if (num2 < m_maxSize)
					{
						int num3 = m_maxSize - 1;
						object y = (num2 != m_maxSize - 1) ? m_keys[num3 - 1] : key;
						int num4;
						try
						{
							num4 = m_comparer.Compare(m_keys[num3], y);
						}
						catch (Exception e2)
						{
							if (AsynchronousExceptionDetection.IsStoppingException(e2))
							{
								throw;
							}
							throw new ReportProcessingException(m_filters.RegisterComparisonError());
						}
						if (num4 != 0)
						{
							for (int i = num3; i < count; i++)
							{
								FilterKey filterKey = m_keys[i];
								m_values.Remove(filterKey.ValueIndex);
							}
							m_keys.RemoveRange(num3, count - num3);
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
					catch (Exception e3)
					{
						if (AsynchronousExceptionDetection.IsStoppingException(e3))
						{
							throw;
						}
						throw new ReportProcessingException(m_filters.RegisterComparisonError());
					}
				}
				if (flag)
				{
					key.ValueIndex = m_values.Add(value);
					m_keys.Insert(num2, key);
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

			internal override bool Add(FilterKey key, object value, FilterInfo owner)
			{
				if (m_keys == null)
				{
					InitLists(500, 500);
				}
				int index;
				try
				{
					index = Search(key);
				}
				catch (Exception e)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e))
					{
						throw;
					}
					throw new ReportProcessingException(m_filters.RegisterComparisonError());
				}
				key.ValueIndex = m_values.Add(value);
				m_keys.Insert(index, key);
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
						FilterKey filterKey = m_keys[i];
						m_values.Remove(filterKey.ValueIndex);
					}
					m_keys.RemoveRange(num, count - num);
					return;
				}
				if (m_keys != null)
				{
					m_keys.Dispose();
				}
				if (m_values != null)
				{
					m_values.Dispose();
				}
				m_keys = null;
				m_values = null;
			}
		}

		private sealed class FilterInfo
		{
			private double m_percentage = -1.0;

			private Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators m_operator = Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.TopPercent;

			private MySortedList m_dataInstances;

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

			internal Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators Operator
			{
				get
				{
					return m_operator;
				}
				set
				{
					m_operator = value;
				}
			}

			internal int InstanceCount => m_dataInstances.Count;

			internal IEnumerator<object> Instances => m_dataInstances.Instances;

			internal FilterInfo(MySortedList dataInstances)
			{
				Global.Tracer.Assert(dataInstances != null, "(null != dataInstances)");
				m_dataInstances = dataInstances;
			}

			internal bool Add(object key, object dataInstance)
			{
				FilterKey filterKey = new FilterKey();
				filterKey.Key = key;
				return m_dataInstances.Add(filterKey, dataInstance, this);
			}

			internal void RemoveAll()
			{
				m_dataInstances.Clear();
			}

			internal void TrimInstanceSet(int maxSize)
			{
				m_dataInstances.TrimInstanceSet(maxSize, this);
			}
		}

		private FilterTypes m_filterType;

		private IReference<Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner> m_owner;

		private Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner m_ownerObj;

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.Filter> m_filters;

		private Microsoft.ReportingServices.ReportProcessing.ObjectType m_objectType;

		private string m_objectName;

		private OnDemandProcessingContext m_processingContext;

		private int m_startFilterIndex;

		private int m_currentSpecialFilterIndex = -1;

		private FilterInfo m_filterInfo;

		private bool m_failFilters;

		private int m_scalabilityPriority;

		private int m_id = int.MinValue;

		internal bool FailFilters
		{
			set
			{
				m_failFilters = value;
			}
		}

		int IStaticReferenceable.ID => m_id;

		internal Filters(FilterTypes filterType, IReference<Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner> owner, List<Microsoft.ReportingServices.ReportIntermediateFormat.Filter> filters, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, OnDemandProcessingContext processingContext, int scalabilityPriority)
			: this(filterType, filters, objectType, objectName, processingContext, scalabilityPriority)
		{
			m_owner = owner;
		}

		internal Filters(FilterTypes filterType, RuntimeParameterDataSet owner, List<Microsoft.ReportingServices.ReportIntermediateFormat.Filter> filters, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, OnDemandProcessingContext processingContext, int scalabilityPriority)
			: this(filterType, filters, objectType, objectName, processingContext, scalabilityPriority)
		{
			m_ownerObj = owner;
		}

		private Filters(FilterTypes filterType, List<Microsoft.ReportingServices.ReportIntermediateFormat.Filter> filters, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, OnDemandProcessingContext processingContext, int scalabilityPriority)
		{
			m_filterType = filterType;
			m_filters = filters;
			m_objectType = objectType;
			m_objectName = objectName;
			m_processingContext = processingContext;
			m_scalabilityPriority = scalabilityPriority;
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
				switch (fieldStatus)
				{
				case DataFieldStatus.UnSupportedDataType:
					throw new ReportProcessingException(string.Format(CultureInfo.CurrentCulture, RPRes.Keys.GetString(ProcessingErrorCode.rsInvalidExpressionDataType.ToString()), m_objectType, m_objectName, propertyName), ErrorCode.rsFilterEvaluationError);
				default:
					throw new ReportProcessingException(ErrorCode.rsFilterFieldError, m_objectType, m_objectName, propertyName, Microsoft.ReportingServices.RdlExpressions.ReportRuntime.GetErrorName(fieldStatus, null));
				case DataFieldStatus.None:
					throw new ReportProcessingException(ErrorCode.rsFilterEvaluationError, m_objectType, m_objectName, propertyName);
				}
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
					Microsoft.ReportingServices.ReportIntermediateFormat.Filter filter = m_filters[num];
					if (Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.Like == filter.Operator)
					{
						Microsoft.ReportingServices.RdlExpressions.StringResult stringResult = m_processingContext.ReportRuntime.EvaluateFilterStringExpression(filter, m_objectType, m_objectName);
						ThrowIfErrorOccurred("FilterExpression", stringResult.ErrorOccurred, stringResult.FieldStatus);
						Global.Tracer.Assert(filter.Values != null, "(null != filter.Values)");
						Global.Tracer.Assert(1 <= filter.Values.Count, "(1 <= filter.Values.Count)");
						Microsoft.ReportingServices.RdlExpressions.StringResult stringResult2 = m_processingContext.ReportRuntime.EvaluateFilterStringValue(filter, 0, m_objectType, m_objectName);
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
						Microsoft.ReportingServices.RdlExpressions.VariantResult variantResult = m_processingContext.ReportRuntime.EvaluateFilterVariantExpression(filter, m_objectType, m_objectName);
						ThrowIfErrorOccurred("FilterExpression", variantResult.ErrorOccurred, variantResult.FieldStatus);
						object value = variantResult.Value;
						if (filter.Operator == Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.Equal || Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.NotEqual == filter.Operator || Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.GreaterThan == filter.Operator || Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.GreaterThanOrEqual == filter.Operator || Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.LessThan == filter.Operator || Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.LessThanOrEqual == filter.Operator)
						{
							object value2 = EvaluateFilterValue(filter);
							int num2 = 0;
							try
							{
								num2 = Compare(value, value2);
							}
							catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError)
							{
								throw new ReportProcessingException(RegisterSpatialTypeComparisonError(reportProcessingException_SpatialTypeComparisonError.Type));
							}
							catch (ReportProcessingException_ComparisonError e)
							{
								throw new ReportProcessingException(RegisterComparisonError(e));
							}
							catch (Exception e2)
							{
								if (AsynchronousExceptionDetection.IsStoppingException(e2))
								{
									throw;
								}
								throw new ReportProcessingException(RegisterComparisonError());
							}
							if (flag)
							{
								switch (filter.Operator)
								{
								case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.Equal:
									if (num2 != 0)
									{
										flag = false;
									}
									break;
								case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.NotEqual:
									if (num2 == 0)
									{
										flag = false;
									}
									break;
								case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.GreaterThan:
									if (0 >= num2)
									{
										flag = false;
									}
									break;
								case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.GreaterThanOrEqual:
									if (0 > num2)
									{
										flag = false;
									}
									break;
								case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.LessThan:
									if (0 <= num2)
									{
										flag = false;
									}
									break;
								case Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.LessThanOrEqual:
									if (0 < num2)
									{
										flag = false;
									}
									break;
								}
							}
						}
						else if (Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.In == filter.Operator)
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
												if (Compare(value, item) == 0)
												{
													flag = true;
													break;
												}
											}
										}
										else if (Compare(value, array[i]) == 0)
										{
											flag = true;
										}
										if (flag)
										{
											goto IL_05e1;
										}
									}
									catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError2)
									{
										throw new ReportProcessingException(RegisterSpatialTypeComparisonError(reportProcessingException_SpatialTypeComparisonError2.Type));
									}
									catch (ReportProcessingException_ComparisonError e3)
									{
										throw new ReportProcessingException(RegisterComparisonError(e3));
									}
									catch (Exception e4)
									{
										if (AsynchronousExceptionDetection.IsStoppingException(e4))
										{
											throw;
										}
										throw new ReportProcessingException(RegisterComparisonError());
									}
								}
							}
						}
						else if (Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.Between == filter.Operator)
						{
							object[] array2 = EvaluateFilterValues(filter);
							flag = false;
							Global.Tracer.Assert(array2 != null && 2 == array2.Length, "(null != values && 2 == values.Length)");
							try
							{
								if (0 <= Compare(value, array2[0]) && 0 >= Compare(value, array2[1]))
								{
									flag = true;
								}
							}
							catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError3)
							{
								throw new ReportProcessingException(RegisterSpatialTypeComparisonError(reportProcessingException_SpatialTypeComparisonError3.Type));
							}
							catch (ReportProcessingException_ComparisonError e5)
							{
								throw new ReportProcessingException(RegisterComparisonError(e5));
							}
							catch (RSException)
							{
								throw;
							}
							catch (Exception e6)
							{
								if (AsynchronousExceptionDetection.IsStoppingException(e6))
								{
									throw;
								}
								throw new ReportProcessingException(RegisterComparisonError());
							}
						}
						else if (Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.TopN == filter.Operator || Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.BottomN == filter.Operator)
						{
							if (m_filterInfo == null)
							{
								Global.Tracer.Assert(filter.Values != null && 1 == filter.Values.Count, "(null != filter.Values && 1 == filter.Values.Count)");
								Microsoft.ReportingServices.RdlExpressions.IntegerResult integerResult = m_processingContext.ReportRuntime.EvaluateFilterIntegerValue(filter, 0, m_objectType, m_objectName);
								ThrowIfErrorOccurred("FilterValue", integerResult.ErrorOccurred, integerResult.FieldStatus);
								int value3 = integerResult.Value;
								IComparer comparer = (Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.TopN != filter.Operator) ? ((IComparer)new MyBottomComparer(m_processingContext.ProcessingComparer)) : ((IComparer)new MyTopComparer(m_processingContext.ProcessingComparer));
								InitFilterInfos(new MySortedListWithMaxSize(comparer, value3, this), num);
							}
							SortAndSave(value, dataInstance);
							flag = false;
							specialFilter = true;
						}
						else if (Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.TopPercent == filter.Operator || Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.BottomPercent == filter.Operator)
						{
							if (m_filterInfo == null)
							{
								Global.Tracer.Assert(filter.Values != null && 1 == filter.Values.Count, "(null != filter.Values && 1 == filter.Values.Count)");
								Microsoft.ReportingServices.RdlExpressions.FloatResult floatResult = m_processingContext.ReportRuntime.EvaluateFilterIntegerOrFloatValue(filter, 0, m_objectType, m_objectName);
								ThrowIfErrorOccurred("FilterValue", floatResult.ErrorOccurred, floatResult.FieldStatus);
								double value4 = floatResult.Value;
								IComparer comparer2 = (Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.TopPercent != filter.Operator) ? ((IComparer)new MyBottomComparer(m_processingContext.ProcessingComparer)) : ((IComparer)new MyTopComparer(m_processingContext.ProcessingComparer));
								InitFilterInfos(new MySortedListWithoutMaxSize(comparer2, this), num);
								m_filterInfo.Percentage = value4;
								m_filterInfo.Operator = filter.Operator;
							}
							SortAndSave(value, dataInstance);
							flag = false;
							specialFilter = true;
						}
					}
					goto IL_05e1;
					IL_05e1:
					if (!flag)
					{
						return false;
					}
				}
			}
			return flag;
		}

		private int Compare(object value1, object value2)
		{
			return m_processingContext.ProcessingComparer.Compare(value1, value2);
		}

		internal void FinishReadingGroups(AggregateUpdateContext context)
		{
			FinishFilters(context);
		}

		internal void FinishReadingRows()
		{
			FinishFilters(null);
		}

		private void FinishFilters(AggregateUpdateContext context)
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
			IEnumerator<object> instances = filterInfo.Instances;
			if (instances != null)
			{
				try
				{
					Microsoft.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner filterOwner;
					if (m_owner != null)
					{
						m_owner.PinValue();
						filterOwner = m_owner.Value();
					}
					else
					{
						filterOwner = m_ownerObj;
					}
					while (instances.MoveNext())
					{
						object current = instances.Current;
						if (FilterTypes.GroupFilter == m_filterType)
						{
							IReference<RuntimeGroupLeafObj> reference = (IReference<RuntimeGroupLeafObj>)current;
							using (reference.PinValue())
							{
								RuntimeGroupLeafObj runtimeGroupLeafObj = reference.Value();
								runtimeGroupLeafObj.SetupEnvironment();
								if (flag || PassFilters(current))
								{
									runtimeGroupLeafObj.PostFilterNextRow(context);
								}
								else
								{
									runtimeGroupLeafObj.FailFilter();
								}
							}
						}
						else
						{
							((DataFieldRow)current).SetFields(m_processingContext.ReportObjectModel.FieldsImpl);
							if (flag || PassFilters(current))
							{
								filterOwner.PostFilterNextRow();
							}
						}
					}
				}
				finally
				{
					if (m_owner != null)
					{
						m_owner.UnPinValue();
					}
				}
			}
			filterInfo.RemoveAll();
			filterInfo = null;
			FinishFilters(context);
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

		private ProcessingMessageList RegisterSpatialTypeComparisonError(string type)
		{
			m_processingContext.ErrorContext.Register(ProcessingErrorCode.rsCannotCompareSpatialType, Severity.Error, m_objectType, m_objectName, type);
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
					instanceCount = ((filterInfo.Operator != Microsoft.ReportingServices.ReportIntermediateFormat.Filter.Operators.BottomPercent) ? ((int)Math.Ceiling(num)) : ((int)Math.Floor(num)));
				}
				catch (Exception e)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(e))
					{
						throw;
					}
					throw new ReportProcessingException(ErrorCode.rsFilterEvaluationError, "FilterValues");
				}
			}
			filterInfo.TrimInstanceSet(instanceCount);
		}

		private object EvaluateFilterValue(Microsoft.ReportingServices.ReportIntermediateFormat.Filter filterDef)
		{
			Global.Tracer.Assert(filterDef.Values != null, "(filterDef.Values != null)");
			Global.Tracer.Assert(filterDef.Values.Count > 0, "(filterDef.Values.Count > 0)");
			Microsoft.ReportingServices.RdlExpressions.VariantResult variantResult = m_processingContext.ReportRuntime.EvaluateFilterVariantValue(filterDef, 0, m_objectType, m_objectName);
			ThrowIfErrorOccurred("FilterValue", variantResult.ErrorOccurred, variantResult.FieldStatus);
			return variantResult.Value;
		}

		private object[] EvaluateFilterValues(Microsoft.ReportingServices.ReportIntermediateFormat.Filter filterDef)
		{
			if (filterDef.Values != null)
			{
				object[] array = new object[filterDef.Values.Count];
				for (int num = filterDef.Values.Count - 1; num >= 0; num--)
				{
					Microsoft.ReportingServices.RdlExpressions.VariantResult variantResult = m_processingContext.ReportRuntime.EvaluateFilterVariantValue(filterDef, num, m_objectType, m_objectName);
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

		void IStaticReferenceable.SetID(int id)
		{
			m_id = id;
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IStaticReferenceable.GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Filters;
		}
	}
}
