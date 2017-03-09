/**
 * @file FirmwareRequest.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using HDK.Models.Enum;

namespace HDK.Models.Requests
{
    public class FirmwareRequest
    {
        public int? ID { get; set; }

        public string Label { get; set; }

        public int? FirmwareID { get; set; }

        public FirmwareType? Type { get; set; }
    }
}
