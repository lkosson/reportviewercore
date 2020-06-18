using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.Common
{
	internal sealed class CommonDataComparer : IDataComparer, IEqualityComparer, IEqualityComparer<object>, IComparer, IComparer<object>
	{
		private const bool DefaultThrowExceptionOnComparisonFailure = false;

		private readonly CompareInfo m_compareInfo;

		private readonly CompareOptions m_compareOptions;

		private readonly CultureInfo m_cultureInfo;

		private readonly bool m_nullsAsBlanks;

		public CompareInfo CompareInfo => m_compareInfo;

		public CompareOptions CompareOptions => m_compareOptions;

		internal CommonDataComparer(CompareInfo compareInfo, CompareOptions compareOptions, bool nullsAsBlanks)
		{
			m_compareInfo = compareInfo;
			if (m_compareInfo == null)
			{
				throw new ArgumentNullException("compareInfo");
			}
			m_compareOptions = compareOptions;
			m_cultureInfo = new CultureInfo(m_compareInfo.Name);
			m_nullsAsBlanks = nullsAsBlanks;
		}

		bool IEqualityComparer.Equals(object x, object y)
		{
			return InternalCompareTo(x, y, throwExceptionOnComparisonFailure: false) == 0;
		}

		bool IEqualityComparer<object>.Equals(object x, object y)
		{
			return InternalCompareTo(x, y, throwExceptionOnComparisonFailure: false) == 0;
		}

		int IComparer.Compare(object x, object y)
		{
			return InternalCompareTo(x, y, throwExceptionOnComparisonFailure: false);
		}

		int IComparer<object>.Compare(object x, object y)
		{
			return InternalCompareTo(x, y, throwExceptionOnComparisonFailure: false);
		}

		int IDataComparer.Compare(object x, object y, bool extendedTypeComparisons)
		{
			return InternalCompareTo(x, y, throwExceptionOnComparisonFailure: false);
		}

		int IDataComparer.Compare(object x, object y, bool throwExceptionOnComparisonFailure, bool extendedTypeComparisons, out bool validComparisonResult)
		{
			validComparisonResult = true;
			return InternalCompareTo(x, y, throwExceptionOnComparisonFailure);
		}

		public int GetHashCode(object obj)
		{
			if (obj == null)
			{
				return 0;
			}
			if (obj is string)
			{
				string text = (string)obj;
				if ((CompareOptions.IgnoreCase & m_compareOptions) != 0)
				{
					text = text.ToUpper(m_cultureInfo);
				}
				return text.GetHashCode();
			}
			return (obj as ICustomComparable)?.GetHashCode(this) ?? obj.GetHashCode();
		}

		private int InternalCompareTo(object x, object y, bool throwExceptionOnComparisonFailure)
		{
			string text = x as string;
			string text2 = y as string;
			if (text != null && text2 != null)
			{
				return m_compareInfo.Compare(text, text2, m_compareOptions);
			}
			DataTypeCode dataTypeCode = ObjectSerializer.GetDataTypeCode(x);
			DataTypeCode dataTypeCode2 = ObjectSerializer.GetDataTypeCode(y);
			if (dataTypeCode == DataTypeCode.Empty && dataTypeCode2 == DataTypeCode.Empty)
			{
				return 0;
			}
			if (dataTypeCode == DataTypeCode.Empty)
			{
				if (m_nullsAsBlanks && ComparerUtility.IsNumericLessThanZero(y))
				{
					return 1;
				}
				return -1;
			}
			if (dataTypeCode2 == DataTypeCode.Empty)
			{
				if (m_nullsAsBlanks && ComparerUtility.IsNumericLessThanZero(x))
				{
					return -1;
				}
				return 1;
			}
			if (dataTypeCode != dataTypeCode2)
			{
				switch (ComparerUtility.GetCommonVariantConversionType(dataTypeCode, dataTypeCode2))
				{
				case DataTypeCode.Double:
				{
					double num = 0.0;
					double num2 = 0.0;
					if (dataTypeCode == DataTypeCode.DateTime)
					{
						num = ((DateTime)x).ToOADate();
						num2 = Convert.ToDouble(y, m_cultureInfo);
					}
					else if (dataTypeCode2 == DataTypeCode.DateTime)
					{
						num2 = ((DateTime)y).ToOADate();
						num = Convert.ToDouble(x, m_cultureInfo);
					}
					else
					{
						num = Convert.ToDouble(x, m_cultureInfo);
						num2 = Convert.ToDouble(y, m_cultureInfo);
					}
					int num3 = num.CompareTo(num2);
					if (num3 == 0)
					{
						return CompareNumericDateVariantTypes(dataTypeCode, dataTypeCode2, throwExceptionOnComparisonFailure);
					}
					return num3;
				}
				case DataTypeCode.Decimal:
				{
					decimal num4 = Convert.ToDecimal(x, m_cultureInfo);
					decimal value = Convert.ToDecimal(y, m_cultureInfo);
					int num5 = num4.CompareTo(value);
					if (num5 == 0)
					{
						return CompareNumericDateVariantTypes(dataTypeCode, dataTypeCode2, throwExceptionOnComparisonFailure);
					}
					return num5;
				}
				case DataTypeCode.Int64:
				{
					long num6 = Convert.ToInt64(x, m_cultureInfo);
					long value2 = Convert.ToInt64(y, m_cultureInfo);
					int num7 = num6.CompareTo(value2);
					if (num7 == 0)
					{
						return CompareNumericDateVariantTypes(dataTypeCode, dataTypeCode2, throwExceptionOnComparisonFailure);
					}
					return num7;
				}
				case DataTypeCode.Unknown:
					if (ComparerUtility.IsNonNumericVariant(dataTypeCode) || ComparerUtility.IsNonNumericVariant(dataTypeCode2))
					{
						return CompareToNonNumericVariantTypes(dataTypeCode, dataTypeCode2, x, y, throwExceptionOnComparisonFailure);
					}
					break;
				}
			}
			ICustomComparable customComparable = x as ICustomComparable;
			ICustomComparable customComparable2 = y as ICustomComparable;
			if (customComparable != null && customComparable2 != null)
			{
				return customComparable.CompareTo(customComparable2, this);
			}
			IComparable left = (IComparable)x;
			IComparable right = (IComparable)y;
			return Compare(left, right, throwExceptionOnComparisonFailure);
		}

		private int Compare(IComparable left, IComparable right, bool throwExceptionOnComparisonFailure)
		{
			if (left == right)
			{
				return 0;
			}
			try
			{
				return left.CompareTo(right);
			}
			catch (ArgumentException)
			{
				if (throwExceptionOnComparisonFailure)
				{
					throw new CommonDataComparerException(left.GetType().ToString(), right.GetType().ToString());
				}
				return -1;
			}
		}

		private static int CompareNumericDateVariantTypes(DataTypeCode x, DataTypeCode y, bool throwExceptionOnComparisonFailure)
		{
			switch (x)
			{
			case DataTypeCode.DateTime:
				return 1;
			case DataTypeCode.Double:
				if (ComparerUtility.IsLessThanReal(y))
				{
					return 1;
				}
				return -1;
			case DataTypeCode.Decimal:
				if (ComparerUtility.IsLessThanCurrency(y))
				{
					return 1;
				}
				return -1;
			case DataTypeCode.Int64:
				if (ComparerUtility.IsLessThanInt64(y))
				{
					return 1;
				}
				return -1;
			case DataTypeCode.Int32:
				return -1;
			default:
				if (throwExceptionOnComparisonFailure)
				{
					throw new CommonDataComparerException(x.ToString(), y.ToString());
				}
				return -1;
			}
		}

		private static int CompareToNonNumericVariantTypes(DataTypeCode xDataType, DataTypeCode yDataType, object x, object y, bool throwExceptionOnComparisonFailure)
		{
			if (ComparerUtility.IsNumericDateVariant(xDataType) && ComparerUtility.IsNonNumericVariant(yDataType))
			{
				return -1;
			}
			if (ComparerUtility.IsNonNumericVariant(xDataType) && ComparerUtility.IsNumericDateVariant(yDataType))
			{
				return 1;
			}
			if (xDataType == DataTypeCode.String && yDataType == DataTypeCode.Boolean)
			{
				return -1;
			}
			if (xDataType == DataTypeCode.Boolean && yDataType == DataTypeCode.String)
			{
				return 1;
			}
			if (throwExceptionOnComparisonFailure)
			{
				throw new CommonDataComparerException(x.ToString(), y.ToString());
			}
			return -1;
		}
	}
}
