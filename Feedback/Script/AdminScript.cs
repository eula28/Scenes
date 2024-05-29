using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminScript : MonoBehaviour
{
    FirebaseFirestore db;
    // Start is called before the first frame update
    void Start()
    {
       // Check if the FirebaseController instance exists
        if (FirebaseController.Instance != null)
        {
            db = FirebaseFirestore.DefaultInstance;
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }


}
