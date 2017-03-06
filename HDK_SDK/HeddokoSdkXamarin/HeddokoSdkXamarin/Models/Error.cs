/**
 * @file Error.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using HeddokoSdkXamarin.Models.Enum;

namespace HeddokoSdkXamarin.Models
{
    public class Error
    {
        public ErrorAPIType? Code { get; set; }
        public string Message { get; set; }
    }
}
