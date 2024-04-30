using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterManager : MonoBehaviour
{
    public CharacterDatabase characterDB;
    public MeshRenderer art3d;
    public TextMeshProUGUI nameText;
    private int selectedOption = 0;

    void Start()
    {
        if (!PlayerPrefs.HasKey("selectedOption"))
        {
            selectedOption = 0;
        }
        else
        {
            Load();
        }
        UpdateCharacter(selectedOption);
    }
    public void NextOption()
    {
       
        selectedOption++;

        if (selectedOption >= characterDB.characterCount)
        {
            selectedOption = 0;
        }
        Destroy(art3d.gameObject);
        UpdateCharacter(selectedOption);
      
    }
    public void BackOption()
    {
       
        selectedOption--;
        if (selectedOption < 0)
        {
            selectedOption = characterDB.characterCount - 1;
        }
        Destroy(art3d.gameObject);
        UpdateCharacter(selectedOption);
        
    }

    private void UpdateCharacter(int selectedOption)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Character character = characterDB.GetCharacter(selectedOption);
            GameObject characterObject = Instantiate(character.characters3D);
            MeshRenderer meshRenderer = characterObject.GetComponent<MeshRenderer>();
            art3d.material = meshRenderer.sharedMaterial;
            art3d = meshRenderer;
            nameText.text = character.characterName;
            characterObject.transform.position = new Vector3((float)-0.29, (float)-1.055561, (float)0.4895401);
            characterObject.transform.rotation = characterObject.transform.localRotation = Quaternion.Euler((float)19.454, (float)196.335, (float)4.901);
            characterObject.transform.localScale = new Vector3((float)1.89441, (float)1.498345, (float)1);

    }
    private void Load()
    {
        selectedOption = PlayerPrefs.GetInt("selectedOption");
    }
    public void Save()
    {
        PlayerPrefs.SetInt("selectedOption", selectedOption);
    }
    public void ChangeScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
    
}

 
