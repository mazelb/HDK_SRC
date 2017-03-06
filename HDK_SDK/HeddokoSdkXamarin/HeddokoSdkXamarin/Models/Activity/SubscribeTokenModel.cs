/**
 * @file SubscribeTokenModel.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using HeddokoSdkXamarin.Models.Enum;

namespace HeddokoSdkXamarin.Models.Activity
{
    public class SubscribeTokenModel
    {
        public string Token { get; set; }

        public DeviceType Type { get; set; }
    }
}