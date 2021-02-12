﻿using ArkhamOverlay.Utils;
using PageController;
using System.Windows.Media;

namespace ArkhamOverlay.CardButtons {
    public interface IButton {
        string Text { get; }

        void LeftClick();

        void RightClick();

        bool IsToggled { get; }
    }

    public abstract class Button : ViewModel, IButton {

        private string _text;
        public string Text {
            get => _text;
            set {
                _text = value;
                NotifyPropertyChanged(nameof(Text));
            }
        }

        public Brush BorderBrush { get { return IsToggled ? new SolidColorBrush(Colors.DarkGoldenrod) : new SolidColorBrush(Colors.Black); } }

        public virtual ImageSource ButtonImage { get { return ImageUtils.CreateSolidColorImage(Colors.DarkGray); } }

        public abstract void LeftClick();

        public virtual void RightClick() {
            //by default, do nothing
        }

        private bool _isToggled;
        public bool IsToggled {
            get => _isToggled;
            set {
                _isToggled = value;
                NotifyPropertyChanged(nameof(BorderBrush));
            }
        }
    }
}
