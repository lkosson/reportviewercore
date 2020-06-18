using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ReportCompileTime
	{
		private sealed class ExprCompileTimeInfo
		{
			internal ExpressionInfo ExpressionInfo;

			internal ObjectType OwnerObjectType;

			internal string OwnerObjectName;

			internal string OwnerPropertyName;

			internal int NumErrors;

			internal int NumWarnings;

			internal ExprCompileTimeInfo(ExpressionInfo expression, ExpressionParser.ExpressionContext context)
			{
				ExpressionInfo = expression;
				OwnerObjectType = context.ObjectType;
				OwnerObjectName = context.ObjectName;
				OwnerPropertyName = context.PropertyName;
				NumErrors = 0;
				NumWarnings = 0;
			}
		}

		private sealed class ExprCompileTimeInfoList : ArrayList
		{
			internal new ExprCompileTimeInfo this[int exprCTId] => (ExprCompileTimeInfo)base[exprCTId];
		}

		private sealed class CodeModuleClassInstanceDeclCompileTimeInfo
		{
			internal int NumErrors;

			internal int NumWarnings;
		}

		private sealed class CodeModuleClassInstanceDeclCompileTimeInfoList : Hashtable
		{
			internal new CodeModuleClassInstanceDeclCompileTimeInfo this[object id]
			{
				get
				{
					CodeModuleClassInstanceDeclCompileTimeInfo codeModuleClassInstanceDeclCompileTimeInfo = (CodeModuleClassInstanceDeclCompileTimeInfo)base[id];
					if (codeModuleClassInstanceDeclCompileTimeInfo == null)
					{
						codeModuleClassInstanceDeclCompileTimeInfo = new CodeModuleClassInstanceDeclCompileTimeInfo();
						base.Add(id, codeModuleClassInstanceDeclCompileTimeInfo);
					}
					return codeModuleClassInstanceDeclCompileTimeInfo;
				}
			}
		}

		private ExpressionParser m_langParser;

		private ErrorContext m_errorContext;

		private ExprHostBuilder m_builder;

		private ExprCompileTimeInfoList m_ctExprList;

		private CodeModuleClassInstanceDeclCompileTimeInfoList m_ctClassInstDeclList;

		private int m_customCodeNumErrors;

		private int m_customCodeNumWarnings;

		private ArrayList m_reportLevelFieldReferences;

		internal ExprHostBuilder Builder => m_builder;

		internal bool BodyRefersToReportItems => m_langParser.BodyRefersToReportItems;

		internal bool PageSectionRefersToReportItems => m_langParser.PageSectionRefersToReportItems;

		internal int NumberOfAggregates => m_langParser.NumberOfAggregates;

		internal int LastAggregateID => m_langParser.LastID;

		internal bool ValueReferenced => m_langParser.ValueReferenced;

		internal bool ValueReferencedGlobal => m_langParser.ValueReferencedGlobal;

		internal ReportCompileTime(ExpressionParser langParser, ErrorContext errorContext)
		{
			m_langParser = langParser;
			m_errorContext = errorContext;
			m_builder = new ExprHostBuilder();
		}

		internal ExpressionInfo ParseExpression(string expression, ExpressionParser.ExpressionContext context)
		{
			ExpressionInfo expressionInfo = m_langParser.ParseExpression(expression, context);
			ProcessExpression(expressionInfo, context);
			return expressionInfo;
		}

		internal ExpressionInfo ParseExpression(string expression, ExpressionParser.ExpressionContext context, ExpressionParser.DetectionFlags flag, out bool reportParameterReferenced, out string reportParameterName, out bool userCollectionReferenced)
		{
			ExpressionInfo expressionInfo = m_langParser.ParseExpression(expression, context, flag, out reportParameterReferenced, out reportParameterName, out userCollectionReferenced);
			ProcessExpression(expressionInfo, context);
			return expressionInfo;
		}

		internal ExpressionInfo ParseExpression(string expression, ExpressionParser.ExpressionContext context, out bool userCollectionReferenced)
		{
			ExpressionInfo expressionInfo = m_langParser.ParseExpression(expression, context, out userCollectionReferenced);
			ProcessExpression(expressionInfo, context);
			return expressionInfo;
		}

		internal void ConvertFields2ComplexExpr()
		{
			if (m_reportLevelFieldReferences != null)
			{
				for (int num = m_reportLevelFieldReferences.Count - 1; num >= 0; num--)
				{
					ExprCompileTimeInfo exprCompileTimeInfo = (ExprCompileTimeInfo)m_reportLevelFieldReferences[num];
					m_langParser.ConvertField2ComplexExpr(ref exprCompileTimeInfo.ExpressionInfo);
					RegisterExpression(exprCompileTimeInfo);
				}
			}
		}

		internal void ResetValueReferencedFlag()
		{
			m_langParser.ResetValueReferencedFlag();
		}

		internal byte[] Compile(Report report, AppDomain compilationTempAppDomain, bool refusePermissions)
		{
			byte[] result = null;
			RevertImpersonationContext.Run(delegate
			{
				result = InternalCompile(report, compilationTempAppDomain, refusePermissions);
			});
			return result;
		}

		private void ProcessExpression(ExpressionInfo expression, ExpressionParser.ExpressionContext context)
		{
			if (expression.Type == ExpressionInfo.Types.Expression)
			{
				RegisterExpression(new ExprCompileTimeInfo(expression, context));
				ProcessAggregateParams(expression, context);
			}
			else if (expression.Type == ExpressionInfo.Types.Aggregate)
			{
				ProcessAggregateParams(expression, context);
			}
			else if (expression.Type == ExpressionInfo.Types.Field && context.Location == LocationFlags.None)
			{
				if (m_reportLevelFieldReferences == null)
				{
					m_reportLevelFieldReferences = new ArrayList();
				}
				m_reportLevelFieldReferences.Add(new ExprCompileTimeInfo(expression, context));
			}
		}

		private void RegisterExpression(ExprCompileTimeInfo exprCTInfo)
		{
			if (m_ctExprList == null)
			{
				m_ctExprList = new ExprCompileTimeInfoList();
			}
			exprCTInfo.ExpressionInfo.CompileTimeID = m_ctExprList.Add(exprCTInfo);
		}

		private void ProcessAggregateParams(ExpressionInfo expression, ExpressionParser.ExpressionContext context)
		{
			if (expression.Aggregates != null)
			{
				for (int num = expression.Aggregates.Count - 1; num >= 0; num--)
				{
					ProcessAggregateParam(expression.Aggregates[num], context);
				}
			}
			if (expression.RunningValues != null)
			{
				for (int num2 = expression.RunningValues.Count - 1; num2 >= 0; num2--)
				{
					ProcessAggregateParam(expression.RunningValues[num2], context);
				}
			}
		}

		private void ProcessAggregateParam(DataAggregateInfo aggregate, ExpressionParser.ExpressionContext context)
		{
			if (aggregate != null && aggregate.Expressions != null)
			{
				for (int i = 0; i < aggregate.Expressions.Length; i++)
				{
					ProcessAggregateParam(aggregate.Expressions[i], context);
				}
			}
		}

		private void ProcessAggregateParam(ExpressionInfo expression, ExpressionParser.ExpressionContext context)
		{
			if (expression != null && expression.Type == ExpressionInfo.Types.Expression)
			{
				RegisterExpression(new ExprCompileTimeInfo(expression, context));
			}
		}

		private byte[] InternalCompile(Report report, AppDomain compilationTempAppDomain, bool refusePermissions)
		{
			if (m_builder.HasExpressions)
			{
				CompilerParameters compilerParameters = new CompilerParameters();
				compilerParameters.OutputAssembly = string.Format(CultureInfo.InvariantCulture, "{0}{1}.dll", Path.GetTempPath(), report.ExprHostAssemblyName);
				compilerParameters.GenerateExecutable = false;
				compilerParameters.GenerateInMemory = false;
				compilerParameters.IncludeDebugInformation = false;
				compilerParameters.ReferencedAssemblies.Add("System.dll");
				compilerParameters.ReferencedAssemblies.Add(typeof(ReportObjectModelProxy).Assembly.Location);
				compilerParameters.CompilerOptions += m_langParser.GetCompilerArguments();
				if (report.CodeModules != null)
				{
					ResolveAssemblylocations(report.CodeModules, compilerParameters, m_errorContext, compilationTempAppDomain);
				}
				CompilerResults compilerResults = null;
				try
				{
					CodeCompileUnit exprHost = m_builder.GetExprHost(report.IntermediateFormatVersion, refusePermissions);
					report.CompiledCodeGeneratedWithRefusedPermissions = refusePermissions;
					CodeDomProvider codeCompiler = m_langParser.GetCodeCompiler();
					compilerResults = codeCompiler.CompileAssemblyFromDom(compilerParameters, exprHost);
					if (compilerResults.NativeCompilerReturnValue != 0 || compilerResults.Errors.Count > 0)
					{
						if (Global.Tracer.TraceVerbose)
						{
							try
							{
								using (MemoryStream memoryStream = new MemoryStream())
								{
									IndentedTextWriter indentedTextWriter = new IndentedTextWriter(new StreamWriter(memoryStream), "    ");
									codeCompiler.GenerateCodeFromCompileUnit(exprHost, indentedTextWriter, new CodeGeneratorOptions());
									indentedTextWriter.Flush();
									memoryStream.Position = 0L;
									StreamReader streamReader = new StreamReader(memoryStream);
									Global.Tracer.Trace(streamReader.ReadToEnd());
								}
							}
							catch
							{
							}
						}
						ParseErrors(compilerResults, report.CodeClasses);
						return new byte[0];
					}
					using (FileStream fileStream = File.OpenRead(compilerResults.PathToAssembly))
					{
						byte[] array = new byte[fileStream.Length];
						int num = fileStream.Read(array, 0, (int)fileStream.Length);
						Global.Tracer.Assert(num == fileStream.Length, "(read == fs.Length)");
						return array;
					}
				}
				finally
				{
					if (compilerResults != null && compilerResults.PathToAssembly != null)
					{
						File.Delete(compilerResults.PathToAssembly);
					}
				}
			}
			return new byte[0];
		}

		private void ResolveAssemblylocations(StringList codeModules, CompilerParameters options, ErrorContext errorContext, AppDomain compilationTempAppDomain)
		{
			AssemblyLocationResolver assemblyLocationResolver = AssemblyLocationResolver.CreateResolver(compilationTempAppDomain);
			for (int num = codeModules.Count - 1; num >= 0; num--)
			{
				try
				{
					options.ReferencedAssemblies.Add(assemblyLocationResolver.LoadAssemblyAndResolveLocation(codeModules[num]));
				}
				catch (Exception ex)
				{
					ProcessingMessage processingMessage = errorContext.Register(ProcessingErrorCode.rsErrorLoadingCodeModule, Severity.Error, ObjectType.Report, null, null, codeModules[num], ex.Message);
					if (Global.Tracer.TraceError && processingMessage != null)
					{
						Global.Tracer.Trace(TraceLevel.Error, processingMessage.Message + Environment.NewLine + ex.ToString());
					}
				}
			}
		}

		private void ParseErrors(CompilerResults results, CodeClassList codeClassInstDecls)
		{
			int count = results.Errors.Count;
			if (results.NativeCompilerReturnValue != 0 && count == 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsUnexpectedCompilerError, Severity.Error, ObjectType.Report, null, null, results.NativeCompilerReturnValue.ToString(CultureInfo.CurrentCulture));
			}
			for (int i = 0; i < count; i++)
			{
				CompilerError error = results.Errors[i];
				int id;
				switch (m_builder.ParseErrorSource(error, out id))
				{
				case ExprHostBuilder.ErrorSource.Expression:
				{
					ExprCompileTimeInfo exprCompileTimeInfo = m_ctExprList[id];
					RegisterError(error, ref exprCompileTimeInfo.NumErrors, ref exprCompileTimeInfo.NumWarnings, exprCompileTimeInfo.OwnerObjectType, exprCompileTimeInfo.OwnerObjectName, exprCompileTimeInfo.OwnerPropertyName, ProcessingErrorCode.rsCompilerErrorInExpression);
					break;
				}
				case ExprHostBuilder.ErrorSource.CustomCode:
					RegisterError(error, ref m_customCodeNumErrors, ref m_customCodeNumWarnings, ObjectType.Report, null, null, ProcessingErrorCode.rsCompilerErrorInCode);
					break;
				case ExprHostBuilder.ErrorSource.CodeModuleClassInstanceDecl:
				{
					if (m_ctClassInstDeclList == null)
					{
						m_ctClassInstDeclList = new CodeModuleClassInstanceDeclCompileTimeInfoList();
					}
					CodeModuleClassInstanceDeclCompileTimeInfo codeModuleClassInstanceDeclCompileTimeInfo = m_ctClassInstDeclList[id];
					RegisterError(error, ref codeModuleClassInstanceDeclCompileTimeInfo.NumErrors, ref codeModuleClassInstanceDeclCompileTimeInfo.NumWarnings, ObjectType.CodeClass, codeClassInstDecls[id].ClassName, null, ProcessingErrorCode.rsCompilerErrorInClassInstanceDeclaration);
					break;
				}
				default:
					m_errorContext.Register(ProcessingErrorCode.rsUnexpectedCompilerError, Severity.Error, ObjectType.Report, null, null, FormatError(error));
					throw new ReportProcessingException(m_errorContext.Messages);
				}
			}
		}

		private void RegisterError(CompilerError error, ref int numErrors, ref int numWarnings, ObjectType objectType, string objectName, string propertyName, ProcessingErrorCode errorCode)
		{
			if ((error.IsWarning ? numWarnings : numErrors) < 1)
			{
				bool flag = false;
				Severity severity;
				if (error.IsWarning)
				{
					flag = true;
					severity = Severity.Warning;
					numWarnings++;
				}
				else
				{
					flag = true;
					severity = Severity.Error;
					numErrors++;
				}
				if (flag)
				{
					m_errorContext.Register(errorCode, severity, objectType, objectName, propertyName, FormatError(error), error.Line.ToString(CultureInfo.CurrentCulture));
				}
			}
		}

		private string FormatError(CompilerError error)
		{
			return string.Format(CultureInfo.CurrentCulture, "[{0}] {1}", error.ErrorNumber, error.ErrorText);
		}
	}
}
