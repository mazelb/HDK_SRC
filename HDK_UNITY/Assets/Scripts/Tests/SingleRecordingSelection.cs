﻿/** 
* @file SingleRecordingSelection.cs
* @brief Contains the SingleRecordingSelection  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.IO;
using Assets.Scripts.Frames_Recorder.FramesRecording;
using Assets.Scripts.Localization;
using Assets.Scripts.UI.ModalWindow;
using Assets.Scripts.UI.RecordingLoading;
using Assets.Scripts.UI.Settings;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Tests
{
    public delegate void StartLoading();

    public delegate void FinishLoading();
    /// <summary>
    /// singleton that loads a single recording
    /// </summary>
    public class SingleRecordingSelection : MonoBehaviour
    {
        public Rect mRect;
        private static SingleRecordingSelection sInstance;
     
        public StartLoading StartLoadingEvent;
        public FinishLoading FinishLoadingEvent;
        public static Action<BodyFrame[]> ReadProtoFileAction;
        public RecordingLoader RecordingLoader;
        //Panel that will cover other ui elements, thereby dissallowing their controls
        public GameObject DisablerPanel;

        public RectTransform SizeControlRectTransform;

        public static SingleRecordingSelection Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = GameObject.FindObjectOfType<SingleRecordingSelection>();
                }
                return sInstance;
            }
        }

        private void Start()
        {
            UniFileBrowser.use.SendWindowCloseMessage(HideDisablerPanel);
        }

        /// <summary>
        /// opens a File browser dialog to select a recording with an optional callback after file is completed loading
        /// </summary>
        public void OpenFileBrowseDialog(Action<BodyFramesRecordingBase> vCallback = null)
        {
            SetTransform();
            DisablerPanel.SetActive(true);
            if (vCallback != null)
            {
                 RecordingLoader.SetCallbackAction(vCallback);
            }
            var vPaths = new[] { "dat", "hsm" };
            //initialize the browser settings
#if DEBUG  
            vPaths = new[] { "csv", "dat", "hsm" };
#endif
            UniFileBrowser.use.SetFileExtensions(vPaths);
            UniFileBrowser.use.allowMultiSelect = false;
            UniFileBrowser.use.showVolumes = true;
            UniFileBrowser.use.OpenFileWindow(SelectRecordingFile);
        }

        /// <summary>
        /// Load a recording file without the need of a file browser dialog
        /// </summary>
        /// <param name="vPath">the path of the file to load</param>
        /// <param name="vCallback">on load completion, initiate callback.</param>
        public void LoadFile(string vPath ,Action<BodyFramesRecordingBase> vCallback)
        {
            RecordingLoader.LoadFile(vPath,vCallback);
        }

        /// <summary>
        /// Callback, on file selection
        /// </summary>
        /// <param name="vRecordingSelected"></param>
        private void SelectRecordingFile(string vRecordingSelected)
        {
            FileInfo vInfo = new FileInfo(vRecordingSelected);
            if (vInfo.Name.Equals("logindex.dat", StringComparison.CurrentCultureIgnoreCase))
            {
                string vTopLabel =   LocalizationBinderContainer.GetString(KeyMessage.TopLabelLogindexOpeningErrMsg );
                string vContent =LocalizationBinderContainer.GetString(KeyMessage.LogindexOpeningErrMsg);
                UnityAction vOnYes = () => OpenFileBrowseDialog();
                UnityAction vOnNo = () => { };
                ModalPanel.Instance().Choice(vTopLabel, vContent, vOnYes, vOnNo);
                return;
            }
            if (StartLoadingEvent != null)
            {
                StartLoadingEvent();
            }
         }

  

        /// <summary>
        /// Disables the disabler panel
        /// </summary>
        private void HideDisablerPanel()
        {
            DisablerPanel.SetActive(false);
        }
        /// <summary>
        /// Sets the transform of the unifilebrowser. It uses the old UI system as its front end
        /// </summary>
        private void SetTransform()
        {
            Rect vSizeRect = mRect = RectTransformToScreenSpace(SizeControlRectTransform);
            Vector2 vPos = new Vector2(vSizeRect.x, vSizeRect.y);
            Vector2 vSize = new Vector2(vSizeRect.width, vSizeRect.height);
            UniFileBrowser.use.SetFileWindowSize(vSize);
            UniFileBrowser.use.SetFileWindowPosition(vPos);
        }

        /// <summary>
        /// get the recttransform's rect and rect of its rect in screen space
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Rect RectTransformToScreenSpace(RectTransform transform)
        {
            Vector2 vSize = Vector2.Scale(transform.rect.size, transform.lossyScale);
            return new Rect((Vector2)transform.position - (vSize * 0.5f), vSize);
        }

    }
}