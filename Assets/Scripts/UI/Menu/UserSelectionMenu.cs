using RecoDeli.Scripts.SaveManagement;
using RecoDeli.Scripts.Settings;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI.Menu
{
    public class UserSelectionMenu : ModalWindow
    {

        private TextField usernameField;
        private Button createUserButton;
        private Button closeButton;
        private Button backButton;
        private VisualElement userSlotsContainer;
        private Button resetButton;
        private Label creatingUserLabel;

        private bool switchingUser;
        private bool resettingUser;
        private int creatingUserSlot;

        protected override void Awake()
        {
            base.Awake();

            usernameField = RootElement.Q<TextField>("username-field");
            createUserButton = RootElement.Q<Button>("create-user-button");
            closeButton = RootElement.Q<Button>("close-button");
            backButton = RootElement.Q<Button>("back-button");
            userSlotsContainer = RootElement.Q<VisualElement>("user-slots-container");
            resetButton = RootElement.Q<Button>("save-reset-button");
            creatingUserLabel = RootElement.Q<Label>("user-creation-window-label");

            createUserButton.clicked += OnCreateUser;
            resetButton.clicked += StartCurrentUserReset;
            backButton.clicked += () => ToggleUserCreation(false);
            usernameField.RegisterValueChangedCallback((v) => createUserButton.SetEnabled(v.newValue.Length > 0));

            SetSwitchingUser(false);
        }

        public override void Open()
        {
            base.Open();
            UpdateSlotsButtons();

            resetButton.SetEnabled(switchingUser);
        }

        private void UpdateSlotsButtons()
        {
            var saveCount = RecoDeliGame.Settings.UserSaveCount;

            userSlotsContainer.Clear();

            for (int i = 0; i < saveCount; i++)
            {
                var slotButton = new Button();
                slotButton.name = "user-slot";
                var slotLabel = new Label();
                slotButton.Add(slotLabel);

                var userSlot = i;

                if (SaveManager.TryLoadSlot(userSlot, out var slot))
                {
                    var hoursPlayed = Mathf.Floor(slot.PlayTime / 3600.0f);
                    var minutesPlayed = Mathf.Floor((slot.PlayTime / 60.0f) % 60.0f);
                    slotLabel.text = $"{slot.Name} ({hoursPlayed}:{minutesPlayed:00})";
                    slotButton.clicked += () => SaveSlotSelected(userSlot);
                }
                else
                {
                    slotButton.clicked += () => StartUserCreation(userSlot);
                    slotLabel.text = "<empty>";
                }

                slotButton.SetEnabled(!switchingUser || SaveManager.CurrentSlot != i);

                userSlotsContainer.Add(slotButton);
            }
        }

        private void StartUserCreation(int i)
        {
            creatingUserSlot = i;
            ToggleUserCreation(true);
        }

        private void StartCurrentUserReset()
        {
            resettingUser = true;
            StartUserCreation(SaveManager.CurrentSlot);
        }

        private void ToggleUserCreation(bool state)
        {
            RootElement.EnableInClassList("creating-user", state);
            if (state)
            {
                var label = resettingUser ? "OVERRIDING USER" : "CREATING NEW USER";
                creatingUserLabel.text = label;

                var actionLabel = resettingUser ? "OVERRIDE" : "CREATE";
                createUserButton.text = actionLabel;
            }
            usernameField.value = "";
        }

        private void OnCreateUser()
        {
            SaveManager.ForceLoadNewSave(creatingUserSlot);
            SaveManager.CurrentSave.Name = usernameField.value;
            SaveManager.Save();

            ToggleUserCreation(false);
            Close();
        }

        private void SaveSlotSelected(int i)
        {
            SaveManager.Save();
            SaveManager.Load(i, force: true);
            Close();
        }

        public void SetSwitchingUser(bool switching)
        {
            switchingUser = switching;

            closeButton.SetEnabled(switching);
        }
    }
}
