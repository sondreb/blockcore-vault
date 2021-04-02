using System.Threading.Tasks;

namespace Blockcore.Vault.Authentication
{
    public interface IGetApiKeyQuery
    {
        Task<ApiKey> Execute(string providedApiKey);
    }
}
