using RecoDeli.Scripts.Level;
using RecoDeli.Scripts.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class SimpleAssignmentMenu : ModalWindow
    {
        [SerializeField] private List<string> mainLevelsList;

        private VisualElement builtInLevelsContainer;
        private VisualElement customLevelsContainer;

        private void Start()
        {
            builtInLevelsContainer = RootElement.Q<VisualElement>("built-in-levels");
            customLevelsContainer = RootElement.Q<VisualElement>("custom-levels");

            CreateMainLevelsButtons();
            CreateCustomLevelsButtons();

            Open();
        }

        private void CreateMainLevelsButtons()
        {
            if (mainLevelsList.Count > 0)
            {
                builtInLevelsContainer.Clear();
            }
            foreach (var level in mainLevelsList)
            {
                CreateLevelButton(level, builtInLevelsContainer);
            }
        }

        private void CreateCustomLevelsButtons()
        {
            var customLevelsList = LevelLoader.GetLevelFilesList().Where(level => !mainLevelsList.Contains(level));
            if (customLevelsList.Count() > 0)
            {
                customLevelsContainer.Clear();
            }
            foreach (var levelName in customLevelsList)
            {
                CreateLevelButton(levelName, customLevelsContainer);
            }
        }

        private void CreateLevelButton(string levelName, VisualElement container)
        {
            var button = new Button();
            button.text = levelName.ToUpper();
            button.clicked += () => LoadLevel(levelName);

            container.Add(button);
        }

        private void LoadLevel(string levelName)
        {
            StartCoroutine(LoadLevelCoroutine(levelName));
        }
        private IEnumerator LoadLevelCoroutine(string levelName)
        {
            //levelSelectionSound.Play();
            //ownCanvasGroup.alpha = 0.0f;
            //loadingScreenCanvasGroup.alpha = 1.0f;
            //loadingScreenText.text = levelName.ToUpper();

            yield return 0;

            RecoDeliGame.OpenLevel(levelName);
        }
    }
}
