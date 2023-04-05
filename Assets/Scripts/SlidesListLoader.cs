using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using SimpleJSON;

public class SlidesListLoader : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    private string url = "http://localhost:5000/slides";

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
            List<string> files = ParseFileList(jsonString);
            PopulateDropdown(files);
        }
        else
        {
            Debug.LogError("Error getting file list: " + request.error);
        }
    }

    List<string> ParseFileList(string jsonString)
    {
        JSONArray jsonArray = JSON.Parse(jsonString) as JSONArray;
        List<string> fileList = new List<string>();

        foreach (JSONNode node in jsonArray)
        {
            fileList.Add(node.Value);
        }

        return fileList;
    }

    void PopulateDropdown(List<string> files)
    {
        dropdown.options.Clear();

        foreach (string file in files)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(file));
        }

        dropdown.RefreshShownValue();
    }
}
