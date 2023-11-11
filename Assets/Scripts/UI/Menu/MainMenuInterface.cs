using RecoDeli.Scripts.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RecoDeli.Scripts.UI
{
    public class MainMenuInterface : MonoBehaviour
    {
        private void Awake()
        {
            RecoDeliGame.Initialize();
        }
    }
}
