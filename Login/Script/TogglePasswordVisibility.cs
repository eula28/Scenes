using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TogglePasswordVisibility : MonoBehaviour
{
    public TMP_InputField passwordInputField;
    public Toggle showPasswordToggle;

    void Start()
    {
        // Ensure the toggle is unchecked initially
        showPasswordToggle.isOn = false;

        // Add listener to handle toggle change
        showPasswordToggle.onValueChanged.AddListener(delegate { PasswordVisibility(showPasswordToggle); });

        // Set the InputField to password mode initially
        passwordInputField.contentType = TMP_InputField.ContentType.Password;
        passwordInputField.ForceLabelUpdate();
    }

    void PasswordVisibility(Toggle toggle)
    {
        if (toggle.isOn)
        {
            // Show the password
            passwordInputField.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            // Hide the password
            passwordInputField.contentType = TMP_InputField.ContentType.Password;
        }

        // Refresh the InputField to apply the change
        passwordInputField.ForceLabelUpdate();
    }
}
