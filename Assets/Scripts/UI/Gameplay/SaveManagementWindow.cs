using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class SaveManagementWindow : MonoBehaviour
    {
        [SerializeField] private UIDocument saveManagementDocument;

        private Button closeButton;

        private void Awake()
        {
            closeButton = saveManagementDocument.rootVisualElement.Q<Button>("close-button");

            closeButton.clicked += () => SetDisplay(false);

            SetDisplay(false);
        }

        public void SetDisplay(bool state)
        {
            saveManagementDocument.rootVisualElement.SetEnabled(state);
        }
    }
}
