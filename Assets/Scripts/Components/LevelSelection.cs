using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare.Scripts.Components
{
    public class LevelSelection : MonoBehaviour
    {
       
        private void Start()
        {
        
        }

        public void ClickedLevel(int id)
        {
            Debug.Log(id);
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
