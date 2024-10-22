﻿// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BlockadeLabsSDK.Proxy
{
    /// <inheritdoc />
    public abstract class AbstractAuthenticationFilter : IAuthenticationFilter
    {
        /// <inheritdoc />
        public virtual void ValidateAuthentication(IHeaderDictionary request) { }

        /// <inheritdoc />
        public abstract Task ValidateAuthenticationAsync(IHeaderDictionary request);
    }
}
