using UnityEngine;
using UnityEngine.UI;

public class Try : MonoBehaviour
{
    public Button exit, juanito, light;
    public GameObject panel, juanitoP;

   public void Exit()
    {
        panel.gameObject.SetActive(false);
        light.gameObject.SetActive(true);
    }
    public void Light()
    {
        panel.gameObject.SetActive(true);
        light.gameObject.SetActive(false);
    }
    public void Juanito()
    {
        juanitoP.SetActive(true);
        panel.SetActive(false);
    }



}
