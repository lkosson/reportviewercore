using System;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal sealed class UserImpl : User
	{
		private string m_userID;

		private string m_language;

		private UserProfileState m_allowUserProfileState;

		private UserProfileState m_hasUserProfileState;

		private UserProfileState m_location = UserProfileState.InReport;

		private bool m_indirectQueryReference;

		internal const string Name = "User";

		internal const string FullName = "Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.User";

		public override object this[string key]
		{
			get
			{
				UpdateUserProfileState();
				if (!(key == "UserID"))
				{
					if (key == "Language")
					{
						return m_language;
					}
					throw new ArgumentOutOfRangeException("key");
				}
				return m_userID;
			}
		}

		public override string UserID
		{
			get
			{
				UpdateUserProfileState();
				return m_userID;
			}
		}

		public override string Language
		{
			get
			{
				UpdateUserProfileState();
				return m_language;
			}
		}

		internal UserProfileState UserProfileLocation
		{
			get
			{
				return m_location;
			}
			set
			{
				m_location = value;
			}
		}

		internal UserProfileState HasUserProfileState
		{
			get
			{
				return m_hasUserProfileState;
			}
			set
			{
				m_hasUserProfileState = value;
			}
		}

		internal bool IndirectQueryReference
		{
			get
			{
				return m_indirectQueryReference;
			}
			set
			{
				m_indirectQueryReference = value;
			}
		}

		internal UserImpl(string userID, string language, UserProfileState allowUserProfileState)
		{
			m_userID = userID;
			m_language = language;
			m_allowUserProfileState = allowUserProfileState;
		}

		internal void UpdateUserProfileState()
		{
			m_hasUserProfileState |= m_location;
			if (m_indirectQueryReference)
			{
				m_hasUserProfileState |= UserProfileState.InQuery;
				if ((m_allowUserProfileState & UserProfileState.InQuery) == 0)
				{
					throw new ReportProcessingException_UserProfilesDependencies();
				}
			}
			if ((m_allowUserProfileState & m_location) == 0)
			{
				throw new ReportProcessingException_UserProfilesDependencies();
			}
		}
	}
}
