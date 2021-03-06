﻿using Emo.Common.Enums;
using Emo.Common.Utils;

namespace Emo.Common.Tcp.Requests {
    public class ButtonImageRequest : Request {
        public ButtonImageRequest() : base(AoTcpRequest.GetButtonImage) {
        }

        public CardGroupId CardGroupId { get; set; }
        public ButtonMode? ButtonMode { get; set;  }
        public int? ZoneIndex { get; set; }
        public int? Index { get; set; }
    }
}
