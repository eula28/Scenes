using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TransparencyController : MonoBehaviour
{
    public Image targetImage;  // The image whose transparency you want to change
    public Button yourButton;  // The button that triggers the action

    void Start()
    {
        if (yourButton != null)
        {
            yourButton.onClick.AddListener(ChangeTransparency);
        }
    }

    void ChangeTransparency()
    {
        if (targetImage != null)
        {
            // Start the coroutine to change transparency
            StartCoroutine(ChangeTransparencyCoroutine());
        }
    }

    IEnumerator ChangeTransparencyCoroutine()
    {
        // Set transparency to 30%
        SetTransparency(0.3f);
        
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);
        
        // Set transparency to 0% (fully transparent)
        SetTransparency(0f);
    }

    void SetTransparency(float alpha)
    {
        Color color = targetImage.color;
        color.a = alpha;
        targetImage.color = color;
    }
}
