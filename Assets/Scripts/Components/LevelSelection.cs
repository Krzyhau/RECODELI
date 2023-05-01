using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LudumDare.Scripts.Components
{
    public class LevelSelection : MonoBehaviour
    {
        [SerializeField] private CanvasGroup groupToHide;


        private void Start()
        {
        
        }

        public void ClickedLevel(int id)
        {
            StartCoroutine(LevelTransition(id));
        }

        private IEnumerator LevelTransition(int id)
        {
            groupToHide.alpha = 0.0f;
            groupToHide.interactable = false;
            yield return new WaitForEndOfFrame();
            SceneManager.LoadScene($"Level{id}");
        }

        public static bool IsLevelUnlocked(int id)
        {
            return id == 0 || IsLevelCompleted(id - 1);
        }

        public static bool IsLevelCompleted(int id)
        {
            return id <= PlayerPrefs.GetInt("LevelCompletion", 0);
        }

        public static void SetLevelCompleted(int id)
        {
            if (!IsLevelCompleted(id))
            {
                PlayerPrefs.SetInt("LevelCompletion", id);
            }
        }
    }
}
