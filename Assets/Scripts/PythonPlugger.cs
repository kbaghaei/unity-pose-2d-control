using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using DefaultNamespace;
using MiscUtil.Collections.Extensions;
using Debug = UnityEngine.Debug;

// used this link as reference: https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.beginoutputreadline?view=net-5.0

public class PythonPlugger : MonoBehaviour
{
    public KeyPointsPack2D latestPack;
    
    public KeyPointsPack2D getLatestPack2D()
    {
        return latestPack;
    }

    private static PythonPlugger theOnlyPlugger = null;
    public static PythonPlugger MainPlugger()
    {
        return theOnlyPlugger;
    }

    private Process pythonProc;
    void Start()
    {
        pythonProc = new Process();
        string nyme = @"C:\Users\MyLocalUser\python_projs\3d-pose-in-unity\call_from_csharp.bat";
        pythonProc.StartInfo.FileName = nyme;
        pythonProc.StartInfo.UseShellExecute = false;
        pythonProc.StartInfo.RedirectStandardOutput = true;
        pythonProc.StartInfo.RedirectStandardError = true;
        pythonProc.OutputDataReceived += UpdateKeyPointsPack2DHandler;
        pythonProc.ErrorDataReceived += ErrorDataHandler;
        pythonProc.Start();
        pythonProc.BeginOutputReadLine();
        theOnlyPlugger = this;
    }

    void UpdateKeyPointsPack2DHandler(object sendingProcess, DataReceivedEventArgs data)
    {
        if (!String.IsNullOrEmpty(data.Data))
        {
            try
            {
                latestPack = KeyPointsPack2D.GetPackCommaSeparatedString(data.Data);
            }
            catch (Exception e)
            {
                Debug.LogError("There was an exception:" + e.ToString());
                Debug.LogError("Error data: " + data.Data);
            }
        }
    }

    void ErrorDataHandler(object sendingProcess, DataReceivedEventArgs data)
    {
        Debug.LogError("An error occured in the pipeline from python: " + data.Data);
    }
    
    // Update is called once per frame

    public string currentFryme = "ZEEEROO";
    public bool debugOnlyStopProcess = false;
    void Update()
    {
        currentFryme = Time.frameCount.ToString();
        if (debugOnlyStopProcess)
        {
            pythonProc.Kill();
            pythonProc.Dispose();
        }
    }
}