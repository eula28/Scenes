using System.Collections;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;

public class Player : MonoBehaviour
{
    public CharacterDatabase characterDB;
    public MeshRenderer art3d;
    private int selectedOption;
    private string gender;
    private bool isDataLoaded = false;

    void Start()
    {
        if (FirebaseController.Instance != null)
        {
            Debug.Log("Fetching user model gender from Firebase...");
            FirebaseController.Instance.GetUserModelGender(FirebaseController.Instance.user.UserId, DisplayUserModelGender);

        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
       StartCoroutine(InitializeCharacter());
        

    }
    
       
       
    
    private IEnumerator InitializeCharacter()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        yield return new WaitUntil(() => isDataLoaded);
        Debug.Log("Data loaded from Firebase.");
        UpdateCharacter(selectedOption);
    }

    private void UpdateCharacter(int selectedOption)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Character character = characterDB.GetCharacter(selectedOption);
        if (character == null)
        {
            Debug.LogError("Character not found for selectedOption: " + selectedOption);
            return;
        }

        GameObject characterObject = Instantiate(character.characters3D);
        MeshRenderer meshRenderer = characterObject.GetComponent<MeshRenderer>();
        art3d.material = meshRenderer.sharedMaterial;
        art3d = meshRenderer;
        characterObject.transform.position = new Vector3(-0.37f, -0.69f, 0.45f);
        characterObject.transform.rotation = Quaternion.Euler(19.454f, 196.335f, 4.901f);
        characterObject.transform.localScale = new Vector3(1.89441f, 1.498345f, 1f);
    }

    private void DisplayUserModelGender(string gendermodel, int modelnumber)
    {
        Debug.Log($"Received gender: {gendermodel}, model number: {modelnumber}");
        selectedOption = modelnumber;
        gender = gendermodel;
        isDataLoaded = true;
    }
}
