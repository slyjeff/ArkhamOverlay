﻿using ArkhamOverlay.Common;
using ArkhamOverlay.Common.Enums;
using ArkhamOverlay.Common.Services;
using ArkhamOverlay.Events;
using StreamDeckPlugin.Events;
using System.Collections.Generic;
using System.Linq;

namespace StreamDeckPlugin.Services {
    /// <summary>
    /// Stores information foreach card group
    /// </summary>
    public interface ICardGroupStore {
        /// <summary>
        /// Retrieve Card group Info from the store
        /// </summary>
        /// <param name="cardGroupId">Id of the card group info to retrieve</param>
        /// <returns>Card group info</returns>
        ICardGroupInfo GetCardGroupInfo(CardGroupId cardGroupId);
    }

    /// <summary>
    /// Stores information for each card group
    /// </summary>
    public class CardGroupStore : ICardGroupStore {
        private readonly object _cacheLock = new object();
        private readonly IDictionary<CardGroupId, ICardGroupInfo> _cardGroupInfoCache = new Dictionary<CardGroupId, ICardGroupInfo>();

        private readonly IEventBus _eventBus;
        private readonly IImageService _imageService;

        public CardGroupStore(IEventBus eventBus, IImageService imageService) {
            _eventBus = eventBus;
            _imageService = imageService;
            eventBus.SubscribeToImageLoadedEvent(e => ImageLoaded(e.ImageId));
            _eventBus.SubscribeToCardGroupChanged(CardGroupChangedHandler);
        }

        /// <summary>
        /// Retrieve Card group Info from the store
        /// </summary>
        /// <param name="cardGroupId">Id of the card group info to retrieve</param>
        /// <returns>Card group info</returns>
        public ICardGroupInfo GetCardGroupInfo(CardGroupId cardGroupId) {
            if (!_cardGroupInfoCache.ContainsKey(cardGroupId)) {
                return default;
            }

            return _cardGroupInfoCache[cardGroupId];
        }

        /// <summary>
        /// Respond to a Card Group changing by updating the store
        /// </summary>
        /// <param name="eventData">Info about the card group</param>
        private void CardGroupChangedHandler(CardGroupChanged eventData) {
            lock (_cacheLock) {
                _cardGroupInfoCache[eventData.CardGroupId] = eventData;
            }

            if (eventData.IsImageAvailable) {
                _imageService.LoadImage(eventData.ImageId, eventData.CardGroupId);
            }
            _eventBus.PublishStreamDeckCardGroupInfoChanged(eventData);
        }

        /// <summary>
        /// Let anyone know who is listening to card group changes that any with this image id has changed
        /// </summary>
        /// <param name="imageId">Publish event for all card groups with this image id</param>
        private void ImageLoaded(string imageId) {
            var cardGroupsForImage = from cardGroupInfo in _cardGroupInfoCache.Values
                                     where cardGroupInfo.ImageId == imageId
                                     select cardGroupInfo;

            foreach (var cardGroupInfo in cardGroupsForImage) {
                _eventBus.PublishStreamDeckCardGroupInfoChanged(cardGroupInfo);
            }
        }
    }
}
