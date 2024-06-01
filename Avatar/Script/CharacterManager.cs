using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public CharacterDatabase characterDB;
    public MeshRenderer art3d;
    public TextMeshProUGUI nameText;
    private int selectedOption;
    private string gender;
    private bool isDataLoaded = false;
    string userId;
    int user = 0;
    public Button save;

    public TextMeshProUGUI userPointsText;
    public TextMeshProUGUI TaskNeeded;
    public Image TaskBg;

    void Start()
    {
        if (FirebaseController.Instance != null)
        {
            FirebaseController.Instance.GetUserModelGender(FirebaseController.Instance.user.UserId, DisplayUserModelGender);
            userId = FirebaseController.Instance.user.UserId;
            DisplayUserAchievements(userId);
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }

        StartCoroutine(InitializeCharacter());
    }
    
    private IEnumerator InitializeCharacter()
    {
        // Destroy all previous children (previously instantiated models)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        yield return new WaitUntil(() => isDataLoaded);
        Debug.Log("Data loaded from Firebase.");
        UpdateCharacter(selectedOption);
    }
    
    void DisplayUserAchievements(string userId)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference userRef = db.Collection("users").Document(userId);

        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to fetch user data: " + task.Exception);
                return;
            }

            DocumentSnapshot userSnapshot = task.Result;
            if (!userSnapshot.Exists)
            {
                Debug.LogWarning("User not found.");
                return;
            }

            int userPoints = userSnapshot.GetValue<int>("points");
            Debug.Log("Checked1.");
            UpdateUserPointsUI(userPoints);


        });
    }
    void UpdateUserPointsUI(int userPoints)
    {
        userPointsText.text = userPoints.ToString();
        user = userPoints;
        Debug.Log("Checked:P.");
    }

    public void NextOption()
    {
        selectedOption++;
        UpdateSelectedOption();
        UpdateCharacter(selectedOption);

        AvatarUnlocked();

        
    }

    public void BackOption()
    {
        selectedOption--;
        UpdateSelectedOption();
        UpdateCharacter(selectedOption);
        AvatarUnlocked();
    }

    public void AvatarUnlocked()
    {
        if (gender == "Male-Avatar")
        {
            if (selectedOption == 1)
            {
                if (user >= 350)
                {
                    Unlocked();
                }
                else
                {
                    TaskBg.gameObject.SetActive(true);
                    TaskNeeded.text = "Earn 300 points to get this Avatar".ToString();
                    save.interactable = false;
                    Debug.Log("Selected Option " + selectedOption);
                }

            }
            else if (selectedOption == 2)
            {
                if (user >= 400)
                {
                    Unlocked();
                }
                else
                {
                    TaskBg.gameObject.SetActive(true);
                    TaskNeeded.text = "Earn 400 points to get this Avatar".ToString();
                    save.interactable = false;
                    Debug.Log("Selected Option " + selectedOption);
                }

            }
            else if (selectedOption == 3)
            {
                if (user >= 450)
                {
                    Unlocked();
                }
                else
                {
                    TaskBg.gameObject.SetActive(true);
                    TaskNeeded.text = "Earn 450 points to get this Avatar".ToString();
                    save.interactable = false;
                    Debug.Log("Selected Option " + selectedOption);
                }

            }
            else if (selectedOption == 4)
            {
                if (user >= 500)
                {
                    Unlocked();
                }
                else
                {
                    TaskBg.gameObject.SetActive(true);
                    TaskNeeded.text = "Earn 500 points to get this Avatar".ToString();
                    save.interactable = false;
                    Debug.Log("Selected Option " + selectedOption);
                }

            }
            else
            {
                Unlocked();
            }

        }
  
        else if (gender == "Female-Avatar")
        {
            if (selectedOption == 6)
            {
                if (user >= 350)
                {
                    Unlocked();
                }
                else
                {
                    TaskBg.gameObject.SetActive(true);
                    TaskNeeded.text = "Earn 300 points to get this Avatar".ToString();
                    save.interactable = false;
                    Debug.Log("Selected Option " + selectedOption);
                }

            }
            else if (selectedOption == 7)
            {
                if (user >= 400)
                {
                    Unlocked();
                }
                else
                {
                    TaskBg.gameObject.SetActive(true);
                    TaskNeeded.text = "Earn 400 points to get this Avatar".ToString();
                    save.interactable = false;
                    Debug.Log("Selected Option " + selectedOption);
                }

            }
            else if (selectedOption == 8)
            {
                if (user >= 450)
                {
                    Unlocked();
                }
                else
                {
                    TaskBg.gameObject.SetActive(true);
                    TaskNeeded.text = "Earn 450 points to get this Avatar".ToString();
                    save.interactable = false;
                    Debug.Log("Selected Option " + selectedOption);
                }

            }
            else if (selectedOption == 8)
            {
                if (user >= 500)
                {
                    Unlocked();
                }
                else
                {
                    TaskBg.gameObject.SetActive(true);
                    TaskNeeded.text = "Earn 500 points to get this Avatar".ToString();
                    save.interactable = false;
                    Debug.Log("Selected Option " + selectedOption);
                }

            }
            else if (selectedOption == 9)
            {
                if (user >= 550)
                {
                    Unlocked();
                }
                else
                {
                    TaskBg.gameObject.SetActive(true);
                    TaskNeeded.text = "Earn 550 points to get this Avatar".ToString();
                    save.interactable = false;
                    Debug.Log("Selected Option " + selectedOption);
                }

            }
            else
            {
                TaskBg.gameObject.SetActive(false);
                save.interactable = true;
                TaskNeeded.text = "".ToString();
            }
        }
    }
    public void Unlocked()
    {
        TaskBg.gameObject.SetActive(false);
        save.interactable = true;
        TaskNeeded.text = "".ToString();
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

        characterObject.transform.position = new Vector3(-0.29f, -1.055561f, 0.4895401f);
        characterObject.transform.rotation = Quaternion.Euler(20f, 180f, 0f);
        characterObject.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void DisplayUserModelGender(string gendermodel, int modelnumber)
    {
        selectedOption = modelnumber;
        gender = gendermodel;
        isDataLoaded = true;
        Debug.Log("gender: " + gendermodel + " model: " + modelnumber);
    }

    public void Save()
    {
        updateUserModel();
    }

    public void ChangeScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
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
