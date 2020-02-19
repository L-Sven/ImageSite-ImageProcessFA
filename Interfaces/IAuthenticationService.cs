using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace lu.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> ValidateToken(string token);
    }
}