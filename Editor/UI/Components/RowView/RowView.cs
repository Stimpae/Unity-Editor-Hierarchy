using System;
using System.Collections;
using System.Collections.Generic;
using Pastime_Hierarchy.Editor.UI.Components.RowView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pastime_Hierarchy.Editor.UI.Components {

    
    public class RowView<TRow, TItem> : ListView
    {
        public RowView(List<TRow> rows, string title, IRowItemFactory<TItem> factory, Func<TRow, List<TItem>> itemSelector) {
            this.name = title;
            this.itemsSource = rows;
            showAlternatingRowBackgrounds = AlternatingRowBackground.None;
            reorderable = true;
            reorderMode = ListViewReorderMode.Animated;
            fixedItemHeight = 22;

            makeItem = () => new Row<TItem>(factory);
            bindItem = (element, index) => {
                if (element is Row<TItem> row)
                    row.Bind(index, itemSelector(rows[index]),this);
            };
        }
    }
}