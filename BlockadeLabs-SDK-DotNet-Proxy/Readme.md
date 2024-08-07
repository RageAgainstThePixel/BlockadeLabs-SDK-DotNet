# BlockadeLabs-SDK-DotNet-Proxy

[![NuGet version (BlockadeLabs-SDK-DotNet-Proxy)](https://img.shields.io/nuget/v/BlockadeLabs-SDK-DotNet-Proxy.svg?label=BlockadeLabs-SDK-DotNet-Proxy&logo=nuget)](https://www.nuget.org/packages/BlockadeLabs-SDK-DotNet-Proxy/)

A simple Proxy API gateway for [BlockadeLabs-SDK-DotNet](https://github.com/RageAgainstThePixel/BlockadeLabs-SDK-DotNet) to make authenticated requests from a front end application without exposing your API keys.

## Getting started

### Install from NuGet

Install package [`BlockadeLabs-SDK-DotNet-Proxy` from Nuget](https://www.nuget.org/packages/BlockadeLabs-SDK-DotNet-Proxy/).  Here's how via command line:

```powershell
Install-Package BlockadeLabs-SDK-DotNet-Proxy
```

## Documentation

Using either the [BlockadeLabs-SDK-DotNet](https://github.com/RageAgainstThePixel/BlockadeLabs-SDK-DotNet) or [com.rest.blockadelabs](https://github.com/RageAgainstThePixel/com.rest.blockadelabs) packages directly in your front-end app may expose your API keys and other sensitive information. To mitigate this risk, it is recommended to set up an intermediate API that makes requests to BlockadeLabs on behalf of your front-end app. This library can be utilized for both front-end and intermediary host configurations, ensuring secure communication with the BlockadeLabs API.

### Front End Example

In the front end example, you will need to securely authenticate your users using your preferred OAuth provider. Once the user is authenticated, exchange your custom auth token with your API key on the backend.

Follow these steps:

1. Setup a new project using either the [BlockadeLabs-SDK-DotNet](https://github.com/RageAgainstThePixel/BlockadeLabs-SDK-DotNet) or [com.rest.blockadelabs](https://github.com/RageAgainstThePixel/com.rest.blockadelabs) packages.
2. Authenticate users with your OAuth provider.
3. After successful authentication, create a new `BlockadeLabsAuthentication` object and pass in the custom token as your apiKey.
4. Create a new `BlockadeLabsSettings` object and specify the domain where your intermediate API is located.
5. Pass your new `auth` and `settings` objects to the `BlockadeLabsClient` constructor when you create the client instance.

Here's an example of how to set up the front end:

```csharp
var authToken = await LoginAsync();
var auth = new BlockadeLabsAuthentication(authToken);
var settings = new BlockadeLabsSettings(domain: "api.your-custom-domain.com");
using var api = new BlockadeLabsClient(auth, settings);
```

This setup allows your front end application to securely communicate with your backend that will be using the BlockadeLabs-SDK-DotNet-Proxy, which then forwards requests to the BlockadeLabs API. This ensures that your BlockadeLabs API keys and other sensitive information remain secure throughout the process.

#### Back End Example

In this example, we demonstrate how to set up and use `BlockadeLabsProxy` in a new ASP.NET Core web app. The proxy server will handle authentication and forward requests to the BlockadeLabs API, ensuring that your API keys and other sensitive information remain secure.

1. Create a new [ASP.NET Core minimal web API](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-6.0) project.
2. Add the BlockadeLabs-SDK-DotNet nuget package to your project.
    - Powershell install: `Install-Package BlockadeLabs-SDK-DotNet-Proxy`
    - dotnet: `dotnet add package BlockadeLabs-SDK-DotNet-Proxy`
    - Manually editing .csproj: `<PackageReference Include="BlockadeLabs-SDK-DotNet-Proxy" />`
3. Create a new class that inherits from `AbstractAuthenticationFilter` and override the `ValidateAuthenticationAsync` method. This will implement the `IAuthenticationFilter` that you will use to check user session token against your internal server.
4. In `Program.cs`, create a new proxy web application by calling `BlockadeLabsProxy.CreateWebApplication` method, passing your custom `AuthenticationFilter` as a type argument.
5. Create `BlockadeLabsAuthentication` as you would normally and load your API key from environment variable.

```csharp
using BlockadeLabsSDK;
using BlockadeLabsSDK.Proxy;
using System.Security.Authentication;

public partial class Program
{
    private class AuthenticationFilter : AbstractAuthenticationFilter
    {
        public override async Task ValidateAuthenticationAsync(IHeaderDictionary request)
        {
            await Task.CompletedTask; // remote resource call

            // You will need to implement your own class to properly test
            // custom issued tokens you've setup for your end users.
            if (!request["x-api-key"].ToString().Contains(TestUserToken))
            {
                throw new AuthenticationException("User is not authorized");
            }
        }
    }

    public static void Main(string[] args)
    {
        var auth = BlockadeLabsAuthentication.LoadFromEnvironment();
        using var blockadeLabsClient = new BlockadeLabsClient(auth);
        BlockadeLabsProxy.CreateWebApplication<AuthenticationFilter>(args, blockadeLabsClient).Run();
    }
}
```

Once you have set up your proxy server, your end users can now make authenticated requests to your proxy api instead of directly to the BlockadeLabs API. The proxy server will handle authentication and forward requests to the BlockadeLabs API, ensuring that your API keys and other sensitive information remain secure.
