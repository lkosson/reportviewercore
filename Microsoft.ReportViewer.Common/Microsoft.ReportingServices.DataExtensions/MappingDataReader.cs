using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.ReportingServices.DataExtensions
{
	internal sealed class MappingDataReader : IDisposable
	{
		private string m_dataSetName;

		private IDataReader m_dataReader;

		private IDataReaderExtension m_dataReaderExtension;

		private IDataReaderFieldProperties m_dataFieldProperties;

		private int[] m_aliasIndexToFieldIndex;

		private string[] m_fieldNames;

		private readonly DataSourceErrorInspector m_errorInspector;

		public string[] FieldNames => m_fieldNames;

		public bool ReaderExtensionsSupported => m_dataReaderExtension != null;

		public bool IsAggregateRow => m_dataReaderExtension.IsAggregateRow;

		public int AggregationFieldCount => m_dataReaderExtension.AggregationFieldCount;

		public bool ReaderFieldProperties => m_dataFieldProperties != null;

		public MappingDataReader(string dataSetName, IDataReader sourceReader, string[] aliases, string[] fieldNames, DataSourceErrorInspector errorInspector)
		{
			if (sourceReader == null)
			{
				if (Global.Tracer.TraceError)
				{
					Global.Tracer.Trace(TraceLevel.Error, "The source data reader is null. Cannot read results.");
				}
				throw new ReportProcessingException(ErrorCode.rsErrorCreatingDataReader, dataSetName.MarkAsPrivate());
			}
			Global.Tracer.Assert(aliases != null, "(null != aliases)");
			m_dataSetName = dataSetName;
			m_dataReader = sourceReader;
			m_fieldNames = fieldNames;
			m_errorInspector = errorInspector;
			m_dataReaderExtension = (sourceReader as IDataReaderExtension);
			m_dataFieldProperties = (sourceReader as IDataReaderFieldProperties);
			if (fieldNames == null)
			{
				if (Global.Tracer.TraceInfo)
				{
					Global.Tracer.Trace(TraceLevel.Info, "The array of field names is null. Aliases will map positionally to the data reader fields.");
				}
				for (int i = 0; i < aliases.Length; i++)
				{
					m_aliasIndexToFieldIndex[i] = i;
				}
				return;
			}
			Global.Tracer.Assert(aliases.Length == fieldNames.Length, " (aliases.Length == fieldNames.Length)");
			m_aliasIndexToFieldIndex = new int[aliases.Length];
			if (fieldNames == null)
			{
				if (Global.Tracer.TraceWarning)
				{
					Global.Tracer.Trace(TraceLevel.Warning, "The data reader does not have any fields.");
				}
				for (int j = 0; j < aliases.Length; j++)
				{
					m_aliasIndexToFieldIndex[j] = -1;
				}
				return;
			}
			for (int k = 0; k < aliases.Length; k++)
			{
				string text = fieldNames[k];
				if (text != null)
				{
					int num;
					try
					{
						num = m_dataReader.GetOrdinal(text);
					}
					catch (RSException)
					{
						throw;
					}
					catch (Exception ex2)
					{
						if (AsynchronousExceptionDetection.IsStoppingException(ex2))
						{
							throw;
						}
						Global.Tracer.Trace(TraceLevel.Warning, "An exception occurred while trying to map a data set field.  Field: '{0}' DataField: '{1}' Details: {2}", aliases[k].MarkAsModelInfo(), fieldNames[k].MarkAsModelInfo(), ex2.Message);
						num = -1;
					}
					m_aliasIndexToFieldIndex[k] = num;
				}
				else
				{
					m_aliasIndexToFieldIndex[k] = -1;
					m_fieldNames[k] = aliases[k];
				}
			}
			if (Global.Tracer.TraceVerbose)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "Mapping data reader successfully initialized.");
			}
		}

		public bool GetNextRow()
		{
			try
			{
				return m_dataReader.Read();
			}
			catch (Exception ex)
			{
				if (m_errorInspector != null && m_errorInspector.TryInterpretProviderErrorCode(ex, out ErrorCode errorCode))
				{
					string text = string.Format(CultureInfo.CurrentCulture, RPRes.Keys.GetString(ErrorCode.rsErrorReadingNextDataRow.ToString()), m_dataSetName);
					throw new ReportProcessingQueryException(errorCode, ex, text);
				}
				throw new ReportProcessingException(ErrorCode.rsErrorReadingNextDataRow, ex, m_dataSetName.MarkAsPrivate());
			}
		}

		private void GenerateFieldErrorException(Exception e)
		{
			if (e == null)
			{
				throw new ReportProcessingException_FieldError(DataFieldStatus.IsError, null);
			}
			if (e is ReportProcessingException)
			{
				throw new ReportProcessingException_FieldError(DataFieldStatus.IsMissing, e.Message);
			}
			if (e is OverflowException)
			{
				throw new ReportProcessingException_FieldError(DataFieldStatus.Overflow, e.ToString());
			}
			if (e is NotSupportedException)
			{
				throw new ReportProcessingException_FieldError(DataFieldStatus.UnSupportedDataType, null);
			}
			throw new ReportProcessingException_FieldError(DataFieldStatus.IsError, e.ToString());
		}

		public object GetFieldValue(int aliasIndex)
		{
			try
			{
				return m_dataReader.GetValue(GetFieldIndex(aliasIndex));
			}
			catch (Exception e)
			{
				GenerateFieldErrorException(e);
			}
			return null;
		}

		public bool IsAggregationField(int aliasIndex)
		{
			try
			{
				Global.Tracer.Assert(m_dataReaderExtension != null, "(null != m_dataReaderExtension)");
				return m_dataReaderExtension.IsAggregationField(GetFieldIndex(aliasIndex));
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(ErrorCode.rsErrorReadingDataAggregationField, innerException, m_dataSetName.MarkAsPrivate());
			}
		}

		public int GetPropertyCount(int aliasIndex)
		{
			try
			{
				return m_dataFieldProperties.GetPropertyCount(GetFieldIndex(aliasIndex));
			}
			catch (Exception e)
			{
				GenerateFieldErrorException(e);
			}
			return 0;
		}

		public string GetPropertyName(int aliasIndex, int propertyIndex)
		{
			try
			{
				return m_dataFieldProperties.GetPropertyName(GetFieldIndex(aliasIndex), propertyIndex);
			}
			catch (Exception e)
			{
				GenerateFieldErrorException(e);
			}
			return null;
		}

		public object GetPropertyValue(int aliasIndex, int propertyIndex)
		{
			try
			{
				return m_dataFieldProperties.GetPropertyValue(GetFieldIndex(aliasIndex), propertyIndex);
			}
			catch (Exception e)
			{
				GenerateFieldErrorException(e);
			}
			return null;
		}

		void IDisposable.Dispose()
		{
			if (Global.Tracer.TraceVerbose)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "Closing the data reader in Dispose().");
			}
			if (m_dataReader != null)
			{
				m_dataReader.Dispose();
			}
			m_dataReader = null;
			m_aliasIndexToFieldIndex = null;
		}

		private int GetFieldIndex(int aliasIndex)
		{
			int num = -1;
			try
			{
				num = m_aliasIndexToFieldIndex[aliasIndex];
			}
			catch (IndexOutOfRangeException)
			{
			}
			if (num < 0)
			{
				throw new ReportProcessingException(ErrorCode.rsNoFieldDataAtIndex, aliasIndex + 1);
			}
			return num;
		}
	}
}
