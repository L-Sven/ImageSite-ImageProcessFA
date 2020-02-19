using System;
using System.Net.Http;
using System.Threading.Tasks;
using lu.Interfaces;
using lu.Models;

namespace lu.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private string _validateUrl;

        public AuthenticationService()
        {
            _validateUrl = Environment.GetEnvironmentVariable(Constants.AUTH_AUTHORITY_URI + Constants.AUTH_FUNC_VALIDATE_JWT);
        }

        public async Task<bool> ValidateToken(string token)
        {
            var result = await new HttpClient().PostAsJsonAsync<Jwt>(_validateUrl, new Jwt() { Token = token });

            return result.IsSuccessStatusCode;
        }
    }
}