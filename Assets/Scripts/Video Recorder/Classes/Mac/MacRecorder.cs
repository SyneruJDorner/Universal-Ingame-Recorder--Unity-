using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using UnityEngine;

public class MacRecorder
{
#if UNITY_STANDALONE_OSX
    public IngameVideoRecorder IGVR;

    [DllImport ("libc", EntryPoint = "chmod", SetLastError = true)]
    private static extern int sys_chmod (string path, uint mode);

    public void StopProgram(Process proc)
    {
        UnityEngine.Debug.Log("Killed process on MAC");

        int IDstring = System.Convert.ToInt32(proc.Id.ToString());
        Process tempProc = Process.GetProcessById(IDstring);

        tempProc.CloseMainWindow();
        tempProc.WaitForExit();
    }

    public void Init(IngameVideoRecorder _IGVR)
    {
        IGVR = _IGVR;
    }

    public void Start()
    {
        sys_chmod(Application.streamingAssetsPath + @"/FFMPEG/MAC/ffmpeg", 755);
    }

    public void RecordScreen()
    {
        if (IGVR.SetRecordScreen == false || IGVR.recording == true)
            return;

        UnityEngine.Debug.Log("Started Recording!");
        IGVR.proc = new Process();
        IGVR.proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        IGVR.proc.StartInfo.CreateNoWindow = true;

        IGVR.proc.StartInfo.FileName = Application.streamingAssetsPath + @"/FFMPEG/MAC/ffmpeg";
        IGVR.proc.StartInfo.Arguments = " -y -f avfoundation -i 1 -pix_fmt yuv420p -framerate 10 -vcodec libx264 -preset ultrafast -vsync 2 " + IGVR.savePath + IGVR.filePath;

        UnityEngine.Debug.Log(IGVR.proc.StartInfo.FileName + IGVR.proc.StartInfo.Arguments);
        IGVR.proc.Start();
        IGVR.recording = true;
    }
#endif
}
