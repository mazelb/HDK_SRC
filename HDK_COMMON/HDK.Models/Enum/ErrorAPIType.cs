/**
 * @file ErrorAPIType.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
namespace HDK.Models.Enum
{
    //sync with Heddoko.DAL
    public enum ErrorAPIType
    {
        Info,
        Warning,
        Error,
        EmailOrPassword,
        SigninValidation,
        UserIsBanned,
        UserIsNotActive,
        UserIsExist,
        WrongEmailForgotPassword,
        OldPasswordWrongMessage,
        UserNotFound,
        WrongObjectAccess,
        ObjectNotFound,
        ObjectWasDeleted,
        LicenseIsNotReady,
        KitID,
        AssetType,
        UserIsNotApproved
    }
}
