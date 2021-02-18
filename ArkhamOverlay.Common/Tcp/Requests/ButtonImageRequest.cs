﻿using ArkhamOverlay.Common.Enums;
using ArkhamOverlay.Common.Utils;

namespace ArkhamOverlay.Common.Tcp.Requests {
    public class ButtonImageRequest : Request, IButtonContext {
        public ButtonImageRequest() : base(AoTcpRequest.GetButtonImage) {
        }

        public CardGroupId CardGroupId { get; set; }
        public ButtonMode ButtonMode { get; set;  }
        public int Index { get; set; }
    }
}
