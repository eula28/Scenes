using System.Collections;
using UnityEngine;
using TMPro;

public class LocationDisplay : MonoBehaviour
{
    public TextMeshProUGUI locationText;
    private string latitudeLongitude;

    IEnumerator Start()
    {
        // Check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            locationText.text = "Location services not enabled";
            yield break;
        }

        // Start service before querying location
        Input.location.Start(1f, 1f);

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            locationText.text = "Timed out";
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            locationText.text = "Unable to determine device location";
            yield break;
        }
        else
        {
            // Start the location update loop
            StartCoroutine(UpdateLocation());
        }
    }

    private IEnumerator UpdateLocation()
    {
        while (true)
        {
            // Update the location value
            if (Input.location.status == LocationServiceStatus.Running)
            {
                latitudeLongitude = Input.location.lastData.latitude + "\n" + Input.location.lastData.longitude;
                locationText.text = latitudeLongitude;
            }
            else
            {
                locationText.text = "Unable to determine device location";
            }

            // Wait for 1 second before updating again
            yield return new WaitForSeconds(1);
        }
    }

    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = latitudeLongitude;
        Debug.Log("Copied to Clipboard: " + latitudeLongitude);
        ToastManager.ShowToast("CURRENT LOCATION COPIED!");
    }
}
