using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class DataValueList : ArrayList
	{
		internal new DataValue this[int index] => (DataValue)base[index];

		internal DataValueList()
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

		internal void Initialize(string prefix, bool isCustomProperty, InitializationContext context)
		{
			Initialize(prefix, -1, -1, isCustomProperty, context);
		}

		internal void Initialize(string prefix, int rowIndex, int cellIndex, bool isCustomProperty, InitializationContext context)
		{
			int count = Count;
			CustomPropertyUniqueNameValidator validator = new CustomPropertyUniqueNameValidator();
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

		internal DataValueInstanceList EvaluateExpressions(ObjectType objectType, string objectName, string prefix, ReportProcessing.ProcessingContext pc)
		{
			return EvaluateExpressions(objectType, objectName, prefix, -1, -1, pc);
		}

		internal DataValueInstanceList EvaluateExpressions(ObjectType objectType, string objectName, string prefix, int rowIndex, int cellIndex, ReportProcessing.ProcessingContext pc)
		{
			int count = Count;
			DataValueInstanceList dataValueInstanceList = new DataValueInstanceList(count);
			bool flag = rowIndex < 0;
			CustomPropertyUniqueNameValidator customPropertyUniqueNameValidator = null;
			if (flag)
			{
				customPropertyUniqueNameValidator = new CustomPropertyUniqueNameValidator();
			}
			for (int i = 0; i < count; i++)
			{
				DataValue dataValue = this[i];
				DataValueInstance dataValueInstance = new DataValueInstance();
				bool flag2 = true;
				string propertyNameValue = null;
				if (dataValue.Name != null)
				{
					if (ExpressionInfo.Types.Constant != dataValue.Name.Type)
					{
						dataValueInstance.Name = pc.ReportRuntime.EvaluateDataValueNameExpression(dataValue, objectType, objectName, CreatePropertyNameString(prefix, rowIndex + 1, cellIndex + 1, i + 1) + ".Name");
						propertyNameValue = dataValueInstance.Name;
					}
					else
					{
						propertyNameValue = dataValue.Name.Value;
					}
				}
				if (flag)
				{
					flag2 = customPropertyUniqueNameValidator.Validate(Severity.Warning, objectType, objectName, propertyNameValue, pc.ErrorContext);
				}
				if (flag2)
				{
					Global.Tracer.Assert(dataValue.Value != null);
					if (ExpressionInfo.Types.Constant != dataValue.Value.Type)
					{
						dataValueInstance.Value = pc.ReportRuntime.EvaluateDataValueValueExpression(dataValue, objectType, objectName, CreatePropertyNameString(prefix, rowIndex + 1, cellIndex + 1, i + 1) + ".Value");
					}
				}
				dataValueInstanceList.Add(dataValueInstance);
			}
			return dataValueInstanceList;
		}

		internal DataValueList DeepClone(InitializationContext context)
		{
			int count = Count;
			DataValueList dataValueList = new DataValueList(count);
			for (int i = 0; i < count; i++)
			{
				dataValueList.Add(this[i].DeepClone(context));
			}
			return dataValueList;
		}
	}
}
