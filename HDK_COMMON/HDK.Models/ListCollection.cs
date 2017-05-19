/**
 * @file ListCollection.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using System.Collections.Generic;

namespace HDK.Models
{
    public class ListCollection<T> : BaseModel
    {
        public int TotalCount { get; set; }

        public List<T> Collection { get; set; }
    }
}
