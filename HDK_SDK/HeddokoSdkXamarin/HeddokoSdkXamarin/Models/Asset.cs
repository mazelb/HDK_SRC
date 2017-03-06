/**
 * @file Asset.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using HeddokoSdkXamarin.Models.Enum;

namespace HeddokoSdkXamarin.Models
{
    public class Asset : BaseModel
    {
        public AssetType Type { get; set; }

        public string Url { get; set; }

        public int? KitID { get; set; }

        public int? UserID { get; set; }

        public User User { get; set; }

        public string Name { get; set; }
    }
}
