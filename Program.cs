using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FileServer
{
    class Program
    {
        static void Main(string[] args)
        {
            const int port = 5000;
            string baseDirectory = @"C:\FileServer";

            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }

            Console.WriteLine("Starting File Server...");
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine("Server listening on port " + port.ToString());

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected.");

                using (NetworkStream stream = client.GetStream())
                {
                    // Read request
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string requestedFile = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    string filePath = Path.Combine(baseDirectory, requestedFile);

                    if (File.Exists(filePath))
                    {
                        byte[] fileBytes = File.ReadAllBytes(filePath);
                        stream.Write(fileBytes, 0, fileBytes.Length);
                        Console.WriteLine("File " + requestedFile + " sent.");
                    }
                    else
                    {
                        string errorMessage = "File not found";
                        byte[] errorBytes = Encoding.UTF8.GetBytes(errorMessage);
                        stream.Write(errorBytes, 0, errorBytes.Length);
                        Console.WriteLine("File" + requestedFile + " not found.");
                    }
                }

                client.Close();
            }
        }
    }
}
