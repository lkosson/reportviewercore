namespace Microsoft.ReportingServices.DataProcessing
{
	public interface IDbCommandRewriter
	{
		string RewrittenCommandText
		{
			get;
		}
	}
}
