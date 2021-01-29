using System.Collections.Generic;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class OmittedHeaderStack : Stack<OmittedHeaderData>
	{
		public string GetHeaders(int column, int currentLevel, string idPrefix)
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					OmittedHeaderData current = enumerator.Current;
					string text = current.IDs[column];
					if (text != null && current.Level > currentLevel)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(" ");
						}
						if (idPrefix != null)
						{
							stringBuilder.Append(idPrefix);
						}
						stringBuilder.Append(text);
					}
				}
			}
			return stringBuilder.ToString();
		}

		public void PopLevel(int level)
		{
			if (base.Count == 0)
			{
				return;
			}
			OmittedHeaderData omittedHeaderData = Peek();
			while (omittedHeaderData.Level < level && base.Count != 0)
			{
				Pop();
				if (base.Count > 0)
				{
					omittedHeaderData = Peek();
					continue;
				}
				break;
			}
		}

		public void Push(int level, int column, int colspan, string id, int columnCount)
		{
			PopLevel(level);
			OmittedHeaderData omittedHeaderData = null;
			if (base.Count > 0)
			{
				omittedHeaderData = Peek();
				if (omittedHeaderData.Level != level)
				{
					omittedHeaderData = null;
				}
			}
			if (omittedHeaderData == null)
			{
				omittedHeaderData = new OmittedHeaderData();
				omittedHeaderData.IDs = new string[columnCount];
				omittedHeaderData.Level = level;
				Push(omittedHeaderData);
			}
			int num = column + colspan;
			for (int i = column; i < num; i++)
			{
				omittedHeaderData.IDs[i] = id;
			}
		}
	}
}
