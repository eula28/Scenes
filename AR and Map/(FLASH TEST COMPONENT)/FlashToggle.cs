using UnityEngine;
using Vuforia;

public class FlashToggle : MonoBehaviour
{
    private bool isFlashOn = false;

    public void ToggleFlash()
    {
        isFlashOn = !isFlashOn;
        VuforiaBehaviour.Instance.CameraDevice.SetFlash(isFlashOn);
    }
}
