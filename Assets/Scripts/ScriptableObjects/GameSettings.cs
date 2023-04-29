using UnityEngine;

namespace LudumDare.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/Game Settings", order = 1)]
    public class GameSettings : ScriptableObject
    {
        [Range(0.1f, 3f)]
        [SerializeField] private float simulationSpeed = 1f;

        public float SimulationSpeed => simulationSpeed;
    }
}
