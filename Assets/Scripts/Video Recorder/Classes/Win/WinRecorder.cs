using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using UnityEngine;

public class WinRecorder
{
#if UNITY_STANDALONE_WIN
    public IngameVideoRecorder IGVR;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool AttachConsole(uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    static extern bool FreeConsole();

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add);

    delegate bool ConsoleCtrlDelegate(CtrlTypes CtrlType);

    // Enumerated type for the control messages sent to the handler routine
    enum CtrlTypes : uint
    {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT,
        CTRL_CLOSE_EVENT,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT
    }

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GenerateConsoleCtrlEvent(CtrlTypes dwCtrlEvent, uint dwProcessGroupId);

    public void StopProgram(Process proc)
    {
        //This does not require the console window to be visible.
        if (AttachConsole((uint)proc.Id))
        {
            // Disable Ctrl-C handling for our program
            SetConsoleCtrlHandler(null, true);
            GenerateConsoleCtrlEvent(CtrlTypes.CTRL_C_EVENT, 0);

            //Moved this command up on suggestion from Timothy Jannace (see comments below)
            FreeConsole();

            // Must wait here. If we don't and re-enable Ctrl-C
            // handling below too fast, we might terminate ourselves.
            proc.WaitForExit(2000);

            //Re-enable Ctrl-C handling or any subsequently started
            //programs will inherit the disabled state.
            SetConsoleCtrlHandler(null, false);
        }
    }

    public void Init(IngameVideoRecorder _IGVR)
    {
        IGVR = _IGVR;
    }

    public void Start()
    {

    }

    public void RecordScreen()
    {
        if (IGVR.SetRecordScreen == false || IGVR.recording == true)
            return;

        UnityEngine.Debug.Log("Started Recording!");
        IGVR.proc = new Process();
        IGVR.proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        IGVR.proc.StartInfo.CreateNoWindow = true;

        IGVR.proc.StartInfo.FileName = Application.streamingAssetsPath + @"/FFMPEG/Windows/ffmpeg.exe";
        IGVR.proc.StartInfo.Arguments = " -y -hwaccel cuda -hwaccel_output_format cuda -f gdigrab -framerate 30 -s 1920x1080 -i desktop -preset ultrafast -crf 0 " + IGVR.savePath + IGVR.filePath;

        UnityEngine.Debug.Log(IGVR.proc.StartInfo.FileName + IGVR.proc.StartInfo.Arguments);
        IGVR.proc.Start();
        IGVR.recording = true;
    }
#endif
}
