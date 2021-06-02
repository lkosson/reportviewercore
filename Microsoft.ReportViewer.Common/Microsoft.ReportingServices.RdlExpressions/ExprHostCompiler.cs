using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.RdlExpressions
{
	internal sealed class ExprHostCompiler
	{
		private sealed class ExprCompileTimeInfo
		{
			internal Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ExpressionInfo;

			internal Microsoft.ReportingServices.ReportProcessing.ObjectType OwnerObjectType;

			internal string OwnerObjectName;

			internal string OwnerPropertyName;

			internal int NumErrors;

			internal int NumWarnings;

			internal ExprCompileTimeInfo(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ExpressionParser.ExpressionContext context)
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

		private IExpressionHostAssemblyHolder m_expressionHostAssemblyHolder;

		internal ExprHostBuilder Builder => m_builder;

		internal bool BodyRefersToReportItems => m_langParser.BodyRefersToReportItems;

		internal bool PageSectionRefersToReportItems => m_langParser.PageSectionRefersToReportItems;

		internal bool PageSectionRefersToOverallTotalPages => m_langParser.PageSectionRefersToOverallTotalPages;

		internal bool PageSectionRefersToTotalPages => m_langParser.PageSectionRefersToTotalPages;

		internal int NumberOfAggregates => m_langParser.NumberOfAggregates;

		internal int LastAggregateID => m_langParser.LastID;

		internal int LastLookupID => m_langParser.LastLookupID;

		internal bool PreviousAggregateUsed => m_langParser.PreviousAggregateUsed;

		internal bool AggregateOfAggregateUsed => m_langParser.AggregateOfAggregatesUsed;

		internal bool AggregateOfAggregateUsedInUserSort => m_langParser.AggregateOfAggregatesUsedInUserSort;

		internal bool ValueReferenced => m_langParser.ValueReferenced;

		internal bool ValueReferencedGlobal => m_langParser.ValueReferencedGlobal;

		internal ExprHostCompiler(ExpressionParser langParser, ErrorContext errorContext)
		{
			m_langParser = langParser;
			m_errorContext = errorContext;
			m_builder = new ExprHostBuilder();
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ParseExpression(string expression, ExpressionParser.EvaluationMode evaluationMode, ExpressionParser.ExpressionContext context)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = m_langParser.ParseExpression(expression, context, evaluationMode);
			ProcessExpression(expressionInfo, context);
			return expressionInfo;
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo ParseExpression(string expression, ExpressionParser.ExpressionContext context, ExpressionParser.EvaluationMode evaluationMode, out bool userCollectionReferenced)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = m_langParser.ParseExpression(expression, context, evaluationMode, out userCollectionReferenced);
			ProcessExpression(expressionInfo, context);
			return expressionInfo;
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo CreateScopedFirstAggregate(string fieldName, string dataSetName)
		{
			return m_langParser.CreateScopedFirstAggregate(fieldName, dataSetName);
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

		internal void ResetPageSectionRefersFlags()
		{
			m_langParser.ResetPageSectionRefersFlags();
		}

		internal byte[] Compile(IExpressionHostAssemblyHolder expressionHostAssemblyHolder, AppDomain compilationTempAppDomain, bool refusePermissions, PublishingVersioning versioning)
		{
			byte[] result = null;
			if (m_builder.HasExpressions && versioning.IsRdlFeatureRestricted(RdlFeatures.ComplexExpression))
			{
				m_errorContext.Register(ProcessingErrorCode.rsInvalidComplexExpressionInReport, Severity.Error, Microsoft.ReportingServices.ReportProcessing.ObjectType.Report, "Report", "Body");
			}
			else
			{
				m_expressionHostAssemblyHolder = expressionHostAssemblyHolder;
				RevertImpersonationContext.Run(delegate
				{
					result = InternalCompile(compilationTempAppDomain, refusePermissions);
				});
			}
			return result;
		}

		private void ProcessExpression(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ExpressionParser.ExpressionContext context)
		{
			if (expression.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression)
			{
				RegisterExpression(new ExprCompileTimeInfo(expression, context));
				ProcessAggregateParams(expression, context);
			}
			else if (expression.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Aggregate)
			{
				ProcessAggregateParams(expression, context);
			}
			else if (expression.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Field && context.Location == Microsoft.ReportingServices.ReportPublishing.LocationFlags.None)
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

		private void ProcessAggregateParams(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ExpressionParser.ExpressionContext context)
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

		private void ProcessAggregateParam(Microsoft.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregate, ExpressionParser.ExpressionContext context)
		{
			if (aggregate != null && aggregate.Expressions != null)
			{
				for (int i = 0; i < aggregate.Expressions.Length; i++)
				{
					ProcessAggregateParam(aggregate.Expressions[i], context);
				}
			}
		}

		private void ProcessAggregateParam(Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, ExpressionParser.ExpressionContext context)
		{
			if (expression != null && expression.Type == Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Expression)
			{
				RegisterExpression(new ExprCompileTimeInfo(expression, context));
			}
		}

		private byte[] InternalCompile(AppDomain compilationTempAppDomain, bool refusePermissions)
		{
			if (m_builder.HasExpressions)
			{
				CompilerParameters compilerParameters = new CompilerParameters();
				compilerParameters.OutputAssembly = string.Format(CultureInfo.InvariantCulture, "{0}{1}.dll", Path.GetTempPath(), m_expressionHostAssemblyHolder.ExprHostAssemblyName);
				compilerParameters.GenerateExecutable = false;
				compilerParameters.GenerateInMemory = false;
				compilerParameters.IncludeDebugInformation = false;
				compilerParameters.ReferencedAssemblies.Add("System.dll");

				string pathForRV = typeof(ReportObjectModelProxy).Assembly.Location;
				if (String.IsNullOrEmpty(pathForRV)) pathForRV = System.Reflection.Assembly.GetExecutingAssembly().Location;
				if (String.IsNullOrEmpty(pathForRV)) pathForRV = Process.GetCurrentProcess().MainModule.FileName;
				compilerParameters.ReferencedAssemblies.Add(pathForRV);

				compilerParameters.CompilerOptions += m_langParser.GetCompilerArguments();
				if (m_expressionHostAssemblyHolder.CodeModules != null)
				{
					ResolveAssemblylocations(m_expressionHostAssemblyHolder.CodeModules, compilerParameters, m_errorContext, compilationTempAppDomain);
				}
				CompilerResults compilerResults = null;
				try
				{
					ProcessingIntermediateFormatVersion version = new ProcessingIntermediateFormatVersion(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatVersion.Current);
					CodeCompileUnit exprHost = m_builder.GetExprHost(version, refusePermissions);
					m_expressionHostAssemblyHolder.CompiledCodeGeneratedWithRefusedPermissions = refusePermissions;
					CodeDomProvider codeCompiler = m_langParser.GetCodeCompiler();
					compilerResults = codeCompiler.CompileAssemblyFromDom(compilerParameters, exprHost);
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
								Global.Tracer.Trace(streamReader.ReadToEnd().MarkAsPrivate());
							}
						}
						catch
						{
						}
					}
					if (compilerResults.NativeCompilerReturnValue != 0 || compilerResults.Errors.Count > 0)
					{
						ParseErrors(compilerResults);
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

		private void ResolveAssemblylocations(List<string> codeModules, CompilerParameters options, ErrorContext errorContext, AppDomain compilationTempAppDomain)
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
					ProcessingMessage processingMessage = errorContext.Register(ProcessingErrorCode.rsErrorLoadingCodeModule, Severity.Error, m_expressionHostAssemblyHolder.ObjectType, null, null, codeModules[num], ex.Message);
					if (Global.Tracer.TraceError && processingMessage != null)
					{
						Global.Tracer.Trace(TraceLevel.Error, processingMessage.Message + Environment.NewLine + ex.ToString());
					}
				}
			}
		}

		private void ParseErrors(CompilerResults results)
		{
			int count = results.Errors.Count;
			if (results.NativeCompilerReturnValue != 0 && count == 0)
			{
				m_errorContext.Register(ProcessingErrorCode.rsUnexpectedCompilerError, Severity.Error, m_expressionHostAssemblyHolder.ObjectType, null, null, results.NativeCompilerReturnValue.ToString(CultureInfo.InvariantCulture));
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
					RegisterError(error, ref m_customCodeNumErrors, ref m_customCodeNumWarnings, m_expressionHostAssemblyHolder.ObjectType, null, null, ProcessingErrorCode.rsCompilerErrorInCode);
					break;
				case ExprHostBuilder.ErrorSource.CodeModuleClassInstanceDecl:
				{
					if (m_ctClassInstDeclList == null)
					{
						m_ctClassInstDeclList = new CodeModuleClassInstanceDeclCompileTimeInfoList();
					}
					CodeModuleClassInstanceDeclCompileTimeInfo codeModuleClassInstanceDeclCompileTimeInfo = m_ctClassInstDeclList[id];
					RegisterError(error, ref codeModuleClassInstanceDeclCompileTimeInfo.NumErrors, ref codeModuleClassInstanceDeclCompileTimeInfo.NumWarnings, Microsoft.ReportingServices.ReportProcessing.ObjectType.CodeClass, m_expressionHostAssemblyHolder.CodeClasses[id].ClassName, null, ProcessingErrorCode.rsCompilerErrorInClassInstanceDeclaration);
					break;
				}
				default:
					m_errorContext.Register(ProcessingErrorCode.rsUnexpectedCompilerError, Severity.Error, m_expressionHostAssemblyHolder.ObjectType, null, null, FormatError(error));
					throw new ReportProcessingException(m_errorContext.Messages);
				}
			}
		}

		private void RegisterError(CompilerError error, ref int numErrors, ref int numWarnings, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, ProcessingErrorCode errorCode)
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
					m_errorContext.Register(errorCode, severity, objectType, objectName, propertyName, FormatError(error), error.Line.ToString(CultureInfo.InvariantCulture));
				}
			}
		}

		private string FormatError(CompilerError error)
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0}] {1}", error.ErrorNumber, error.ErrorText);
		}
	}
}
