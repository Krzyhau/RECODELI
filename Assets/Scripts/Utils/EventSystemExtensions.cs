using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        public static bool HasInputFieldSelected(this EventSystem eventSystem)
        {
            var selectedObject = eventSystem.currentSelectedGameObject;
            if (selectedObject == null) return false;
            if (selectedObject.GetComponent<TMP_InputField>() != null) return true;
            if (selectedObject.GetComponent<InputField>() != null) return true;
            return false;
        }
    }
}
