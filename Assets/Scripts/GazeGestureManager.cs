using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.WSA.Input;

public class GazeGestureManager : MonoBehaviour
{
    public static GazeGestureManager Instance { get; private set; }

    // Represents the hologram that is currently being gazed at.
    public GameObject FocusedObject { get; private set; }
    public GameObject oldFocusedObject;

    GestureRecognizer recognizer;

    void Awake()
    {
        Instance = this;

        //set up a GestureRecognizer to detect gestures.
        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.DoubleTap | GestureSettings.NavigationX);

        //subscribe to Tapped event
        recognizer.Tapped += (args) =>
        {
            if (FocusedObject != null)
            {
               FocusedObject.SendMessageUpwards("OnSelect", SendMessageOptions.DontRequireReceiver);
            }
        };

        //subscribe to Navigation events
        recognizer.NavigationStarted += (args) =>
        {
            if (FocusedObject != null)
            {
                Vector3 rightPosition = InputTracking.GetLocalPosition(XRNode.RightHand);
                FocusedObject.SendMessageUpwards("OnNavigationStarted", rightPosition, SendMessageOptions.DontRequireReceiver);
                oldFocusedObject = FocusedObject;
            }
        };

        recognizer.NavigationUpdated += (args) =>
        {
            if (oldFocusedObject!= null)
            {
                Vector3 rightPosition = InputTracking.GetLocalPosition(XRNode.RightHand);
                oldFocusedObject.SendMessageUpwards("OnNavigationUpdated", rightPosition, SendMessageOptions.DontRequireReceiver);
            }
        };

        recognizer.NavigationCompleted += (args) =>
        {
            oldFocusedObject = null;
        };

        //start gesture recognition
        recognizer.StartCapturingGestures();
    }

    void Update()
    { 
        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            // If the raycast hit a hologram, use that as the focused object.
            FocusedObject = hitInfo.collider.gameObject;
        }
        else
        {
            // If the raycast did not hit a hologram, clear the focused object.
            FocusedObject = null;
        }
    }
}