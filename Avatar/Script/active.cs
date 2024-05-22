using UnityEngine;
using UnityEngine.SceneManagement;

public class active : MonoBehaviour
{
    public void ChangeScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
}
