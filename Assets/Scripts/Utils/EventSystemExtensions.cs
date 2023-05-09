using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RecoDeli.Scripts.Utils
{
    public static class EventSystemExtensions
    {
        public static bool IsPointerOverGameObject(this EventSystem eventSystem, GameObject gameObject)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            foreach(var result in raycastResults)
            {
                if (result.gameObject == gameObject) return true;
            }

            return false;
        }
    }
}
