﻿
using BeyondTools.VFS.Crypto;
using EndFieldPS.Commands;
using EndFieldPS.Network;
using EndFieldPS.Protocol;
using EndFieldPS.Resource;
using Google.Protobuf;
using Newtonsoft.Json;
using Pastel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.Http;
using static EndFieldPS.Dispatch;


namespace EndFieldPS
{
    public class Server
    {
        public class HandlerAttribute : Attribute
        {
            public CsMessageId CmdId { get; set; }
            public HandlerAttribute(CsMessageId cmdID)
            {
                this.CmdId = cmdID;
            }
            public delegate void HandlerDelegate(Player client, int cmdId, Network.Packet packet);
        }
        public class CommandAttribute : Attribute
        {
            public string command;
            public string desc;

            public CommandAttribute(string cmdName, string desc = "No description")
            {
                this.command = cmdName;
                this.desc = desc;
            }
            public delegate void HandlerDelegate(string command, string[] args);
        }
        public static List<Player> clients = new List<Player>();
        public static string ServerVersion = "1.0.1";
        public static bool Initialized = false;
        public static bool showLogs = true;
        public static SQLiteConnection _db;
        public static Dispatch dispatch;
        public static ResourceManager resourceManager;
        public static ConfigFile config;

      
   
        public static ResourceManager GetResources()
        {
            return resourceManager;
        }
        public void Start(bool hideLogs = false, ConfigFile config = null)
        {
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Type[] types = assembly.GetTypes();

                foreach (var type in types)
                {

                    NotifyManager.AddReqGroupHandler(type);
                    CommandManager.AddReqGroupHandler(type);
                }

                NotifyManager.Init();
                CommandManager.Init();
            }
            
            Logger.Initialize(); // can also pass hideLogs here
            showLogs = !hideLogs;
            // showLogs = false;
            Print($"Logs are {(showLogs ? "enabled" : "disabled")}");
            Server.config = config;
            ResourceManager.Init();
            new Thread(new ThreadStart(DispatchServer)).Start();
            
            IPAddress ipAddress = IPAddress.Parse(config.ServerIp);
            int port = config.LocalPort;
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            new Thread(new ThreadStart(CmdListener)).Start();
            
            try
            {
                serverSocket.Bind(new IPEndPoint(ipAddress, port));
                serverSocket.Listen(config.MaxClients);
                Print($"Server listening on {ipAddress}:{port}");
                Initialized = true;
                while (true)
                {
                    Socket clientSocket = serverSocket.Accept();
                    
                    if (clientSocket.Connected)
                    {
                        Player client = new Player(clientSocket);
                        clients.Add(client);
                        client.receivorThread.Start();

                        Print("Connected new client: " + clients.Count());
                    }


                }
            }
            catch (Exception ex)
            {
                Print($"Error: {ex.Message}");
            }
            finally
            {
                // Arresta il server
                serverSocket.Close();
                Print("Server stopped.");
            }

        }
        public void CmdListener()
        {
            while (true)
            {
                try
                {
                    string cmd = Console.ReadLine()!;
                    string[] split = cmd.Split(" ");
                    string[] args = cmd.Split(" ").Skip(1).ToArray();
                    string command = split[0].ToLower();
                    CommandManager.Notify(command, args);
                }
                catch (Exception ex)
                {
                    Print(ex.Message);
                }

            } 

        }
        public void DispatchServer()
        {
            dispatch = new Dispatch();
            dispatch.Start();
        }
        public static CsMessageId[] hideLog = [];


        public static uint GetCurrentSeconds()
        {
            return (uint)DateTime.Now.Millisecond / 1000;
        }
       
        public static string ColoredText(string text, string color)
        {
            return text.Pastel(color);
        }
        public static void Print(string text)
        {
            Logger.Log(text);
            Console.WriteLine($"[{ColoredText("Server", "03fcce")}] " + text);
        }
        public static void Shutdown()
        {
            //TODO
        }
    }
}
