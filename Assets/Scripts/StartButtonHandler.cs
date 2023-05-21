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
        // Field for the object containing the Spotlight lights
        public GameObject spotlightsParent;
        public VideoCaptureCtrl videoCaptureCtrl;

        // Reference to all Spotlight lights
        private Light[] spotlights;

        private void Start()
        {
            // Find all Spotlight lights that are children of the spotlightsParent object
            spotlights = spotlightsParent.GetComponentsInChildren<Light>();
        }

        public void OnPushButtonPress()
        {
            Debug.Log("Button Pressed");
            Debug.Log(videoCaptureCtrl.status);

            // Toggle video recording on button press
            if (videoCaptureCtrl.status != VideoCaptureCtrlBase.StatusType.STARTED)
            {
                videoCaptureCtrl.StartCapture();
            }
            else
            {
                videoCaptureCtrl.StopCapture();
            }
            

            // Toggle directional light and Spotlight lights on button press
            StartCoroutine(ToggleDirectionalLight());
            StartCoroutine(ToggleSpotlights());
        }

        // Coroutine to control the on/off state of the directional light
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

        // Coroutine to control the on/off state of the Spotlight lights
        private IEnumerator ToggleSpotlights()
        {
            Debug.Log("Toggling spotlights");

            float step = 0.05f;

            // Iterates over all Spotlight lights and adjust their intensity
            foreach (Light spotlight in spotlights)
            {
                float currentIntensity = spotlight.intensity;
                float targetIntensity = currentIntensity == 0f ? 1f : 0f;

                while (Mathf.Abs(spotlight.intensity - targetIntensity) > 0.01f)
                {
                    currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, step);
                    spotlight.intensity = currentIntensity;
                    yield return null;
                }
            }
        }
    }
}
