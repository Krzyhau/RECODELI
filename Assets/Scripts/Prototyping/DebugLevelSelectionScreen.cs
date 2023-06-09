using RecoDeli.Scripts.Level;
using RecoDeli.Scripts.Settings;
using RecoDeli.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RecoDeli.Scripts.Prototyping
{
    public class DebugLevelSelection : MonoBehaviour
    {
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
            //customLevelsListContainer.transform.Clear();
        }

        private void CreateLevelButton(string levelName, RectTransform container)
        {
            var button = Instantiate(levelSelectionButtonPrefab, container);
            button.GetComponentInChildren<TMP_Text>().text = levelName.ToUpper();

            button.onClick.AddListener(() => {
                levelSelectionSound.Play();
                RecoDeliGame.OpenLevel(levelName);
            });
        }
    }
}
