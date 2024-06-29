# BlockadeLabs-SDK-DotNet

[![Discord](https://img.shields.io/discord/855294214065487932.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.gg/xQgMW9ufN4)
[![NuGet version (BlockadeLabs-SDK-DotNet)](https://img.shields.io/nuget/v/BlockadeLabs-SDK-DotNet.svg?label=BlockadeLabs-SDK-DotNet&logo=nuget)](https://www.nuget.org/packages/BlockadeLabs-SDK-DotNet/)
[![NuGet version (BlockadeLabs-SDK-DotNet-Proxy)](https://img.shields.io/nuget/v/BlockadeLabs-SDK-DotNet-Proxy.svg?label=BlockadeLabs-SDK-DotNet-Proxy&logo=nuget)](https://www.nuget.org/packages/BlockadeLabs-SDK-DotNet-Proxy/)
[![Nuget Publish](https://github.com/RageAgainstThePixel/BlockadeLabs-SDK-DotNet/actions/workflows/Publish-Nuget.yml/badge.svg)](https://github.com/RageAgainstThePixel/BlockadeLabs-SDK-DotNet/actions/workflows/Publish-Nuget.yml)

A simple C# .NET client library for [BlockadeLabs] to use through their RESTful API.
An BlockadeLabs API account subscription is required.

## Requirements

- This library targets .NET 8.0 and above.
- It should work across console apps, winforms, wpf, asp.net, etc.
- It should also work across Windows, Linux, and Mac.

## Getting started

### Install from NuGet

Install package [`BlockadeLabs-SDK-DotNet` from Nuget](https://www.nuget.org/packages/BlockadeLabs-SDK-DotNet/).  Here's how via command line:

```powershell
Install-Package BlockadeLabs-SDK-DotNet
```

## Documentation

### Table of Contents

- [Authentication](#authentication)
- [BlockadeLabs API Proxy](#blockadelabs-api-proxy)
- [Skyboxes](#skyboxes)
  - [Get Skybox Styles](#get-skybox-styles)
  - [Get Skybox Style Families](#get-skybox-style-families)
  - [Get Skybox Export Options](#get-skybox-export-options)
  - [Generate Skybox](#generate-skybox)
  - [Get Skybox by Id](#get-skybox)
  - [Request Skybox Export](#request-skybox-export)
  - [Delete Skybox by Id](#delete-skybox)
  - [Get Skybox History](#get-skybox-history)
  - [Cancel Skybox Generation](#cancel-skybox-generation)
  - [Cancel All Pending Skybox Generations](#cancel-all-pending-skybox-generations)

### Authentication

There are 3 ways to provide your API keys, in order of precedence:

> [!WARNING]
> We recommended using the environment variables to load the API key instead of having it hard coded in your source. It is not recommended use this method in production, but only for accepting user credentials, local testing and quick start scenarios.

1. [Pass keys directly with constructor](#pass-keys-directly-with-constructor) :warning:
2. [Load key from configuration file](#load-key-from-configuration-file)
3. [Use System Environment Variables](#use-system-environment-variables)

#### Pass keys directly with constructor

> [!WARNING]
> We recommended using the environment variables to load the API key instead of having it hard coded in your source. It is not recommended use this method in production, but only for accepting user credentials, local testing and quick start scenarios.

```csharp
using var api = new BlockadeLabsClient("api-key");
```

Or create a `BlockadeLabsAuthentication` object manually

```csharp
using var api = new BlockadeLabsClient(new BlockadeLabsAuthentication("api-key"));
```

#### Load key from configuration file

Attempts to load api keys from a configuration file, by default `.blockadelabs` in the current directory, optionally traversing up the directory tree or in the user's home directory.

To create a configuration file, create a new text file named `.blockadelabs` and containing the line:

##### Json format

```json
{
    "apiKey": "your-api-key"
}
```

You can also load the configuration file directly with known path by calling static methods in `BlockadeLabsAuthentication`:

- Loads the default `.blockadelabs` config in the specified directory:

```csharp
using var api = new BlockadeLabsClient(BlockadeLabsAuthentication.LoadFromDirectory("path/to/your/directory"));
```

- Loads the configuration file from a specific path. File does not need to be named `.blockadelabs` as long as it conforms to the json format:

```csharp
using var api = new BlockadeLabsClient(BlockadeLabsAuthentication.LoadFromPath("path/to/your/file.json"));
```

#### Use System Environment Variables

Use your system's environment variables specify an api key and organization to use.

- Use `BLOCKADELABS_API_KEY` for your api key.

```csharp
using var api = new BlockadeLabsClient(BlockadeLabsAuthentication.LoadFromEnvironment());
```

### [BlockadeLabs API Proxy](BlockadeLabs-SDK-DotNet-Proxy/Readme.md)

[![NuGet version (BlockadeLabs-SDK-DotNet-Proxy)](https://img.shields.io/nuget/v/BlockadeLabs-SDK-DotNet-Proxy.svg?label=BlockadeLabs-SDK-DotNet-Proxy&logo=nuget)](https://www.nuget.org/packages/BlockadeLabs-SDK-DotNet-Proxy/)

Using either the [BlockadeLabs-SDK-DotNet](https://github.com/RageAgainstThePixel/BlockadeLabs-SDK-DotNet) or [com.rest.blockadelabs](https://github.com/RageAgainstThePixel/com.rest.blockadelabs) packages directly in your front-end app may expose your API keys and other sensitive information. To mitigate this risk, it is recommended to set up an intermediate API that makes requests to BlockadeLabs on behalf of your front-end app. This library can be utilized for both front-end and intermediary host configurations, ensuring secure communication with the BlockadeLabs API.

#### Front End Example

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
    - Manually editing .csproj: `<PackageReference Include="BlockadeLabs-SDK-DotNet-Proxy" />`
3. Create a new class that inherits from `AbstractAuthenticationFilter` and override the `ValidateAuthentication` method. This will implement the `IAuthenticationFilter` that you will use to check user session token against your internal server.
4. In `Program.cs`, create a new proxy web application by calling `BlockadeLabsProxy.CreateWebApplication` method, passing your custom `AuthenticationFilter` as a type argument.
5. Create `BlockadeLabsAuthentication` as you would normally and load your API key from environment variable.

```csharp
public partial class Program
{
    private class AuthenticationFilter : AbstractAuthenticationFilter
    {
        public override void ValidateAuthentication(IHeaderDictionary request)
        {
            // You will need to implement your own class to properly test
            // custom issued tokens you've setup for your end users.
            if (!request["x-api-key"].ToString().Contains(TestUserToken))
            {
                throw new AuthenticationException("User is not authorized");
            }
        }

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

### Skyboxes

#### [Get Skybox Styles](https://api-documentation.blockadelabs.com/api/skybox.html#get-skybox-styles)

Returns the list of predefined styles that can influence the overall aesthetic of your skybox generation.

```csharp
using var api = new BlockadeLabsClient();
var skyboxStyles = await api.SkyboxEndpoint.GetSkyboxStylesAsync(SkyboxModel.Model3);

foreach (var skyboxStyle in skyboxStyles)
{
    Console.WriteLine($"{skyboxStyle.Name}");
}
```

#### [Get Skybox Style Families](https://api-documentation.blockadelabs.com/api/skybox.html#get-skybox-style-families)

Returns the list of predefined styles that can influence the overall aesthetic of your skybox generation, sorted by style family. This route can be used in order to build a menu of styles sorted by family.

```csharp
using var api = new BlockadeLabsClient();
var skyboxFamilyStyles = await BlockadeLabsClient.SkyboxEndpoint.GetSkyboxStyleFamiliesAsync(SkyboxModel.Model3);

foreach (var skyboxStyle in skyboxFamilyStyles)
{
    Console.WriteLine($"{skyboxStyle.Name}");
}
```

#### [Get Skybox Export Options](https://api-documentation.blockadelabs.com/api/skybox-exports.html#get-export-types)

Returns the list of all available export types.

```csharp
using var api = new BlockadeLabsClient();
var exportOptions = await api.SkyboxEndpoint.GetAllSkyboxExportOptionsAsync();

foreach (var exportOption in exportOptions)
{
    Console.WriteLine($"{exportOption.Id}: {exportOption.Name} | {exportOption.Key}");
}

var request = new SkyboxRequest("mars", enhancePrompt: true);
// Generates ALL export options for the skybox
var skyboxInfo = await api.SkyboxEndpoint.GenerateSkyboxAsync(request, exportOptions);
Console.WriteLine($"Successfully created skybox: {skyboxInfo.Id}");
```

#### [Generate Skybox](https://api-documentation.blockadelabs.com/api/skybox.html#generate-skybox)

Generate a skybox.

```csharp
using var api = new BlockadeLabsClient();
var skyboxStyles = await BlockadeLabsClient.SkyboxEndpoint.GetSkyboxStylesAsync(SkyboxModel.Model3);
var request = new SkyboxRequest(skyboxStyles.First(), "mars", enhancePrompt: true);

// You can also get progress callbacks when the generation progress has changed/updated
var progress = new Progress<SkyboxInfo>(async progress =>
{
    Console.WriteLine(progress);
});

var skyboxInfo = await api.SkyboxEndpoint.GenerateSkyboxAsync(request, progressCallback: progress);
Console.WriteLine($"Successfully created skybox: {skyboxInfo.Id}");
```

#### [Get Skybox](https://api-documentation.blockadelabs.com/api/skybox.html#get-skybox-by-id)

Returns the skybox metadata for the given skybox id.

```csharp
var skyboxId = 42;
using var api = new BlockadeLabsClient();
var skyboxInfo = await api.SkyboxEndpoint.GetSkyboxInfoAsync(skyboxId);
Console.WriteLine($"Skybox: {result.Id}");
```

#### [Request Skybox Export](https://api-documentation.blockadelabs.com/api/skybox-exports.html#request-export)

Exports the skybox with the requested export type.

> [!NOTE]
> You can also specify the export types when initially generating a skybox.

```csharp
var skyboxId = 42;
using var api = new BlockadeLabsClient();
var skyboxInfo = await api.SkyboxEndpoint.GetSkyboxInfoAsync(skyboxId);
skyboxInfo = await api.SkyboxEndpoint.ExportSkyboxAsync(skyboxInfo, DefaultExportOptions.DepthMap_PNG);

foreach (var exportInfo in skyboxInfo.Exports)
{
    Console.WriteLine($"{exportInfo.Key} -> {exportInfo.Value}");
}
```

#### [Delete Skybox](https://api-documentation.blockadelabs.com/api/skybox.html#delete)

Deletes a skybox by id.

```csharp
var skyboxId = 42;
var result = await api.SkyboxEndpoint.DeleteSkyboxAsync(skybox);
// result == true
```

#### [Get Skybox History](https://api-documentation.blockadelabs.com/api/skybox.html#get-history)

Gets the previously generated skyboxes.

```csharp
var history = await api.SkyboxEndpoint.GetSkyboxHistoryAsync();
Console.WriteLine($"Found {history.TotalCount} skyboxes");

foreach (var skybox in history.Skyboxes)
{
    Console.WriteLine($"{skybox.Id} {skybox.Title} status: {skybox.Status}");
}
```

#### [Cancel Skybox Generation](https://api-documentation.blockadelabs.com/api/skybox.html#cancel-generation)

Cancels a pending skybox generation request by id.

```csharp
var skyboxId = 42;
var result = await CancelSkyboxGenerationAsync(skyboxId);
// result == true
```

> [!NOTE]
> This is automatically done when cancelling a skybox generation using cancellation token.

#### [Cancel All Pending Skybox Generations](https://api-documentation.blockadelabs.com/api/skybox.html#cancel-all-pending-generations)

Cancels ALL pending skybox generation requests.

```csharp
var result = await api.SkyboxEndpoint.CancelAllPendingSkyboxGenerationsAsync();
Console.WriteLine(result ? "All pending generations successfully cancelled" : "No pending generations");
```
