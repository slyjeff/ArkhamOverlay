﻿using Emo.Common;
using Emo.Common.Enums;
using Emo.Common.Services;
using Emo.Common.Utils;
using System;
using System.Collections.Generic;

namespace Emo.Events {
    public enum ChangeAction { Add, Update }

    public class ButtonInfoChanged : ICrossAppEvent, IButtonContext, ICardInfo {
        public ButtonInfoChanged(CardGroupId cardGroupId, ButtonMode buttonMode, int zoneIndex, int index, string name, string code, bool isToggled, bool imageAvailable, ChangeAction action, IList<ButtonOption> buttonOptions) {
            CardGroupId = cardGroupId;
            ButtonMode = buttonMode;
            ZoneIndex = zoneIndex;
            Index = index;
            Name = name;
            Code = code;
            IsToggled = isToggled;
            ImageAvailable = imageAvailable;
            Action = action;
            ButtonOptions = buttonOptions;
        }

        public CardGroupId CardGroupId { get; }
        public ButtonMode ButtonMode { get; }
        public int ZoneIndex { get; }
        public int Index { get; }
        public string Name { get; }
        public string Code { get; }
        public bool IsToggled { get; }
        public bool ImageAvailable { get; }
        public ChangeAction Action { get; }
        public IList<ButtonOption> ButtonOptions { get; }
    }

    public static class ButtonInfoChangedExtensions {
        public static void PublishButtonInfoChanged(this IEventBus eventBus, CardGroupId cardGroupId, ButtonMode buttonMode, int zoneIndex, int index, string name, bool isToggled, ChangeAction action) {
            eventBus.Publish(new ButtonInfoChanged(cardGroupId, buttonMode, zoneIndex, index, name, string.Empty, isToggled, false, action, new List<ButtonOption>()));
        }

        public static void PublishButtonInfoChanged(this IEventBus eventBus, CardGroupId cardGroupId, ButtonMode buttonMode, int zoneIndex, int index, string name, string code, bool isToggled, bool imageAvailable, ChangeAction action, IList<ButtonOption> buttonOptions) {
            eventBus.Publish(new ButtonInfoChanged(cardGroupId, buttonMode, zoneIndex, index, name, code, isToggled, imageAvailable, action, buttonOptions));
        }

        public static void SubscribeToButtonInfoChanged(this IEventBus eventBus, Action<ButtonInfoChanged> callback) {
            eventBus.Subscribe(callback);
        }
        public static void UnsubscribeFromButtonInfoChanged(this IEventBus eventBus, Action<ButtonInfoChanged> callback) {
            eventBus.Unsubscribe(callback);
        }
    }
}
