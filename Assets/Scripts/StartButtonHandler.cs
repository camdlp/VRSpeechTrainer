using UnityEngine;
using RockVR.Video;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

namespace UnityEngine.XR.Content.Interaction
{
    public class StartButtonHandler : MonoBehaviour
    {
        public XRPushButton xrPushButton;
        public Light directionalLight;
        public VideoCaptureCtrl videoCaptureCtrl;

        public void OnPushButtonPress()
        {
            Debug.Log("Button Pressed");
            Debug.Log(videoCaptureCtrl.status);

            if (videoCaptureCtrl.status != VideoCaptureCtrlBase.StatusType.STARTED)
            {
                videoCaptureCtrl.StartCapture();
            }
            else
            {
                videoCaptureCtrl.StopCapture();
            }

            StartCoroutine(ToggleDirectionalLight());
        }

        private IEnumerator ToggleDirectionalLight()
        {
            Debug.Log("Toggling directional light");

            float currentIntensity = directionalLight.intensity;
            float targetIntensity = currentIntensity == 0f ? 1f : 0f;
            float step = 0.05f;

            while (Mathf.Abs(directionalLight.intensity - targetIntensity) > 0.01f)
            {
                currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, step);
                directionalLight.intensity = currentIntensity;
                yield return null;
            }
        }
    }
}
