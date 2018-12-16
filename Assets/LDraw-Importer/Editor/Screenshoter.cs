using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public static class Screenshoter
{
  
    [MenuItem("Screenshoter/Take Screenshot")]
    public static void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot(DateTime.Now.Ticks + ".png");
    }
	
}
