﻿using ArkhamOverlay.CardButtons;
using ArkhamOverlay.Services;
using ArkhamOverlay.Utils;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace ArkhamOverlay.Data {
    public delegate void CardToggledEvent(ICardButton card);

    public class Card : IHasImageButton {
        public Card() {
        }

        public Card(ArkhamDbCard arkhamDbCard, int count, bool isPlayerCard, bool cardBack = false, bool isBonded = false) {
            Code = arkhamDbCard.Code;
            Count = count;
            Name = arkhamDbCard.Xp == "0" || string.IsNullOrEmpty(arkhamDbCard.Xp) ? arkhamDbCard.Name : arkhamDbCard.Name + " (" + arkhamDbCard.Xp + ")";
            NameWithoutXp = arkhamDbCard.Name;
            Xp = arkhamDbCard.Xp == null ? 0 : int.Parse(arkhamDbCard.Xp);
            Faction = GetFaction(arkhamDbCard.Faction_Name);
            Type = GetCardType(arkhamDbCard.Type_Code);
            ImageSource = cardBack ? arkhamDbCard.BackImageSrc : arkhamDbCard.ImageSrc;
            IsPlayerCard = isPlayerCard;
            IsBonded = isBonded;
            if (cardBack) {
                Name += " (Back)";
            }

            //stometimes if we are closing, this will be null and we can just bail
            if (Application.Current == null) {
                return;
            }
            this.LoadImage(ImageSource);
        }

        public Card(LocalManifestCard localCard, bool cardBack) {
            Code = "";
            Count = 1;
            Name = localCard.Name;
            NameWithoutXp = localCard.Name;
            Xp = 0;
            Faction = Faction.Other;
            Type = (CardType)Enum.Parse(typeof(CardType), localCard.CardType);
            ImageSource = cardBack ? localCard.BackFilePath : localCard.FilePath;
            IsPlayerCard = false;
            if (cardBack) {
                Name += " (Back)";
            }

            //stometimes if we are closing, this will be null and we can just bail
            if (Application.Current == null) {
                return;
            }
            this.LoadImage(ImageSource);
        }

        public string Name { get; }
        public string NameWithoutXp { get; }
        public int Xp { get; }
        public string Code { get; }
        public Faction Faction { get; set; }
        public int Count { get; set; }
        public string ImageSource { get; }
        public ImageSource Image { get; set; }
        public ImageSource ButtonImage { get; set; }
        public byte[] ButtonImageAsBytes { get; set; }
        public bool IsBonded { get; }

        public CardType Type { get; }
        
        CardType IHasImageButton.ImageCardType { get { return Type; } }

        public Color CardColor {
            get {
                switch (Type) {
                    case CardType.Agenda:
                        return Colors.Chocolate;
                    case CardType.Act:
                        return Colors.BurlyWood;
                    case CardType.Enemy:
                        return Colors.SlateBlue;
                    case CardType.Treachery:
                        return Colors.SlateGray;
                    case CardType.Asset:
                    case CardType.Event:
                    case CardType.Skill:
                        switch (Faction) {
                            case Faction.Guardian:
                                return Colors.DarkBlue;
                            case Faction.Seeker:
                                return Colors.DarkGoldenrod;
                            case Faction.Rogue:
                                return Colors.DarkGreen;
                            case Faction.Survivor:
                                return Colors.DarkRed;
                            case Faction.Mystic:
                                return Colors.Indigo;
                            default:
                                return Colors.DarkGray;
                        }
                    default:
                        return Colors.DarkGray;
                }
            }
        }

        public bool IsHorizontal { get { return Type == CardType.Act || Type == CardType.Agenda; } }

        public bool IsPlayerCard { get; private set; }

        public Card FlipSideCard { get; set; }

        public event Action<bool> IsDisplayedOnOverlayChanged;
        
        private bool _isDisplayedOnOverlay = false;
        public bool IsDisplayedOnOverlay {
            get => _isDisplayedOnOverlay;
            set {
                _isDisplayedOnOverlay = value;
                IsDisplayedOnOverlayChanged?.Invoke(_isDisplayedOnOverlay);
            }
        }

        private CardType GetCardType(string typeCode) {
            if(Enum.TryParse(typeCode, ignoreCase: true, out CardType type)) {
                return type;
            } else {
                return CardType.Other;
            }
        }

        private Faction GetFaction(string factionCode) {
            if (Enum.TryParse(factionCode, ignoreCase: true, out Faction type)) {
                return type;
            }
            else {
                return Faction.Other;
            }
        }
    }
}
