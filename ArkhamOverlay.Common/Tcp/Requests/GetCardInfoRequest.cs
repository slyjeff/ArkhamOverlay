﻿using ArkhamOverlay.Common.Enums;

namespace ArkhamOverlay.Common.Tcp.Requests {
    public class GetCardInfoRequest : Request {
        public GetCardInfoRequest() : base(AoTcpRequest.GetCardInfo) {
        }

        public Deck Deck { get; set; }
        public int Index { get; set; }
        public bool FromCardSet { get; set; }
    }
}
