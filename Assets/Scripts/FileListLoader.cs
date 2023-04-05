using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class FileListLoader : MonoBehaviour
{
    public Dropdown dropdown;
    private string url = "http://localhost/slides";

    void Start()
    {
        StartCoroutine(GetFileList());
    }

    IEnumerator GetFileList()
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonString = request.downloadHandler.text;
            List<string> files = JsonUtility.FromJson<List<string>>(jsonString);
            PopulateDropdown(files);
        }
        else
        {
            Debug.LogError("Error getting file list: " + request.error);
        }
    }

    void PopulateDropdown(List<string> files)
    {
        dropdown.options.Clear();

        foreach (string file in files)
        {
            dropdown.options.Add(new Dropdown.OptionData(file));
        }

        dropdown.RefreshShownValue();
    }
}
