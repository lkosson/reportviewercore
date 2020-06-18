using Microsoft.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class SharedDataSetParameterNameMapper
	{
		internal static void MakeUnique(List<Microsoft.ReportingServices.ReportIntermediateFormat.ParameterValue> queryParameters)
		{
			if (queryParameters == null)
			{
				return;
			}
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>(StringComparer.Ordinal);
			int count = queryParameters.Count;
			for (int i = 0; i < count; i++)
			{
				string text = queryParameters[i].UniqueName;
				if (text == null)
				{
					text = queryParameters[i].Name;
				}
				if (!dictionary.TryGetValue(text, out bool value))
				{
					dictionary.Add(text, value: false);
					continue;
				}
				if (!value)
				{
					dictionary[text] = true;
				}
				text = MakeNameUnique(dictionary, i + 1, text);
				dictionary.Add(text, value: false);
				queryParameters[i].UniqueName = text;
			}
			for (int j = 0; j < count; j++)
			{
				string text2 = queryParameters[j].UniqueName;
				if (text2 == null)
				{
					text2 = queryParameters[j].Name;
				}
				if (dictionary[text2])
				{
					string text3 = MakeNameUnique(dictionary, j + 1, text2);
					dictionary.Remove(text2);
					dictionary.Add(text3, value: false);
					queryParameters[j].UniqueName = text3;
				}
			}
		}

		private static string MakeNameUnique(Dictionary<string, bool> parameterNameDuplicates, int position, string uniqueName)
		{
			uniqueName = AppendPosition(uniqueName, position);
			while (parameterNameDuplicates.ContainsKey(uniqueName))
			{
				uniqueName = AppendPosition(uniqueName, position);
			}
			return uniqueName;
		}

		private static string AppendPosition(string originalName, int position)
		{
			return originalName + "_" + position.ToString(CultureInfo.InvariantCulture);
		}
	}
}
