/**
 * @file IAssestReader.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
namespace HeddokoSdkXamarin.Test.Interfaces
{
    public interface IAssestReader
    {
        byte[] ReadAllBytes(string filename);
    }
}
