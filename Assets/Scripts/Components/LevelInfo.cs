using UnityEngine;

namespace LudumDare.Scripts.Components
{
    class LevelInfo : MonoBehaviour
    {
        [SerializeField] private int id;

        public int ID => id;
        public static LevelInfo Current { get; private set; }

        private void Awake() => Current = this;
    }
}
