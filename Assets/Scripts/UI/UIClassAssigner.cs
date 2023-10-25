using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.Assets.Scripts.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class UIClassAssigner : MonoBehaviour
    {
        [SerializeField] private string className;
        private void Awake()
        {
            GetComponent<UIDocument>().rootVisualElement.AddToClassList(className);
        }
    }
}
