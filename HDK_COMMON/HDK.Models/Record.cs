/**
 * @file Record.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using System.Collections.Generic;

namespace HDK.Models
{
    public class Record : BaseModel
    {
        public int? UserID { get; set; }

        public virtual User User { get; set; }

        public int? KitID { get; set; }

        public List<Asset> Assets { get; set; }
    }
}
