using UnityEngine.UIElements;

namespace Pastime_Hierarchy.Editor.UI.Components.RowView {
    public interface IRowItemFactory<in T> {
        VisualElement CreateItem(int rowIndex, int itemIndex, T data, ListView listView);
        void OnAddItem(int rowIndex);
        void OnRemoveRow(int rowIndex);
        bool CanAddMoreItems(int rowIndex);
    }
}