using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace LudumDare.Scripts.Components
{
    public class LevelButton : MonoBehaviour
    {
        [SerializeField] private Transform greenLight;
        [SerializeField] private LevelSelection levelSelection;
        [SerializeField] private int id;

        private CanvasGroup ownGroup;

        private void OnEnable()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            GetComponent<Button>().onClick.RemoveListener(OnClick);
        }

        private void Awake()
        {
            ownGroup = GetComponent<CanvasGroup>();
            ownGroup.alpha = 0.0f;
            ownGroup.interactable = false;

            greenLight.gameObject.SetActive(LevelSelection.IsLevelCompleted(id));

            if (LevelSelection.IsLevelUnlocked(id))
            {
                StartCoroutine(RevealRoutine());
            }
        }

        private IEnumerator RevealRoutine()
        {
            yield return new WaitForSeconds(id * 0.05f);
            ownGroup.alpha = 1.0f;
            ownGroup.interactable = true;
        }

        private void OnClick()
        {
            levelSelection.ClickedLevel(id);
        }
    }
}
