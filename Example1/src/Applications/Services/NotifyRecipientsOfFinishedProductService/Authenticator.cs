using System;
using RestSharpClient.Contracts;
using Thinktecture.IdentityModel.Client;

namespace NotifyRecipientsOfFinishedProductService
{
    public class Authenticator : IAuthenticator
    {
        private readonly Func<TokenResponse> _tokenFactory;

        public Authenticator(Func<TokenResponse> tokenFactory)
        {
            _tokenFactory = tokenFactory;
        }

        public void Authenticate(IRestRequest request) 
        {
            var token = _tokenFactory.Invoke();
            request.AddHeader("Authorization", $"Bearer {token.AccessToken}");
        }
    }
}
