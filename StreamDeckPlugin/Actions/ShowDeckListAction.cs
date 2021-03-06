﻿using Emo.Common;
using Emo.Common.Enums;
using Emo.Common.Events;
using Emo.Common.Services;
using Emo.Common.Utils;
using Newtonsoft.Json.Linq;
using SharpDeck;
using SharpDeck.Events.Received;
using SharpDeck.Manifest;
using StreamDeckPlugin.Events;
using StreamDeckPlugin.Services;
using System.Threading.Tasks;

namespace StreamDeckPlugin.Actions {
    public class ShowDeckSettings {
        public string Deck { get; set; }
    }

    [StreamDeckAction("Show Deck List", "emo.showdecklist")]
    public class ShowDeckListAction : StreamDeckAction {
        private readonly IEventBus _eventBus = ServiceLocator.GetService<IEventBus>();
        private ActionWithDeckSettings _settings = new ActionWithDeckSettings();
        private readonly ICardGroupStore _cardGroupStore = ServiceLocator.GetService<ICardGroupStore>();
        private readonly IImageService _imageService = ServiceLocator.GetService<IImageService>();

        public CardGroupId CardGroupId {
            get {
                if (_settings == null) {
                    return CardGroupId.Player1;
                }

                return _settings.Deck.AsCardGroupId();
            }
        }

        public ShowDeckListAction() {
            _eventBus.SubscribeToStreamDeckCardGroupInfoChanged(CardGroupInfoChangedHandler);
        }

        protected override Task OnWillAppear(ActionEventArgs<AppearancePayload> args) {
            _settings = args.Payload.GetSettings<ActionWithDeckSettings>();

            UpdateImage();

            return Task.CompletedTask;
        }

        protected override Task OnKeyUp(ActionEventArgs<KeyPayload> args) {
            _eventBus.PublishShowDeckListRequest(CardGroupId);
            return Task.CompletedTask;
        }

        protected async override Task OnSendToPlugin(ActionEventArgs<JObject> args) {
            _settings.Deck = args.Payload["deck"].Value<string>();

            UpdateImage();

            await SetSettingsAsync(_settings);
        }

        private void CardGroupInfoChangedHandler(CardGroupInfoChanged eventData) {
            if (eventData.CardGroupInfo.CardGroupId != CardGroupId) {
                return;
            }

            UpdateImage();
        }

        private void UpdateImage() {
            var cardGroupInfo = _cardGroupStore.GetCardGroupInfo(CardGroupId);
            if (cardGroupInfo == default(ICardGroupInfo)) {
                SetImageAsync(null);
                return;
            }

            SetImageAsync(_imageService.GetImage(cardGroupInfo.ImageId));
        }
    }
}