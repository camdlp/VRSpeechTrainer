using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HttpServer : MonoBehaviour
{
    private HttpListener listener;
    public string[] folders;
    private string indexHtmlContent;
    private string dataPath;


    void Start()
    {
        dataPath = Application.dataPath;
        TextAsset htmlAsset = Resources.Load<TextAsset>("index");
        if (htmlAsset != null)
        {
            indexHtmlContent = htmlAsset.text;
        }
        else
        {
            Debug.LogError("Failed to load index.html");
            return;
        }

        listener = new HttpListener();
        listener.Prefixes.Add("http://" + GetLocalIPAddress() + ":8080/");
        listener.Start();
        Debug.Log("HTTP server started at " + GetLocalIPAddress() + ":8080");

        Task.Run(async () =>
        {
            try
            {
                while (true)
                {
                    HttpListenerContext context = await listener.GetContextAsync();

                    if (context.Request.Url.AbsolutePath == "/api/list")
                    {
                        Debug.Log("Request: " + context.Request.Url.AbsolutePath);
                        HandleApiRequest(context, folders);
                    }
                    else if (context.Request.Url.AbsolutePath.StartsWith("/api/upload"))
                    {
                        string folderPath = context.Request.Url.AbsolutePath.Substring(11); // Excluye "/api/upload"
                        HandleUploadRequest(context, dataPath, folderPath);
                    }
                    else if (context.Request.Url.AbsolutePath == "/favicon.ico")
                    {
                        context.Response.StatusCode = 404;
                        context.Response.Close();
                    }
                    else
                    {
                        if (context.Request.Url.AbsolutePath == "/")
                        {
                            ServeHtmlPage(context, indexHtmlContent);
                        }
                        else
                        {
                            context.Response.StatusCode = 404;
                            context.Response.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error in HTTP server: " + e.Message);
            }
        });
    }



    void OnDestroy()
    {
        listener.Stop();
        Debug.Log("HTTP server stopped");
    }

    private string GetLocalIPAddress()
    {
        string localIP = "";
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("10.0.2.4", 65530); // Esta dirección IP es la dirección IP de la interfaz de bucle invertido de Android
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            localIP = endPoint.Address.ToString();
        }
        return localIP;
    }

    private void HandleApiRequest(HttpListenerContext context, string[] folders)
    {
        string jsonResponse = ListFoldersAsJson(folders);
        Debug.Log("JSON Response: " + jsonResponse); // Agrega esta línea
        byte[] buffer = Encoding.UTF8.GetBytes(jsonResponse);
        context.Response.ContentType = "application/json";
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.Close();
    }


    private string ListFoldersAsJson(string[] folders)
    {
        var foldersInfo = new List<Dictionary<string, object>>();
        string playerDataPath = Path.Combine(Application.dataPath, "PlayerData");

        foreach (string folder in folders)
        {
            string absoluteFolderPath = Path.Combine(playerDataPath, folder);

            if (Directory.Exists(absoluteFolderPath))
            {
                var folderInfo = new Dictionary<string, object>();
                folderInfo["path"] = absoluteFolderPath;
                folderInfo["files"] = Directory.GetFiles(absoluteFolderPath);
                folderInfo["directories"] = Directory.GetDirectories(absoluteFolderPath);
                foldersInfo.Add(folderInfo);
            }
        }

        return JsonUtility.ToJson(new { folders = foldersInfo });
    }


    private void ServeHtmlPage(HttpListenerContext context, string htmlContent)
    {
        Debug.Log("HTML Content: " + htmlContent); // Agrega esta línea
        byte[] buffer = Encoding.UTF8.GetBytes(htmlContent);
        context.Response.ContentType = "text/html";
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
    }

    private async void HandleUploadRequest(HttpListenerContext context, string dataPath, string folderPath)
    {
        if (context.Request.HttpMethod != "POST")
        {
            context.Response.StatusCode = 400;
            context.Response.Close();
            return;
        }

        string boundary = null;
        var contentType = context.Request.ContentType;

        if (!string.IsNullOrEmpty(contentType) && contentType.Contains("boundary="))
        {
            int boundaryIndex = contentType.IndexOf("boundary=") + "boundary=".Length;
            boundary = contentType.Substring(boundaryIndex);
        }

        if (string.IsNullOrEmpty(boundary))
        {
            context.Response.StatusCode = 400;
            context.Response.Close();
            return;
        }
        Debug.Log("Boundary: " + boundary); // comment

        string tempFolderPath = Path.Combine(dataPath, "PlayerData", folderPath);
        Directory.CreateDirectory(tempFolderPath);
        string tempFilePath = Path.Combine(tempFolderPath, Guid.NewGuid().ToString());

        using (var fileStream = File.Create(tempFilePath))
        {
            await context.Request.InputStream.CopyToAsync(fileStream);
        }

        context.Response.StatusCode = 200;
        context.Response.Close();
    }




#if UNITY_EDITOR
    private void OnValidate()
    {
        TextAsset htmlAsset = Resources.Load<TextAsset>("index");
        if (htmlAsset != null)
        {
            indexHtmlContent = htmlAsset.text;
        }
        else
        {
            Debug.LogError("Failed to load index.html");
        }
    }
#endif

}

