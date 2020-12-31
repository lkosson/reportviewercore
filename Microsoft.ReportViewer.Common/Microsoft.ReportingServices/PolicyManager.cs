using System;
using System.Runtime.Loader;
using System.Security.Policy;

namespace Microsoft.ReportingServices
{
	internal abstract class PolicyManager
	{
		public event EventHandler PolicyChanged;

		public abstract AssemblyLoadContext CreateAssemblyLoadContextWithPolicy(string appDomainName, Evidence evidence, AppDomainSetup setupInfo, SandboxCasPolicySettings casSettings);

		protected void OnPolicyChanged()
		{
			if (this.PolicyChanged != null)
			{
				this.PolicyChanged(this, EventArgs.Empty);
			}
		}
	}
}
