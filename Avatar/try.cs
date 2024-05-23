using UnityEngine;
using UnityEngine.UI;

public class Try : MonoBehaviour
{
    public Button exit, juanito;
    public GameObject panel, juanitoP;

   public void Exit()
    {
        panel.SetActive(false);
    }
    public void Juanito()
    {
        juanitoP.SetActive(true);
        panel.SetActive(false);
    }



}
