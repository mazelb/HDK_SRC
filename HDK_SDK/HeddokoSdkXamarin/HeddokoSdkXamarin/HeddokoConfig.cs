/**
 * @file HeddokoConfig.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
namespace HeddokoSdkXamarin
{
    public class HeddokoConfig
    {
        public string ServerUrl { get; set; }

        public string APIUrl => $"{ServerUrl}api/v1";

        public string OauthUrl => $"{ServerUrl}oauth2/token";

        public static string JWTSecret { get; set; }

        public int ReconnectionAttempts { get; set; }

        /// <summary>
        /// Time in miliseconds between reconnections
        /// </summary>
        public int ReconnectionDelay { get; set; }

        public HeddokoConfig(string serverUrl, string jwtSecret, int reconnectionAttempts, int reconnectionDelay)
        {
            ServerUrl = serverUrl;
            JWTSecret = jwtSecret;
            ReconnectionAttempts = reconnectionAttempts;
            ReconnectionDelay = reconnectionDelay;
        }
    }
}
