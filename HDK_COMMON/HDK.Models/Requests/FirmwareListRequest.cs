/**
 * @file FirmwareListRequest.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using HDK.Models.Enum;

namespace HDK.Models.Requests
{
    public class FirmwareListRequest : ListRequest
    {
        public FirmwareType Type { get; set; }
    }
}
