using RecoDeli.Scripts.Settings;
using RecoDeli.Scripts.UI.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class MainMenuInterface : MonoBehaviour
    {
        [SerializeField] private UIDocument mainMenuDocument;

        [SerializeField] private TaskMenu taskMenu;
        [SerializeField] private UserSelectionMenu userSelectionMenu;
        [SerializeField] private SettingsMenu settingsMenu;

        private VisualElement menuRoot;

        private bool loggingIn;

        private enum MenuOption
        {
            None,
            Tasks,
            CustomTasks,
            Settings,
            SwitchUser,
            Exit
        }

        private Dictionary<MenuOption, (Button MenuButton, ModalWindow Window)> MenuOptions;

        private MenuOption CurrentMenu = MenuOption.None;

        private void Awake()
        {
            menuRoot = mainMenuDocument.rootVisualElement;

            RecoDeliGame.Initialize();
            menuRoot.SetEnabled(false);

            MenuOptions = new()
            {
                { MenuOption.Tasks, (menuRoot.Q<Button>("tasks-button"), taskMenu) },
                { MenuOption.CustomTasks, (menuRoot.Q<Button>("custom-tasks-button"), null) },
                { MenuOption.Settings, (menuRoot.Q<Button>("settings-button"), settingsMenu) },
                { MenuOption.SwitchUser, (menuRoot.Q<Button>("switch-user-button"), userSelectionMenu) },
                { MenuOption.Exit, (menuRoot.Q<Button>("exit-button"), null) },
            };

            foreach((var option, (var button, var menu)) in MenuOptions)
            {
                if(menu != null)
                {
                    menu.gameObject.SetActive(true);
                }

                if(option == MenuOption.CustomTasks)
                {
                    button.style.display = DisplayStyle.None;
                }
                else if(option == MenuOption.Exit)
                {
                    button.clicked += RecoDeliGame.QuitThisFuckingPieceOfShitImmediately;
                }
                else
                {
                    button.clicked += () => OpenMenu(option);
                }
            }
        }

        private void Start()
        {
            userSelectionMenu.Open();
            loggingIn = true;
        }

        private void Update()
        {
            if(loggingIn && !userSelectionMenu.Opened)
            {
                mainMenuDocument.rootVisualElement.SetEnabled(true);
                loggingIn = false;
                userSelectionMenu.SetSwitchingUser(true);
            }

            if(!loggingIn && MenuOptions.TryGetValue(CurrentMenu, out var components))
            {
                if(components.Window != null && !components.Window.Opened)
                {
                    OpenMenu(MenuOption.None);
                }
            }
        }

        private void OpenMenu(MenuOption menuOption)
        {
            CurrentMenu = menuOption;
            foreach ((var option, (var button, var menu)) in MenuOptions)
            {
                button.SetEnabled(menuOption != option);

                if (menu == null) continue;

                if(menuOption == option && !menu.Opened)
                {
                    menu.Open();
                }
                else if(menuOption != option && menu.Opened)
                {
                    menu.Close();
                }
            }
        }
    }
}
