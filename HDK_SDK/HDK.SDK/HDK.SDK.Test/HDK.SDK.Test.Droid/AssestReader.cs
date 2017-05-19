/**
 * @file AssestReader.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using System.IO;
using HeddokoSdkXamarin.Test.Droid;
using HeddokoSdkXamarin.Test.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(AssestReader))]
namespace HeddokoSdkXamarin.Test.Droid
{
    public class AssestReader : IAssestReader
    {
        public byte[] ReadAllBytes(string filename)
        {
            int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];

            using (Stream stream = Forms.Context.Assets.Open(filename))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, bufferSize)) != 0)
                    {
                        ms.Write(buffer, 0, bytesRead);
                    }

                    return ms.ToArray();
                }
            }

        }
    }
}