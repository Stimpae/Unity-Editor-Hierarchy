using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pastime_Hierarchy.Editor.UI.Components {
    public class PaletteListView<T> : ListView where T : VisualElement {
        public PaletteListView(string name, IList itemsSource, Func<int, T> makeItem, Action<int, T> bindItem) {
            this.name = name;
            this.itemsSource = itemsSource;
            showAlternatingRowBackgrounds = AlternatingRowBackground.None;
            reorderable = true;
            reorderMode = ListViewReorderMode.Animated;
            fixedItemHeight = 22;
            this.makeItem = () => makeItem(-1);
            this.bindItem = (element, index) => bindItem(index, (T)element);
        }
    }
}