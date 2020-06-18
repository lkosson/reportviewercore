using Microsoft.ReportingServices.DataProcessing;
using System.Collections;
using System.Data;

namespace Microsoft.ReportingServices.DataExtensions
{
	internal class ParameterCollectionWrapper : BaseDataWrapper, Microsoft.ReportingServices.DataProcessing.IDataParameterCollection, IEnumerable
	{
		public sealed class Enumerator : IEnumerator
		{
			private IEnumerator m_underlyingEnumerator;

			object IEnumerator.Current => Current;

			public ParameterWrapper Current => (ParameterWrapper)m_underlyingEnumerator.Current;

			internal Enumerator(IEnumerator underlyingEnumerator)
			{
				m_underlyingEnumerator = underlyingEnumerator;
			}

			public bool MoveNext()
			{
				return m_underlyingEnumerator.MoveNext();
			}

			public void Reset()
			{
				m_underlyingEnumerator.Reset();
			}
		}

		private ArrayList m_parameters = new ArrayList();

		protected System.Data.IDataParameterCollection UnderlyingCollection => (System.Data.IDataParameterCollection)base.UnderlyingObject;

		protected ArrayList Parameters => m_parameters;

		protected internal ParameterCollectionWrapper(System.Data.IDataParameterCollection paramCollection)
			: base(paramCollection)
		{
		}

		public virtual int Add(Microsoft.ReportingServices.DataProcessing.IDataParameter parameter)
		{
			ParameterWrapper parameterWrapper = (ParameterWrapper)parameter;
			int result = UnderlyingCollection.Add(parameterWrapper.UnderlyingParameter);
			Parameters.Add(parameterWrapper);
			return result;
		}

		public virtual Enumerator GetEnumerator()
		{
			return new Enumerator(Parameters.GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
