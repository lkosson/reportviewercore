using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class ReportContext
	{
		private class SubreportContext : IDisposable
		{
			private readonly string m_name;

			private Action m_callback;

			internal string Name => m_name;

			internal SubreportContext(string name, Action callback)
			{
				m_name = name;
				m_callback = callback;
			}

			void IDisposable.Dispose()
			{
				m_callback();
				m_callback = null;
			}
		}

		private Stack<SubreportContext> m_subreports;

		internal ReportContext()
		{
			m_subreports = new Stack<SubreportContext>();
		}

		internal IDisposable EnterSubreport(RPLElementPropsDef subreportDef)
		{
			RPLItemPropsDef rPLItemPropsDef = subreportDef as RPLItemPropsDef;
			SubreportContext subreportContext = new SubreportContext(rPLItemPropsDef.Name, PopSubreport);
			PushSubreport(subreportContext);
			return subreportContext;
		}

		internal IEnumerable<string> GetPath()
		{
			return m_subreports.Select((SubreportContext s) => s.Name);
		}

		private void PushSubreport(SubreportContext subreport)
		{
			m_subreports.Push(subreport);
		}

		private void PopSubreport()
		{
			m_subreports.Pop();
		}
	}
}
