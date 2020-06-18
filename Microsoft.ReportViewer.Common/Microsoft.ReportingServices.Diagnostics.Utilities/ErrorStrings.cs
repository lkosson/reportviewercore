using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
	[CompilerGenerated]
	internal class ErrorStrings
	{
		[CompilerGenerated]
		public class Keys
		{
			private static ResourceManager resourceManager = new ResourceManager(typeof(ErrorStrings).FullName, typeof(ErrorStrings).Module.Assembly);

			private static CultureInfo _culture = null;

			public const string InvalidKeyValue = "InvalidKeyValue";

			public const string InvalidConfigElement = "InvalidConfigElement";

			public const string CouldNotFindElement = "CouldNotFindElement";

			public const string DuplicateConfigElement = "DuplicateConfigElement";

			public const string EmptyExtensionName = "EmptyExtensionName";

			public const string SameExtensionName = "SameExtensionName";

			public const string SameEventType = "SameEventType";

			public const string NoEventForEventProcessor = "NoEventForEventProcessor";

			public const string rsEventExtensionNotFoundException = "rsEventExtensionNotFoundException";

			public const string rsEventMaxRetryExceededException = "rsEventMaxRetryExceededException";

			public const string UIServerLoopback = "UIServerLoopback";

			public const string DataSourceConnectionErrorNotVisible = "DataSourceConnectionErrorNotVisible";

			public const string UserNameUnknown = "UserNameUnknown";

			public const string rsMissingRequiredPropertyForItemType = "rsMissingRequiredPropertyForItemType";

			public const string rsParameterTypeMismatch = "rsParameterTypeMismatch";

			public const string rsInvalidParameterCombination = "rsInvalidParameterCombination";

			public const string rsStoredParameterNotFound = "rsStoredParameterNotFound";

			public const string rsItemAlreadyExists = "rsItemAlreadyExists";

			public const string rsInvalidMove = "rsInvalidMove";

			public const string rsInvalidDestination = "rsInvalidDestination";

			public const string rsReservedItem = "rsReservedItem";

			public const string rsProcessingError = "rsProcessingError";

			public const string rsReadOnlyProperty = "rsReadOnlyProperty";

			public const string rsStreamNotFound = "rsStreamNotFound";

			public const string rsMissingSessionId = "rsMissingSessionId";

			public const string rsExecutionNotFound = "rsExecutionNotFound";

			public const string rsQueryExecutionNotAllowed = "rsQueryExecutionNotAllowed";

			public const string rsReportNotReady = "rsReportNotReady";

			public const string rsReportSnapshotEnabled = "rsReportSnapshotEnabled";

			public const string rsReportSnapshotNotEnabled = "rsReportSnapshotNotEnabled";

			public const string rsOperationPreventsUnattendedExecution = "rsOperationPreventsUnattendedExecution";

			public const string rsInvalidReportLink = "rsInvalidReportLink";

			public const string rsSubreportFromSnapshot = "rsSubreportFromSnapshot";

			public const string rsSPSiteNotFound = "rsSPSiteNotFound";

			public const string rsCachingNotEnabled = "rsCachingNotEnabled";

			public const string rsInvalidSearchOperator = "rsInvalidSearchOperator";

			public const string rsQueryTimeout = "rsQueryTimeout";

			public const string rsReportHistoryNotFound = "rsReportHistoryNotFound";

			public const string rsSchedulerNotResponding = "rsSchedulerNotResponding";

			public const string rsSchedulerNotRespondingPreventsPinning = "rsSchedulerNotRespondingPreventsPinning";

			public const string rsHasUserProfileDependencies = "rsHasUserProfileDependencies";

			public const string rsScheduleNotFound = "rsScheduleNotFound";

			public const string rsScheduleAlreadyExists = "rsScheduleAlreadyExists";

			public const string rsSharePoitScheduleAlreadyExists = "rsSharePoitScheduleAlreadyExists";

			public const string rsScheduleDateTimeRangeException = "rsScheduleDateTimeRangeException";

			public const string rsInvalidSqlAgentJob = "rsInvalidSqlAgentJob";

			public const string rsUserCannotOwnSubscription = "rsUserCannotOwnSubscription";

			public const string rsCannotActivateSubscription = "rsCannotActivateSubscription";

			public const string rsSubscriptionNotFound = "rsSubscriptionNotFound";

			public const string rsCacheRefreshPlanNotFound = "rsCacheRefreshPlanNotFound";

			public const string rsDeliveryExtensionNotFound = "rsDeliveryExtensionNotFound";

			public const string rsDeliverError = "rsDeliverError";

			public const string rsCannotPrepareQuery = "rsCannotPrepareQuery";

			public const string rsInvalidExtensionParameter = "rsInvalidExtensionParameter";

			public const string rsInvalidSubscription = "rsInvalidSubscription";

			public const string rsPBIServiceUnavailable = "rsPBIServiceUnavailable";

			public const string rsUnknownEventType = "rsUnknownEventType";

			public const string rsCannotSubscribeToEvent = "rsCannotSubscribeToEvent";

			public const string rsReservedRole = "rsReservedRole";

			public const string rsTaskNotFound = "rsTaskNotFound";

			public const string rsMixedTasks = "rsMixedTasks";

			public const string rsEmptyRole = "rsEmptyRole";

			public const string rsInheritedPolicy = "rsInheritedPolicy";

			public const string rsInheritedPolicyModelItem = "rsInheritedPolicyModelItem";

			public const string rsInvalidPolicyDefinition = "rsInvalidPolicyDefinition";

			public const string rsRoleAlreadyExists = "rsRoleAlreadyExists";

			public const string rsRoleNotFound = "rsRoleNotFound";

			public const string rsCannotDeleteRootPolicy = "rsCannotDeleteRootPolicy";

			public const string rsAccessDenied = "rsAccessDenied";

			public const string rsSecureConnectionRequired = "rsSecureConnectionRequired";

			public const string rsAssemblyNotPermissioned = "rsAssemblyNotPermissioned";

			public const string rsBatchNotFound = "rsBatchNotFound";

			public const string rsModelItemNotFound = "rsModelItemNotFound";

			public const string rsModelRootPolicyRequired = "rsModelRootPolicyRequired";

			public const string rsModelIDMismatch = "rsModelIDMismatch";

			public const string rsModelNotGenerated = "rsModelNotGenerated";

			public const string rsModelGenerationNotSupported = "rsModelGenerationNotSupported";

			public const string rsModelGenerationError = "rsModelGenerationError";

			public const string rsInvalidReportServerDatabase = "rsInvalidReportServerDatabase";

			public const string rsSharePointObjectModelNotInstalled = "rsSharePointObjectModelNotInstalled";

			public const string rsReportServerDatabaseUnavailable = "rsReportServerDatabaseUnavailable";

			public const string rsReportServerDatabaseError = "rsReportServerDatabaseError";

			public const string rsSemanticQueryExtensionNotFound = "rsSemanticQueryExtensionNotFound";

			public const string rsEvaluationCopyExpired = "rsEvaluationCopyExpired";

			public const string rsServerBusy = "rsServerBusy";

			public const string rsFailedToDecryptConfigInformation = "rsFailedToDecryptConfigInformation";

			public const string rsReportServerDisabled = "rsReportServerDisabled";

			public const string rsReportServerKeyContainerError = "rsReportServerKeyContainerError";

			public const string rsKeyStateNotValid = "rsKeyStateNotValid";

			public const string rsReportServerNotActivated = "rsReportServerNotActivated";

			public const string rsReportServerServiceUnavailable = "rsReportServerServiceUnavailable";

			public const string rsAccessDeniedToSecureData = "rsAccessDeniedToSecureData";

			public const string rsInvalidModelDrillthroughReport = "rsInvalidModelDrillthroughReport";

			public const string rsCannotValidateEncryptedData = "rsCannotValidateEncryptedData";

			public const string rsRemotePublicKeyUnavailable = "rsRemotePublicKeyUnavailable";

			public const string rsFailedToExportSymmetricKey = "rsFailedToExportSymmetricKey";

			public const string rsBackupKeyPasswordInvalid = "rsBackupKeyPasswordInvalid";

			public const string rsErrorOpeningConnection = "rsErrorOpeningConnection";

			public const string rsAppDomainManagerError = "rsAppDomainManagerError";

			public const string rsHttpRuntimeError = "rsHttpRuntimeError";

			public const string rsHttpRuntimeInternalError = "rsHttpRuntimeInternalError";

			public const string rsHttpRuntimeClientDisconnectionError = "rsHttpRuntimeClientDisconnectionError";

			public const string rsReportBuilderFileTransmissionError = "rsReportBuilderFileTransmissionError";

			public const string rsInternalResourceNotSpecifiedError = "rsInternalResourceNotSpecifiedError";

			public const string rsInternalResourceNotFoundError = "rsInternalResourceNotFoundError";

			public const string SkuNonSqlDataSources = "SkuNonSqlDataSources";

			public const string SkuOtherSkuDatasources = "SkuOtherSkuDatasources";

			public const string SkuRemoteDataSources = "SkuRemoteDataSources";

			public const string SkuCaching = "SkuCaching";

			public const string SkuExecutionSnapshots = "SkuExecutionSnapshots";

			public const string SkuHistory = "SkuHistory";

			public const string SkuDelivery = "SkuDelivery";

			public const string SkuScheduling = "SkuScheduling";

			public const string SkuExtensibility = "SkuExtensibility";

			public const string SkuCustomAuth = "SkuCustomAuth";

			public const string SkuSharepoint = "SkuSharepoint";

			public const string SkuScaleOut = "SkuScaleOut";

			public const string SkuSubscriptions = "SkuSubscriptions";

			public const string SkuDataDrivenSubscriptions = "SkuDataDrivenSubscriptions";

			public const string SkuCustomRolesSecurity = "SkuCustomRolesSecurity";

			public const string SkuReportBuilder = "SkuReportBuilder";

			public const string SkuModelItemSecurity = "SkuModelItemSecurity";

			public const string SkuDynamicDrillthrough = "SkuDynamicDrillthrough";

			public const string SkuNoCpuThrottling = "SkuNoCpuThrottling";

			public const string SkuNoMemoryThrottling = "SkuNoMemoryThrottling";

			public const string SkuEventGeneration = "SkuEventGeneration";

			public const string SkuComponentLibrary = "SkuComponentLibrary";

			public const string SkuSharedDataset = "SkuSharedDataset";

			public const string SkuDataAlerting = "SkuDataAlerting";

			public const string SkuCrescent = "SkuCrescent";

			public const string SkuKpiItems = "SkuKpiItems";

			public const string SkuMobileReportItems = "SkuMobileReportItems";

			public const string rsRestrictedItem = "rsRestrictedItem";

			public const string rsSharePointError = "rsSharePointError";

			public const string rsSharePointContentDBAccessError = "rsSharePointContentDBAccessError";

			public const string rsStoredCredentialsOutOfSync = "rsStoredCredentialsOutOfSync";

			public const string rsODCVersionNotSupported = "rsODCVersionNotSupported";

			public const string rsOperationNotSupportedSharePointMode = "rsOperationNotSupportedSharePointMode";

			public const string rsOperationNotSupportedNativeMode = "rsOperationNotSupportedNativeMode";

			public const string rsUnsupportedParameterForMode = "rsUnsupportedParameterForMode";

			public const string rsContainerNotSupported = "rsContainerNotSupported";

			public const string rsPropertyDisabled = "rsPropertyDisabled";

			public const string rsPropertyDisabledNativeMode = "rsPropertyDisabledNativeMode";

			public const string rsInvalidRSDSSchema = "rsInvalidRSDSSchema";

			public const string rsSecurityZoneNotSupported = "rsSecurityZoneNotSupported";

			public const string rsAuthenticationExtensionError = "rsAuthenticationExtensionError";

			public const string rsFileSize = "rsFileSize";

			public const string rsRdceExtraElementError = "rsRdceExtraElementError";

			public const string rsRdceMismatchError = "rsRdceMismatchError";

			public const string rsRdceInvalidRdlError = "rsRdceInvalidRdlError";

			public const string rsRdceInvalidConfigurationError = "rsRdceInvalidConfigurationError";

			public const string rsRdceInvalidItemTypeError = "rsRdceInvalidItemTypeError";

			public const string rsRdceInvalidExecutionOptionError = "rsRdceInvalidExecutionOptionError";

			public const string rsRdceInvalidCacheOptionError = "rsRdceInvalidCacheOptionError";

			public const string rsRdceWrappedException = "rsRdceWrappedException";

			public const string rsRdceMismatchRdlVersion = "rsRdceMismatchRdlVersion";

			public const string rsInvalidOperation = "rsInvalidOperation";

			public const string rsAuthorizationExtensionError = "rsAuthorizationExtensionError";

			public const string rsDataCacheMismatch = "rsDataCacheMismatch";

			public const string rsSoapExtensionInvalidPreambleLengthError = "rsSoapExtensionInvalidPreambleLengthError";

			public const string rsSoapExtensionInvalidPreambleError = "rsSoapExtensionInvalidPreambleError";

			public const string rsUrlRemapError = "rsUrlRemapError";

			public const string rsRequestThroughHttpRedirectorNotSupportedError = "rsRequestThroughHttpRedirectorNotSupportedError";

			public const string rsUnhandledHttpApplicationError = "rsUnhandledHttpApplicationError";

			public const string rsInvalidCatalogRecord = "rsInvalidCatalogRecord";

			public const string rsUnknownFeedColumnType = "rsUnknownFeedColumnType";

			public const string rsFeedValueOutOfRange = "rsFeedValueOutOfRange";

			public const string rsMissingFeedColumnException = "rsMissingFeedColumnException";

			public const string rFeedColumnTypeMismatchException = "rFeedColumnTypeMismatchException";

			public const string rsClaimsToWindowsTokenError = "rsClaimsToWindowsTokenError";

			public const string rsClaimsToWindowsTokenLoginTypeError = "rsClaimsToWindowsTokenLoginTypeError";

			public const string GetExternalImagesInvalidNamespace = "GetExternalImagesInvalidNamespace";

			public const string GetExternalImagesInvalidSyntax = "GetExternalImagesInvalidSyntax";

			public const string rsSecureStoreContextUrlNotSpecified = "rsSecureStoreContextUrlNotSpecified";

			public const string rsSecureStoreInvalidLookupContext = "rsSecureStoreInvalidLookupContext";

			public const string rsSecureStoreCannotRetrieveCredentials = "rsSecureStoreCannotRetrieveCredentials";

			public const string rsSecureStoreMissingCredentialFields = "rsSecureStoreMissingCredentialFields";

			public const string rsSecureStoreAmbiguousCredentialFields = "rsSecureStoreAmbiguousCredentialFields";

			public const string rsSecureStoreUnsupportedCredentialField = "rsSecureStoreUnsupportedCredentialField";

			public const string rsSecureStoreUnsupportedSharePointVersion = "rsSecureStoreUnsupportedSharePointVersion";

			public const string ProductName = "ProductName";

			public const string ProductNameAndVersion = "ProductNameAndVersion";

			public const string rsMissingParameter = "rsMissingParameter";

			public const string rsInvalidParameter = "rsInvalidParameter";

			public const string rsInvalidElement = "rsInvalidElement";

			public const string rsInvalidXml = "rsInvalidXml";

			public const string rsUnrecognizedXmlElement = "rsUnrecognizedXmlElement";

			public const string rsMissingElement = "rsMissingElement";

			public const string rsElementTypeMismatch = "rsElementTypeMismatch";

			public const string rsInvalidElementCombination = "rsInvalidElementCombination";

			public const string rsInvalidMultipleElementCombination = "rsInvalidMultipleElementCombination";

			public const string rsMalformedXml = "rsMalformedXml";

			public const string rsInvalidItemPath = "rsInvalidItemPath";

			public const string rsItemPathLengthExceeded = "rsItemPathLengthExceeded";

			public const string rsInvalidItemName = "rsInvalidItemName";

			public const string rsItemNotFound = "rsItemNotFound";

			public const string rsItemContentInvalid = "rsItemContentInvalid";

			public const string rsWrongItemType = "rsWrongItemType";

			public const string rsSnapshotVersionMismatch = "rsSnapshotVersionMismatch";

			public const string rsReadOnlyReportParameter = "rsReadOnlyReportParameter";

			public const string rsReadOnlyDataSetParameter = "rsReadOnlyDataSetParameter";

			public const string rsUnknownReportParameter = "rsUnknownReportParameter";

			public const string rsUnknownDataSetParameter = "rsUnknownDataSetParameter";

			public const string rsReportParameterValueNotSet = "rsReportParameterValueNotSet";

			public const string rsDataSetParameterValueNotSet = "rsDataSetParameterValueNotSet";

			public const string rsReportParameterTypeMismatch = "rsReportParameterTypeMismatch";

			public const string rsInvalidReportParameter = "rsInvalidReportParameter";

			public const string rsDataSourceNotFound = "rsDataSourceNotFound";

			public const string rsDataSourceNoPromptException = "rsDataSourceNoPromptException";

			public const string rsInvalidDataSourceCredentialSetting = "rsInvalidDataSourceCredentialSetting";

			public const string rsInvalidDataSourceCredentialSettingForITokenDataExtension = "rsInvalidDataSourceCredentialSettingForITokenDataExtension";

			public const string rsWindowsIntegratedSecurityDisabled = "rsWindowsIntegratedSecurityDisabled";

			public const string internalDataSourceNotFound = "internalDataSourceNotFound";

			public const string cannotBuildExternalConnectionString = "cannotBuildExternalConnectionString";

			public const string rsDataSourceDisabled = "rsDataSourceDisabled";

			public const string rsInvalidDataSourceReference = "rsInvalidDataSourceReference";

			public const string rsInvalidDataSetReference = "rsInvalidDataSetReference";

			public const string rsInvalidDataSourceType = "rsInvalidDataSourceType";

			public const string rsInvalidDataSourceCount = "rsInvalidDataSourceCount";

			public const string rsCannotRetrieveModel = "rsCannotRetrieveModel";

			public const string rsModelRetrievalCanceled = "rsModelRetrievalCanceled";

			public const string rsExecuteQueriesFailure = "rsExecuteQueriesFailure";

			public const string rsDataSetExecutionError = "rsDataSetExecutionError";

			public const string rsUnknownUserName = "rsUnknownUserName";

			public const string rsInternalError = "rsInternalError";

			public const string rsStreamOperationFailed = "rsStreamOperationFailed";

			public const string rsNotSupported = "rsNotSupported";

			public const string rsNotEnabled = "rsNotEnabled";

			public const string rsReportServerDatabaseLogonFailed = "rsReportServerDatabaseLogonFailed";

			public const string rsDataExtensionNotFound = "rsDataExtensionNotFound";

			public const string rsReportTimeoutExpired = "rsReportTimeoutExpired";

			public const string rsJobWasCanceled = "rsJobWasCanceled";

			public const string rsOperationNotSupported = "rsOperationNotSupported";

			public const string rsServerConfigurationError = "rsServerConfigurationError";

			public const string rsWinAuthz = "rsWinAuthz";

			public const string rsWinAuthz5 = "rsWinAuthz5";

			public const string rsWinAuthz1355 = "rsWinAuthz1355";

			public const string rsEventLogSourceNotFound = "rsEventLogSourceNotFound";

			public const string rsLogonFailed = "rsLogonFailed";

			public const string rsEncryptedDataUnavailable = "rsEncryptedDataUnavailable";

			public const string rsErrorNotVisibleToRemoteBrowsers = "rsErrorNotVisibleToRemoteBrowsers";

			public const string rsFileExtensionRequired = "rsFileExtensionRequired";

			public const string rsFileExtensionViolation = "rsFileExtensionViolation";

			public const string rsDataSetNotFound = "rsDataSetNotFound";

			public const string rsComponentPublishingError = "rsComponentPublishingError";

			public const string rsInvalidProgressiveFormatError = "rsInvalidProgressiveFormatError";

			public const string rsProgressiveFormatElementMissingError = "rsProgressiveFormatElementMissingError";

			public const string rsProgressiveMessageWriteError = "rsProgressiveMessageWriteError";

			public const string rsProgressiveMessageWriteElementError = "rsProgressiveMessageWriteElementError";

			public const string LogClientTraceEventsInvalidSyntax = "LogClientTraceEventsInvalidSyntax";

			public const string LogClientTraceEventsInvalidNamespace = "LogClientTraceEventsInvalidNamespace";

			public const string rsVersionMismatch = "rsVersionMismatch";

			public const string rsClosingRegisteredStreamException = "rsClosingRegisteredStreamException";

			public const string rsInvalidSessionId = "rsInvalidSessionId";

			public const string rsInvalidConcurrentRenderEditSessionRequest = "rsInvalidConcurrentRenderEditSessionRequest";

			public const string rsSessionNotFound = "rsSessionNotFound";

			public const string rsReportSerializationError = "rsReportSerializationError";

			public const string rsInvalidSessionCatalogItems = "rsInvalidSessionCatalogItems";

			public const string MultipleSessionCatalogItemsNotSupported = "MultipleSessionCatalogItemsNotSupported";

			public const string rsApiVersionDiscontinued = "rsApiVersionDiscontinued";

			public const string rsApiVersionNotRecognized = "rsApiVersionNotRecognized";

			public const string rsRequestEncodingFormatException = "rsRequestEncodingFormatException";

			public const string rsCertificateMissingOrInvalid = "rsCertificateMissingOrInvalid";

			public const string rsResolutionFailureException = "rsResolutionFailureException";

			public const string rsReportServerStorageSingleRefreshConnectionExpected = "rsReportServerStorageSingleRefreshConnectionExpected";

			public const string rsReportServerStorageRefreshConnectionNotValidated = "rsReportServerStorageRefreshConnectionNotValidated";

			public const string rsOnPremConnectionBuilderUnknownError = "rsOnPremConnectionBuilderUnknownError";

			public const string rsOnPremConnectionBuilderConnectionStringMissing = "rsOnPremConnectionBuilderConnectionStringMissing";

			public const string rsOnPremConnectionBuilderMissingEffectiveUsername = "rsOnPremConnectionBuilderMissingEffectiveUsername";

			public const string rsIdentityClaimsMissingOrInvalid = "rsIdentityClaimsMissingOrInvalid";

			public const string rsSystemResourcePackageMetadataNotFound = "rsSystemResourcePackageMetadataNotFound";

			public const string rsSystemResourcePackageMetadataInvalid = "rsSystemResourcePackageMetadataInvalid";

			public const string rsSystemResourcePackageReferencedItemMissing = "rsSystemResourcePackageReferencedItemMissing";

			public const string rsSystemResourcePackageRequiredItemMissing = "rsSystemResourcePackageRequiredItemMissing";

			public const string rsSystemResourcePackageItemContentTypeMismatch = "rsSystemResourcePackageItemContentTypeMismatch";

			public const string rsSystemResourcePackageItemExtensionMismatch = "rsSystemResourcePackageItemExtensionMismatch";

			public const string rsSystemResourcePackageValidationFailed = "rsSystemResourcePackageValidationFailed";

			public const string rsSystemResourcePackageWrongType = "rsSystemResourcePackageWrongType";

			public const string rsAuthorizationTokenInvalidOrExpired = "rsAuthorizationTokenInvalidOrExpired";

			public static CultureInfo Culture
			{
				get
				{
					return _culture;
				}
				set
				{
					_culture = value;
				}
			}

			private Keys()
			{
			}

			public static string GetString(string key)
			{
				return resourceManager.GetString(key, _culture);
			}

			public static string GetString(string key, object arg0)
			{
				return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0);
			}

			public static string GetString(string key, object arg0, object arg1)
			{
				return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0, arg1);
			}

			public static string GetString(string key, object arg0, object arg1, object arg2)
			{
				return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0, arg1, arg2);
			}
		}

		public static CultureInfo Culture
		{
			get
			{
				return Keys.Culture;
			}
			set
			{
				Keys.Culture = value;
			}
		}

		public static string InvalidKeyValue => Keys.GetString("InvalidKeyValue");

		public static string EmptyExtensionName => Keys.GetString("EmptyExtensionName");

		public static string UIServerLoopback => Keys.GetString("UIServerLoopback");

		public static string DataSourceConnectionErrorNotVisible => Keys.GetString("DataSourceConnectionErrorNotVisible");

		public static string UserNameUnknown => Keys.GetString("UserNameUnknown");

		public static string rsInvalidParameterCombination => Keys.GetString("rsInvalidParameterCombination");

		public static string rsProcessingError => Keys.GetString("rsProcessingError");

		public static string rsStreamNotFound => Keys.GetString("rsStreamNotFound");

		public static string rsMissingSessionId => Keys.GetString("rsMissingSessionId");

		public static string rsQueryExecutionNotAllowed => Keys.GetString("rsQueryExecutionNotAllowed");

		public static string rsReportNotReady => Keys.GetString("rsReportNotReady");

		public static string rsReportSnapshotEnabled => Keys.GetString("rsReportSnapshotEnabled");

		public static string rsReportSnapshotNotEnabled => Keys.GetString("rsReportSnapshotNotEnabled");

		public static string rsOperationPreventsUnattendedExecution => Keys.GetString("rsOperationPreventsUnattendedExecution");

		public static string rsInvalidReportLink => Keys.GetString("rsInvalidReportLink");

		public static string rsSubreportFromSnapshot => Keys.GetString("rsSubreportFromSnapshot");

		public static string rsQueryTimeout => Keys.GetString("rsQueryTimeout");

		public static string rsSchedulerNotResponding => Keys.GetString("rsSchedulerNotResponding");

		public static string rsSchedulerNotRespondingPreventsPinning => Keys.GetString("rsSchedulerNotRespondingPreventsPinning");

		public static string rsScheduleDateTimeRangeException => Keys.GetString("rsScheduleDateTimeRangeException");

		public static string rsUserCannotOwnSubscription => Keys.GetString("rsUserCannotOwnSubscription");

		public static string rsCannotActivateSubscription => Keys.GetString("rsCannotActivateSubscription");

		public static string rsDeliveryExtensionNotFound => Keys.GetString("rsDeliveryExtensionNotFound");

		public static string rsDeliverError => Keys.GetString("rsDeliverError");

		public static string rsCannotPrepareQuery => Keys.GetString("rsCannotPrepareQuery");

		public static string rsMixedTasks => Keys.GetString("rsMixedTasks");

		public static string rsEmptyRole => Keys.GetString("rsEmptyRole");

		public static string rsCannotDeleteRootPolicy => Keys.GetString("rsCannotDeleteRootPolicy");

		public static string rsSecureConnectionRequired => Keys.GetString("rsSecureConnectionRequired");

		public static string rsModelRootPolicyRequired => Keys.GetString("rsModelRootPolicyRequired");

		public static string rsModelIDMismatch => Keys.GetString("rsModelIDMismatch");

		public static string rsModelNotGenerated => Keys.GetString("rsModelNotGenerated");

		public static string rsModelGenerationNotSupported => Keys.GetString("rsModelGenerationNotSupported");

		public static string rsModelGenerationError => Keys.GetString("rsModelGenerationError");

		public static string rsReportServerDatabaseUnavailable => Keys.GetString("rsReportServerDatabaseUnavailable");

		public static string rsReportServerDatabaseError => Keys.GetString("rsReportServerDatabaseError");

		public static string rsEvaluationCopyExpired => Keys.GetString("rsEvaluationCopyExpired");

		public static string rsServerBusy => Keys.GetString("rsServerBusy");

		public static string rsReportServerDisabled => Keys.GetString("rsReportServerDisabled");

		public static string rsKeyStateNotValid => Keys.GetString("rsKeyStateNotValid");

		public static string rsReportServerNotActivated => Keys.GetString("rsReportServerNotActivated");

		public static string rsAccessDeniedToSecureData => Keys.GetString("rsAccessDeniedToSecureData");

		public static string rsCannotValidateEncryptedData => Keys.GetString("rsCannotValidateEncryptedData");

		public static string rsRemotePublicKeyUnavailable => Keys.GetString("rsRemotePublicKeyUnavailable");

		public static string rsFailedToExportSymmetricKey => Keys.GetString("rsFailedToExportSymmetricKey");

		public static string rsBackupKeyPasswordInvalid => Keys.GetString("rsBackupKeyPasswordInvalid");

		public static string rsInternalResourceNotSpecifiedError => Keys.GetString("rsInternalResourceNotSpecifiedError");

		public static string SkuNonSqlDataSources => Keys.GetString("SkuNonSqlDataSources");

		public static string SkuOtherSkuDatasources => Keys.GetString("SkuOtherSkuDatasources");

		public static string SkuRemoteDataSources => Keys.GetString("SkuRemoteDataSources");

		public static string SkuCaching => Keys.GetString("SkuCaching");

		public static string SkuExecutionSnapshots => Keys.GetString("SkuExecutionSnapshots");

		public static string SkuHistory => Keys.GetString("SkuHistory");

		public static string SkuDelivery => Keys.GetString("SkuDelivery");

		public static string SkuScheduling => Keys.GetString("SkuScheduling");

		public static string SkuExtensibility => Keys.GetString("SkuExtensibility");

		public static string SkuCustomAuth => Keys.GetString("SkuCustomAuth");

		public static string SkuSharepoint => Keys.GetString("SkuSharepoint");

		public static string SkuScaleOut => Keys.GetString("SkuScaleOut");

		public static string SkuSubscriptions => Keys.GetString("SkuSubscriptions");

		public static string SkuDataDrivenSubscriptions => Keys.GetString("SkuDataDrivenSubscriptions");

		public static string SkuCustomRolesSecurity => Keys.GetString("SkuCustomRolesSecurity");

		public static string SkuReportBuilder => Keys.GetString("SkuReportBuilder");

		public static string SkuModelItemSecurity => Keys.GetString("SkuModelItemSecurity");

		public static string SkuDynamicDrillthrough => Keys.GetString("SkuDynamicDrillthrough");

		public static string SkuNoCpuThrottling => Keys.GetString("SkuNoCpuThrottling");

		public static string SkuNoMemoryThrottling => Keys.GetString("SkuNoMemoryThrottling");

		public static string SkuEventGeneration => Keys.GetString("SkuEventGeneration");

		public static string SkuComponentLibrary => Keys.GetString("SkuComponentLibrary");

		public static string SkuSharedDataset => Keys.GetString("SkuSharedDataset");

		public static string SkuDataAlerting => Keys.GetString("SkuDataAlerting");

		public static string SkuCrescent => Keys.GetString("SkuCrescent");

		public static string SkuKpiItems => Keys.GetString("SkuKpiItems");

		public static string SkuMobileReportItems => Keys.GetString("SkuMobileReportItems");

		public static string rsSharePointError => Keys.GetString("rsSharePointError");

		public static string rsSharePointContentDBAccessError => Keys.GetString("rsSharePointContentDBAccessError");

		public static string rsODCVersionNotSupported => Keys.GetString("rsODCVersionNotSupported");

		public static string rsOperationNotSupportedSharePointMode => Keys.GetString("rsOperationNotSupportedSharePointMode");

		public static string rsOperationNotSupportedNativeMode => Keys.GetString("rsOperationNotSupportedNativeMode");

		public static string rsContainerNotSupported => Keys.GetString("rsContainerNotSupported");

		public static string rsPropertyDisabled => Keys.GetString("rsPropertyDisabled");

		public static string rsPropertyDisabledNativeMode => Keys.GetString("rsPropertyDisabledNativeMode");

		public static string rsInvalidRSDSSchema => Keys.GetString("rsInvalidRSDSSchema");

		public static string rsSecurityZoneNotSupported => Keys.GetString("rsSecurityZoneNotSupported");

		public static string rsFileSize => Keys.GetString("rsFileSize");

		public static string rsRdceInvalidRdlError => Keys.GetString("rsRdceInvalidRdlError");

		public static string rsRdceInvalidConfigurationError => Keys.GetString("rsRdceInvalidConfigurationError");

		public static string rsRdceInvalidExecutionOptionError => Keys.GetString("rsRdceInvalidExecutionOptionError");

		public static string rsRdceInvalidCacheOptionError => Keys.GetString("rsRdceInvalidCacheOptionError");

		public static string rsRdceWrappedException => Keys.GetString("rsRdceWrappedException");

		public static string rsRdceMismatchRdlVersion => Keys.GetString("rsRdceMismatchRdlVersion");

		public static string rsInvalidOperation => Keys.GetString("rsInvalidOperation");

		public static string rsAuthorizationExtensionError => Keys.GetString("rsAuthorizationExtensionError");

		public static string rsDataCacheMismatch => Keys.GetString("rsDataCacheMismatch");

		public static string rsSoapExtensionInvalidPreambleError => Keys.GetString("rsSoapExtensionInvalidPreambleError");

		public static string rsRequestThroughHttpRedirectorNotSupportedError => Keys.GetString("rsRequestThroughHttpRedirectorNotSupportedError");

		public static string rsUnhandledHttpApplicationError => Keys.GetString("rsUnhandledHttpApplicationError");

		public static string rsInvalidCatalogRecord => Keys.GetString("rsInvalidCatalogRecord");

		public static string rsClaimsToWindowsTokenError => Keys.GetString("rsClaimsToWindowsTokenError");

		public static string rsClaimsToWindowsTokenLoginTypeError => Keys.GetString("rsClaimsToWindowsTokenLoginTypeError");

		public static string GetExternalImagesInvalidNamespace => Keys.GetString("GetExternalImagesInvalidNamespace");

		public static string GetExternalImagesInvalidSyntax => Keys.GetString("GetExternalImagesInvalidSyntax");

		public static string rsSecureStoreContextUrlNotSpecified => Keys.GetString("rsSecureStoreContextUrlNotSpecified");

		public static string rsSecureStoreInvalidLookupContext => Keys.GetString("rsSecureStoreInvalidLookupContext");

		public static string rsSecureStoreUnsupportedSharePointVersion => Keys.GetString("rsSecureStoreUnsupportedSharePointVersion");

		public static string ProductName => Keys.GetString("ProductName");

		public static string rsInvalidXml => Keys.GetString("rsInvalidXml");

		public static string rsSnapshotVersionMismatch => Keys.GetString("rsSnapshotVersionMismatch");

		public static string rsInvalidDataSourceCredentialSetting => Keys.GetString("rsInvalidDataSourceCredentialSetting");

		public static string rsInvalidDataSourceCredentialSettingForITokenDataExtension => Keys.GetString("rsInvalidDataSourceCredentialSettingForITokenDataExtension");

		public static string rsWindowsIntegratedSecurityDisabled => Keys.GetString("rsWindowsIntegratedSecurityDisabled");

		public static string internalDataSourceNotFound => Keys.GetString("internalDataSourceNotFound");

		public static string rsDataSourceDisabled => Keys.GetString("rsDataSourceDisabled");

		public static string rsModelRetrievalCanceled => Keys.GetString("rsModelRetrievalCanceled");

		public static string rsInternalError => Keys.GetString("rsInternalError");

		public static string rsStreamOperationFailed => Keys.GetString("rsStreamOperationFailed");

		public static string rsNotSupported => Keys.GetString("rsNotSupported");

		public static string rsNotEnabled => Keys.GetString("rsNotEnabled");

		public static string rsReportServerDatabaseLogonFailed => Keys.GetString("rsReportServerDatabaseLogonFailed");

		public static string rsReportTimeoutExpired => Keys.GetString("rsReportTimeoutExpired");

		public static string rsJobWasCanceled => Keys.GetString("rsJobWasCanceled");

		public static string rsLogonFailed => Keys.GetString("rsLogonFailed");

		public static string rsEncryptedDataUnavailable => Keys.GetString("rsEncryptedDataUnavailable");

		public static string rsErrorNotVisibleToRemoteBrowsers => Keys.GetString("rsErrorNotVisibleToRemoteBrowsers");

		public static string rsFileExtensionRequired => Keys.GetString("rsFileExtensionRequired");

		public static string rsComponentPublishingError => Keys.GetString("rsComponentPublishingError");

		public static string LogClientTraceEventsInvalidSyntax => Keys.GetString("LogClientTraceEventsInvalidSyntax");

		public static string LogClientTraceEventsInvalidNamespace => Keys.GetString("LogClientTraceEventsInvalidNamespace");

		public static string rsVersionMismatch => Keys.GetString("rsVersionMismatch");

		public static string rsClosingRegisteredStreamException => Keys.GetString("rsClosingRegisteredStreamException");

		public static string rsReportSerializationError => Keys.GetString("rsReportSerializationError");

		public static string MultipleSessionCatalogItemsNotSupported => Keys.GetString("MultipleSessionCatalogItemsNotSupported");

		public static string rsRequestEncodingFormatException => Keys.GetString("rsRequestEncodingFormatException");

		public static string rsOnPremConnectionBuilderUnknownError => Keys.GetString("rsOnPremConnectionBuilderUnknownError");

		public static string rsOnPremConnectionBuilderConnectionStringMissing => Keys.GetString("rsOnPremConnectionBuilderConnectionStringMissing");

		public static string rsOnPremConnectionBuilderMissingEffectiveUsername => Keys.GetString("rsOnPremConnectionBuilderMissingEffectiveUsername");

		public static string rsSystemResourcePackageMetadataNotFound => Keys.GetString("rsSystemResourcePackageMetadataNotFound");

		public static string rsSystemResourcePackageMetadataInvalid => Keys.GetString("rsSystemResourcePackageMetadataInvalid");

		public static string rsSystemResourcePackageValidationFailed => Keys.GetString("rsSystemResourcePackageValidationFailed");

		public static string rsAuthorizationTokenInvalidOrExpired => Keys.GetString("rsAuthorizationTokenInvalidOrExpired");

		protected ErrorStrings()
		{
		}

		public static string InvalidConfigElement(string element)
		{
			return Keys.GetString("InvalidConfigElement", element);
		}

		public static string CouldNotFindElement(string element)
		{
			return Keys.GetString("CouldNotFindElement", element);
		}

		public static string DuplicateConfigElement(string element)
		{
			return Keys.GetString("DuplicateConfigElement", element);
		}

		public static string SameExtensionName(string name)
		{
			return Keys.GetString("SameExtensionName", name);
		}

		public static string SameEventType(string type)
		{
			return Keys.GetString("SameEventType", type);
		}

		public static string NoEventForEventProcessor(string name)
		{
			return Keys.GetString("NoEventForEventProcessor", name);
		}

		public static string rsEventExtensionNotFoundException(string @event)
		{
			return Keys.GetString("rsEventExtensionNotFoundException", @event);
		}

		public static string rsEventMaxRetryExceededException(string @event)
		{
			return Keys.GetString("rsEventMaxRetryExceededException", @event);
		}

		public static string rsMissingRequiredPropertyForItemType(string propertyName)
		{
			return Keys.GetString("rsMissingRequiredPropertyForItemType", propertyName);
		}

		public static string rsParameterTypeMismatch(string parameterName)
		{
			return Keys.GetString("rsParameterTypeMismatch", parameterName);
		}

		public static string rsStoredParameterNotFound(string StoredParameterID)
		{
			return Keys.GetString("rsStoredParameterNotFound", StoredParameterID);
		}

		public static string rsItemAlreadyExists(string itemPath)
		{
			return Keys.GetString("rsItemAlreadyExists", itemPath);
		}

		public static string rsInvalidMove(string itemPath, string targetPath)
		{
			return Keys.GetString("rsInvalidMove", itemPath, targetPath);
		}

		public static string rsInvalidDestination(string sourcePath, string targetPath)
		{
			return Keys.GetString("rsInvalidDestination", sourcePath, targetPath);
		}

		public static string rsReservedItem(string itemPath)
		{
			return Keys.GetString("rsReservedItem", itemPath);
		}

		public static string rsReadOnlyProperty(string property)
		{
			return Keys.GetString("rsReadOnlyProperty", property);
		}

		public static string rsExecutionNotFound(string executionID)
		{
			return Keys.GetString("rsExecutionNotFound", executionID);
		}

		public static string rsSPSiteNotFound(string id)
		{
			return Keys.GetString("rsSPSiteNotFound", id);
		}

		public static string rsCachingNotEnabled(string item)
		{
			return Keys.GetString("rsCachingNotEnabled", item);
		}

		public static string rsInvalidSearchOperator(string operation)
		{
			return Keys.GetString("rsInvalidSearchOperator", operation);
		}

		public static string rsReportHistoryNotFound(string reportPath, string snapshotId)
		{
			return Keys.GetString("rsReportHistoryNotFound", reportPath, snapshotId);
		}

		public static string rsHasUserProfileDependencies(string reportName)
		{
			return Keys.GetString("rsHasUserProfileDependencies", reportName);
		}

		public static string rsScheduleNotFound(string name)
		{
			return Keys.GetString("rsScheduleNotFound", name);
		}

		public static string rsScheduleAlreadyExists(string name)
		{
			return Keys.GetString("rsScheduleAlreadyExists", name);
		}

		public static string rsSharePoitScheduleAlreadyExists(string name, string path)
		{
			return Keys.GetString("rsSharePoitScheduleAlreadyExists", name, path);
		}

		public static string rsInvalidSqlAgentJob(string taskName)
		{
			return Keys.GetString("rsInvalidSqlAgentJob", taskName);
		}

		public static string rsSubscriptionNotFound(string name)
		{
			return Keys.GetString("rsSubscriptionNotFound", name);
		}

		public static string rsCacheRefreshPlanNotFound(string name)
		{
			return Keys.GetString("rsCacheRefreshPlanNotFound", name);
		}

		public static string rsInvalidExtensionParameter(string reason)
		{
			return Keys.GetString("rsInvalidExtensionParameter", reason);
		}

		public static string rsInvalidSubscription(string ID)
		{
			return Keys.GetString("rsInvalidSubscription", ID);
		}

		public static string rsPBIServiceUnavailable(string correlationId)
		{
			return Keys.GetString("rsPBIServiceUnavailable", correlationId);
		}

		public static string rsUnknownEventType(string eventType)
		{
			return Keys.GetString("rsUnknownEventType", eventType);
		}

		public static string rsCannotSubscribeToEvent(string eventType)
		{
			return Keys.GetString("rsCannotSubscribeToEvent", eventType);
		}

		public static string rsReservedRole(string roleName)
		{
			return Keys.GetString("rsReservedRole", roleName);
		}

		public static string rsTaskNotFound(string taskName)
		{
			return Keys.GetString("rsTaskNotFound", taskName);
		}

		public static string rsInheritedPolicy(string itemPath)
		{
			return Keys.GetString("rsInheritedPolicy", itemPath);
		}

		public static string rsInheritedPolicyModelItem(string itemPath, string modelItemID)
		{
			return Keys.GetString("rsInheritedPolicyModelItem", itemPath, modelItemID);
		}

		public static string rsInvalidPolicyDefinition(string principalName)
		{
			return Keys.GetString("rsInvalidPolicyDefinition", principalName);
		}

		public static string rsRoleAlreadyExists(string roleName)
		{
			return Keys.GetString("rsRoleAlreadyExists", roleName);
		}

		public static string rsRoleNotFound(string roleName)
		{
			return Keys.GetString("rsRoleNotFound", roleName);
		}

		public static string rsAccessDenied(string userName)
		{
			return Keys.GetString("rsAccessDenied", userName);
		}

		public static string rsAssemblyNotPermissioned(string assemblyName)
		{
			return Keys.GetString("rsAssemblyNotPermissioned", assemblyName);
		}

		public static string rsBatchNotFound(string batchId)
		{
			return Keys.GetString("rsBatchNotFound", batchId);
		}

		public static string rsModelItemNotFound(string modelPath, string modelItemID)
		{
			return Keys.GetString("rsModelItemNotFound", modelPath, modelItemID);
		}

		public static string rsInvalidReportServerDatabase(string storedVersion, string expectedVersion)
		{
			return Keys.GetString("rsInvalidReportServerDatabase", storedVersion, expectedVersion);
		}

		public static string rsSharePointObjectModelNotInstalled(string error)
		{
			return Keys.GetString("rsSharePointObjectModelNotInstalled", error);
		}

		public static string rsSemanticQueryExtensionNotFound(string extension)
		{
			return Keys.GetString("rsSemanticQueryExtensionNotFound", extension);
		}

		public static string rsFailedToDecryptConfigInformation(string configElement)
		{
			return Keys.GetString("rsFailedToDecryptConfigInformation", configElement);
		}

		public static string rsReportServerKeyContainerError(string accountName)
		{
			return Keys.GetString("rsReportServerKeyContainerError", accountName);
		}

		public static string rsReportServerServiceUnavailable(string serviceName)
		{
			return Keys.GetString("rsReportServerServiceUnavailable", serviceName);
		}

		public static string rsInvalidModelDrillthroughReport(string reportName)
		{
			return Keys.GetString("rsInvalidModelDrillthroughReport", reportName);
		}

		public static string rsErrorOpeningConnection(string dataSourceName)
		{
			return Keys.GetString("rsErrorOpeningConnection", dataSourceName);
		}

		public static string rsAppDomainManagerError(string appDomain)
		{
			return Keys.GetString("rsAppDomainManagerError", appDomain);
		}

		public static string rsHttpRuntimeError(string appDomain)
		{
			return Keys.GetString("rsHttpRuntimeError", appDomain);
		}

		public static string rsHttpRuntimeInternalError(string appDomain)
		{
			return Keys.GetString("rsHttpRuntimeInternalError", appDomain);
		}

		public static string rsHttpRuntimeClientDisconnectionError(string appdomain, string hr)
		{
			return Keys.GetString("rsHttpRuntimeClientDisconnectionError", appdomain, hr);
		}

		public static string rsReportBuilderFileTransmissionError(string fileName)
		{
			return Keys.GetString("rsReportBuilderFileTransmissionError", fileName);
		}

		public static string rsInternalResourceNotFoundError(string imageId)
		{
			return Keys.GetString("rsInternalResourceNotFoundError", imageId);
		}

		public static string rsRestrictedItem(string itemPath)
		{
			return Keys.GetString("rsRestrictedItem", itemPath);
		}

		public static string rsStoredCredentialsOutOfSync(string path)
		{
			return Keys.GetString("rsStoredCredentialsOutOfSync", path);
		}

		public static string rsUnsupportedParameterForMode(string mode, string parameterName)
		{
			return Keys.GetString("rsUnsupportedParameterForMode", mode, parameterName);
		}

		public static string rsAuthenticationExtensionError(string message)
		{
			return Keys.GetString("rsAuthenticationExtensionError", message);
		}

		public static string rsRdceExtraElementError(string nodeName)
		{
			return Keys.GetString("rsRdceExtraElementError", nodeName);
		}

		public static string rsRdceMismatchError(string rdceSet, string rdceConfigured)
		{
			return Keys.GetString("rsRdceMismatchError", rdceSet, rdceConfigured);
		}

		public static string rsRdceInvalidItemTypeError(string type)
		{
			return Keys.GetString("rsRdceInvalidItemTypeError", type);
		}

		public static string rsSoapExtensionInvalidPreambleLengthError(string length)
		{
			return Keys.GetString("rsSoapExtensionInvalidPreambleLengthError", length);
		}

		public static string rsUrlRemapError(string url)
		{
			return Keys.GetString("rsUrlRemapError", url);
		}

		public static string rsUnknownFeedColumnType(string column)
		{
			return Keys.GetString("rsUnknownFeedColumnType", column);
		}

		public static string rsFeedValueOutOfRange(string column)
		{
			return Keys.GetString("rsFeedValueOutOfRange", column);
		}

		public static string rsMissingFeedColumnException(string column)
		{
			return Keys.GetString("rsMissingFeedColumnException", column);
		}

		public static string rFeedColumnTypeMismatchException(string column)
		{
			return Keys.GetString("rFeedColumnTypeMismatchException", column);
		}

		public static string rsSecureStoreCannotRetrieveCredentials(string innerMessage)
		{
			return Keys.GetString("rsSecureStoreCannotRetrieveCredentials", innerMessage);
		}

		public static string rsSecureStoreMissingCredentialFields(string appId)
		{
			return Keys.GetString("rsSecureStoreMissingCredentialFields", appId);
		}

		public static string rsSecureStoreAmbiguousCredentialFields(string appId)
		{
			return Keys.GetString("rsSecureStoreAmbiguousCredentialFields", appId);
		}

		public static string rsSecureStoreUnsupportedCredentialField(string appId)
		{
			return Keys.GetString("rsSecureStoreUnsupportedCredentialField", appId);
		}

		public static string ProductNameAndVersion(string version)
		{
			return Keys.GetString("ProductNameAndVersion", version);
		}

		public static string rsMissingParameter(string parameterName)
		{
			return Keys.GetString("rsMissingParameter", parameterName);
		}

		public static string rsInvalidParameter(string parameterName)
		{
			return Keys.GetString("rsInvalidParameter", parameterName);
		}

		public static string rsInvalidElement(string elementName)
		{
			return Keys.GetString("rsInvalidElement", elementName);
		}

		public static string rsUnrecognizedXmlElement(string elementName)
		{
			return Keys.GetString("rsUnrecognizedXmlElement", elementName);
		}

		public static string rsMissingElement(string elementName)
		{
			return Keys.GetString("rsMissingElement", elementName);
		}

		public static string rsElementTypeMismatch(string name)
		{
			return Keys.GetString("rsElementTypeMismatch", name);
		}

		public static string rsInvalidElementCombination(string firstElementName, string secondElementName)
		{
			return Keys.GetString("rsInvalidElementCombination", firstElementName, secondElementName);
		}

		public static string rsInvalidMultipleElementCombination(string firstElementName, string secondElementName, string thirdElement)
		{
			return Keys.GetString("rsInvalidMultipleElementCombination", firstElementName, secondElementName, thirdElement);
		}

		public static string rsMalformedXml(string error)
		{
			return Keys.GetString("rsMalformedXml", error);
		}

		public static string rsInvalidItemPath(string itemPath, int maxLength)
		{
			return Keys.GetString("rsInvalidItemPath", itemPath, maxLength);
		}

		public static string rsItemPathLengthExceeded(string itemPath, int maxLength)
		{
			return Keys.GetString("rsItemPathLengthExceeded", itemPath, maxLength);
		}

		public static string rsInvalidItemName(string itemName, int maxLength)
		{
			return Keys.GetString("rsInvalidItemName", itemName, maxLength);
		}

		public static string rsItemNotFound(string itemPath)
		{
			return Keys.GetString("rsItemNotFound", itemPath);
		}

		public static string rsItemContentInvalid(string itemPath)
		{
			return Keys.GetString("rsItemContentInvalid", itemPath);
		}

		public static string rsWrongItemType(string itemPath)
		{
			return Keys.GetString("rsWrongItemType", itemPath);
		}

		public static string rsReadOnlyReportParameter(string parameterName)
		{
			return Keys.GetString("rsReadOnlyReportParameter", parameterName);
		}

		public static string rsReadOnlyDataSetParameter(string parameterName)
		{
			return Keys.GetString("rsReadOnlyDataSetParameter", parameterName);
		}

		public static string rsUnknownReportParameter(string parameterName)
		{
			return Keys.GetString("rsUnknownReportParameter", parameterName);
		}

		public static string rsUnknownDataSetParameter(string parameterName)
		{
			return Keys.GetString("rsUnknownDataSetParameter", parameterName);
		}

		public static string rsReportParameterValueNotSet(string parameterName)
		{
			return Keys.GetString("rsReportParameterValueNotSet", parameterName);
		}

		public static string rsDataSetParameterValueNotSet(string parameterName)
		{
			return Keys.GetString("rsDataSetParameterValueNotSet", parameterName);
		}

		public static string rsReportParameterTypeMismatch(string parameterName)
		{
			return Keys.GetString("rsReportParameterTypeMismatch", parameterName);
		}

		public static string rsInvalidReportParameter(string parameterName)
		{
			return Keys.GetString("rsInvalidReportParameter", parameterName);
		}

		public static string rsDataSourceNotFound(string dataSource)
		{
			return Keys.GetString("rsDataSourceNotFound", dataSource);
		}

		public static string rsDataSourceNoPromptException(string dataSource)
		{
			return Keys.GetString("rsDataSourceNoPromptException", dataSource);
		}

		public static string cannotBuildExternalConnectionString(string dataSource)
		{
			return Keys.GetString("cannotBuildExternalConnectionString", dataSource);
		}

		public static string rsInvalidDataSourceReference(string dataSourceName)
		{
			return Keys.GetString("rsInvalidDataSourceReference", dataSourceName);
		}

		public static string rsInvalidDataSetReference(string dataSetName)
		{
			return Keys.GetString("rsInvalidDataSetReference", dataSetName);
		}

		public static string rsInvalidDataSourceType(string dataSourcePath)
		{
			return Keys.GetString("rsInvalidDataSourceType", dataSourcePath);
		}

		public static string rsInvalidDataSourceCount(string reportPath)
		{
			return Keys.GetString("rsInvalidDataSourceCount", reportPath);
		}

		public static string rsCannotRetrieveModel(string itemPath)
		{
			return Keys.GetString("rsCannotRetrieveModel", itemPath);
		}

		public static string rsExecuteQueriesFailure(string dataSourcePath)
		{
			return Keys.GetString("rsExecuteQueriesFailure", dataSourcePath);
		}

		public static string rsDataSetExecutionError(string dataSetPath)
		{
			return Keys.GetString("rsDataSetExecutionError", dataSetPath);
		}

		public static string rsUnknownUserName(string userName)
		{
			return Keys.GetString("rsUnknownUserName", userName);
		}

		public static string rsDataExtensionNotFound(string extension)
		{
			return Keys.GetString("rsDataExtensionNotFound", extension);
		}

		public static string rsOperationNotSupported(string operation)
		{
			return Keys.GetString("rsOperationNotSupported", operation);
		}

		public static string rsServerConfigurationError(string additionalMsg)
		{
			return Keys.GetString("rsServerConfigurationError", additionalMsg);
		}

		public static string rsWinAuthz(string methodName, string errorCode, string userName)
		{
			return Keys.GetString("rsWinAuthz", methodName, errorCode, userName);
		}

		public static string rsWinAuthz5(string methodName, string userName)
		{
			return Keys.GetString("rsWinAuthz5", methodName, userName);
		}

		public static string rsWinAuthz1355(string methodName, string userName)
		{
			return Keys.GetString("rsWinAuthz1355", methodName, userName);
		}

		public static string rsEventLogSourceNotFound(string source)
		{
			return Keys.GetString("rsEventLogSourceNotFound", source);
		}

		public static string rsFileExtensionViolation(string target, string source)
		{
			return Keys.GetString("rsFileExtensionViolation", target, source);
		}

		public static string rsDataSetNotFound(string dataSet)
		{
			return Keys.GetString("rsDataSetNotFound", dataSet);
		}

		public static string rsInvalidProgressiveFormatError(string command)
		{
			return Keys.GetString("rsInvalidProgressiveFormatError", command);
		}

		public static string rsProgressiveFormatElementMissingError(string key)
		{
			return Keys.GetString("rsProgressiveFormatElementMissingError", key);
		}

		public static string rsProgressiveMessageWriteError(string command)
		{
			return Keys.GetString("rsProgressiveMessageWriteError", command);
		}

		public static string rsProgressiveMessageWriteElementError(string key, string command)
		{
			return Keys.GetString("rsProgressiveMessageWriteElementError", key, command);
		}

		public static string rsInvalidSessionId(string sessionId)
		{
			return Keys.GetString("rsInvalidSessionId", sessionId);
		}

		public static string rsInvalidConcurrentRenderEditSessionRequest(string sessionId)
		{
			return Keys.GetString("rsInvalidConcurrentRenderEditSessionRequest", sessionId);
		}

		public static string rsSessionNotFound(string sessionId, string userName)
		{
			return Keys.GetString("rsSessionNotFound", sessionId, userName);
		}

		public static string rsInvalidSessionCatalogItems(string details)
		{
			return Keys.GetString("rsInvalidSessionCatalogItems", details);
		}

		public static string rsApiVersionDiscontinued(string serverVersion, string clientVersion)
		{
			return Keys.GetString("rsApiVersionDiscontinued", serverVersion, clientVersion);
		}

		public static string rsApiVersionNotRecognized(string serverVersion, string clientVersion)
		{
			return Keys.GetString("rsApiVersionNotRecognized", serverVersion, clientVersion);
		}

		public static string rsCertificateMissingOrInvalid(string certificateId)
		{
			return Keys.GetString("rsCertificateMissingOrInvalid", certificateId);
		}

		public static string rsResolutionFailureException(string databaseFullName)
		{
			return Keys.GetString("rsResolutionFailureException", databaseFullName);
		}

		public static string rsReportServerStorageSingleRefreshConnectionExpected(string modelId, string actualCount)
		{
			return Keys.GetString("rsReportServerStorageSingleRefreshConnectionExpected", modelId, actualCount);
		}

		public static string rsReportServerStorageRefreshConnectionNotValidated(string modelId, string refreshConnectionId)
		{
			return Keys.GetString("rsReportServerStorageRefreshConnectionNotValidated", modelId, refreshConnectionId);
		}

		public static string rsIdentityClaimsMissingOrInvalid(string identityClaims)
		{
			return Keys.GetString("rsIdentityClaimsMissingOrInvalid", identityClaims);
		}

		public static string rsSystemResourcePackageReferencedItemMissing(string itemName)
		{
			return Keys.GetString("rsSystemResourcePackageReferencedItemMissing", itemName);
		}

		public static string rsSystemResourcePackageRequiredItemMissing(string itemName)
		{
			return Keys.GetString("rsSystemResourcePackageRequiredItemMissing", itemName);
		}

		public static string rsSystemResourcePackageItemContentTypeMismatch(string itemName, string contentType)
		{
			return Keys.GetString("rsSystemResourcePackageItemContentTypeMismatch", itemName, contentType);
		}

		public static string rsSystemResourcePackageItemExtensionMismatch(string itemName, string extension)
		{
			return Keys.GetString("rsSystemResourcePackageItemExtensionMismatch", itemName, extension);
		}

		public static string rsSystemResourcePackageWrongType(string typeName)
		{
			return Keys.GetString("rsSystemResourcePackageWrongType", typeName);
		}
	}
}
