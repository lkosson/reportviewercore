using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Microsoft.ReportingServices.Diagnostics.Utilities
{
    /// <summary>
    /// Probes a server's WWW-Authenticate header to determine supported authentication schemes
    /// and returns the most appropriate HttpClientCredentialType for client configuration.
    /// </summary>
    public static class SSRSHttpAuthenticationProbe
    {
        //
        // Summary:
        //     Specifies protocols for authentication.
        [Flags]
        public enum SSRSAuthenticationSchemes
        {
            //
            // Summary:
            //     No authentication is allowed. A client requesting an System.Net.HttpListener
            //     object with this flag set will always receive a 403 Forbidden status. Use this
            //     flag when a resource should never be served to a client.
            None = 0,
            //
            // Summary:
            //     Negotiates with the client to determine the authentication scheme. If both client
            //     and server support Kerberos, it is used; otherwise, NTLM is used.
            Negotiate = 1,
            //
            // Summary:
            //     Specifies NTLM authentication.
            Ntlm = 2,
            //
            // Summary:
            //     Specifies anonymous authentication.
            Anonymous = 32768
        }

        private static readonly ConcurrentDictionary<string, HttpClientCredentialType> _credentialTypeCache =
            new ConcurrentDictionary<string, HttpClientCredentialType>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Detects the authentication schemes supported by the server and returns the most suitable
        /// HttpClientCredentialType to use on the client side. Results are cached per host authority.
        /// </summary>
        /// <param name="uri">The URI of the server endpoint to probe.</param>
        /// <returns>The most suitable HttpClientCredentialType based on server support.</returns>
        /// <remarks>
        /// Priority order (highest to lowest):
        /// 1. NTLM (maps to HttpClientCredentialType.Ntlm) - default for backward compatibility and supported by all SSRS versions
        /// 2. Kerberos (maps to HttpClientCredentialType.Windows) - preferred for Kerberos
        /// 3. None (maps to HttpClientCredentialType.None) - anonymous access
        /// </remarks>
        public static HttpClientCredentialType DetectSupportedCredentialType(Uri uri)
        {
            return DetectSupportedCredentialTypeAsync(uri).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously detects the authentication schemes supported by the server.
        /// Results are cached per host authority.
        /// </summary>
        public static async Task<HttpClientCredentialType> DetectSupportedCredentialTypeAsync(Uri uri)
        {
            var cacheKey = GetCacheKey(uri);

            if (_credentialTypeCache.TryGetValue(cacheKey, out var cachedType))
            {
                return cachedType;
            }

            var schemes = await GetSupportedAuthenticationSchemesAsync(uri);
            var credentialType = MapToHttpClientCredentialType(schemes);

            _credentialTypeCache.TryAdd(cacheKey, credentialType);

            return credentialType;
        }

        /// <summary>
        /// Gets the authentication schemes advertised by the server via the WWW-Authenticate header.
        /// </summary>
        public static async Task<SSRSAuthenticationSchemes> GetSupportedAuthenticationSchemesAsync(Uri uri)
        {
            using (var handler = new HttpClientHandler { AllowAutoRedirect = false })
            using (var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) })
            {
                try
                {
                    // Make an unauthenticated request to trigger a 401 response with WWW-Authenticate header
                    var response = await client.GetAsync(uri);

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return ParseWwwAuthenticateHeader(response);
                    }

                    // For other status codes, default to Negotiate as a safe fallback
                    return SSRSAuthenticationSchemes.Ntlm;
                }
                catch
                {
                    // On error, default to Negotiate to maintain backward compatibility
                    return SSRSAuthenticationSchemes.Ntlm;
                }
            }
        }

        private static string GetCacheKey(Uri uri)
        {
            // Cache by scheme + host + port to handle different auth configs per server
            return $"{uri.Scheme}://{uri.Authority}";
        }

        /// <summary>
        /// Clears the cached credential type for a specific URI.
        /// </summary>
        public static void InvalidateCache(Uri uri)
        {
            var cacheKey = GetCacheKey(uri);
            _credentialTypeCache.TryRemove(cacheKey, out _);
        }

        /// <summary>
        /// Clears all cached credential types.
        /// </summary>
        public static void ClearCache()
        {
            _credentialTypeCache.Clear();
        }

        private static SSRSAuthenticationSchemes ParseWwwAuthenticateHeader(HttpResponseMessage response)
        {
            var schemes = SSRSAuthenticationSchemes.None;

            if (!response.Headers.TryGetValues("WWW-Authenticate", out var authHeaders))
            {
                return SSRSAuthenticationSchemes.Ntlm; // Default fallback
            }

            foreach (var header in authHeaders)
            {
                // WWW-Authenticate can have multiple schemes separated by commas or in separate headers
                var schemeParts = header.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var schemePart in schemeParts)
                {
                    var scheme = schemePart.Trim().Split(' ')[0].ToUpperInvariant();

                    switch (scheme)
                    {
                        case "NEGOTIATE":
                            schemes |= SSRSAuthenticationSchemes.Negotiate;
                            break;
                        case "NTLM":
                            schemes |= SSRSAuthenticationSchemes.Ntlm;
                            break;
                    }
                }
            }

            return schemes == SSRSAuthenticationSchemes.None ? SSRSAuthenticationSchemes.Ntlm : schemes;
        }

        /// <summary>
        /// Maps the detected authentication schemes to the most suitable HttpClientCredentialType.
        /// </summary>
        private static HttpClientCredentialType MapToHttpClientCredentialType(SSRSAuthenticationSchemes schemes)
        {
            // Priority: Negotiate > NTLM > Digest > Basic > None
            // This ensures we use the most secure/capable option available
            if (schemes.HasFlag(SSRSAuthenticationSchemes.Ntlm))
            {
                return HttpClientCredentialType.Ntlm;
            }

            if (schemes.HasFlag(SSRSAuthenticationSchemes.Negotiate))
            {
                return HttpClientCredentialType.Windows;
            }

            // Default fallback
            return HttpClientCredentialType.Ntlm;
        }
    }
}