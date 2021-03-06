﻿using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using Emo.Common.Tcp;
using Emo.Common.Tcp.Requests;
using Emo.Common.Tcp.Responses;
using Emo.Common.Services;

namespace StreamDeckPlugin.Utils {
    public interface ITcpRequestHandler : IRequestHandler {
        bool RequestReceivedRecently { get; set; }
    }

    public class TcpRequestHandler : ITcpRequestHandler {
        private readonly ICrossAppEventBus _crossAppEventBus;

        public TcpRequestHandler(ICrossAppEventBus crossAppEventBus) {
            _crossAppEventBus = crossAppEventBus;
        }

        public bool RequestReceivedRecently { get; set; } 

        public void HandleRequest(TcpRequest request) {
            RequestReceivedRecently = true;

            Console.WriteLine("Handling Request: " + request.RequestType.ToString());
            switch (request.RequestType) {
                case AoTcpRequest.ConnectionIsAlive:
                    HandleConnectionIsAliveRequest(request);
                    break;
                case AoTcpRequest.EventBus:
                    HandleEventBusRequest(request);
                    break;
            }
        }

        private static void Send(Socket socket, string data) {
            var byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), socket);
        }

        private static void SendCallback(IAsyncResult ar) {
            try {
                var socket = (Socket)ar.AsyncState;

                var bytesSent = socket.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
        private void HandleConnectionIsAliveRequest(TcpRequest request) {
            Send(request.Socket, new OkResponse().ToString());
        }

        private void HandleEventBusRequest(TcpRequest request) {
            Send(request.Socket, new OkResponse().ToString());

            var eventBusRequest = JsonConvert.DeserializeObject<EventBusRequest>(request.Body);
            _crossAppEventBus.ReceiveMessage(eventBusRequest);
        }
    }
}
