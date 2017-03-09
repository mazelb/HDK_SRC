/**
 * @file IFileManager.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/

namespace HDK.SDK.Interfaces
{
    public interface IFileManager
    {
        byte[] ReadAllBytes(string filename);

        void CreateAndWriteFile(string filename, byte[] data);
    }
}
