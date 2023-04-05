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

    private bool primaryButtonReleased;
    private bool secondaryButtonReleased;


    private void Start()
    {
        primaryButtonReleased = true;
        secondaryButtonReleased = true;

        // Agregar listener al evento onValueChanged del dropdown
        slideDropdown.onValueChanged.AddListener(delegate
        {
            OnSlideSelected();
        });

        // Llamar al método OnSlideSelected al iniciar la escena
        OnSlideSelected();
    }

    private void Update()
    {
        // Check if the A button on the right controller is pressed
        InputDeviceCharacteristics characteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right;
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);

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


    public IEnumerator GetSlides(string slideFilename)
    {
        string apiURL = $"http://localhost:5000/api/download_slides?filename={slideFilename}";
        using (UnityWebRequest www = UnityWebRequest.Get(apiURL))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
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

    public void OnSlideSelected()
    {
        string selectedSlide = slideDropdown.options[slideDropdown.value].text;
        if (string.IsNullOrEmpty(selectedSlide)) { return; }
        loadingPanel.SetActive(true);
        StartCoroutine(GetSlides(selectedSlide));
    }

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
