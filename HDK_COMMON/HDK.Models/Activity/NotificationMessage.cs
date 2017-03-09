/**
 * @file NotificationMessage.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using HDK.Models.Enum;

namespace HDK.Models.Activity
{
    public class NotificationMessage
    {
        public UserEventType Type { get; set; }

        public string Text { get; set; }
    }
}