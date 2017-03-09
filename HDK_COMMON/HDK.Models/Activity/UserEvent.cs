/**
 * @file UserEvent.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using System;
using HDK.Models.Enum;

namespace HDK.Models.Activity
{
    public class UserEvent
    {
        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        public UserEventType Type { get; set; }

        public int UserId { get; set; }

        public ReadStatus ReadStatus { get; set; }

        public UserEventStatus Status { get; set; }

        public string Message { get; set; }
    }
}
