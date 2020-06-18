using Microsoft.ReportingServices.Diagnostics.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal sealed class AggregatesImpl : Aggregates
	{
		private bool m_lockAdd;

		private Hashtable m_collection;

		private Hashtable m_duplicateNames;

		private IErrorContext m_iErrorContext;

		internal const string Name = "Aggregates";

		internal const string FullName = "Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.Aggregates";

		public override object this[string key]
		{
			get
			{
				DataAggregateObj aggregateObj = GetAggregateObj(key);
				if (aggregateObj == null && m_duplicateNames != null)
				{
					string text = (string)m_duplicateNames[key];
					if (text != null)
					{
						aggregateObj = GetAggregateObj(text);
					}
				}
				if (aggregateObj == null)
				{
					return null;
				}
				aggregateObj.UsedInExpression = true;
				DataAggregateObjResult dataAggregateObjResult = aggregateObj.AggregateResult();
				if (dataAggregateObjResult.HasCode)
				{
					if (dataAggregateObjResult.FieldStatus == DataFieldStatus.None && dataAggregateObjResult.Code != 0)
					{
						m_iErrorContext.Register(dataAggregateObjResult.Code, dataAggregateObjResult.Severity, dataAggregateObjResult.Arguments);
					}
					if (dataAggregateObjResult.ErrorOccurred)
					{
						throw new ReportProcessingException_InvalidOperationException();
					}
				}
				if (dataAggregateObjResult.ErrorOccurred)
				{
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				return dataAggregateObjResult.Value;
			}
		}

		internal ICollection Objects => m_collection.Values;

		internal AggregatesImpl(IErrorContext iErrorContext)
			: this(lockAdd: false, iErrorContext)
		{
		}

		internal AggregatesImpl(bool lockAdd, IErrorContext iErrorContext)
		{
			m_lockAdd = lockAdd;
			m_collection = new Hashtable();
			m_duplicateNames = null;
			m_iErrorContext = iErrorContext;
		}

		internal void Add(DataAggregateObj newObject)
		{
			Global.Tracer.Assert(!newObject.NonAggregateMode, "( !newObject.NonAggregateMode )");
			try
			{
				if (m_lockAdd)
				{
					Monitor.Enter(m_collection);
				}
				m_collection.Add(newObject.Name, newObject);
				PopulateDuplicateNames(newObject.Name, newObject.DuplicateNames);
			}
			finally
			{
				if (m_lockAdd)
				{
					Monitor.Exit(m_collection);
				}
			}
		}

		internal void Set(string name, DataAggregateInfo aggregateDef, StringList duplicateNames, DataAggregateObjResult aggregateResult)
		{
			DataAggregateObj aggregateObj = GetAggregateObj(name);
			if (aggregateObj == null)
			{
				try
				{
					if (m_lockAdd)
					{
						Monitor.Enter(m_collection);
					}
					aggregateObj = new DataAggregateObj(aggregateDef, aggregateResult);
					m_collection.Add(name, aggregateObj);
					PopulateDuplicateNames(name, duplicateNames);
				}
				finally
				{
					if (m_lockAdd)
					{
						Monitor.Exit(m_collection);
					}
				}
			}
			else
			{
				aggregateObj.Set(aggregateResult);
			}
		}

		internal DataAggregateObj GetAggregateObj(string name)
		{
			return (DataAggregateObj)m_collection[name];
		}

		private void PopulateDuplicateNames(string name, StringList duplicateNames)
		{
			if (duplicateNames != null && 0 < duplicateNames.Count)
			{
				if (m_duplicateNames == null)
				{
					m_duplicateNames = new Hashtable();
				}
				for (int i = 0; i < duplicateNames.Count; i++)
				{
					m_duplicateNames[duplicateNames[i]] = name;
				}
			}
		}

		internal void ResetUsedInExpression()
		{
			foreach (DataAggregateObj value in m_collection.Values)
			{
				value.UsedInExpression = false;
			}
		}

		internal void AddFieldsUsedInExpression(List<string> fieldsUsedInValueExpression)
		{
			foreach (DataAggregateObj value in m_collection.Values)
			{
				if (value.UsedInExpression && value.AggregateDef != null && value.AggregateDef.FieldsUsedInValueExpression != null)
				{
					fieldsUsedInValueExpression.AddRange(value.AggregateDef.FieldsUsedInValueExpression);
				}
			}
		}
	}
}
