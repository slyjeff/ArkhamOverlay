﻿using Emo.Common.Enums;
using SharpDeck;
using SharpDeck.Manifest;

namespace StreamDeckPlugin.Actions {

    [StreamDeckAction("Track Resources", "emo.trackresources")]
    public class TrackResourcesAction : TrackStatAction {
        public TrackResourcesAction() : base(StatType.Resources) {

        }
    }
}