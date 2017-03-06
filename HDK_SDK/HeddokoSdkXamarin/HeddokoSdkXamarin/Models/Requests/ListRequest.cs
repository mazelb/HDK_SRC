/**
 * @file ListRequest.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
namespace HeddokoSdkXamarin.Models.Requests
{
    public class ListRequest
    {
        public int Take { get; set; }

        public int? Skip { get; set; }
    }
}
