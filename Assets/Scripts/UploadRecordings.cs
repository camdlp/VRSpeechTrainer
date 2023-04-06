using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using RockVR.Video;

public class UploadRecordings : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string recordingsDirectory = Application.dataPath + "/PlayerData/Recordings";
        FileSystemWatcher watcher = new FileSystemWatcher(recordingsDirectory);
        watcher.Filter = "*.mp4";
        watcher.Created += OnFileCreated;
        watcher.EnableRaisingEvents = true;
    }

    void OnFileCreated(object source, FileSystemEventArgs e)
    {
        if (Path.GetExtension(e.FullPath) == ".mp4")
        {
            Debug.Log("Detected new recording: " + e.FullPath);
            UploadRecordingToServer(e.FullPath);
        }
    }

    public void UploadRecordingToServer(string recordingFilePath)
    {
        Debug.Log("Instance video status" + VideoCaptureCtrl.instance.status);
        string uploadUrl = "http://localhost:5000/api/upload";

        UnityWebRequest uploadRequest = UnityWebRequest.Post(uploadUrl, "");
        uploadRequest.uploadHandler = new UploadHandlerFile(recordingFilePath);
        uploadRequest.SendWebRequest();

        Debug.Log("Uploading " + recordingFilePath);

        // Wait for upload to complete
        while (!uploadRequest.isDone)
        {
            // Wait until request is complete
        }

        // Check if upload was successful
        if (uploadRequest.responseCode == 200)
        {
            // Check if recording exists on server
            string getRecordingsUrl = "http://localhost:5000/recordings";
            UnityWebRequest getRecordingsRequest = UnityWebRequest.Get(getRecordingsUrl);
            getRecordingsRequest.SendWebRequest();

            // Wait for request to complete
            while (!getRecordingsRequest.isDone)
            {
                // Wait until request is complete
            }

            // Check if recording exists on server
            if (getRecordingsRequest.responseCode == 200 && getRecordingsRequest.downloadHandler.text.Contains(Path.GetFileName(recordingFilePath)))
            {
                // Delete recording from local directory
                File.Delete(recordingFilePath);
                Debug.Log("Deleted " + recordingFilePath);
            }
            else
            {
                Debug.LogError("Recording upload successful but not found on server: " + recordingFilePath);
            }
        }
        else
        {
            Debug.LogError("Failed to upload recording: " + recordingFilePath);
        }
    }
}
