using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using SimpleJSON;

// Class for loading the list of recordings from the API
public class RecordingsListLoader : MonoBehaviour
{
    // Dropdown UI component
    public TMP_Dropdown dropdown;

    // Server URL to fetch recordings list
    private string url = "http://localhost:5000/recordings";

    // Fetch the file list at the start of the script
    void Start()
    {
        StartCoroutine(GetFileList());
    }

    // Coroutine to fetch file list from server
    IEnumerator GetFileList()
    {
        // Send a GET request to the server
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Get the response text
            string jsonString = request.downloadHandler.text;
            List<string> files = ParseFileList(jsonString);
            PopulateDropdown(files);
        }
        else
        {
            Debug.LogError("Error getting file list: " + request.error);
        }
    }

    // Parse the file list from the JSON response
    List<string> ParseFileList(string jsonString)
    {
        // Parse the JSON string into a JSON array
        JSONArray jsonArray = JSON.Parse(jsonString) as JSONArray;
        List<string> fileList = new List<string>();

        // Extract each file name and add it to the list
        foreach (JSONNode node in jsonArray)
        {
            fileList.Add(node.Value);
        }

        return fileList;
    }

    // Populate the dropdown with the file list
    void PopulateDropdown(List<string> files)
    {
        // Clear the dropdown options
        dropdown.options.Clear();

        // Add each file as a dropdown option
        foreach (string file in files)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(file));
        }

        // Refresh the displayed value
        dropdown.RefreshShownValue();
    }
}
