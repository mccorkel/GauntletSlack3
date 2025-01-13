using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

public class CustomAuthorizationMessageHandler : DelegatingHandler
{
    private readonly IAccessTokenProvider _tokenProvider;
    private readonly NavigationManager _navigationManager;

    public CustomAuthorizationMessageHandler(IAccessTokenProvider tokenProvider, NavigationManager navigationManager)
    {
        _tokenProvider = tokenProvider;
        _navigationManager = navigationManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var tokenResult = await _tokenProvider.RequestAccessToken();
        
        if (tokenResult.TryGetToken(out var token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Value);
        }
        else
        {
            _navigationManager.NavigateTo(tokenResult.RedirectUrl);
        }

        return await base.SendAsync(request, cancellationToken);
    }
} 