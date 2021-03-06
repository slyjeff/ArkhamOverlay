﻿using System.Windows;
using System.Windows.Media;

namespace EideticMemoryOverlay.PluginApi.Interfaces {
    public interface IHasImageButton {
        string Name { get; }
        string ImageId { get; }
        string ImageSource { get; set; }
        ImageSource Image { get; set; }
        ImageSource ButtonImage { get; set; }
        byte[] ButtonImageAsBytes { get; set; }
        Point GetCropStartingPoint();
    }
}
