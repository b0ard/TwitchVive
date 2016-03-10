using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WandController : MonoBehaviour {
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)_trackedObj.index); } }
    private SteamVR_TrackedObject _trackedObj;

    private HashSet<InteractableItem> objectsHoveringOver = new HashSet<InteractableItem>();

    [SerializeField]
    private InteractableItem closestItem = null;
    [SerializeField]
    private InteractableItem interactingItem = null;

    void Awake() {
        _trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (controller == null) {
            Debug.Log("Controller is null");
            return;
        }

        if (controller.GetPressDown(gripButton)) {
            float minDistance = float.MaxValue;

            float distance;
            foreach (InteractableItem item in objectsHoveringOver) {
                distance = (item.transform.position - transform.position).sqrMagnitude;

                if (distance < minDistance) {
                    minDistance = distance;
                    closestItem = item;
                }
            }

            interactingItem = closestItem;
            if (interactingItem) {
                if (interactingItem.isInteracting()) {
                    interactingItem.EndInteraction(this);
                }

                interactingItem.BeginInteraction(this);
            }
            closestItem = null; // TODO: should this be here?
        }

        if (controller.GetPressUp(gripButton) && interactingItem) { 
            interactingItem.EndInteraction(this);
        }
	}

    private void OnTriggerEnter(Collider collider) {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem) {
            objectsHoveringOver.Add(collidedItem);
        }
    }

    private void OnTriggerExit(Collider collider) {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();

        if (collidedItem) {
            objectsHoveringOver.Remove(collider.GetComponent<InteractableItem>());
        }
    }
}
