/**
 * @file JWTHelper.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using System.Collections.Generic;
using JWT;
using JWT.exceptions;

namespace HDK.SDK
{
    internal class JWTHelper
    {
        public static string Create(string payload, string jwtSecret)
        {
            var obj = new Dictionary<string, object>
            {
                { "token", payload }
            };

            return JsonWebToken.Encode(obj, HeddokoConfig.JWTSecret, JwtHashAlgorithm.HS256);
        }

        public static string Verify(string token)
        {
            try
            {
                var obj = JsonWebToken.DecodeToObject(token, HeddokoConfig.JWTSecret) as IDictionary<string, object>;

                return (string)obj["token"];
            }
            catch (SignatureVerificationException)
            {
                //TODO: log?
            }

            return null;
        }
    }
}
