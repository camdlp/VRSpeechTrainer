using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HttpServer : MonoBehaviour
{
    private HttpListener listener;

    void Start()
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://" + GetLocalIPAddress() + ":8080/");
        listener.Start();
        Debug.Log("HTTP server started at " + GetLocalIPAddress() + ":8080");

        Task.Run(async () =>
        {
            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();

                // Handle request
                string responseString = "Hello, world!";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.Close();

                // Show message in console
                Debug.Log("Received request from " + context.Request.RemoteEndPoint);
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
}

