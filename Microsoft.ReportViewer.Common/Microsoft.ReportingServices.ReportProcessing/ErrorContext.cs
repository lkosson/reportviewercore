using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal abstract class ErrorContext
	{
		protected bool m_hasError;

		protected bool m_suspendErrors;

		protected ProcessingMessageList m_messages;

		internal bool HasError
		{
			get
			{
				return m_hasError;
			}
			set
			{
				m_hasError = value;
			}
		}

		internal bool SuspendErrors
		{
			get
			{
				return m_suspendErrors;
			}
			set
			{
				m_suspendErrors = value;
			}
		}

		internal ProcessingMessageList Messages => m_messages;

		internal int MessageCount
		{
			get
			{
				if (m_messages != null)
				{
					return m_messages.Count;
				}
				return 0;
			}
		}

		internal abstract ProcessingMessage Register(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, params string[] arguments);

		internal abstract ProcessingMessage Register(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, ProcessingMessageList innerMessages, params string[] arguments);

		internal virtual void Register(RSException rsException, ObjectType objectType)
		{
			if (m_suspendErrors)
			{
				return;
			}
			m_hasError = true;
			if (m_messages == null)
			{
				m_messages = new ProcessingMessageList();
			}
			ProcessingMessage processingMessage = CreateProcessingMessage(rsException.Code, objectType, rsException.Message);
			m_messages.Add(processingMessage);
			for (rsException = (rsException.InnerException as RSException); rsException != null; rsException = (rsException.InnerException as RSException))
			{
				ProcessingMessage processingMessage2 = CreateProcessingMessage(rsException.Code, objectType, rsException.Message);
				if (processingMessage.ProcessingMessages == null)
				{
					processingMessage.ProcessingMessages = new ProcessingMessageList(1);
				}
				processingMessage.ProcessingMessages.Add(processingMessage2);
				processingMessage = processingMessage2;
			}
		}

		internal static ProcessingMessage CreateProcessingMessage(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, ProcessingMessageList innerMessages, params string[] arguments)
		{
			objectName = objectName.MarkAsPrivate();
			propertyName = propertyName.MarkAsPrivate();
			object[] messageArgs = GetMessageArgs(objectType, objectName, propertyName, arguments);
			string message = string.Format(CultureInfo.CurrentCulture, RPRes.Keys.GetString(code.ToString()), messageArgs);
			return new ProcessingMessage(code, severity, objectType, objectName, propertyName, message, innerMessages);
		}

		protected ProcessingMessage CreateProcessingMessage(ErrorCode code, ObjectType objectType, string messageString)
		{
			return new ProcessingMessage(code, Severity.Error, objectType, null, null, messageString, null);
		}

		private static object[] GetMessageArgs(ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			object[] array = new object[3 + ((arguments != null) ? arguments.Length : 0)];
			array[0] = GetLocalizedObjectTypeString(objectType);
			array[1] = GetLocalizedObjectNameString(objectType, objectName);
			array[2] = GetLocalizedPropertyNameString(propertyName);
			if (arguments != null)
			{
				for (int i = 0; i < arguments.Length; i++)
				{
					array[3 + i] = arguments[i];
				}
			}
			return array;
		}

		internal static string GetLocalizedObjectTypeString(ObjectType objectType)
		{
			switch (objectType)
			{
			case ObjectType.Report:
				return RPRes.rsObjectTypeReport;
			case ObjectType.PageHeader:
			case ObjectType.PageFooter:
				return RPRes.rsObjectTypePage;
			case ObjectType.Line:
				return RPRes.rsObjectTypeLine;
			case ObjectType.Rectangle:
				return RPRes.rsObjectTypeRectangle;
			case ObjectType.Checkbox:
				return RPRes.rsObjectTypeCheckbox;
			case ObjectType.Textbox:
				return RPRes.rsObjectTypeTextbox;
			case ObjectType.Image:
				return RPRes.rsObjectTypeImage;
			case ObjectType.Subreport:
				return RPRes.rsObjectTypeSubreport;
			case ObjectType.ActiveXControl:
				return RPRes.rsObjectTypeActiveXControl;
			case ObjectType.List:
				return RPRes.rsObjectTypeList;
			case ObjectType.Matrix:
				return RPRes.rsObjectTypeMatrix;
			case ObjectType.Table:
				return RPRes.rsObjectTypeTable;
			case ObjectType.OWCChart:
				return RPRes.rsObjectTypeOWCChart;
			case ObjectType.Grouping:
				return RPRes.rsObjectTypeGrouping;
			case ObjectType.ReportParameter:
				return RPRes.rsObjectTypeReportParameter;
			case ObjectType.DataSource:
				return RPRes.rsObjectTypeDataSource;
			case ObjectType.DataSet:
				return RPRes.rsObjectTypeDataSet;
			case ObjectType.Field:
				return RPRes.rsObjectTypeField;
			case ObjectType.Query:
				return RPRes.rsObjectTypeQuery;
			case ObjectType.QueryParameter:
				return RPRes.rsObjectTypeQueryParameter;
			case ObjectType.EmbeddedImage:
				return RPRes.rsObjectTypeEmbeddedImage;
			case ObjectType.ReportItem:
				return RPRes.rsObjectTypeReportItem;
			case ObjectType.Subtotal:
				return RPRes.rsObjectTypeSubtotal;
			case ObjectType.CodeClass:
				return RPRes.rsObjectTypeCodeClass;
			case ObjectType.CustomReportItem:
				return RPRes.rsObjectTypeCustomReportItem;
			case ObjectType.Chart:
				return RPRes.rsObjectTypeChart;
			case ObjectType.GaugePanel:
				return RPRes.rsObjectTypeGaugePanel;
			case ObjectType.Map:
				return RPRes.rsObjectTypeMap;
			case ObjectType.MapDataRegion:
				return RPRes.rsObjectTypeMapDataRegion;
			case ObjectType.Tablix:
				return RPRes.rsObjectTypeTablix;
			case ObjectType.Page:
				return RPRes.rsObjectTypePage;
			case ObjectType.Paragraph:
				return RPRes.rsObjectTypeParagraph;
			case ObjectType.TextRun:
				return RPRes.rsObjectTypeTextRun;
			case ObjectType.ReportSection:
				return RPRes.rsObjectTypeReportSection;
			case ObjectType.SharedDataSet:
				return RPRes.rsObjectTypeSharedDataSet;
			case ObjectType.TablixCell:
				return RPRes.rsObjectTypeTablixCell;
			case ObjectType.GaugeCell:
				return RPRes.rsObjectTypeGaugeCell;
			case ObjectType.MapCell:
				return RPRes.rsObjectTypeMapCell;
			case ObjectType.DataCell:
				return RPRes.rsObjectTypeDataCell;
			case ObjectType.ChartDataPoint:
				return RPRes.rsObjectTypeChartDataPoint;
			case ObjectType.DataShape:
				return RPRes.rsObjectTypeDataShape;
			case ObjectType.DataShapeMember:
				return RPRes.rsObjectTypeDataShapeMember;
			case ObjectType.DataShapeIntersection:
				return RPRes.rsObjectTypeDataShapeIntersection;
			case ObjectType.DataBinding:
				return RPRes.rsObjectTypeDataBinding;
			case ObjectType.Calculation:
				return RPRes.rsObjectTypeCalculation;
			case ObjectType.ParameterLayout:
				return RPRes.rsObjectTypeParameterLayout;
			default:
				Global.Tracer.Assert(condition: false);
				return null;
			}
		}

		private static string GetLocalizedObjectNameString(ObjectType objectType, string objectName)
		{
			switch (objectType)
			{
			case ObjectType.Report:
				return RPRes.rsObjectNameBody;
			case ObjectType.PageHeader:
				return RPRes.rsObjectNameHeader;
			case ObjectType.PageFooter:
				return RPRes.rsObjectNameFooter;
			default:
				return objectName;
			}
		}

		private static string GetLocalizedPropertyNameString(string propertyName)
		{
			return propertyName;
		}
	}
}
