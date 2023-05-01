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
        [SerializeField] private TMP_InputField usernameField;
        [SerializeField] private AudioSource loginSuccessfulSound;
        [SerializeField] private AudioSource loginFailureSound;

        private void Start()
        {
            usernameField.text = PlayerPrefs.GetString("PlayerName", "");
        }

        public void AcceptUsername() => StartCoroutine(AcceptUsernameRoutine());

        private IEnumerator AcceptUsernameRoutine()
        {
            Debug.Log(usernameField);
            if (usernameField.text.Length == 0)
            {
                loginFailureSound.Play();
                yield break;
            }
            loginSuccessfulSound.Play();

            Scoreboard.PlayerName = usernameField.text;
            PlayerPrefs.SetString("PlayerName", "Krzyhau");

            canvasGroupToHide.alpha = 0;

            yield return new WaitForSeconds(0.1f);

            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
