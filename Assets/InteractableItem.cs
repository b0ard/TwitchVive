using UnityEngine;
using System.Collections;

public class InteractableItem : MonoBehaviour {
    public Rigidbody rigidbody;

    [SerializeField]
    private bool currentlyInteracting;

    [SerializeField]
    private WandController attachedWand;

    [SerializeField]
    private float velocityFactor = 20000f;

    [SerializeField]
    private float rotationFactor = 400f;

    // For determining position
    private Vector3 posDelta;

    // For determining rotation
    private Quaternion rotationDelta;
    private float angle;
    private Vector3 axis;

    private Transform interactionPoint;

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
        velocityFactor /= rigidbody.mass;
        rotationFactor /= rigidbody.mass;

        interactionPoint = new GameObject().transform;
	}
	
	// Update is called once per frame
	void Update () {
	    if (attachedWand) {
            // TODO: maybe configurable joint
            posDelta = attachedWand.transform.position - interactionPoint.position; 
            this.rigidbody.velocity = posDelta * velocityFactor * Time.fixedDeltaTime;

            rotationDelta = attachedWand.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
            rotationDelta.ToAngleAxis(out angle, out axis);

            if (angle > 180) {
                angle -= 360;
            }

            this.rigidbody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotationFactor;
        }
    }

    public void BeginInteraction(WandController wand) {
        attachedWand = wand;
        interactionPoint.position = wand.transform.position;
        interactionPoint.rotation = wand.transform.rotation;
        interactionPoint.SetParent(transform, true);

        currentlyInteracting = true;
    }

    public void EndInteraction(WandController wand) {
        if (wand == attachedWand) {
            attachedWand = null;
            currentlyInteracting = false;
        }
    }

    public bool isInteracting() {
        return currentlyInteracting;
    }
}
