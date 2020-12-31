﻿using ArkhamOverlay.Data;
using ArkhamOverlay.TcpUtils;
using ArkhamOverlay.TcpUtils.Requests;
using ArkhamOverlay.TcpUtils.Responses;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;

namespace ArkhamOverlay.Services {
    internal class RequestHandler {
        private readonly AppData _appData;

        public RequestHandler(AppData appData) {
            _appData = appData;
        }

        public void HandleRequest(TcpRequest request) {
            Console.WriteLine("Handling Request: " + request.RequestType.AsString());
            if (request.RequestType == AoTcpRequest.GetCardInfo) {
                HandleGetCardInfo(request);
            }

            if (request.RequestType == AoTcpRequest.ClickCardButton) {
                HandleClick(request);
            }
        }

        private void HandleGetCardInfo(TcpRequest request) {
            var getCardInfoRequest = JsonConvert.DeserializeObject<GetCardInfoRequest>(request.Body);
            var cardIndex = getCardInfoRequest.Index;

            var cards = _appData.Game.Players[0].SelectableCards.CardButtons;

            var cardButton = (cardIndex < cards.Count) ? cards[cardIndex] : null;
            SendCardInfoResponse(request.Socket, cardButton);
        }

        private void HandleClick(TcpRequest request) {
            var clickCardButtonRequest = JsonConvert.DeserializeObject<ClickCardButtonRequest>(request.Body);
            var cardIndex = clickCardButtonRequest.Index;

            var cards = _appData.Game.Players[0].SelectableCards.CardButtons;

            var cardButton = (cardIndex < cards.Count) ? cards[cardIndex] : null;
            cardButton.Click();

            SendCardInfoResponse(request.Socket, cardButton);
        }

        private void  SendCardInfoResponse(Socket socket, ICardButton cardButton) {
            var cardInfoReponse = (cardButton == null)
                ? new CardInfoReponse { CardButtonType = CardButtonType.Unknown, Name = "" }
                : new CardInfoReponse { CardButtonType = GetCardType(cardButton as Card), Name = cardButton.Name };

            Send(socket, cardInfoReponse.ToString());
        }

        private static CardButtonType GetCardType(Card card) {
            if (card == null) {
                return CardButtonType.Action;
            }

            switch (card.Type) {
                case CardType.Scenario:
                    return TcpUtils.CardButtonType.Scenario;
                case CardType.Agenda:
                    return TcpUtils.CardButtonType.Agenda;
                case CardType.Act:
                    return TcpUtils.CardButtonType.Act;
                case CardType.Location:
                    return TcpUtils.CardButtonType.Location;
                case CardType.Enemy:
                    return TcpUtils.CardButtonType.Enemy;
                case CardType.Treachery:
                    return TcpUtils.CardButtonType.Treachery;
                case CardType.Player:
                    switch (card.Faction) {
                        case Faction.Guardian:
                            return TcpUtils.CardButtonType.Guardian;
                        case Faction.Seeker:
                            return TcpUtils.CardButtonType.Seeker;
                        case Faction.Rogue:
                            return TcpUtils.CardButtonType.Rogue;
                        case Faction.Survivor:
                            return TcpUtils.CardButtonType.Survivor;
                        case Faction.Mystic:
                            return TcpUtils.CardButtonType.Mystic;
                        default:
                            return TcpUtils.CardButtonType.Unknown;
                    }
                default:
                    return TcpUtils.CardButtonType.Unknown;
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

    }
}
