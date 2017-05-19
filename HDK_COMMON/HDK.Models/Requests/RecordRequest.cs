/**
 * @file RecordRequest.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using System.Collections.Generic;

namespace HDK.Models.Requests
{
    public class RecordRequest
    {
        public string Label { get; set; }

        public int? KitID { get; set; }

        public List<AssetFile> Files { get; set; }
    }
}
