﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ArkhamOverlay.Data {
    public class Game : INotifyPropertyChanged {
        public Game() {
            Players = new List<Player>();
            EncounterSets = new List<EncounterSet>();
            ScenarioCards = new SelectableCards { Name = "Act/Agenda/Scenario Reference" };
            LocationCards = new SelectableCards { Name = "Location" };
            EncounterDeckCards = new SelectableCards { Name = "Encounter Deck" }; ;
        }

        public string Name { get; set; }

        public string Scenario { get; set; }

        public IList<EncounterSet> EncounterSets { get; set; }

        [JsonIgnore]
        public SelectableCards ScenarioCards { get; set; }

        [JsonIgnore]
        public SelectableCards LocationCards { get; set; }

        [JsonIgnore]
        public SelectableCards EncounterDeckCards { get; set; }

        public IList<Player> Players { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName) {
            var handler = PropertyChanged;
            if (handler == null) {
                return;
            }
            handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnPlayersChanged() {
            OnPropertyChanged(nameof(Players));
        }
        public void OnEncounterSetsChanged() {
            OnPropertyChanged(nameof(EncounterSets));
        }

        internal bool IsEncounterSetSelected(string code) {
            return EncounterSets.Any(x => x.Code == code);
        }
    }
}
