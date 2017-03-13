using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelectionScript : MonoBehaviour, ISelectHandler, IDeselectHandler, IUpdateSelectedHandler {

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("Select!");
        if (gameObject.name.Equals("ButtonShipSystems"))
        {
            GameObject[] SUButtons = GameObject.FindGameObjectsWithTag("SUButtons");
            if (SUButtons != null)
            {
                foreach (GameObject subutton in SUButtons)
                {
                    subutton.SetActive(true);
                }
            }
        }
    }

	public void OnDeselect(BaseEventData eventData)
    {
        return;
        Debug.Log("Deselect! " + eventData.selectedObject.tag);

        if (eventData.selectedObject.tag.Equals("SUButtons"))
            return;
        if (gameObject.name.Equals("ButtonShipSystems"))
        {
            GameObject[] SUButtons = GameObject.FindGameObjectsWithTag("SUButtons");
            if (SUButtons != null)
            {
                foreach (GameObject subutton in SUButtons)
                {
                    subutton.SetActive(false);
                }
            }
        }
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
        Debug.Log(eventData.selectedObject.tag);
    }
}
