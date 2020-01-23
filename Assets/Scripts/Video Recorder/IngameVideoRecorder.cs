using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using UnityEngine;

public class IngameVideoRecorder : MonoBehaviour
{
    [HideInInspector] public bool SetRecordScreen = false;
    [HideInInspector] public Process proc = null;
    public bool recording = false;
    public string savePath => "\"" + Application.streamingAssetsPath + "/ScreenRecorder";
    public string filePath => "/out_video.mp4\"";

    private MacRecorder macRecorder = new MacRecorder();
    private WinRecorder winRecorder = new WinRecorder();

    public void Start()
    {

        recording = false;
        SetRecordScreen = false;


#if     UNITY_STANDALONE_WIN
        winRecorder.Init(this);
        winRecorder.Start();
#endif 

#if     UNITY_STANDALONE_OSX
        macRecorder.Init(this);
        macRecorder.Start();
#endif 
    }

    public void OnApplicationQuit()
    {
        if (proc != null)
        {
#if UNITY_STANDALONE_WIN
            winRecorder.StopProgram(proc);
#endif

#if UNITY_STANDALONE_OSX
            macRecorder.StopProgram(proc);
#endif

            UnityEngine.Debug.Log("Stopped Recording!");
            proc = null;
            recording = false;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            SetRecordScreen = !SetRecordScreen;
            if (recording == true && SetRecordScreen == false)
            {
#if UNITY_STANDALONE_WIN
                winRecorder.StopProgram(proc);
#endif

#if UNITY_STANDALONE_OSX
                macRecorder.StopProgram(proc);
#endif
                UnityEngine.Debug.Log("Stopped Recording!");
                proc = null;
                recording = false;
            }
        }

#if UNITY_STANDALONE_WIN
        winRecorder.RecordScreen();
#endif

#if UNITY_STANDALONE_OSX
        macRecorder.RecordScreen();
#endif
    }
}
