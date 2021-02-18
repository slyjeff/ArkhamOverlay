﻿using ArkhamOverlay.Common.Enums;

namespace ArkhamOverlay.Common.Tcp.Requests {
    public class UpdateInvestigatorImageRequest : Request {
        public UpdateInvestigatorImageRequest() : base(AoTcpRequest.UpdateInvestigatorImage) {
        }

        public CardGroupId CardGroup { get; set; }
        public byte[] Bytes { get; set; }
    }
}
