using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CompiledTextRunInstanceCollection : ReportElementInstanceCollectionBase<CompiledTextRunInstance>, IList<ICompiledTextRunInstance>, ICollection<ICompiledTextRunInstance>, IEnumerable<ICompiledTextRunInstance>, IEnumerable
	{
		private CompiledRichTextInstance m_compiledRichTextInstance;

		private List<CompiledTextRunInstance> m_compiledTextRunInstances;

		public override CompiledTextRunInstance this[int i] => m_compiledTextRunInstances[i];

		public override int Count => m_compiledTextRunInstances.Count;

		ICompiledTextRunInstance IList<ICompiledTextRunInstance>.this[int index]
		{
			get
			{
				return m_compiledTextRunInstances[index];
			}
			set
			{
				m_compiledTextRunInstances[index] = (CompiledTextRunInstance)value;
			}
		}

		int ICollection<ICompiledTextRunInstance>.Count => m_compiledTextRunInstances.Count;

		bool ICollection<ICompiledTextRunInstance>.IsReadOnly => false;

		internal CompiledTextRunInstanceCollection(CompiledRichTextInstance compiledRichTextInstance)
		{
			m_compiledRichTextInstance = compiledRichTextInstance;
			m_compiledTextRunInstances = new List<CompiledTextRunInstance>();
		}

		int IList<ICompiledTextRunInstance>.IndexOf(ICompiledTextRunInstance item)
		{
			return m_compiledTextRunInstances.IndexOf((CompiledTextRunInstance)item);
		}

		void IList<ICompiledTextRunInstance>.Insert(int index, ICompiledTextRunInstance item)
		{
			m_compiledTextRunInstances.Insert(index, (CompiledTextRunInstance)item);
		}

		void IList<ICompiledTextRunInstance>.RemoveAt(int index)
		{
			m_compiledTextRunInstances.RemoveAt(index);
		}

		void ICollection<ICompiledTextRunInstance>.Add(ICompiledTextRunInstance item)
		{
			m_compiledTextRunInstances.Add((CompiledTextRunInstance)item);
		}

		void ICollection<ICompiledTextRunInstance>.Clear()
		{
			m_compiledTextRunInstances.Clear();
		}

		bool ICollection<ICompiledTextRunInstance>.Contains(ICompiledTextRunInstance item)
		{
			return m_compiledTextRunInstances.Contains((CompiledTextRunInstance)item);
		}

		void ICollection<ICompiledTextRunInstance>.CopyTo(ICompiledTextRunInstance[] array, int arrayIndex)
		{
			CompiledTextRunInstance[] array2 = new CompiledTextRunInstance[array.Length];
			m_compiledTextRunInstances.CopyTo(array2, arrayIndex);
			for (int i = 0; i < array2.Length; i++)
			{
				array[i] = array2[i];
			}
		}

		bool ICollection<ICompiledTextRunInstance>.Remove(ICompiledTextRunInstance item)
		{
			return m_compiledTextRunInstances.Remove((CompiledTextRunInstance)item);
		}

		IEnumerator<ICompiledTextRunInstance> IEnumerable<ICompiledTextRunInstance>.GetEnumerator()
		{
			foreach (CompiledTextRunInstance compiledTextRunInstance in m_compiledTextRunInstances)
			{
				yield return compiledTextRunInstance;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)m_compiledTextRunInstances).GetEnumerator();
		}
	}
}
