﻿using EideticMemoryOverlay.PluginApi;
using PageController;

namespace Emo.Pages.SelectCards {
    public class SelectCardsViewModel : ViewModel {
        public virtual ICardGroup CardGroup { get; set; }
    }
}
