/**
 * @file ErrorCollection.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using System;
using System.Collections.Generic;

namespace HeddokoSdkXamarin.Models
{
    public class ErrorCollection
    {
        public List<Error> Errors { get; set; }
        
        public string Method { get; set; }

        public string ID { get; set; }

        public Guid Guid { get; set; }
    }
}
