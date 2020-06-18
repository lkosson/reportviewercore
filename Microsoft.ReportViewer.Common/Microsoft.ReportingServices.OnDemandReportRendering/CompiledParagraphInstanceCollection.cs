using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class CompiledParagraphInstanceCollection : ReportElementInstanceCollectionBase<CompiledParagraphInstance>, IList<ICompiledParagraphInstance>, ICollection<ICompiledParagraphInstance>, IEnumerable<ICompiledParagraphInstance>, IEnumerable
	{
		private CompiledRichTextInstance m_compiledRichTextInstance;

		private List<CompiledParagraphInstance> m_compiledParagraphInstances;

		public override CompiledParagraphInstance this[int i] => m_compiledParagraphInstances[i];

		public override int Count => m_compiledParagraphInstances.Count;

		ICompiledParagraphInstance IList<ICompiledParagraphInstance>.this[int index]
		{
			get
			{
				return m_compiledParagraphInstances[index];
			}
			set
			{
				m_compiledParagraphInstances[index] = (CompiledParagraphInstance)value;
			}
		}

		int ICollection<ICompiledParagraphInstance>.Count => m_compiledParagraphInstances.Count;

		bool ICollection<ICompiledParagraphInstance>.IsReadOnly => false;

		internal CompiledParagraphInstanceCollection(CompiledRichTextInstance compiledRichTextInstance)
		{
			m_compiledRichTextInstance = compiledRichTextInstance;
			m_compiledParagraphInstances = new List<CompiledParagraphInstance>();
		}

		int IList<ICompiledParagraphInstance>.IndexOf(ICompiledParagraphInstance item)
		{
			return m_compiledParagraphInstances.IndexOf((CompiledParagraphInstance)item);
		}

		void IList<ICompiledParagraphInstance>.Insert(int index, ICompiledParagraphInstance item)
		{
			m_compiledParagraphInstances.Insert(index, (CompiledParagraphInstance)item);
		}

		void IList<ICompiledParagraphInstance>.RemoveAt(int index)
		{
			m_compiledParagraphInstances.RemoveAt(index);
		}

		void ICollection<ICompiledParagraphInstance>.Add(ICompiledParagraphInstance item)
		{
			m_compiledParagraphInstances.Add((CompiledParagraphInstance)item);
		}

		void ICollection<ICompiledParagraphInstance>.Clear()
		{
			m_compiledParagraphInstances.Clear();
		}

		bool ICollection<ICompiledParagraphInstance>.Contains(ICompiledParagraphInstance item)
		{
			return m_compiledParagraphInstances.Contains((CompiledParagraphInstance)item);
		}

		void ICollection<ICompiledParagraphInstance>.CopyTo(ICompiledParagraphInstance[] array, int arrayIndex)
		{
			CompiledParagraphInstance[] array2 = new CompiledParagraphInstance[array.Length];
			m_compiledParagraphInstances.CopyTo(array2, arrayIndex);
			for (int i = 0; i < array2.Length; i++)
			{
				array[i] = array2[i];
			}
		}

		bool ICollection<ICompiledParagraphInstance>.Remove(ICompiledParagraphInstance item)
		{
			return m_compiledParagraphInstances.Remove((CompiledParagraphInstance)item);
		}

		IEnumerator<ICompiledParagraphInstance> IEnumerable<ICompiledParagraphInstance>.GetEnumerator()
		{
			foreach (CompiledParagraphInstance compiledParagraphInstance in m_compiledParagraphInstances)
			{
				yield return compiledParagraphInstance;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)m_compiledParagraphInstances).GetEnumerator();
		}
	}
}
