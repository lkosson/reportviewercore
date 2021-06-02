using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualBasic;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.ReportingServices.RdlExpressions
{
	internal sealed class VBExpressionCodeProvider : VBCodeProvider
	{
		private const string LineContinuation = " _";

		private const string LineTooLongError = "BC30494";

		private const int MaxLineLength = 65535;

		private void CheckAndAddReference(List<MetadataReference> roslynReferences, string file)
		{
			if (String.IsNullOrEmpty(file)) return;
			if (!File.Exists(file)) return;
			roslynReferences.Add(MetadataReference.CreateFromFile(file));
		}

		public override CompilerResults CompileAssemblyFromDom(CompilerParameters options, params CodeCompileUnit[] compilationUnits)
		{
			var writer = new StringWriter();
			GenerateCodeFromCompileUnit(compilationUnits[0], writer, new CodeGeneratorOptions());
			var roslynTree = SyntaxFactory.ParseSyntaxTree(writer.ToString(), null, "");
			var roslynOptions = new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
			var roslynReferences = new List<MetadataReference>();
			CheckAndAddReference(roslynReferences, System.Reflection.Assembly.Load("System.Private.CoreLib").Location);
			CheckAndAddReference(roslynReferences, System.Reflection.Assembly.Load("Microsoft.VisualBasic.Core").Location);
			CheckAndAddReference(roslynReferences, System.Reflection.Assembly.Load("System.Runtime").Location);
			CheckAndAddReference(roslynReferences, System.Reflection.Assembly.Load("System.Text.RegularExpressions").Location);
			foreach (var assembly in options.ReferencedAssemblies)
			{
				if (assembly == "System.dll") continue;
				CheckAndAddReference(roslynReferences, assembly);
			}
			var roslynCompilation = VisualBasicCompilation.Create(Path.GetFileNameWithoutExtension(options.OutputAssembly), new[] { roslynTree }, options: roslynOptions, references: roslynReferences);
			var roslynAssembly = new MemoryStream();
			var result = roslynCompilation.Emit(roslynAssembly);
			if (!result.Success)
			{
				var error = result.Diagnostics.Where(e => e.Severity == DiagnosticSeverity.Error).FirstOrDefault();
				if (error != null) throw new InvalidOperationException(error.ToString());
				throw new Exception();
			}
			var assemblyFile = Path.GetTempFileName();
			File.WriteAllBytes(assemblyFile, roslynAssembly.ToArray());
			var compilerResults = new CompilerResults(new TempFileCollection()) { PathToAssembly = assemblyFile };
			return compilerResults;
		}

		private CompilerResults CompileAssemblyFromDomWithRetry(CompilerParameters options, CodeCompileUnit compilationUnit)
		{
			options.TempFiles.KeepFiles = true;
			CompilerResults compilerResults = base.CompileAssemblyFromDom(options, compilationUnit);
			TempFileCollection tempFiles = compilerResults.TempFiles;
			try
			{
				if (compilerResults.Errors.HasErrors)
				{
					options.TempFiles = new TempFileCollection();
					return RetryCompile(options, compilerResults.Errors) ?? compilerResults;
				}
				return compilerResults;
			}
			finally
			{
				foreach (object item in tempFiles)
				{
					string text = item as string;
					if (text != null)
					{
						try
						{
							File.Delete(text);
						}
						catch
						{
						}
					}
				}
			}
		}

		private CompilerResults RetryCompile(CompilerParameters options, CompilerErrorCollection compilerErrorCollection)
		{
			CompilerResults result = null;
			string text = null;
			List<int> list = new List<int>();
			foreach (CompilerError item in compilerErrorCollection)
			{
				if (string.Equals(item.ErrorNumber, "BC30494", StringComparison.OrdinalIgnoreCase))
				{
					list.Add(item.Line);
					if (text == null)
					{
						text = item.FileName;
					}
				}
			}
			if (text != null)
			{
				List<string> list2 = File.ReadAllLines(text).ToList();
				SplitLongLines(list2, list);
				File.WriteAllLines(text, list2.ToArray());
				options.TempFiles = new TempFileCollection();
				result = base.CompileAssemblyFromFile(options, text);
			}
			return result;
		}

		private static void SplitLongLines(List<string> lines, List<int> longLineNumbers)
		{
			if (longLineNumbers.Count > 0)
			{
				longLineNumbers.Sort();
			}
			for (int num = longLineNumbers.Count - 1; num >= 0; num--)
			{
				int num2 = longLineNumbers[num] - 1;
				string text = lines[num2];
				lines.RemoveAt(num2);
				while (text.Length > 65535)
				{
					int num3 = FindSafeSplitPosition(text);
					if (num3 <= 0)
					{
						lines.Insert(num2, text);
						return;
					}
					string item = text.Substring(0, num3) + " _";
					lines.Insert(num2, item);
					num2++;
					text = text.Substring(num3 + 1);
				}
				lines.Insert(num2, text);
			}
		}

		private static int FindSafeSplitPosition(string line)
		{
			int num = line.LastIndexOf(' ', 65535 - " _".Length);
			bool flag = true;
			while (flag && num > 80)
			{
				int num2 = line.LastIndexOf('"', num, 80);
				flag = (num2 != -1);
				if (flag)
				{
					num = line.LastIndexOf(' ', num2);
				}
			}
			if (flag)
			{
				return -1;
			}
			return num;
		}
	}
}
