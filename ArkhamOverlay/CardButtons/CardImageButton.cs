﻿using ArkhamOverlay.Common.Services;
using ArkhamOverlay.Common.Utils;
using ArkhamOverlay.Data;
using ArkhamOverlay.Events;
using System.Windows.Media;

namespace ArkhamOverlay.CardButtons {
    public abstract class CardImageButton : Button {
        private readonly IEventBus _eventBus = ServiceLocator.GetService<IEventBus>();

        public CardImageButton(CardTemplate cardTemplate) {
            CardTemplate = cardTemplate;
            Text = cardTemplate.Name;
            IsToggled = cardTemplate.IsDisplayedOnOverlay;
        }

        public CardTemplate CardTemplate { get; }

        public override ImageSource ButtonImage { get { return CardTemplate.ButtonImage; } }

        public override void LeftClick() {
            _eventBus.PublishToggleCardVisibilityRequest(CardTemplate);
        }
    }
}
