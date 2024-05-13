using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterDatabase characterDB;
    public MeshRenderer art3d;
    public static int pick;
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
        int p = StyleBtn.p;
        Debug.Log(p);
        if (p == 1)
        {
            Destroy(art3d.gameObject);
            UpdateCharacter(selectedOption);
        }
        else
        {
            Destroy(art3d.gameObject);
            Starter(selectedOption);
        }

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
        characterObject.transform.position = new Vector3((float)-0.37, (float)-0.88, (float)0.4499969);
        characterObject.transform.rotation = characterObject.transform.localRotation = Quaternion.Euler((float)19.454, (float)196.335, (float)4.901);
        characterObject.transform.localScale = new Vector3((float)1.89441, (float)1.498345, (float)1);

    }

    private void Load()
    {
        selectedOption = PlayerPrefs.GetInt("selectedOption");
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

