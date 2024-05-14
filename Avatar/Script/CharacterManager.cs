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
    private int check = 0;

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
        string gender = AvatarGender.val;
        if (gender == "Male-Avatar" && check == 0)
        {
            selectedOption = 0;
            check = 1;
        }
        else if (gender == "Female-Avatar" && check == 0)
        {
            selectedOption = 5;
            check = 1;
        }
        if(check == 1)
        {
           
            UpdateCharacter(selectedOption);
        }


    }
    public void NextOption()
    {
        selectedOption++;
        string gender = AvatarGender.val;
        int maxMaleIndex = 4;
        int minFemaleIndex = 5;
        int maxFemaleIndex = 9;

        if (gender == "Male-Avatar")
        {
            if (selectedOption < 0 || selectedOption > maxMaleIndex)
            {
                selectedOption = 0;
            }
        }
        else if (gender == "Female-Avatar")
        {
            if (selectedOption < minFemaleIndex || selectedOption > maxFemaleIndex)
            {
                selectedOption = minFemaleIndex;
            }
        }

        Destroy(art3d.gameObject);
        UpdateCharacter(selectedOption);

    }
    public void BackOption()
    {
        selectedOption--;
        string gender = AvatarGender.val;
        int maxMaleIndex = 4;
        int minFemaleIndex = 5;
        int maxFemaleIndex = 9;
        if (gender == "Male-Avatar")
        {
            if (selectedOption < 0 || selectedOption > maxMaleIndex)
            {
                selectedOption = maxMaleIndex;
            }
        }
        else if (gender == "Female-Avatar")
        {
            if (selectedOption < minFemaleIndex || selectedOption > maxFemaleIndex)
            {
                selectedOption = maxFemaleIndex;
            }
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
    private void Starter(int selectedOption)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        string gender = AvatarGender.val;

        if (gender == "Male-Avatar")
        {
            Debug.LogWarning(gender);
            selectedOption = 0;
        }
        else
        {
            Debug.LogWarning(gender);
            selectedOption = 5;
        }

        Character character = characterDB.GetCharacter(selectedOption);
        GameObject characterObject = Instantiate(character.characters3D);
        MeshRenderer meshRenderer = characterObject.GetComponent<MeshRenderer>();
        art3d.material = meshRenderer.sharedMaterial;
        art3d = meshRenderer;
        characterObject.transform.position = new Vector3((float)-0.37, (float)-0.88, (float)0.4499969);
        characterObject.transform.rotation = characterObject.transform.localRotation = Quaternion.Euler((float)19.454, (float)196.335, (float)4.901);
        characterObject.transform.localScale = new Vector3((float)1.89441, (float)1.498345, (float)1);
    }
    

}


