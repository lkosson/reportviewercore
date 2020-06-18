using System;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	[Serializable]
	internal class ArgumentConstraintException : ArgumentException
	{
		private object m_component;

		private string m_property;

		private object m_value;

		private object m_constraint;

		public object Component => m_component;

		public string Property => m_property;

		public object Value => m_value;

		public object Constraint => m_constraint;

		public ArgumentConstraintException(object component, string property, object value, object constraint, string message)
			: base(message, property)
		{
			m_component = component;
			m_property = property;
			m_value = value;
			m_constraint = constraint;
		}

		protected ArgumentConstraintException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
