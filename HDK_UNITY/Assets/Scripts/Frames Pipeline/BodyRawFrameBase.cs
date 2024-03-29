﻿/**
* @file BodyRawFrameBase.cs
* @brief Contains the BodyRawFrameBase class
* @author Mohammed Haider(  mohammed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/
namespace Assets.Scripts.Frames_Pipeline
{
    /// <summary>
    /// The base class for RawFrames
    /// </summary>
    public abstract class BodyRawFrameBase
    {
        /// <summary>
        /// The index of the raw frame
        /// </summary>
         public int Index { get; set; }
        //containing Recording GUID
        public string BodyRecordingGuid { get; set; }

     
        //Containing Body GUID 
        public string BodyGuid { get; set; }

 
        //Containing Suit GUID 
        public string SuitGuid { get; set; }
 


    }
}