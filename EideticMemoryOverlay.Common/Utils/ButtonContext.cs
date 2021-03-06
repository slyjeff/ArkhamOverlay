﻿using Emo.Common.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Emo.Common.Utils {
    /// <summary>
    /// This part of the context is whether this button interacts with the card pool for this card group (logical group of all Card Infos
    /// that the group contains, or Card Zones, which represent actual instances of cards in play
    /// </summary>
    /// <remarks>Should probably be renamed to ButtonType, but I'm not 100% settled on that yet</remarks>
    public enum ButtonMode { Pool, Zone }

    /// <summary>
    /// Identify a button in three parts (eventually four)
    /// </summary>
    public interface IButtonContext {
        CardGroupId CardGroupId { get; }
        ButtonMode ButtonMode { get; }
        int ZoneIndex { get; }
        int Index { get; }
    }

    //todo: consider using IComparable
    public static class ButtonContextExtensions {
        public static bool HasSameContext(this IButtonContext a, IButtonContext b) {
            return a.CardGroupId == b.CardGroupId && a.ButtonMode == b.ButtonMode && a.ZoneIndex == b.ZoneIndex && a.Index == b.Index;
        }

        public static bool IsAfter(this IButtonContext a, IButtonContext b) {
            return a.CardGroupId == b.CardGroupId && a.ButtonMode == b.ButtonMode && a.ZoneIndex == b.ZoneIndex && a.Index > b.Index;
        }

        public static bool IsAtSameIndexOrAfter(this IButtonContext a, IButtonContext b) {
            return a.CardGroupId == b.CardGroupId && a.ButtonMode == b.ButtonMode && a.ZoneIndex == b.ZoneIndex && a.Index >= b.Index;
        }


        public static IEnumerable<T> FindAllWithContext<T>(this IEnumerable<T> list, IButtonContext context) where T : IButtonContext {
            return from potentialContext in list
                   where potentialContext.HasSameContext(context)
                   select potentialContext;
        }
        public static T FirstOrDefaultWithContext<T>(this IEnumerable<T> list, IButtonContext context) where T : IButtonContext {
            return list.FirstOrDefault(x => x.HasSameContext(context));
        }
    }
}
