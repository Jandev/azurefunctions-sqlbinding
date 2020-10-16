
using System.Threading;
using System.Threading.Tasks;

namespace AzureFunctions.SqlBinding
{
	public interface ISqlBindingTokenProvider
	{
		Task<string> GetToken(CancellationToken cancellationToken);
	}
}
