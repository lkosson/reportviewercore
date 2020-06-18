using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Threading;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class UserImpl : User
	{
		private struct UserProfileTrackingContext : IDisposable
		{
			private UserImpl m_userImpl;

			private UserProfileState m_oldLocation;

			internal UserProfileTrackingContext(UserImpl userImpl, UserProfileState oldLocation)
			{
				m_userImpl = userImpl;
				m_oldLocation = oldLocation;
			}

			public void Dispose()
			{
				m_userImpl.m_location = m_oldLocation;
				Monitor.Exit(m_userImpl.m_locationUpdateLock);
			}
		}

		private string m_userID;

		private string m_language;

		private UserProfileState m_allowUserProfileState;

		private UserProfileState m_hasUserProfileState;

		private UserProfileState m_location = UserProfileState.InReport;

		private bool m_indirectQueryReference;

		private object m_locationUpdateLock = new object();

		private OnDemandProcessingContext m_odpContext;

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
					throw new ReportProcessingException_NonExistingUserReference(key);
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

		internal UserProfileState UserProfileLocation => m_location;

		internal UserProfileState HasUserProfileState => m_hasUserProfileState;

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

		internal UserImpl(string userID, string language, UserProfileState allowUserProfileState, OnDemandProcessingContext odpContext)
		{
			m_userID = userID;
			m_language = language;
			m_allowUserProfileState = allowUserProfileState;
			m_odpContext = odpContext;
		}

		internal UserImpl(UserImpl copy, OnDemandProcessingContext odpContext)
		{
			m_userID = copy.m_userID;
			m_language = copy.m_language;
			m_allowUserProfileState = copy.m_allowUserProfileState;
			m_odpContext = odpContext;
		}

		internal IDisposable UpdateUserProfileLocation(UserProfileState newLocation)
		{
			Monitor.Enter(m_locationUpdateLock);
			UserProfileTrackingContext userProfileTrackingContext = new UserProfileTrackingContext(this, m_location);
			m_location = newLocation;
			return userProfileTrackingContext;
		}

		internal UserProfileState UpdateUserProfileLocationWithoutLocking(UserProfileState newLocation)
		{
			UserProfileState location = m_location;
			m_location = newLocation;
			return location;
		}

		private void UpdateUserProfileState()
		{
			Exception exceptionToThrow = null;
			UserProfileState userProfileState = m_hasUserProfileState | m_location;
			if (m_indirectQueryReference)
			{
				userProfileState |= UserProfileState.InQuery;
				if ((m_allowUserProfileState & UserProfileState.InQuery) == 0)
				{
					exceptionToThrow = new ReportProcessingException_UserProfilesDependencies();
				}
			}
			if (m_location != UserProfileState.OnDemandExpressions && (m_allowUserProfileState & m_location) == 0)
			{
				exceptionToThrow = new ReportProcessingException_UserProfilesDependencies();
			}
			UpdateOverallUserProfileState(exceptionToThrow, userProfileState);
		}

		private void UpdateOverallUserProfileState(Exception exceptionToThrow, UserProfileState newState)
		{
			if (newState != m_hasUserProfileState)
			{
				m_hasUserProfileState = newState;
				if (exceptionToThrow == null || !m_odpContext.InSubreport)
				{
					m_odpContext.MergeHasUserProfileState(newState);
				}
			}
			if (exceptionToThrow != null)
			{
				throw exceptionToThrow;
			}
		}

		internal void SetConnectionStringUserProfileDependencyOrThrow()
		{
			Exception exceptionToThrow = null;
			using (UpdateUserProfileLocation(UserProfileState.InQuery))
			{
				UserProfileState newState = m_hasUserProfileState | m_location;
				if ((m_allowUserProfileState & UserProfileState.InQuery) == 0)
				{
					exceptionToThrow = new ReportProcessingException_UserProfilesDependencies();
				}
				UpdateOverallUserProfileState(exceptionToThrow, newState);
			}
		}
	}
}
