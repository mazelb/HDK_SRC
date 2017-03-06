﻿// /**
// * @file RecordingListViewController.cs
// * @brief Contains the RecordingListViewController class
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Assets.Scripts.MainApp;
using Assets.Scripts.UI.RecordingLoading.Model;
using Assets.Scripts.UI.RecordingLoading.View;
using Assets.Scripts.UI.Settings;
using Assets.Scripts.Utils;
using HeddokoSDK;
using HeddokoSDK.Models;
using HeddokoSDK.Models.Requests;
using UIWidgets;
using UnityEngine;

namespace Assets.Scripts.UI.RecordingLoading
{
    public delegate void RecordingReady(RecordingListItem vItem);
    public class RecordingListViewController : MonoBehaviour
    {
        public Camera Cam;
        public event RecordingReady RecordingToBePlayedEvent;
        public GameObject DisablingPanel;
        public RecordingListSyncView View;
        [SerializeField]
        private int mClickCount;
        [SerializeField]
        private int mPreviousItemIndex = -1;
        [SerializeField]
        private float mDoubleClickTimer = 0.25f;
        [SerializeField]
        private float mTimer;
        private RecordingListFetcher mListFetcher;
        private HeddokoDownloadFetcher mRecordingFetcher;
        private List<RecordingListItem> mRecordingItems = new List<RecordingListItem>();
        public ScrollRectEvents ScrollRectEvents;
        public UploadController UploadController;
        private float mFetchCounter = 12f;
        private float mFetchTime = 12f;
        void Start()
        {
            
        }

        public void Init()
        {
            View.OnClickAction += DoubleClickCheck;
            if (mListFetcher == null)
            {
                mListFetcher = new RecordingListFetcher(UserSessionManager.Instance);
                mListFetcher.RecordingListUpdatedHandler += LoadDataThroughUnityThread;

            }
            mListFetcher.Start();
            if (mRecordingFetcher == null)
            {
                mRecordingFetcher = new HeddokoDownloadFetcher(UserSessionManager.Instance);
                mRecordingFetcher.ErrorDownloadingExceptionHandler += ExceptionHandler;
            }
        }

       
        private void ExceptionHandler(Exception vE)
        {
            Debug.Log(vE.Message);
        }

        /// <summary>
        /// Load recording list data into unity thread
        /// </summary>
        /// <param name="vList"></param>
        private void LoadDataThroughUnityThread(List<RecordingListItem> vList)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
            {
                mRecordingItems = vList;
                View.LoadData(mRecordingItems);
            });
        }

      
        

        /// <summary>
        /// Double click checker
        /// </summary>
        private void DoubleClickCheck()
        {
            if (!Cam.isActiveAndEnabled)
            {
                return;
            }
            mClickCount++;
            int vNewSelectedItem = View.SelectedIndex;
            if (vNewSelectedItem != -1)
            {
                if (mClickCount >= 2)
                {
                    if (mPreviousItemIndex == vNewSelectedItem)
                    {
                        DoubleClickAction(vNewSelectedItem);
                    }
                }
            }
            mPreviousItemIndex = vNewSelectedItem;
        }

        private void Update()
        {
            if (Cam.gameObject.activeInHierarchy)
            {
                if (mClickCount >= 1)
                {
                    mTimer += Time.deltaTime;
                    if (mTimer > mDoubleClickTimer)
                    {
                        mTimer = 0;
                        mClickCount = 0;
                    }
                }
            } 
        }


        /// <summary>
        /// A double click action
        /// </summary>
        /// <param name="vItemIndex"></param>
        private void DoubleClickAction(int vItemIndex)
        {
            var vItem = View.GetRecordingItem(vItemIndex);
            ProcessRecording(ref vItem);
        }

        /// <summary>
        /// Start processing the recording and indicate that its ready
        /// </summary>
        /// <param name="vItem"></param>
        public void ProcessRecording(ref RecordingListItem vItem)
        {
            //Check if the item exists in the cache already. else proceed to download it. 
            if (vItem.Location.LocationType == RecordingListItem.LocationType.CachedLocal)
            {
                if (RecordingToBePlayedEvent != null)
                {
                    RecordingToBePlayedEvent(vItem);
                }
            }
            else
            {
                //start downloading file. since this responsiblity will be delegated to a seperate thread, 
                //wait until completed.

                string vCachePath = ApplicationSettings.DownloadCacheFolderPath;
                DirectoryInfo vInfo = new DirectoryInfo(vCachePath);
                var vFilesInfo = vInfo.GetFiles();
                RecordingListItem vRecItem = vItem;
                var vFoundItem = vFilesInfo.FirstOrDefault(x => x.Name.Equals(vRecItem.Name));
                DataFetchingStructure vStructure = new DataFetchingStructure();
                //a cached item has been found
                if (vFoundItem != null)
                {
                    //change the location type
                    vItem.Location.LocationType = RecordingListItem.LocationType.CachedLocal;
                    vItem.Location.RelativePath = vFoundItem.FullName;
                    View.LoadData(mRecordingItems);
                }
                else
                {
                    DisablingPanel.SetActive(true);
                    //start downloading
                    vStructure.DownloadLocation = vCachePath + Path.DirectorySeparatorChar + vItem.Name;
                    vStructure.Item = vItem;
                    vItem.Location.LocationType = RecordingListItem.LocationType.DownloadingAndUnavailable;
                    View.LoadData(mRecordingItems);
                    mRecordingFetcher.DownloadCompletedHandler += DownloadCompletedCallback;
                    ThreadPool.QueueUserWorkItem(mRecordingFetcher.FetchData, vStructure);
                }
            }
        }
        /// <summary>
        /// Callback on when a download has been completed
        /// </summary>
        /// <param name="vHedAsset"></param>
        /// <param name="vItem"></param>
        private void DownloadCompletedCallback(BaseModel vHedAsset, ref RecordingListItem vItem)
        {
            //remove the callback
            mRecordingFetcher.DownloadCompletedHandler -= DownloadCompletedCallback;
            RecordingListItem vNonRefItem = vItem;
            Action vAction = () => UpdateList(vHedAsset, ref vNonRefItem);
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(vAction);
        }

        /// <summary>
        /// update the current list after a download has been completed
        /// </summary>
        /// <param name="vHedAsset"></param>
        /// <param name="vItem"></param>
        private void UpdateList(BaseModel vHedAsset, ref RecordingListItem vItem)
        {
            DisablingPanel.SetActive(false);
            vItem.Location.RelativePath = ApplicationSettings.DownloadCacheFolderPath + Path.DirectorySeparatorChar + vItem.Name;
            vItem.Location.LocationType = RecordingListItem.LocationType.CachedLocal;
            //reload the data
            View.LoadData(mRecordingItems);
            if (RecordingToBePlayedEvent != null)
            {
                RecordingToBePlayedEvent(vItem);
            }
        }

        private void OnApplicationQuit()
        {
            mListFetcher.Stop();
        }

        /// <summary>
        /// Clear data
        /// </summary>
        public void Clear()
        {
            if (mListFetcher != null)
            {
                mListFetcher.Stop();
                mListFetcher.Clear();
            }
            if (View != null)
            {
                View.Clear();
                View.OnClickAction -= DoubleClickCheck;
            }
        }

        /// <summary>
        /// Reset the list
        /// </summary>
        public void ResetDownloadList()
        { 
           Invoke("WaitThenReDownload",1.5f);
        }



        /// <summary>
        /// Wait then redownload the recording list again
        /// </summary>
        /// <param name="vSeconds"></param>
        /// <returns></returns>
        private void WaitThenReDownload( )
        {
            Clear();
            mListFetcher.Start();
            View.OnClickAction += DoubleClickCheck;
        }

  
    }
}