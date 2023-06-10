using RecoDeli.Scripts.Level;
using RecoDeli.Scripts.Settings;
using RecoDeli.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RecoDeli.Scripts.Prototyping
{
    public class DebugLevelSelection : MonoBehaviour
    {
        [SerializeField] private CanvasGroup ownCanvasGroup;
        [SerializeField] private CanvasGroup loadingScreenCanvasGroup;
        [SerializeField] private TMP_Text loadingScreenText;

        [SerializeField] private RectTransform mainLevelsListContainer;
        [SerializeField] private RectTransform customLevelsListContainer;
        [SerializeField] private Button levelSelectionButtonPrefab;
        [SerializeField] private AudioSource levelSelectionSound;

        [SerializeField] private List<string> mainLevelsList;

        private void Start()
        {
            CreateMainLevelsButtons();
            CreateCustomLevelsButtons();
        }

        private void CreateMainLevelsButtons()
        {
            if(mainLevelsList.Count > 0)
            {
                mainLevelsListContainer.transform.Clear();
            }
            foreach (var level in mainLevelsList)
            {
                CreateLevelButton(level, mainLevelsListContainer);
            }
        }

        private void CreateCustomLevelsButtons()
        {
            var customLevelsList = LevelLoader.GetLevelFilesList().Where(level => !mainLevelsList.Contains(level));
            if (customLevelsList.Count() > 0)
            {
                customLevelsListContainer.transform.Clear();
            }
            foreach (var levelName in customLevelsList)
            {
                CreateLevelButton(levelName, customLevelsListContainer);
            }
        }

        private void CreateLevelButton(string levelName, RectTransform container)
        {
            var button = Instantiate(levelSelectionButtonPrefab, container);
            button.GetComponentInChildren<TMP_Text>().text = levelName.ToUpper();

            button.onClick.AddListener(() => LoadLevel(levelName));
        }

        private void LoadLevel(string levelName)
        {
            StartCoroutine(LoadLevelCoroutine(levelName));
        }
        private IEnumerator LoadLevelCoroutine(string levelName)
        {
            levelSelectionSound.Play();
            ownCanvasGroup.alpha = 0.0f;
            loadingScreenCanvasGroup.alpha = 1.0f;
            loadingScreenText.text = levelName.ToUpper();

            yield return 0;

            RecoDeliGame.OpenLevel(levelName);
        }
    }
}
