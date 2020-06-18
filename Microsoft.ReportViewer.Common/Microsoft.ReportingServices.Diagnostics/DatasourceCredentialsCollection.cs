using System;
using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.ReportingServices.Diagnostics
{
	[Serializable]
	internal sealed class DatasourceCredentialsCollection : CollectionBase
	{
		public DatasourceCredentials this[int index] => (DatasourceCredentials)base.InnerList[index];

		public DatasourceCredentialsCollection()
		{
		}

		public DatasourceCredentialsCollection(NameValueCollection userNameParams, NameValueCollection userPwdParams)
		{
			for (int i = 0; i < userNameParams.Count; i++)
			{
				string key = userNameParams.GetKey(i);
				string text = userNameParams.Get(i);
				if (text != null && text.Trim().Length != 0)
				{
					string password = userPwdParams[key];
					DatasourceCredentials datasourceCred = new DatasourceCredentials(key, text, password);
					Add(datasourceCred);
				}
			}
		}

		public int Add(DatasourceCredentials datasourceCred)
		{
			return base.InnerList.Add(datasourceCred);
		}
	}
}
