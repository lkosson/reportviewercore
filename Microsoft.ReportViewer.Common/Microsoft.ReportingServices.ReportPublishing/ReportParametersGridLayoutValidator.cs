using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportPublishing
{
	internal sealed class ReportParametersGridLayoutValidator
	{
		private const int MaxNumberOfRows = 10000;

		private const int MaxNumberOfColumns = 8;

		private const int MaxNumberOfConsecutiveEmptyRows = 20;

		private readonly string[] NoArguments = new string[0];

		private const string NoObjectName = "";

		private const string NoPropertyName = "";

		private List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef> m_parameters;

		private ParametersGridLayout m_paramsLayout;

		private PublishingErrorContext m_errorContext;

		private readonly HashSet<string> m_parameterNames = new HashSet<string>();

		private readonly HashSet<string> m_gridParameterNames = new HashSet<string>();

		private readonly HashSet<long> m_cellAddresses = new HashSet<long>();

		private readonly SortedList<int, bool> m_parameterRowIndexes = new SortedList<int, bool>();

		private int NumberOfRows => m_paramsLayout.NumberOfRows;

		private int NumberOfColumns => m_paramsLayout.NumberOfColumns;

		private ReportParametersGridLayoutValidator(List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef> parameters, ParametersGridLayout paramsLayout, PublishingErrorContext errorContext)
		{
			RSTrace.ProcessingTracer.Assert(paramsLayout != null, "paramsLayout should not be null");
			m_paramsLayout = paramsLayout;
			m_parameters = parameters;
			m_errorContext = errorContext;
			InitParameterNames();
		}

		public static bool Validate(List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef> parameters, ParametersGridLayout paramsLayout, PublishingErrorContext errorContext)
		{
			return new ReportParametersGridLayoutValidator(parameters, paramsLayout, errorContext).Validate();
		}

		private bool Validate()
		{
			if (m_parameterNames.Count == 0)
			{
				if (ValidateNumberOfRows() && ValidateNumberOfColumns())
				{
					return ValidateGridCells();
				}
				return false;
			}
			if (ValidateNumberOfRows() && ValidateNumberOfColumns() && ValidateParametersCount() && ValidateGridCells())
			{
				return ValidateConsecutiveEmptyRowCount();
			}
			return false;
		}

		private void InitParameterNames()
		{
			foreach (Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef parameter in m_parameters)
			{
				m_parameterNames.Add(parameter.Name);
			}
		}

		private static bool IsParamVisible(Microsoft.ReportingServices.ReportIntermediateFormat.ParameterDef param)
		{
			return param.Prompt.Length > 0;
		}

		private bool ValidateNumberOfRows()
		{
			if (ValidatePrerequisite(NumberOfRows > 0, ProcessingErrorCode.rsInvalidParameterLayoutZeroOrLessRowOrCol))
			{
				return ValidatePrerequisite(NumberOfRows <= 10000, ProcessingErrorCode.rsInvalidParameterLayoutNumberOfRowsOrColumnsExceedingLimit, "", "NumberOfRows", 10000.ToString());
			}
			return false;
		}

		private bool ValidateNumberOfColumns()
		{
			if (ValidatePrerequisite(NumberOfColumns > 0, ProcessingErrorCode.rsInvalidParameterLayoutZeroOrLessRowOrCol))
			{
				return ValidatePrerequisite(NumberOfColumns <= 8, ProcessingErrorCode.rsInvalidParameterLayoutNumberOfRowsOrColumnsExceedingLimit, "", "NumberOfColumns", 8.ToString());
			}
			return false;
		}

		private bool ValidateParametersCount()
		{
			bool flag = ValidatePrerequisite(m_parameters.Count <= NumberOfRows * NumberOfColumns, ProcessingErrorCode.rsInvalidParameterLayoutParametersMissingFromPanel);
			if (m_paramsLayout.CellDefinitions != null)
			{
				return flag & ValidatePrerequisite(m_paramsLayout.CellDefinitions.Count == m_parameters.Count, ProcessingErrorCode.rsInvalidParameterLayoutCellDefNotEqualsParameterCount);
			}
			return flag & ValidatePrerequisite(m_parameters.Count == 0, ProcessingErrorCode.rsInvalidParameterLayoutCellDefNotEqualsParameterCount);
		}

		private bool ValidateGridCells()
		{
			if (m_parameterNames.Count == 0 && m_paramsLayout.CellDefinitions == null)
			{
				return true;
			}
			foreach (ParameterGridLayoutCellDefinition cellDefinition in m_paramsLayout.CellDefinitions)
			{
				if (!ValidateGridCell(cellDefinition))
				{
					return false;
				}
			}
			return true;
		}

		private bool ValidateGridCell(ParameterGridLayoutCellDefinition cell)
		{
			string parameterName = cell.ParameterName;
			long item = cell.RowIndex * NumberOfColumns + cell.ColumnIndex;
			int num = 1 & (ValidatePrerequisite(!string.IsNullOrEmpty(parameterName), ProcessingErrorCode.rsInvalidParameterLayoutParameterNameMissing) ? 1 : 0) & (ValidatePrerequisite(!m_gridParameterNames.Contains(parameterName), ProcessingErrorCode.rsInvalidParameterLayoutParameterAppearsTwice, cell.ParameterName) ? 1 : 0) & (ValidatePrerequisite(m_parameterNames.Contains(parameterName), ProcessingErrorCode.rsInvalidParameterLayoutParameterNotVisible, parameterName) ? 1 : 0) & (ValidatePrerequisite(cell.RowIndex >= 0 && cell.RowIndex < NumberOfRows, ProcessingErrorCode.rsInvalidParameterLayoutRowColOutOfBounds) ? 1 : 0) & (ValidatePrerequisite(cell.ColumnIndex >= 0 && cell.ColumnIndex < NumberOfColumns, ProcessingErrorCode.rsInvalidParameterLayoutRowColOutOfBounds) ? 1 : 0) & (ValidatePrerequisite(!m_cellAddresses.Contains(item), ProcessingErrorCode.rsInvalidParameterLayoutCellCollition) ? 1 : 0);
			if (num != 0)
			{
				m_parameterRowIndexes[cell.RowIndex] = true;
				m_gridParameterNames.Add(parameterName);
				m_cellAddresses.Add(item);
			}
			return (byte)num != 0;
		}

		private bool ValidateConsecutiveEmptyRowCount()
		{
			return ValidatePrerequisite(!DoesNumberOfConsecutiveEmptyRowsExceedLimit(), ProcessingErrorCode.rsInvalidParameterLayoutNumberOfConsecutiveEmptyRowsExceedingLimit, "", "", 20.ToString());
		}

		private bool ValidatePrerequisite(bool condition, ProcessingErrorCode errorCode)
		{
			return ValidatePrerequisite(condition, errorCode, "", "", NoArguments);
		}

		private bool ValidatePrerequisite(bool condition, ProcessingErrorCode errorCode, string objectName)
		{
			return ValidatePrerequisite(condition, errorCode, objectName, "", NoArguments);
		}

		private bool ValidatePrerequisite(bool condition, ProcessingErrorCode errorCode, string objectName, string propertyName, params string[] arguments)
		{
			if (!condition)
			{
				m_errorContext.Register(errorCode, Severity.Error, ObjectType.ParameterLayout, objectName, propertyName, arguments);
			}
			return condition;
		}

		private bool DoesNumberOfConsecutiveEmptyRowsExceedLimit()
		{
			int num = 0;
			foreach (int key in m_parameterRowIndexes.Keys)
			{
				if (key - num - 1 > 20)
				{
					return true;
				}
				num = key;
			}
			return NumberOfRows - 1 - num - 1 > 20;
		}
	}
}
