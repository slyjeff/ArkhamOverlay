﻿using ArkhamOverlay.Data;
using PageController;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace ArkhamOverlay.Pages.Overlay {
    public class OverlayViewModel : ViewModel {
        public OverlayViewModel() {
            ActAgendaCards = new ObservableCollection<OverlayCardViewModel>();
            HandCards = new ObservableCollection<OverlayCardViewModel>();
            EncounterCards = new ObservableCollection<OverlayCardViewModel>();
            PlayerCards = new ObservableCollection<OverlayCardViewModel>();
        }

        public virtual AppData AppData { get; set; }
        public virtual double OverlayFontSize { get; set; }
        public virtual double DeckListItemWidth { get; set; }
        public virtual double DeckListHeight { get; set; }
        public virtual Thickness DeckListMargin { get; set; }
        public virtual bool ShowDeckList { get; set; }
        public virtual double StatImageSize { get; set; }

        public virtual IList<DeckListItem> DeckList { get; set; }
        public virtual ObservableCollection<OverlayCardViewModel> ActAgendaCards { get; set; }
        public virtual ObservableCollection<OverlayCardViewModel> HandCards { get; set; }
        public virtual ObservableCollection<OverlayCardViewModel> EncounterCards { get; set; }
        public virtual ObservableCollection<OverlayCardViewModel> PlayerCards { get; set; }
        public IList<ObservableCollection<OverlayCardViewModel>> AllOverlayCards {
            get {
                return new List<ObservableCollection<OverlayCardViewModel>> { ActAgendaCards, HandCards, EncounterCards, PlayerCards };
            }
        }
    }

    public class DeckListItem {
        private Card _card;
        private string _name;

        public DeckListItem(Card card) {
            _card = card;
        }

        public DeckListItem(string name) {
            _name = name;
        }

        public string Name { 
            get {
                if(!string.IsNullOrEmpty(_name)) {
                    return _name;
                }
                var name = _card.NameWithoutXp;
                for (var x = 0; x < _card.Xp; x++) {
                    name += "•";
                }

                return _card.Count + "x " + name; 
            } 
        }

        public Brush Foreground { 
            get {
                if (_card == null) {
                    return new SolidColorBrush(Colors.Black);
                }

                switch (_card.Faction) {
                    case Faction.Guardian: return new SolidColorBrush(Colors.DarkBlue);
                    case Faction.Seeker: return new SolidColorBrush(Colors.DarkGoldenrod);
                    case Faction.Mystic: return new SolidColorBrush(Colors.Purple);
                    case Faction.Rogue: return new SolidColorBrush(Colors.DarkGreen);
                    case Faction.Survivor: return new SolidColorBrush(Colors.DarkRed);
                    default: return new SolidColorBrush(Colors.Black);
                }
            } 
        }
    }
}
