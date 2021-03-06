﻿using Emo.Common.Enums;
using System.Collections.Generic;

namespace Emo.Common {
    /// <summary>
    /// Information about a Card Group
    /// </summary>
    public interface ICardGroupInfo {
        CardGroupId CardGroupId { get; }
        string Name { get; }
        bool IsImageAvailable { get; }
        string ImageId { get; }
        IList<string> Zones { get; }
    }
}
