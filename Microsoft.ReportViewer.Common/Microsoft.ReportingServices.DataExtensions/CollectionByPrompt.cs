using System;
using System.Collections;

namespace Microsoft.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class CollectionByPrompt : ArrayList
	{
		internal new PromptBucket this[int index] => (PromptBucket)base[index];

		internal bool NeedPrompt
		{
			get
			{
				for (int i = 0; i < Count; i++)
				{
					if (this[i].NeedPrompt)
					{
						return true;
					}
				}
				return false;
			}
		}

		internal CollectionByPrompt()
		{
		}

		internal new CollectionByPrompt Clone()
		{
			CollectionByPrompt collectionByPrompt = new CollectionByPrompt();
			for (int i = 0; i < Count; i++)
			{
				collectionByPrompt.Add(this[i].Clone());
			}
			return collectionByPrompt;
		}

		internal void CheckedAdd(DataSourceInfo dataSource)
		{
			if (dataSource.CredentialsRetrieval != DataSourceInfo.CredentialsRetrievalOption.Prompt)
			{
				return;
			}
			PromptBucket bucketByLinkID = GetBucketByLinkID(dataSource.LinkID);
			PromptBucket bucketByOriginalName = GetBucketByOriginalName(dataSource.PromptIdentifier);
			if (bucketByLinkID == null)
			{
				if (bucketByOriginalName == null)
				{
					PromptBucket promptBucket = new PromptBucket();
					promptBucket.Add(dataSource);
					Add(promptBucket);
				}
				else
				{
					bucketByOriginalName.Add(dataSource);
				}
			}
			else if (bucketByOriginalName == null)
			{
				bucketByLinkID.Add(dataSource);
			}
			else if (bucketByLinkID == bucketByOriginalName)
			{
				bucketByLinkID.Add(dataSource);
			}
			else
			{
				bucketByLinkID.AddRange(bucketByOriginalName);
				Remove(bucketByOriginalName);
				bucketByLinkID.Add(dataSource);
			}
		}

		private PromptBucket GetBucketByLinkID(Guid linkID)
		{
			if (linkID == Guid.Empty)
			{
				return null;
			}
			for (int i = 0; i < Count; i++)
			{
				if (this[i].HasItemWithLinkID(linkID))
				{
					return this[i];
				}
			}
			return null;
		}

		internal PromptBucket GetBucketByOriginalName(string originalName)
		{
			for (int i = 0; i < Count; i++)
			{
				if (this[i].HasItemWithOriginalName(originalName))
				{
					return this[i];
				}
			}
			return null;
		}

		internal DataSourcePromptCollection GetPromptRepresentatives(ServerDataSourceSettings serverDatasourceSettings)
		{
			DataSourcePromptCollection dataSourcePromptCollection = new DataSourcePromptCollection();
			for (int i = 0; i < Count; i++)
			{
				dataSourcePromptCollection.Add(this[i].GetRepresentative(), serverDatasourceSettings);
			}
			return dataSourcePromptCollection;
		}
	}
}
