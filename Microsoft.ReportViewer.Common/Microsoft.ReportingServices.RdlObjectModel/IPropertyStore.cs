namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal interface IPropertyStore
	{
		ReportObject Owner
		{
			get;
		}

		IContainedObject Parent
		{
			get;
			set;
		}

		void RemoveProperty(int propertyIndex);

		object GetObject(int propertyIndex);

		T GetObject<T>(int propertyIndex);

		void SetObject(int propertyIndex, object value);

		void RemoveObject(int propertyIndex);

		bool ContainsObject(int propertyIndex);

		int GetInteger(int propertyIndex);

		void SetInteger(int propertyIndex, int value);

		void RemoveInteger(int propertyIndex);

		bool ContainsInteger(int propertyIndex);

		bool GetBoolean(int propertyIndex);

		void SetBoolean(int propertyIndex, bool value);

		void RemoveBoolean(int propertyIndex);

		bool ContainsBoolean(int propertyIndex);

		ReportSize GetSize(int propertyIndex);

		void SetSize(int propertyIndex, ReportSize value);

		void RemoveSize(int propertyIndex);

		bool ContainsSize(int propertyIndex);

		void IterateObjectEntries(VisitPropertyObject visitObject);
	}
}
