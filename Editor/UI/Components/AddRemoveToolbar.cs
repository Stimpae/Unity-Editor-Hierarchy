
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pastime_Hierarchy.Editor.UI.UIElements {
    public class AddRemoveToolbar : Toolbar {
        private bool m_showAddButton = true;
        private bool m_showRemoveButton = true;
        private EditorToolbarButton m_addButton;
        private EditorToolbarButton m_removeButton;
        
        public Action onAddClicked;
        public Action onRemoveClicked;
        
        public bool ShowAddButton {
            get => m_showAddButton;
            set {
                m_showAddButton = value;
                UpdateButtons();
            }
        }
        public bool ShowRemoveButton {
            get => m_showRemoveButton;
            set {
                m_showRemoveButton = value;
                UpdateButtons();
            }
        }
        
        public AddRemoveToolbar(Action onAddClicked, Action onRemoveClicked) {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Pastime Hierarchy/Editor/UI/USS/EditorToolbarStyles.uss");
            if (styleSheet == null) Debug.LogError($"StyleSheet not found at path: Assets/Pastime Hierarchy/Editor/UI/USS/EditorToolbarStyles.uss");
            if (styleSheet != null) styleSheets.Add(styleSheet);
            
            this.onAddClicked = onAddClicked;
            this.onRemoveClicked = onRemoveClicked;
            
            this.AddToClassList("add-remove-toolbar");
            
            var toolbarContainer = new VisualElement();
            toolbarContainer.AddToClassList("add-remove-toolbar__container");
            
            m_addButton = new EditorToolbarButton(this.onAddClicked);
            m_addButton.AddIcon("CreateAddNew");
            m_addButton.SetIconSize(16, 16);
            m_addButton.AddToClassList("add-toolbar__button");
            
            m_removeButton = new EditorToolbarButton(this.onRemoveClicked);
            m_removeButton.AddIcon("Toolbar Minus");
            m_removeButton.SetIconSize(16, 16);
            m_removeButton.AddToClassList("remove-toolbar__button");
            
            toolbarContainer.Add(m_addButton);
            toolbarContainer.Add(m_removeButton);
            
            Add(toolbarContainer);
        }

        private void UpdateButtons() {
            m_addButton.style.display = m_showAddButton ? DisplayStyle.Flex : DisplayStyle.None;
            m_removeButton.style.display = m_showRemoveButton ? DisplayStyle.Flex : DisplayStyle.None;
        }

    }
}