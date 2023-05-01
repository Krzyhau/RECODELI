using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LudumDare.Scripts.Components
{
    public class UsernameField : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroupToHide;

        [SerializeField] private string sceneToLoad;

        public void AcceptUsername() => StartCoroutine(AcceptUsernameRoutine());

        private IEnumerator AcceptUsernameRoutine()
        {
            var username = GetComponent<TMP_InputField>().text;
            if (username.Length <= 0) yield break;

            Scoreboard.PlayerName = username;

            canvasGroupToHide.alpha = 0;

            yield return new WaitForSeconds(0.1f);

            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
