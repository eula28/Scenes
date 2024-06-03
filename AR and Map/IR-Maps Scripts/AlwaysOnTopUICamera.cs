using UnityEngine;

public class AlwaysOnTopUICamera : MonoBehaviour
{
    private Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas component not found!");
            return;
        }

        // Set the sorting order of the Canvas to a high value to ensure it is on top
        canvas.sortingOrder = 1000;
        // Adjust plane distance to ensure the canvas is in front of other objects
        canvas.planeDistance = 1; // Set this to a value that ensures it's in front of 3D objects
    }
}
