using UnityEngine;
using UnityEngine.UI;

public class StyleBtn : MonoBehaviour
{
    public Button style;
    public static int p;

    void Start()
    {
        style.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        p = AvatarGender.nbtn;
        p = 1;

    }
}
