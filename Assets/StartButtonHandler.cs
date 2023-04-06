using UnityEngine;
using RockVR.Video;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Networking;
using System.IO;

namespace UnityEngine.XR.Content.Interaction
{
    public class StartButtonHandler : MonoBehaviour
    {
        public XRPushButton xrPushButton;

        public void OnPushButtonPress()
        {
            // Show in console that the button has been pressed
            Debug.Log("Button Pressed");
            Debug.Log(VideoCaptureCtrl.instance.status);

            if (VideoCaptureCtrl.instance.status != VideoCaptureCtrlBase.StatusType.STARTED)
            {
                VideoCaptureCtrl.instance.StartCapture();
            }
            else
            {
                VideoCaptureCtrl.instance.StopCapture();
            }
        }

        
    }
}
