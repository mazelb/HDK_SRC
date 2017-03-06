/**
 * @file Organization.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using HeddokoSdkXamarin.Models.Enum;

namespace HeddokoSdkXamarin.Models
{
    public class Organization : BaseModel
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string Notes { get; set; }

        public OrganizationStatusType Status { get; set; }

        public string IDView { get; set; }

        public int UserID { get; set; }
    }
}
