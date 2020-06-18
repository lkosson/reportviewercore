using System.Collections;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal delegate T CreateDictionary<T>(int dictionaryLength) where T : IDictionary;
}
