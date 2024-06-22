using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationCheck : MonoBehaviour
{
    [System.Serializable]
    public struct Location
    {
        public float latitude;
        public float longitude;
        public string sceneName;
    }

    // Array of target locations
    public Location[] targetLocations;

    // Tolerance in degrees for considering the user to have reached the target location
    public float tolerance = 0.001f;

    // Time interval in seconds to wait between location checks
    public float checkInterval = 10f;

    void Start()
    {
        StartCoroutine(StartLocationService());
    }

    IEnumerator StartLocationService()
    {
        // Check if the user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location service is not enabled by the user.");
            yield break;
        }

        // Start the location service
        Input.location.Start();

        // Wait until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds, stop the coroutine
        if (maxWait <= 0)
        {
            Debug.Log("Location service initialization timed out.");
            yield break;
        }

        // If the connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location.");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            StartCoroutine(CheckLocationPeriodically());
        }
    }

    IEnumerator CheckLocationPeriodically()
    {
        while (true)
        {
            CheckLocation();
            yield return new WaitForSeconds(checkInterval);
        }
    }

    void CheckLocation()
    {
        float userLatitude = Input.location.lastData.latitude;
        float userLongitude = Input.location.lastData.longitude;

        Debug.Log("User Latitude: " + userLatitude + ", User Longitude: " + userLongitude);

        // Check if the user's location is within the tolerance of any target location
        foreach (var location in targetLocations)
        {
            if (Mathf.Abs(userLatitude - location.latitude) <= tolerance && Mathf.Abs(userLongitude - location.longitude) <= tolerance)
            {
                // Switch to the target scene
                SceneManager.LoadScene(location.sceneName);
                return;
            }
        }
    }

    void OnDisable()
    {
        // Stop the location service if it's running
        if (Input.location.isEnabledByUser)
        {
            Input.location.Stop();
        }
    }
}
