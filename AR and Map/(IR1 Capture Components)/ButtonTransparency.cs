using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChange : MonoBehaviour
{
    private Button button;
    private Image buttonImage;
    private Color originalColor;

    // Public fields to assign the other images in the Inspector
    public Image otherImage1;
    public Image otherImage2;
    private Color otherImage1OriginalColor;
    private Color otherImage2OriginalColor;

    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();
        originalColor = buttonImage.color;

        if (otherImage1 != null)
        {
            otherImage1OriginalColor = otherImage1.color;
        }

        if (otherImage2 != null)
        {
            otherImage2OriginalColor = otherImage2.color;
        }

        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        StartCoroutine(ChangeColorTemporarily());
    }

    IEnumerator ChangeColorTemporarily()
    {
        buttonImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0); // Set button to transparent

        if (otherImage1 != null)
        {
            otherImage1.color = new Color(otherImage1OriginalColor.r, otherImage1OriginalColor.g, otherImage1OriginalColor.b, 0); // Set other image 1 to transparent
        }

        if (otherImage2 != null)
        {
            otherImage2.color = new Color(otherImage2OriginalColor.r, otherImage2OriginalColor.g, otherImage2OriginalColor.b, 0); // Set other image 2 to transparent
        }

        yield return new WaitForSeconds(2);

        buttonImage.color = originalColor; // Revert button to original color

        if (otherImage1 != null)
        {
            otherImage1.color = otherImage1OriginalColor; // Revert other image 1 to original color
        }

        if (otherImage2 != null)
        {
            otherImage2.color = otherImage2OriginalColor; // Revert other image 2 to original color
        }
    }
}
