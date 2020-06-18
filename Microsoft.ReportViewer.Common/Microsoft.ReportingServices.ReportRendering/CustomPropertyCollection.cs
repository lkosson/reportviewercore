using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class CustomPropertyCollection
	{
		private DataValueInstanceList m_instances;

		private DataValueList m_expressions;

		private bool m_isCustomControl;

		private bool m_populated;

		private Hashtable m_uniqueNames;

		private IntList m_expressionIndex;

		public CustomProperty this[string name]
		{
			get
			{
				if (name == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "name");
				}
				if (!m_populated && !m_isCustomControl)
				{
					Populate();
				}
				object obj = m_uniqueNames[name];
				if (obj != null && obj is int)
				{
					GetNameValue((int)obj, out string name2, out object value);
					return new CustomProperty(name2, value);
				}
				return null;
			}
		}

		public CustomProperty this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				string name = null;
				object value = null;
				if (IsCustomControl)
				{
					name = m_instances[index].Name;
					value = m_instances[index].Value;
				}
				else
				{
					if (!m_populated)
					{
						Populate();
					}
					Global.Tracer.Assert(m_expressionIndex.Count <= m_expressions.Count && index <= m_expressionIndex.Count);
					GetNameValue(m_expressionIndex[index], out name, out value);
				}
				return new CustomProperty(name, value);
			}
		}

		public int Count
		{
			get
			{
				if (!m_populated && !m_isCustomControl)
				{
					Populate();
				}
				return m_uniqueNames.Count;
			}
		}

		internal bool IsCustomControl => m_isCustomControl;

		public CustomPropertyCollection()
		{
			m_isCustomControl = true;
			m_instances = new DataValueInstanceList();
			m_uniqueNames = new Hashtable();
		}

		internal CustomPropertyCollection(DataValueList expressions, DataValueInstanceList instances)
		{
			m_expressions = expressions;
			m_instances = instances;
			Global.Tracer.Assert(m_expressions != null);
			Global.Tracer.Assert(m_instances == null || m_instances.Count == m_expressions.Count);
			m_uniqueNames = new Hashtable(m_expressions.Count);
			m_expressionIndex = new IntList(m_expressions.Count);
		}

		public void Add(string propertyName, object propertyValue)
		{
			if (!m_isCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			InternalAdd(propertyName, propertyValue);
		}

		public void Add(CustomProperty property)
		{
			if (!m_isCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			if (property == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "property");
			}
			InternalAdd(property.Name, property.Value);
		}

		internal CustomPropertyCollection DeepClone()
		{
			Global.Tracer.Assert(m_isCustomControl && m_expressions == null);
			CustomPropertyCollection customPropertyCollection = new CustomPropertyCollection();
			if (m_instances != null)
			{
				int count = m_instances.Count;
				customPropertyCollection.m_instances = new DataValueInstanceList(count);
				for (int i = 0; i < count; i++)
				{
					customPropertyCollection.m_instances.Add(m_instances[i].DeepClone());
				}
			}
			return customPropertyCollection;
		}

		private void Populate()
		{
			Global.Tracer.Assert(!m_isCustomControl);
			int count = m_expressions.Count;
			for (int i = 0; i < count; i++)
			{
				GetNameValue(i, out string name, out object _);
				if (name != null && !m_uniqueNames.ContainsKey(name))
				{
					m_uniqueNames.Add(name, i);
					m_expressionIndex.Add(i);
				}
			}
		}

		internal void GetNameValue(int index, out string name, out object value)
		{
			name = null;
			value = null;
			Global.Tracer.Assert(0 <= index && index < m_expressions.Count);
			if (ExpressionInfo.Types.Constant == m_expressions[index].Name.Type)
			{
				name = m_expressions[index].Name.Value;
			}
			else if (m_instances != null)
			{
				name = m_instances[index].Name;
			}
			if (ExpressionInfo.Types.Constant == m_expressions[index].Value.Type)
			{
				value = m_expressions[index].Value.Value;
			}
			else if (m_instances != null)
			{
				value = m_instances[index].Value;
			}
		}

		internal void GetNameValueExpressions(int index, out ExpressionInfo nameExpression, out ExpressionInfo valueExpression, out string name, out object value)
		{
			GetNameValue(index, out name, out value);
			nameExpression = m_expressions[index].Name;
			valueExpression = m_expressions[index].Value;
		}

		private void InternalAdd(string name, object value)
		{
			DataValueInstance dataValueInstance = new DataValueInstance();
			dataValueInstance.Name = name;
			dataValueInstance.Value = value;
			m_uniqueNames.Add(name, dataValueInstance);
			m_instances.Add(dataValueInstance);
		}

		internal DataValueInstanceList Deconstruct()
		{
			return m_instances;
		}
	}
}
