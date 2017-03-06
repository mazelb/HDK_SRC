/**
 * @file FileManager.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using System;
using System.IO;
using HeddokoSdkXamarin.Interfaces;
using HeddokoSdkXamarin.Test.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileManager))]
namespace HeddokoSdkXamarin.Test.Droid
{
    public class FileManager : IFileManager
    {
        public byte[] ReadAllBytes(string filename)
        {
            var filePath = GetFilePath(filename);

            return File.ReadAllBytes(filePath);
        }

        public void CreateAndWriteFile(string filename, byte[] data)
        {
            var filePath = GetFilePath(filename);

            FileStream fileStream = File.Create(filePath, data.Length);

            fileStream.Write(data, 0, data.Length);

            fileStream.Close();
        }

        private static string GetFilePath(string filename)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);

            return filePath;
        }
    }
}