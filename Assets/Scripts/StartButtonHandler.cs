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
        public GameObject spotlightsParent; // Agrega un campo para el objeto que contiene las luces Spotlight
        public VideoCaptureCtrl videoCaptureCtrl;

        private Light[] spotlights; // Almacena una referencia a todas las luces Spotlight

        private void Start()
        {
            // Encuentra todas las luces Spotlight que son hijas del objeto spotlightsParent
            spotlights = spotlightsParent.GetComponentsInChildren<Light>();
        }

        public void OnPushButtonPress()
        {
            Debug.Log("Button Pressed");
            Debug.Log(videoCaptureCtrl.status);

            //if (videoCaptureCtrl.status != VideoCaptureCtrlBase.StatusType.STARTED)
            //{
            //    videoCaptureCtrl.StartCapture();
            //}
            //else
            //{
            //    videoCaptureCtrl.StopCapture();
            //}

            StartCoroutine(ToggleDirectionalLight());
            StartCoroutine(ToggleSpotlights()); // Activa el toggle para todas las luces Spotlight
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

        // Añade una nueva Coroutine para controlar el encendido y apagado de las luces Spotlight
        private IEnumerator ToggleSpotlights()
        {
            Debug.Log("Toggling spotlights");

            float step = 0.05f;

            // Itera sobre todas las luces Spotlight y ajusta su intensidad
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

