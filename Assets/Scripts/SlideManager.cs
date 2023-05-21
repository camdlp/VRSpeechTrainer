using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.IO.Compression;
using System;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class SlideManager : MonoBehaviour
{
    public TMP_Dropdown slideDropdown;
    public RawImage slideImage;
    public GameObject loadingPanel;

    private Texture2D[] slides;
    private int currentSlide = 0;

    // These booleans are used to manage the button states of the VR controller
    private bool primaryButtonReleased;
    private bool secondaryButtonReleased;

    private void Start()
    {
        primaryButtonReleased = true;
        secondaryButtonReleased = true;

        // Add a listener to the dropdown's onValueChanged event
        slideDropdown.onValueChanged.AddListener(delegate
        {
            OnSlideSelected();
        });

        // Load the initially selected slide
        OnSlideSelected();
    }

    // This function is called every frame and handles VR controller inputs
    private void Update()
    {
        // Search for the right VR controller
        InputDeviceCharacteristics characteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right;
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);

        // Handle inputs for the right VR controller
        foreach (InputDevice device in devices)
        {
            if (device.isValid && (device.characteristics & characteristics) == characteristics)
            {
                bool primaryButtonPressed;
                if (device.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonPressed))
                {
                    if (primaryButtonPressed && primaryButtonReleased)
                    {
                        NextSlide();
                        primaryButtonReleased = false;
                    }
                    else if (!primaryButtonPressed)
                    {
                        primaryButtonReleased = true;
                    }
                }

                bool secondaryButtonPressed;
                if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonPressed))
                {
                    if (secondaryButtonPressed && secondaryButtonReleased)
                    {
                        PreviousSlide();
                        secondaryButtonReleased = false;
                    }
                    else if (!secondaryButtonPressed)
                    {
                        secondaryButtonReleased = true;
                    }
                }
            }
        }
    }

    // Coroutine to fetch slides from the server and unzip them
    public IEnumerator GetSlides(string slideFilename)
    {
        string apiURL = $"http://localhost:5000/api/download_slides?filename={slideFilename}";
        using (UnityWebRequest www = UnityWebRequest.Get(apiURL))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                // Debug.LogError(www.error);
            }
            else
            {
                byte[] zipData = www.downloadHandler.data;
                slides = UnzipSlides(zipData);
                if (slides.Length > 0)
                {
                    slideImage.texture = slides[0];
                }
            }
        }

        loadingPanel.SetActive(false);
    }

    // Function to unzip a zip file and return an array of Texture2D
    public Texture2D[] UnzipSlides(byte[] zipData)
    {
        List<Texture2D> textures = new List<Texture2D>();

        using (MemoryStream zipStream = new MemoryStream(zipData))
        {
            using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                        entry.FullName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        entry.FullName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                    {
                        using (Stream entryStream = entry.Open())
                        {
                            byte[] imageData = new byte[entry.Length];
                            entryStream.Read(imageData, 0, imageData.Length);

                            Texture2D tex = new Texture2D(2, 2);
                            ImageConversion.LoadImage(tex, imageData);
                            textures.Add(tex);
                        }
                    }
                }
            }
        }

        return textures.ToArray();
    }

    // This function is called when a new slide is selected in the dropdown
    public void OnSlideSelected()
    {
        string selectedSlide = slideDropdown.options[slideDropdown.value].text;
        if (string.IsNullOrEmpty(selectedSlide)) { return; }
        loadingPanel.SetActive(true);
        StartCoroutine(GetSlides(selectedSlide));
    }

    // Function to navigate to the next slide
    public void NextSlide()
    {
        if (slides != null && slides.Length > 0)
        {
            currentSlide = (currentSlide + 1) % slides.Length;
            if (currentSlide == 0)
            {
                currentSlide = slides.Length - 1;
            }
            slideImage.texture = slides[currentSlide];
        }
    }

    // Function to navigate to the previous slide
    public void PreviousSlide()
    {
        if (slides != null && slides.Length > 0)
        {
            currentSlide = (currentSlide - 1 + slides.Length) % slides.Length;
            if (currentSlide == slides.Length - 1)
            {
                currentSlide = 0;
            }
            slideImage.texture = slides[currentSlide];
        }
    }
}
