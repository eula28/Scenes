using UnityEngine;

public class LogoutButton : MonoBehaviour
{
    public void OnLogoutButtonClick()
    {
        // Check if the FirebaseController instance exists
        if (FirebaseController.Instance != null)
        {
            // Call the LogOut function from the FirebaseController instance
            FirebaseController.Instance.LogOut();
            // Destroy the FirebaseController instance
            Destroy(FirebaseController.Instance.gameObject);
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }
}