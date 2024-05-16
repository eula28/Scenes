using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using ReadyPlayerMe.Core.WebView;
using Firebase.Firestore;
using Firebase.Extensions;

public class CharacterManager : MonoBehaviour
{
    public CharacterDatabase characterDB;
    public MeshRenderer art3d;
    public TextMeshProUGUI nameText;
    private int selectedOption;
    private int check = 0;
    private string gender;

    void Start()
    {
        if (FirebaseController.Instance != null)
        {
            FirebaseController.Instance.GetUserModelGender(FirebaseController.Instance.user.UserId, DisplayUserModelGender);
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }

        if (!PlayerPrefs.HasKey("selectedOption"))
        {
            selectedOption = 0;
        }
        else
        {
            Load();
        }

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

        if (check == 1)
        {
            UpdateCharacter(selectedOption);
        }
    }

    public void NextOption()
    {
        selectedOption++;
        UpdateSelectedOption();
        UpdateCharacter(selectedOption);
    }

    public void BackOption()
    {
        selectedOption--;
        UpdateSelectedOption();
        UpdateCharacter(selectedOption);
    }

    private void UpdateSelectedOption()
    {
        int maxMaleIndex = 4;
        int minFemaleIndex = 5;
        int maxFemaleIndex = 9;

        if (gender == "Male-Avatar")
        {
            if (selectedOption < 0)
            {
                selectedOption = maxMaleIndex;
            }
            else if (selectedOption > maxMaleIndex)
            {
                selectedOption = 0;
            }
        }
        else if (gender == "Female-Avatar")
        {
            if (selectedOption < minFemaleIndex)
            {
                selectedOption = maxFemaleIndex;
            }
            else if (selectedOption > maxFemaleIndex)
            {
                selectedOption = minFemaleIndex;
            }
        }
        PlayerPrefs.SetInt("selectedOption", selectedOption);
        PlayerPrefs.Save();
    }

    private void UpdateCharacter(int selectedOption)
    {
        // Destroy all previous children (previously instantiated models)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Get the new character
        Character character = characterDB.GetCharacter(selectedOption);
        GameObject characterObject = Instantiate(character.characters3D, transform); // Instantiate as a child of current transform
        MeshRenderer meshRenderer = characterObject.GetComponent<MeshRenderer>();

        // Update the renderer and UI
        art3d.material = meshRenderer.sharedMaterial;
        art3d = meshRenderer;
        nameText.text = character.characterName;

        characterObject.transform.position = new Vector3((float)-0.29, (float)-1.055561, (float)0.4895401);
        characterObject.transform.rotation = characterObject.transform.localRotation = Quaternion.Euler((float)20, (float)180, (float)0);
        characterObject.transform.localScale = new Vector3((float)1, (float)1, (float)1);
    }

    private void Load()
    {
        selectedOption = PlayerPrefs.GetInt("selectedOption", 0);
    }

    public void Save()
    {
        updateUserModel();
    }

    public void ChangeScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }

    private void DisplayUserModelGender(string gendermodel, int modelnumber)
    {
        selectedOption = modelnumber;
        gender = gendermodel;
        Debug.Log("gender: " + gendermodel + " model: " + modelnumber);
    }

    public void updateUserModel()
    {
        if (FirebaseController.Instance != null)
        {
            Dictionary<string, object> userUpdate = new Dictionary<string, object>
            {
                { "model number", selectedOption }
            };

            string userId = FirebaseController.Instance.auth.CurrentUser.UserId;
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference userUpdateDocRef = db.Collection("users").Document(userId);

            userUpdateDocRef.UpdateAsync(userUpdate).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to update user data: " + task.Exception);
                    return;
                }
                else
                {
                    SceneManager.LoadScene("Account");
                    Debug.Log("User data updated successfully for user: " + FirebaseController.Instance.auth.CurrentUser.UserId);
                }
            });
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }
}


