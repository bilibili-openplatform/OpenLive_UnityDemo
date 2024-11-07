using System;
using System.Threading.Tasks;
using OpenBLive.Runtime.Data;

#if NET5_0_OR_GREATER
using Websocket.Client;
using System.Net.WebSockets;
#elif UNITY_2020_3_OR_NEWER
using NativeWebSocket;
using OpenBLive.Runtime.Utilities;
using System.Linq;
using System.Collections.Generic;

#endif

namespace OpenBLive.Runtime
{
    public class WebSocketBLiveClient : BLiveClient
    {

        /// <summary>
        ///  wss 长连地址
        /// </summary>
        public IList<string> WssLink;

#if NET5_0_OR_GREATER
        public WebsocketClient clientWebSocket;
#elif UNITY_2020_3_OR_NEWER
        public WebSocket ws;
#endif

        public WebSocketBLiveClient(AppStartInfo info)
        {
            var websocketInfo = info.Data.WebsocketInfo;

            WssLink = websocketInfo.WssLink;
            token = websocketInfo.AuthBody;
        }


        public WebSocketBLiveClient(IList<string> wssLink, string authBody)
        {
            WssLink = wssLink;
            token = authBody;
        }



        public override async void Connect()
        {
            var url = WssLink.FirstOrDefault();
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("wsslink is invalid");
            }
#if NET5_0_OR_GREATER
            clientWebSocket?.Stop(WebSocketCloseStatus.Empty, string.Empty);
            clientWebSocket?.Dispose();

            clientWebSocket = new WebsocketClient(new Uri(url));
            clientWebSocket.MessageReceived.Subscribe(e => ProcessPacket(e.Binary));
            clientWebSocket.DisconnectionHappened.Subscribe(e =>
            {
                if (e.CloseStatus == WebSocketCloseStatus.Empty)
                    Console.WriteLine("WS CLOSED");
                else
                    Console.WriteLine("WS ERROR: " + e.Exception.Message);
            });
            await clientWebSocket.Start();
            if (clientWebSocket.IsStarted)
                OnOpen();
#elif UNITY_2020_3_OR_NEWER
            //尝试释放已连接的ws
            if (ws != null && ws.State != WebSocketState.Closed)
            {
                await ws.Close();
            }

            ws = new WebSocket(url);
            ws.OnOpen += OnOpen;
            ws.OnMessage += data =>
            {
                ProcessPacket(data);
            };
            ws.OnError += msg => { Logger.LogError("WebSocket Error Message: " + msg); };
            ws.OnClose += code => { Logger.Log("WebSocket Close: " + code); };
            await ws.Connect();
#endif
        }



        public override async void Connect(TimeSpan timeSpan, int count)
        {
            var url = WssLink.FirstOrDefault();
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("wsslink is invalid");
            }
#if NET5_0_OR_GREATER
            clientWebSocket?.Stop(WebSocketCloseStatus.Empty, string.Empty);
            clientWebSocket?.Dispose();

            clientWebSocket = new WebsocketClient(new Uri(url));
            clientWebSocket.MessageReceived.Subscribe(e => ProcessPacket(e.Binary));
            clientWebSocket.DisconnectionHappened.Subscribe(e =>
            {
                if (e.CloseStatus == WebSocketCloseStatus.Empty)
                    Console.WriteLine("WS CLOSED");
                else
                    Console.WriteLine("WS ERROR: " + e.Exception.Message);
            });
            await clientWebSocket.Start();
            if (clientWebSocket.IsStarted)
                OnOpen();
#elif UNITY_2020_3_OR_NEWER
            //尝试释放已连接的ws
            if (ws != null && ws.State != WebSocketState.Closed)
            {
                await ws.Close();
            }

            ws = new WebSocket(url);
            ws.OnOpen += OnOpen;
            ws.OnMessage += data =>
            {
                ProcessPacket(data);
            };
            ws.OnError += msg => { Logger.LogError("WebSocket Error Message: " + msg); };
            ws.OnClose += code => { Logger.Log("WebSocket Close: " + code); };
            await ws.Connect(timeSpan, count);
#endif
        }


        public override void Disconnect()
        {
#if NET5_0_OR_GREATER
            clientWebSocket?.Stop(WebSocketCloseStatus.Empty, string.Empty);
            clientWebSocket?.Dispose();
#elif UNITY_2020_3_OR_NEWER
            ws?.Close();
            ws = null;
#endif
        }

        public override void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }

        public override void Send(byte[] packet)
        {
#if NET5_0_OR_GREATER
            clientWebSocket?.Send(packet);
#elif UNITY_2020_3_OR_NEWER
            if (ws.State == WebSocketState.Open)
            {
                ws.Send(packet);
            }
#endif
        }


        public override void Send(Packet packet) => Send(packet.ToBytes);
        public override Task SendAsync(byte[] packet) => Task.Run(() => Send(packet));
        protected override Task SendAsync(Packet packet) => SendAsync(packet.ToBytes);
    }
}