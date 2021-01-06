﻿using PageController;

namespace ArkhamOverlay.Data {
    public class AppData : ViewModel {
        public AppData() {
            Game = new Game();
            Configuration = new Configuration {
                OverlayWidth = 1228,
                OverlayHeight = 720,
                CardHeight = 300
            };
        }

        public Game Game { get; }

        public Configuration Configuration { get; }

        private string _status;

        public string Status {
            get => _status; 
            set {
                _status = value;
                NotifyPropertyChanged(nameof(Status));
            }
        }

        private bool _isActAgendaBarVisible;
        public bool IsActAgendaBarVisible {
            get => _isActAgendaBarVisible;
            set {
                _isActAgendaBarVisible = value;
                NotifyPropertyChanged(nameof(IsActAgendaBarVisible));
            }
        }

        public void OnGameChanged() {
            NotifyPropertyChanged(nameof(Game));
        }
    }
}
