﻿using EideticMemoryOverlay.PluginApi.Buttons;
using Emo.Common.Enums;
using Emo.Common.Utils;

namespace ArkhamHorrorLcg {
    internal class ArkhamCardInfoButton : CardInfoButton {
        internal ArkhamCardInfoButton(ArkhamCardInfo cardInfo, CardGroupId cardGroupId) : base(cardInfo) {
            if (cardInfo.IsPlayerCard) {
                Options.Add(new ButtonOption(ButtonOptionOperation.Add, cardGroupId, zoneIndex: 0));
            }

            if (cardInfo.Type == CardType.Act || cardInfo.Type == CardType.Agenda) {
                Options.Add(new ButtonOption(ButtonOptionOperation.Add, cardGroupId, zoneIndex: 0));
            }

            if (!cardInfo.IsPlayerCard && cardInfo.IsHidden) {
                Options.Add(new ButtonOption(ButtonOptionOperation.Add, CardGroupId.Player1, zoneIndex: 0));
                Options.Add(new ButtonOption(ButtonOptionOperation.Add, CardGroupId.Player2, zoneIndex: 0));
                Options.Add(new ButtonOption(ButtonOptionOperation.Add, CardGroupId.Player3, zoneIndex: 0));
                Options.Add(new ButtonOption(ButtonOptionOperation.Add, CardGroupId.Player4, zoneIndex: 0));
            }

            if (!cardInfo.IsHidden && cardInfo.Type == CardType.Treachery || cardInfo.Type == CardType.Enemy || cardInfo.Type == CardType.Location) {
                Options.Add(new ButtonOption(ButtonOptionOperation.Add, CardGroupId.Player1, zoneIndex: 1));
                Options.Add(new ButtonOption(ButtonOptionOperation.Add, CardGroupId.Player2, zoneIndex: 1));
                Options.Add(new ButtonOption(ButtonOptionOperation.Add, CardGroupId.Player3, zoneIndex: 1));
                Options.Add(new ButtonOption(ButtonOptionOperation.Add, CardGroupId.Player4, zoneIndex: 1));
            }
        }
    }
}
