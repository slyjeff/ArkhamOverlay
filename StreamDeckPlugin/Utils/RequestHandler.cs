﻿using ArkhamOverlay.TcpUtils;
using ArkhamOverlay.TcpUtils.Requests;
using ArkhamOverlay.TcpUtils.Responses;
using ArkhamOverlaySdPlugin.Actions;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;

namespace StreamDeckPlugin.Utils {
    class TcpRequestHandler : IRequestHandler {
        public void HandleRequest(TcpRequest request) {
            Console.WriteLine("Handling Request: " + request.RequestType.AsString());
            switch (request.RequestType) {
                case AoTcpRequest.UpdateCardInfo:
                    UpdateCardInfo(request);
                    break;
            }
        }

        private void UpdateCardInfo(TcpRequest request) {
            var updateCardInfoRequest = JsonConvert.DeserializeObject<UpdateCardInfoRequest>(request.Body);
            if (updateCardInfoRequest == null) {
                return;
            }

            foreach (var cardButtonAction in CardButtonAction.ListOf) {
                if ((cardButtonAction.Deck == updateCardInfoRequest.Deck) &&
                    (cardButtonAction.CardButtonIndex == updateCardInfoRequest.Index) &&
                    cardButtonAction.IsVisible) {
#pragma warning disable CS4014 
                    cardButtonAction.UpdateButtonInfo(updateCardInfoRequest);
#pragma warning restore CS4014 
                }
            }
            Send(request.Socket, new OkResponse().ToString());
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
    }
}