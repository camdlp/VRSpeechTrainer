using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using RockVR.Video;

public class UploadRecordings : MonoBehaviour
{
    void Start()
    {
        // Define the directory where recordings are stored
        string recordingsDirectory = Application.dataPath + "/PlayerData/Recordings";

        // Setup a FileSystemWatcher to monitor for new files in the directory
        FileSystemWatcher watcher = new FileSystemWatcher(recordingsDirectory);
        watcher.Filter = "*.mp4";  // Only watch for .mp4 files
        watcher.Created += OnFileCreated;  // Add event handler for when a file is created
        watcher.EnableRaisingEvents = true;
    }

    // Event handler called when a new file is created
    void OnFileCreated(object source, FileSystemEventArgs e)
    {
        if (Path.GetExtension(e.FullPath) == ".mp4")
        {
            Debug.Log("Detected new recording: " + e.FullPath);
            UploadRecordingToServer(e.FullPath);  // Upload the new recording to the server
        }
    }

    // Method to handle uploading a recording file to the server
    public void UploadRecordingToServer(string recordingFilePath)
    {
        Debug.Log("Instance video status" + VideoCaptureCtrl.instance.status);
        string uploadUrl = "http://localhost:5000/api/upload";

        // Create a UnityWebRequest for the upload
        UnityWebRequest uploadRequest = UnityWebRequest.Post(uploadUrl, "");
        uploadRequest.uploadHandler = new UploadHandlerFile(recordingFilePath);  // Use an UploadHandlerFile to send the file
        uploadRequest.SendWebRequest();

        Debug.Log("Uploading " + recordingFilePath);

        // Wait for upload to complete
        while (!uploadRequest.isDone) { }

        // Check if upload was successful
        if (uploadRequest.responseCode == 200)
        {
            // Check if recording exists on server
            string getRecordingsUrl = "http://localhost:5000/recordings";
            UnityWebRequest getRecordingsRequest = UnityWebRequest.Get(getRecordingsUrl);
            getRecordingsRequest.SendWebRequest();

            // Wait for request to complete
            while (!getRecordingsRequest.isDone) { }

            // If the recording was successfully uploaded and found on the server, delete the local file
            if (getRecordingsRequest.responseCode == 200 && getRecordingsRequest.downloadHandler.text.Contains(Path.GetFileName(recordingFilePath)))
            {
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
