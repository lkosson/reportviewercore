using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.ReportingServices.DataExtensions
{
	internal class CommandWrapper : BaseDataWrapper, Microsoft.ReportingServices.DataProcessing.IDbCommand, IDisposable
	{
		public enum ServerType
		{
			AnalysisServices,
			SQLServer,
			Teradata,
			Oracle,
			SQLServerDataWarehouse
		}

		private const string OracleCommandTypeFullName = "Oracle.ManagedDataAccess.Client.OracleCommand";

		private const string OracleCommandBuilderTypeFullName = "Oracle.ManagedDataAccess.Client.OracleCommandBuilder";

		private TransactionWrapper m_transactionWrapper;

		protected ParameterCollectionWrapper m_parameterCollection;

		public virtual string CommandText
		{
			get
			{
				return UnderlyingCommand.CommandText;
			}
			set
			{
				UnderlyingCommand.CommandText = value;
			}
		}

		public virtual int CommandTimeout
		{
			get
			{
				return UnderlyingCommand.CommandTimeout;
			}
			set
			{
				UnderlyingCommand.CommandTimeout = value;
			}
		}

		public virtual Microsoft.ReportingServices.DataProcessing.CommandType CommandType
		{
			get
			{
				return (Microsoft.ReportingServices.DataProcessing.CommandType)UnderlyingCommand.CommandType;
			}
			set
			{
				UnderlyingCommand.CommandType = (System.Data.CommandType)value;
			}
		}

		public virtual Microsoft.ReportingServices.DataProcessing.IDataParameterCollection Parameters
		{
			get
			{
				if (m_parameterCollection == null)
				{
					m_parameterCollection = CreateParameterCollection();
				}
				return m_parameterCollection;
			}
		}

		public virtual Microsoft.ReportingServices.DataProcessing.IDbTransaction Transaction
		{
			get
			{
				return m_transactionWrapper;
			}
			set
			{
				m_transactionWrapper = (TransactionWrapper)value;
				UnderlyingCommand.Transaction = m_transactionWrapper.UnderlyingTransaction;
			}
		}

		public System.Data.IDbCommand UnderlyingCommand => (System.Data.IDbCommand)base.UnderlyingObject;

		protected internal CommandWrapper(System.Data.IDbCommand command)
			: base(command)
		{
		}

		protected internal virtual void SaveCommandObject(out ArrayList sysParameters, out int sysParameterCount, out string sysCommandText)
		{
			System.Data.IDbCommand underlyingCommand = UnderlyingCommand;
			sysParameterCount = ((underlyingCommand.Parameters != null) ? underlyingCommand.Parameters.Count : 0);
			sysCommandText = string.Copy(underlyingCommand.CommandText);
			if (sysParameterCount == 0)
			{
				sysParameters = null;
				return;
			}
			sysParameters = new ArrayList(sysParameterCount);
			ParameterCollectionWrapper.Enumerator enumerator = m_parameterCollection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterWrapper current = enumerator.Current;
				sysParameters.Add(current);
			}
		}

		protected internal virtual void RestoreCommandObject(ArrayList sysParameters, int sysParameterCount, string sysCommandText)
		{
			System.Data.IDbCommand underlyingCommand = UnderlyingCommand;
			underlyingCommand.CommandText = sysCommandText;
			underlyingCommand.Parameters.Clear();
			if (sysParameterCount == 0)
			{
				return;
			}
			foreach (ParameterWrapper sysParameter in sysParameters)
			{
				underlyingCommand.Parameters.Add(sysParameter.UnderlyingParameter);
			}
		}

		protected internal virtual bool RewriteMultiValueParameters(int sysParameterCount, ServerType serverType)
		{
			if (0 >= sysParameterCount)
			{
				return false;
			}
			bool result = false;
			System.Data.IDbCommand underlyingCommand = UnderlyingCommand;
			StringBuilder stringBuilder = null;
			RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline;
			string str = Regex.Escape("-+()#,:&*/\\^<=>");
			string text = "([" + str + "\\s])";
			bool flag = System.Data.CommandType.StoredProcedure == underlyingCommand.CommandType;
			if (!flag)
			{
				stringBuilder = new StringBuilder(underlyingCommand.CommandText);
			}
			int num = 0;
			ParameterCollectionWrapper.Enumerator enumerator = m_parameterCollection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterWrapper current = enumerator.Current;
				ParameterMultiValueWrapper parameterMultiValueWrapper = current as ParameterMultiValueWrapper;
				if (parameterMultiValueWrapper != null && parameterMultiValueWrapper.Values != null)
				{
					result = true;
					string value = GenerateStringFromMultiValue(parameterMultiValueWrapper, flag, serverType);
					if (flag)
					{
						current.Value = value;
						continue;
					}
					string str2 = "(?<ParameterName>" + Regex.Escape(parameterMultiValueWrapper.ParameterName) + ")";
					MatchCollection matchCollection = new Regex(text + str2 + text, options).Matches(stringBuilder.ToString());
					if (matchCollection.Count > 0)
					{
						if (StringComparer.InvariantCultureIgnoreCase.Compare(parameterMultiValueWrapper.ParameterName, "?") == 0)
						{
							if (num < matchCollection.Count)
							{
								string text2 = matchCollection[num].Result("${ParameterName}");
								if (text2 != null && 1 == text2.Length)
								{
									stringBuilder.Remove(matchCollection[num].Index + 1, text2.Length);
									stringBuilder.Insert(matchCollection[num].Index + 1, value);
								}
							}
						}
						else
						{
							for (int num2 = matchCollection.Count - 1; num2 >= 0; num2--)
							{
								string text3 = matchCollection[num2].Result("${ParameterName}");
								if (text3 != null && 1 < text3.Length)
								{
									RSTrace.DataExtensionTracer.Assert(text3.Length == parameterMultiValueWrapper.ParameterName.Length);
									stringBuilder.Remove(matchCollection[num2].Index + 1, text3.Length);
									stringBuilder.Insert(matchCollection[num2].Index + 1, value);
								}
							}
						}
					}
					if (RSTrace.DataExtensionTracer.TraceVerbose)
					{
						RSTrace.DataExtensionTracer.Trace(TraceLevel.Verbose, "Query rewrite (removed parameter): " + parameterMultiValueWrapper.ParameterName.MarkAsModelInfo());
					}
					underlyingCommand.Parameters.Remove(parameterMultiValueWrapper.UnderlyingParameter);
				}
				else if (parameterMultiValueWrapper != null && parameterMultiValueWrapper.Values == null && StringComparer.InvariantCultureIgnoreCase.Compare(parameterMultiValueWrapper.ParameterName, "?") == 0)
				{
					num++;
				}
			}
			if (!flag)
			{
				if (RSTrace.DataExtensionTracer.TraceVerbose)
				{
					RSTrace.DataExtensionTracer.Trace(TraceLevel.Verbose, "Query rewrite (original query): " + underlyingCommand.CommandText.MarkAsPrivate());
					RSTrace.DataExtensionTracer.Trace(TraceLevel.Verbose, "Query rewrite (rewritten query): " + stringBuilder.ToString().MarkAsPrivate());
				}
				underlyingCommand.CommandText = stringBuilder.ToString();
			}
			return result;
		}

		public virtual Microsoft.ReportingServices.DataProcessing.IDataReader ExecuteReader(Microsoft.ReportingServices.DataProcessing.CommandBehavior behavior)
		{
			return new DataReaderWrapper(UnderlyingCommand.ExecuteReader(ConvertCommandBehavior(behavior)));
		}

		protected System.Data.CommandBehavior ConvertCommandBehavior(Microsoft.ReportingServices.DataProcessing.CommandBehavior behavior)
		{
			if (Microsoft.ReportingServices.DataProcessing.CommandBehavior.SingleResult == behavior && !IsConnectedToAS2005OrLater())
			{
				return System.Data.CommandBehavior.Default;
			}
			return (System.Data.CommandBehavior)behavior;
		}

		private bool IsConnectedToAS2005OrLater()
		{
			if (UnderlyingCommand is OleDbCommand)
			{
				try
				{
					OleDbConnection connection = ((OleDbCommand)UnderlyingCommand).Connection;
					if (connection.Provider.Contains("MSOLAP"))
					{
						string serverVersion = connection.ServerVersion;
						if (Convert.ToInt32(serverVersion.Substring(0, serverVersion.IndexOf(".", StringComparison.Ordinal)), CultureInfo.InvariantCulture) >= 9)
						{
							return true;
						}
					}
				}
				catch (Exception)
				{
				}
			}
			return false;
		}

		protected ArrayList GetStoredProcedureParameters()
		{
			ArrayList oracleParameters;
			return GetStoredProcedureParameters(out oracleParameters);
		}

		protected ArrayList GetStoredProcedureParameters(out ArrayList oracleParameters)
		{
			oracleParameters = null;
			System.Data.IDbCommand underlyingCommand = UnderlyingCommand;
			if (System.Data.CommandType.StoredProcedure != underlyingCommand.CommandType)
			{
				return null;
			}
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList(underlyingCommand.Parameters.Count);
			foreach (System.Data.IDataParameter parameter in underlyingCommand.Parameters)
			{
				arrayList2.Add(parameter);
			}
			underlyingCommand.Parameters.Clear();
			string commandText = underlyingCommand.CommandText;
			if (commandText.Length >= 2 && commandText[0] == '[' && commandText[commandText.Length - 1] == ']')
			{
				underlyingCommand.CommandText = commandText.Substring(1, commandText.Length - 2);
			}
			if (underlyingCommand is SqlCommand)
			{
				SqlCommandBuilder.DeriveParameters((SqlCommand)underlyingCommand);
			}
			if (underlyingCommand.CommandText != commandText)
			{
				underlyingCommand.CommandText = commandText;
			}
			foreach (System.Data.IDataParameter parameter2 in underlyingCommand.Parameters)
			{
				if (0 == 0 && (parameter2.Direction == ParameterDirection.Input || parameter2.Direction == ParameterDirection.InputOutput))
				{
					arrayList.Add(parameter2);
				}
			}
			underlyingCommand.Parameters.Clear();
			foreach (System.Data.IDataParameter item in arrayList2)
			{
				underlyingCommand.Parameters.Add(item);
			}
			return arrayList;
		}

		public virtual Microsoft.ReportingServices.DataProcessing.IDataParameter CreateParameter()
		{
			return new ParameterWrapper(UnderlyingCommand.CreateParameter());
		}

		public virtual void Cancel()
		{
			if (UnderlyingCommand != null && UnderlyingCommand.Connection != null && UnderlyingCommand.Connection.State != 0 && UnderlyingCommand.Connection.State != ConnectionState.Broken)
			{
				UnderlyingCommand.Cancel();
			}
			else if (RSTrace.DataExtensionTracer.TraceWarning)
			{
				RSTrace.DataExtensionTracer.Trace(TraceLevel.Warning, "CommandWrapper.Cancel not called, connection is not valid");
			}
		}

		protected static string GenerateStringFromMultiValue(ParameterMultiValueWrapper parameter, bool isStoredProcedure, ServerType serverType)
		{
			RSTrace.DataExtensionTracer.Assert(parameter != null);
			StringBuilder stringBuilder = new StringBuilder();
			int num = parameter.Values.Length;
			for (int i = 0; i < num; i++)
			{
				object obj = parameter.Values[i];
				StringBuilder stringBuilder2 = null;
				if (obj == null)
				{
					throw new InvalidOperationException("Multi value query parameters cannot contain NULL");
				}
				if (obj is byte[])
				{
					throw new InvalidOperationException("Multi value query parameters cannot contain byte arrays");
				}
				stringBuilder2 = ((obj is byte || obj is sbyte) ? new StringBuilder(((int)obj).ToString(CultureInfo.InvariantCulture)) : ((obj is bool) ? new StringBuilder(((bool)obj) ? "1" : "0") : ((obj is DateTime && serverType == ServerType.Teradata) ? new StringBuilder(((IFormattable)obj).ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)) : ((!(obj is IFormattable)) ? new StringBuilder(obj.ToString()) : new StringBuilder(((IFormattable)obj).ToString(null, CultureInfo.InvariantCulture))))));
				if (serverType != 0)
				{
					stringBuilder2.Replace("'", "''");
					if (!isStoredProcedure)
					{
						if (obj is string || obj is char)
						{
							if (serverType == ServerType.Teradata)
							{
								stringBuilder2.Insert(0, "'");
							}
							else
							{
								stringBuilder2.Insert(0, "N'");
							}
							stringBuilder2.Append("'");
						}
						else if (obj is DateTime)
						{
							switch (serverType)
							{
							case ServerType.SQLServer:
							case ServerType.SQLServerDataWarehouse:
								stringBuilder2.Insert(0, "'");
								stringBuilder2.Append("'");
								break;
							case ServerType.Teradata:
								stringBuilder2.Insert(0, "TIMESTAMP '");
								stringBuilder2.Append("'");
								break;
							default:
								stringBuilder2.Insert(0, "TO_DATE('");
								stringBuilder2.Append("','MM/DD/YYYY HH24:MI:SS')");
								break;
							}
						}
					}
				}
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(stringBuilder2.ToString());
			}
			return stringBuilder.ToString();
		}

		protected override void Dispose(bool disposing)
		{
			m_transactionWrapper = null;
			m_parameterCollection = null;
			base.Dispose(disposing);
		}

		protected virtual ParameterCollectionWrapper CreateParameterCollection()
		{
			return new ParameterCollectionWrapper(UnderlyingCommand.Parameters);
		}
	}
}
