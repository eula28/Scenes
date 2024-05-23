using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Try : MonoBehaviour
{
    public Button IdeaButton,CaptureButton,BackButton, exit, JuanitoButton;
    public GameObject IdeaButtonPanel, juanitoPanel;

    public void ideabutton() {
        IdeaButtonPanel.SetActive(true);
        IdeaButton.gameObject.SetActive(false);
        CaptureButton.gameObject.SetActive(false);
        BackButton.gameObject.SetActive(false);
        juanitoPanel.SetActive(false);
    }
    public void Exit()
    {
        IdeaButtonPanel.SetActive(false);
        IdeaButton.gameObject.SetActive(true);
        CaptureButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);

    }

    public void Juanitobtn()
    {
        juanitoPanel.SetActive(true);
        IdeaButtonPanel.SetActive(false);
        IdeaButton.gameObject.SetActive(true);
        CaptureButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);

    }
    public void JuanitoPanelRemove(){
        juanitoPanel.SetActive(false);
    }



}
