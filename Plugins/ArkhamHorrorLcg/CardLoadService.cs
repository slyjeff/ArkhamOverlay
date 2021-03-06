﻿using ArkhamHorrorLcg.ArkhamDb;
using EideticMemoryOverlay.PluginApi;
using EideticMemoryOverlay.PluginApi.Interfaces;
using EideticMemoryOverlay.PluginApi.LocalCards;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace ArkhamHorrorLcg {
    internal interface ICardLoadService {
        void LoadPlayer(ArkhamPlayer player);
        void LoadPlayerCards(ArkhamPlayer player);
        void LoadAllPlayerCards();
        void LoadAllEncounterCards();
    }

    internal class CardLoadService : ICardLoadService {
        private readonly LoadingStatusService _loadingStatusService;
        private readonly IArkhamDbService _arkhamDbService;
        private readonly ILocalCardsService<ArkhamLocalCard> _localCardsService;
        private readonly ICardImageService _cardImageService;
        private readonly IGameData _game;
        private readonly ILoggingService _logger;
        private readonly IArkhamConfiguration _arkhamConfiguration;

        public CardLoadService(IArkhamDbService arkhamDbService, ILocalCardsService<ArkhamLocalCard> localCardsService, ICardImageService cardImageService, IGameData game, LoadingStatusService loadingStatusService, ILoggingService loggingService, IArkhamConfiguration arkhamConfiguration) {
            _arkhamDbService = arkhamDbService;
            _localCardsService = localCardsService;
            _cardImageService = cardImageService;
            _game = game;
            _loadingStatusService = loadingStatusService;
            _logger = loggingService;
            _arkhamConfiguration = arkhamConfiguration;
        }

        public void LoadPlayer(ArkhamPlayer player) {
            if (string.IsNullOrEmpty(player.DeckId)) {
                _logger.LogWarning($"{player.ID} has no deck ID.");
                player.Clear();
                return;
            }

            _logger.LogMessage($"Loading deck for player {player.ID}.");

            var arkhamDbDeck = _arkhamDbService.GetPlayerDeck(player.DeckId);
            player.CardGroup.Name = arkhamDbDeck.Investigator_Name;
            player.InvestigatorCode = arkhamDbDeck.Investigator_Code;
            player.Slots = arkhamDbDeck.Slots;

            _logger.LogMessage($"Loading investigator card for player {player.ID}.");

            var playerCard = _arkhamDbService.GetCard(player.InvestigatorCode);

            player.Health.Max = playerCard.Health;
            player.Sanity.Max = playerCard.Sanity;

            FixArkhamDbCardImageSource(playerCard);
            var localCard = _localCardsService.AllCards().FirstOrDefault(x => x.ArkhamDbId == arkhamDbDeck.Investigator_Code);
            if (localCard != null) {
                player.ImageSource = localCard.FilePath;
            } else {
                player.ImageSource = playerCard.ImageSrc;
            }

            _cardImageService.LoadImage(player);

            if (Enum.TryParse(playerCard.Faction_Name, ignoreCase: true, out Faction faction)) {
                player.Faction = faction;
            } else {
                _logger.LogWarning($"Could not parse faction {playerCard.Faction_Name}.");
            }

            _logger.LogMessage($"Finished loading player {player.ID}.");

            player.OnPlayerChanged();
        }

        public void LoadAllPlayerCards() {
            foreach (var player in _game.Players) {
                var arkhamPlayer = player as ArkhamPlayer;
                if (arkhamPlayer == default) {
                    continue;
                }

                if (!string.IsNullOrEmpty(arkhamPlayer.DeckId)) {
                    _loadingStatusService.ReportPlayerStatus(arkhamPlayer.ID, Status.LoadingCards);
                    try {
                        LoadPlayerCards(arkhamPlayer);

                        _loadingStatusService.ReportPlayerStatus(arkhamPlayer.ID, Status.Finished);
                    } catch (Exception ex) {
                        _logger.LogException(ex, $"Error loading player cards for player {arkhamPlayer.ID}");
                        _loadingStatusService.ReportPlayerStatus(arkhamPlayer.ID, Status.Error);
                    }
                }
            }
        }

        public void LoadAllEncounterCards() {
            _loadingStatusService.ReportEncounterCardsStatus(Status.LoadingCards);
            _logger.LogMessage("Loading encounter cards.");
            try {
                _game.ScenarioCards.Loading = true;
                _game.LocationCards.Loading = true;
                _game.EncounterDeckCards.Loading = true;

                var cards = GetEncounterCards();

                var scenarioCards = new List<CardInfo>();
                var agendas = new List<CardInfo>();
                var acts = new List<CardInfo>();
                var locations = new List<CardInfo>();
                var treacheries = new List<CardInfo>();
                var enemies = new List<CardInfo>();

                foreach (var card in cards) {
                    switch (card.Type) {
                        case CardType.Scenario:
                            scenarioCards.Add(card);
                            break;
                        case CardType.Agenda:
                            agendas.Add(card);
                            break;
                        case CardType.Act:
                            acts.Add(card);
                            break;
                        case CardType.Location:
                            locations.Add(card);
                            break;
                        case CardType.Treachery:
                        case CardType.Enemy:
                            treacheries.Add(card);
                            break;
                        default:
                            break;
                    }
                }

                scenarioCards.AddRange(agendas);
                scenarioCards.AddRange(acts);

                _loadingStatusService.ReportEncounterCardsStatus(Status.Finished);

                _game.ScenarioCards.LoadCards(scenarioCards);
                _game.LocationCards.LoadCards(locations);
                _game.EncounterDeckCards.LoadCards(treacheries);
            } catch (Exception ex) {
                _logger.LogException(ex, "Error loading encounter cards.");
                _loadingStatusService.ReportEncounterCardsStatus(Status.Error);
            } finally {
                _game.ScenarioCards.Loading = false;
                _game.LocationCards.Loading = false;
                _game.EncounterDeckCards.Loading = false;
            }
            _logger.LogMessage($"Finished loading encounter cards.");
        }

        public void LoadPlayerCards(ArkhamPlayer player) {
            if (player.Slots == null) {
                _logger.LogWarning($"{player.ID} has no cards in deck.");
                return;
            }

            _logger.LogMessage($"Loading cards for player {player.ID}.");

            // TODO: Consider alternatives to loading all cards
            var localCards = _localCardsService.LoadLocalCards();

            player.CardGroup.Loading = true;
            try {
                var cards = new List<CardInfo>();
                foreach (var slot in player.Slots) {
                    ArkhamDbCard arkhamDbCard = _arkhamDbService.GetCard(slot.Key);
                    if (arkhamDbCard != null) {

                        // Override card image with local card if possible
                        FindCardImageSource(arkhamDbCard, localCards);

                        var card = new ArkhamCardInfo(arkhamDbCard, slot.Value, true);

                        _cardImageService.LoadImage(card);

                        cards.Add(card);

                        // Look for bonded cards if present
                        cards.AddRange(GetBondedCards(arkhamDbCard, localCards));

                    } else {
                        _logger.LogError($"Could not find player {player.ID} card: {slot.Key}");
                    }
                }
                player.CardGroup.LoadCards(cards);
            } catch (Exception ex) {
                _logger.LogException(ex, $"Error loading cards for player {player.ID}.");
            } finally {
                player.CardGroup.Loading = false;
            }
            _logger.LogMessage($"Finished loading cards for player {player.ID}.");
        }

        private IEnumerable<CardInfo> GetBondedCards(ArkhamDbCard arkhamDbCard, List<ArkhamLocalCard> localCards) {
            if (arkhamDbCard is ArkhamDbFullCard fullCard && fullCard.Bonded_Cards?.Any() == true) {
                foreach (var bondedCardInfo in fullCard.Bonded_Cards) {
                    ArkhamDbCard bondedArkhamDbCard = _arkhamDbService.GetCard(bondedCardInfo.Code);
                    if (bondedArkhamDbCard != null) {

                        // Override card image with local card if possible
                        FindCardImageSource(bondedArkhamDbCard, localCards);

                        var bondedCard = new ArkhamCardInfo(bondedArkhamDbCard, bondedCardInfo.Count, isPlayerCard: true, isBonded: true);
                        _cardImageService.LoadImage(bondedCard);
                        yield return bondedCard;
                    } else {
                        _logger.LogError($"Could not find bonded card: {bondedCardInfo.Code}, bonded to: {arkhamDbCard.Code}");
                    }
                }
            }
        }

        private List<ArkhamCardInfo> GetEncounterCards() {
            var packsToLoad = new List<Pack>();
            foreach (var pack in _arkhamConfiguration.Packs) {
                foreach (var encounterSet in pack.EncounterSets) {
                    if (_game.IsEncounterSetSelected(encounterSet.Code)) {
                        packsToLoad.Add(pack);
                        break;
                    }
                }
            }

            var allLocalCards = _localCardsService.LoadLocalCards();

            var cards = new List<ArkhamCardInfo>();
            foreach (var pack in packsToLoad) {
                var arkhamDbCards = _arkhamDbService.GetCardsInPack(pack.Code);

                foreach (var arkhamDbCard in arkhamDbCards) {
                    if (!_game.IsEncounterSetSelected(arkhamDbCard.Encounter_Code)) {
                        continue;
                    }

                    // Look for corresponding local card and grab its image. Remove it from the list to avoid duplicates
                    FindCardImageSource(arkhamDbCard, allLocalCards, removeLocalCard: true);

                    var newCard = new ArkhamCardInfo(arkhamDbCard, 1, isPlayerCard: false);
                    _cardImageService.LoadImage(newCard);
                    cards.Add(newCard);
                    if (!string.IsNullOrEmpty(arkhamDbCard.BackImageSrc)) {
                        var newCardBack = new ArkhamCardInfo(arkhamDbCard, 1, isPlayerCard: false, cardBack: true);
                        _cardImageService.LoadImage(newCardBack);
                        newCard.FlipSideCard = newCardBack;
                        newCardBack.FlipSideCard = newCard;
                        cards.Add(newCardBack);
                    }
                }
            }

            var localCards = _localCardsService.LoadLocalCardsFromPacks(_game.LocalPacks);
            foreach (var localCard in localCards) {
                var newLocalCard = new ArkhamCardInfo(localCard, false);
                _cardImageService.LoadImage(newLocalCard);
                cards.Add(newLocalCard);

                if (localCard.HasBack) {
                    var newLocalCardBack = new ArkhamCardInfo(localCard, true);
                    _cardImageService.LoadImage(newLocalCardBack);
                    cards.Add(newLocalCardBack);
                }
            }

            return cards;
        }

        private static void FindCardImageSource(ArkhamDbCard arkhamDbCard, List<ArkhamLocalCard> localCards, bool removeLocalCard = false) {
            FixArkhamDbCardImageSource(arkhamDbCard);

            var localCard = localCards.FirstOrDefault(c => c.ArkhamDbId == arkhamDbCard.Code);
            if (localCard != null) {
                arkhamDbCard.ImageSrc = localCard.FilePath;
                if (localCard.HasBack) {
                    arkhamDbCard.BackImageSrc = localCard.BackFilePath;
                }
                if (removeLocalCard) {
                    localCards.Remove(localCard);
                }
            }
        }

        private static void FixArkhamDbCardImageSource(ArkhamDbCard arkhamDbCard) {
            string arkhamDbPrefix = "https://arkhamdb.com/";
            if (!string.IsNullOrEmpty(arkhamDbCard.ImageSrc) && !arkhamDbCard.ImageSrc.StartsWith(arkhamDbPrefix)) {
                arkhamDbCard.ImageSrc = arkhamDbPrefix + arkhamDbCard.ImageSrc;
            }
            if (!string.IsNullOrEmpty(arkhamDbCard.BackImageSrc) && !arkhamDbCard.BackImageSrc.StartsWith(arkhamDbPrefix)) {
                arkhamDbCard.BackImageSrc = arkhamDbPrefix + arkhamDbCard.BackImageSrc;
            }
        }
    }
}
