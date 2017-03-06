/**
 * @file AssetFile.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using HeddokoSdkXamarin.Models.Enum;

namespace HeddokoSdkXamarin.Models.Requests
{
    public class AssetFile
    {
        public AssetType Type { get; set; }

        public string FileName { get; set; }
    }
}
