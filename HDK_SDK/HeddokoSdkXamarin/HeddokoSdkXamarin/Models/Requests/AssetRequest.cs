/**
 * @file AssetRequest.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using System;
using HeddokoSdkXamarin.Models.Enum;

namespace HeddokoSdkXamarin.Models.Requests
{
    public class AssetRequest
    {
        [Obsolete("Serial is deprecated, please use Label instead.")]
        public string Serial { get; set; }

        public string Label { get; set; }

        public int? KitID { get; set; }

        public AssetType Type { get; set; }
    }
}