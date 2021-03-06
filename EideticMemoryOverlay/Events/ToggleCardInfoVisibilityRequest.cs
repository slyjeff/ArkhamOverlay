﻿using EideticMemoryOverlay.PluginApi;
using Emo.Common.Services;
using System;

namespace Emo.Events {
    public class ToggleCardInfoVisibilityRequest : IEvent {
        public ToggleCardInfoVisibilityRequest(CardInfo cardInfo) {
            CardInfo = cardInfo;
        }

        public CardInfo CardInfo { get; }
    }

    public static class ToggleCardInfoVisibilityRequestExtensions {
        public static void PublishToggleCardInfoVisibilityRequest(this IEventBus eventBus, CardInfo cardInfo) {
            eventBus.Publish(new ToggleCardInfoVisibilityRequest(cardInfo));
        }

        public static void SubscribeToToggleCardInfoVisibilityRequest(this IEventBus eventBus, Action<ToggleCardInfoVisibilityRequest> callback) {
            eventBus.Subscribe(callback);
        }

        public static void UnsubscribeFromToggleCardInfoVisibilityRequest(this IEventBus eventBus, Action<ToggleCardInfoVisibilityRequest> callback) {
            eventBus.Unsubscribe(callback);
        }
    }
}
