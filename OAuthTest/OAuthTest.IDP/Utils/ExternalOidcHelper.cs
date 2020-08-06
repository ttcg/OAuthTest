using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace OAuthTest.IDP.Utils
{
    public static class ExternalOidcHelper
    {
        public static List<SecurityKey> GetSigningKeysFromDemoIdentityServer()
        {             
            var keySet = GetJsonWebKeySet("https://demo.identityserver.io/.well-known/openid-configuration/jwks");

            return GetSecurityKeys(keySet);
        }

        static JsonWebKeySet GetJsonWebKeySet(string keysUri)
        {
            var client = new HttpClient();
            var content = client.GetStringAsync(keysUri).Result;
            var jsonWebKeySet = JsonConvert.DeserializeObject<JsonWebKeySet>(content);

            return jsonWebKeySet;
        }

        static List<SecurityKey> GetSecurityKeys(JsonWebKeySet jsonWebKeySet)
        {
            var keys = new List<SecurityKey>();

            foreach (var key in jsonWebKeySet.Keys)
            {
                if (key.Kty == "RSA")
                {
                    if (key.X5C != null && key.X5C.Length > 0)
                    {
                        string certificateString = key.X5C[0];
                        var certificate = new X509Certificate2(Convert.FromBase64String(certificateString));

                        var x509SecurityKey = new X509SecurityKey(certificate)
                        {
                            KeyId = key.Kid
                        };

                        keys.Add(x509SecurityKey);
                    }
                    else if (!string.IsNullOrWhiteSpace(key.E) && !string.IsNullOrWhiteSpace(key.N))
                    {
                        byte[] exponent = Base64UrlDecode(key.E);
                        byte[] modulus = Base64UrlDecode(key.N);

                        var rsaParameters = new RSAParameters
                        {
                            Exponent = exponent,
                            Modulus = modulus
                        };

                        var rsaSecurityKey = new RsaSecurityKey(rsaParameters)
                        {
                            KeyId = key.Kid
                        };

                        keys.Add(rsaSecurityKey);
                    }
                    else
                    {
                        throw new Exception("JWK data is missing in token validation");
                    }
                }
                else
                {
                    throw new NotImplementedException("Only RSA key type is implemented for token validation");
                }
            }

            return keys;
        }

        static string Base64UrlEncode(byte[] arg)
        {
            string s = Convert.ToBase64String(arg); // Regular base64 encoder
            s = s.Split('=')[0]; // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding
            return s;
        }

        static byte[] Base64UrlDecode(string arg)
        {
            string s = arg;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
                default:
                    throw new System.Exception(
             "Illegal base64url string!");
            }
            return Convert.FromBase64String(s); // Standard base64 decoder
        }
    }
}
