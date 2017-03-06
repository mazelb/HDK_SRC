/**
 * @file User.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using System;
using HeddokoSdkXamarin.Models.Enum;
using Newtonsoft.Json;

namespace HeddokoSdkXamarin.Models
{
    public class User : BaseModel
    {
        public string Email { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Country { get; set; }

        public DateTime? BirthDay { get; set; }

        public UserRoleType RoleType { get; set; }

        public UserStatusType Status { get; set; }

        public DateTime createdAt { get; set; }

        public DateTime? updatedAt { get; set; }

        public string Name { get; set; }

        public string AvatarSrc { get; set; }

        public string Token { get; set; }

        public string LicenseInfoToken { get; set; }

        public Kit Kit { get; set; }

        public Team Team { get; set; }

        public LicenseInfo LicenseInfo
        {
            get
            {
                if (!string.IsNullOrEmpty(LicenseInfoToken))
                {
                    string json = JWTHelper.Verify(LicenseInfoToken);

                    if (string.IsNullOrEmpty(json))
                    {
                        throw new Exception("JWT Verification token is invalid.");
                    }

                    return JsonConvert.DeserializeObject<LicenseInfo>(json);
                }

                return null;
            }
        }
    }
}
