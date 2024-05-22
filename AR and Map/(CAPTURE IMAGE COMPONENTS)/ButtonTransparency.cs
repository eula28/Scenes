using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChange : MonoBehaviour
{
    private Button button;
    private Image buttonImage;
    private Color originalColor;

    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();
        originalColor = buttonImage.color;
        
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        StartCoroutine(ChangeColorTemporarily());
    }

    IEnumerator ChangeColorTemporarily()
    {
        buttonImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0); // Set to transparent
        yield return new WaitForSeconds(2);
        buttonImage.color = originalColor; // Revert to original color
    }
}
