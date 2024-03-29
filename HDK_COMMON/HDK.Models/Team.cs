﻿/**
 * @file Team.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
namespace HDK.Models
{
    public class Team : BaseModel
    {
        public string Name { get; set; }

        public string Address { get; set; }
    }
}
