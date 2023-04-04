using UnityEngine;
using RockVR.Video;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEngine.XR.Content.Interaction
{
    public class StartButtonHandler : MonoBehaviour
    {
        public XRPushButton xrPushButton;

        private void OnEnable()
        {
            xrPushButton.onPress.AddListener(OnPushButtonPress);
            
        }

        private void OnDisable()
        {
            xrPushButton.onPress.RemoveListener(OnPushButtonPress);
        }

        public void OnPushButtonPress()
        {
            // Show in console that the button has been pressed
            Debug.Log("Button Pressed");
            

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