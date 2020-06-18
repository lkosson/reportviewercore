using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;

namespace Microsoft.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class PromptBucket : ArrayList
	{
		internal new DataSourceInfo this[int index] => (DataSourceInfo)base[index];

		internal bool NeedPrompt => GetRepresentative().NeedPrompt;

		internal PromptBucket()
		{
		}

		internal DataSourceInfo GetRepresentative()
		{
			Global.Tracer.Assert(Count > 0, "Prompt Bucket is empty on get representative");
			return this[0];
		}

		internal bool HasItemWithLinkID(Guid linkID)
		{
			for (int i = 0; i < Count; i++)
			{
				if (this[i].LinkID == linkID)
				{
					return true;
				}
			}
			return false;
		}

		internal bool HasItemWithOriginalName(string originalName)
		{
			for (int i = 0; i < Count; i++)
			{
				if (this[i].OriginalName == originalName)
				{
					return true;
				}
			}
			return false;
		}

		internal void SetCredentials(DatasourceCredentials credentials, IDataProtection dataProtection)
		{
			int num = 0;
			while (true)
			{
				if (num < Count)
				{
					DataSourceInfo dataSourceInfo = this[num];
					if (dataSourceInfo.CredentialsRetrieval != DataSourceInfo.CredentialsRetrievalOption.Prompt)
					{
						break;
					}
					dataSourceInfo.SetUserName(credentials.UserName, dataProtection);
					dataSourceInfo.SetPassword(credentials.Password, dataProtection);
					num++;
					continue;
				}
				return;
			}
			throw new InternalCatalogException("Non-promptable data source appeared in prompt collection!");
		}
	}
}
