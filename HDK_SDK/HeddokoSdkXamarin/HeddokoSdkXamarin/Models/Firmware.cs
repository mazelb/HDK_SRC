/**
 * @file Firmware.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using HeddokoSdkXamarin.Models.Enum;

namespace HeddokoSdkXamarin.Models
{
    public class Firmware : BaseModel
    {
        public string Version { get; set; }

        public FirmwareType Type { get; set; }

        public string IDView { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }
    }
}
