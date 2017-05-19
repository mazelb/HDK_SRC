/**
 * @file UserEventType.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
namespace HDK.Models.Enum
{
    public enum UserEventType
    {
        StreamChannelOpened,
        StreamChannelClosed,
        LicenseAddedToOrganization,
        LicenseRemovedFromOrganization,
        LicenseAddedToUser,
        LicenseRemovedFromUser,
        LicenseChangedForUser,
        LicenseExpiring,
        LicenseExpired
    }
}
