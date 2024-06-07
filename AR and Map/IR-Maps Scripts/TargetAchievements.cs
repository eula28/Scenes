using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TargetAchievements : MonoBehaviour
{
    string userId;
    FirebaseFirestore db;
    bool isFound;
    AchievementScript achievementScript = new AchievementScript();

    // Start is called before the first frame update
    void Start()
    {
        if (FirebaseController.Instance != null)
        {
            userId = FirebaseController.Instance.auth.CurrentUser.UserId;
            db = FirebaseFirestore.DefaultInstance;
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }

    public async Task CheckFirestoreUserTarget(string targetname)
    {
        DocumentReference userRef = db.Collection("users").Document(userId).Collection("userTargets").Document(targetname);
        DocumentSnapshot doc = await userRef.GetSnapshotAsync();
        if (doc.Exists)
        {
            // Access the data from the document
            isFound = doc.GetValue<bool>("target found");
            Debug.Log("Target Counted" + isFound);
        }
        else
        {
            Debug.Log("Document does not exist for user: " + userId);
            isFound = false;
        }
    }

    public void UpdateUserTarget(string targetname)
    {
        if (FirebaseController.Instance != null)
        {
            Dictionary<string, object> userTarget = new Dictionary<string, object>
            {
                { "target found", true }
            };
            DocumentReference userUpdateDocRef = db.Collection("users").Document(userId).Collection("userTargets").Document(targetname);

            userUpdateDocRef.UpdateAsync(userTarget).ContinueWithOnMainThread(async task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to update user data: " + task.Exception);
                    return;
                }
                else
                {
                    Debug.Log("User data updated successfully for user: " + FirebaseController.Instance.auth.CurrentUser.UserId);

                    // Increment the discoveries field in the users collection
                    DocumentReference userDocRef = db.Collection("users").Document(userId);
                    Dictionary<string, object> updates = new Dictionary<string, object>
                    {
                        { "discoveries", FieldValue.Increment(1) }
                    };

                    try
                    {
                        await userDocRef.UpdateAsync(updates);
                        Debug.Log("Discoveries field incremented successfully.");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Failed to increment discoveries field: " + e.Message);
                    }
                }
            });
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }


    public async void Juanito()
    {
        string name = "Juanito Reyes Remulla Statue";
        await CheckFirestoreUserTarget(name);

        if (!isFound)
        {
            UpdateUserTarget(name);
            Dictionary<string, object> userActions = new Dictionary<string, object>
            {
                { "5AR", 1 },
                { "10AR", 1 },
                { "15AR", 1 }
            };
            // Use the user ID instead of the username
            await achievementScript.UpdateUserAchievements(userId, userActions);
            Debug.Log("Target Counted");
        }
    }

    public async void CathedralMarker()
    {
        string name = "Imus Cathedral - Marker";
        await CheckFirestoreUserTarget(name);

        if (!isFound)
        {
            UpdateUserTarget(name);
            Dictionary<string, object> userActions = new Dictionary<string, object>
            {
                { "5AR", 1 },
                { "10AR", 1 },
                { "15AR", 1 }
            };
            // Use the user ID instead of the username
            await achievementScript.UpdateUserAchievements(userId, userActions);
            Debug.Log("Target Counted");
        }
    }

    public async void Heritage()
    {
        string name = "Imus Cathedral - Heritage Bells";
        await CheckFirestoreUserTarget(name);

        if (!isFound)
        {
            UpdateUserTarget(name);
            Dictionary<string, object> userActions = new Dictionary<string, object>
            {
                { "5AR", 1 },
                { "10AR", 1 },
                { "15AR", 1 }
            };
            // Use the user ID instead of the username
            await achievementScript.UpdateUserAchievements(userId, userActions);
            Debug.Log("Target Counted");
        }
    }

    public async void Cathedral()
    {
        string name = "Imus Cathedral - The Diocesan Shrine and Parish of Our Lady of the Pillar";
        await CheckFirestoreUserTarget(name);

        if (!isFound)
        {
            UpdateUserTarget(name);
            Dictionary<string, object> userActions = new Dictionary<string, object>
            {
                { "5AR", 1 },
                { "10AR", 1 },
                { "15AR", 1 }
            };
            // Use the user ID instead of the username
            await achievementScript.UpdateUserAchievements(userId, userActions);
            Debug.Log("Target Counted");
        }
    }

    public async void Carabao()
    {
        string name = "Imus City Plaza - Carabao's Club";
        await CheckFirestoreUserTarget(name);

        if (!isFound)
        {
            UpdateUserTarget(name);
            Dictionary<string, object> userActions = new Dictionary<string, object>
            {
                { "5AR", 1 },
                { "10AR", 1 },
                { "15AR", 1 }
            };
            // Use the user ID instead of the username
            await achievementScript.UpdateUserAchievements(userId, userActions);
            Debug.Log("Target Counted");
        }
    }

    public async void Plaza()
    {
        string name = "Imus City Plaza - Logo";
        await CheckFirestoreUserTarget(name);

        if (!isFound)
        {
            UpdateUserTarget(name);
            Dictionary<string, object> userActions = new Dictionary<string, object>
            {
                { "5AR", 1 },
                { "10AR", 1 },
                { "15AR", 1 }
            };
            // Use the user ID instead of the username
            await achievementScript.UpdateUserAchievements(userId, userActions);
            Debug.Log("Target Counted");
        }
    }

    public async void GenTopacio()
    {
        string name = "Imus City Plaza - General Licerio Topacio Statue";
        await CheckFirestoreUserTarget(name);

        if (!isFound)
        {
            UpdateUserTarget(name);
            Dictionary<string, object> userActions = new Dictionary<string, object>
            {
                { "5AR", 1 },
                { "10AR", 1 },
                { "15AR", 1 }
            };
            // Use the user ID instead of the username
            await achievementScript.UpdateUserAchievements(userId, userActions);
            Debug.Log("Target Counted");
        }
    }

    public async void Pillar()
    {
        string name = "Pillar Lodge No.3";
        await CheckFirestoreUserTarget(name);

        if (!isFound)
        {
            UpdateUserTarget(name);
            Dictionary<string, object> userActions = new Dictionary<string, object>
            {
                { "5AR", 1 },
                { "10AR", 1 },
                { "15AR", 1 }
            };
            // Use the user ID instead of the username
            await achievementScript.UpdateUserAchievements(userId, userActions);
            Debug.Log("Target Counted");
        }
    }

    public async void TulayTomas()
    {
        string name = "Tulay Tomas Mascardo";
        await CheckFirestoreUserTarget(name);

        if (!isFound)
        {
            UpdateUserTarget(name);
            Dictionary<string, object> userActions = new Dictionary<string, object>
            {
                { "5AR", 1 },
                { "10AR", 1 },
                { "15AR", 1 }
            };
            // Use the user ID instead of the username
            await achievementScript.UpdateUserAchievements(userId, userActions);
            Debug.Log("Target Counted");
        }
    }

    public async void ImusHistorical()
    {
        string name = "Imus Historical Landmark";
        await CheckFirestoreUserTarget(name);

        if (!isFound)
        {
            UpdateUserTarget(name);
            Dictionary<string, object> userActions = new Dictionary<string, object>
            {
                { "5AR", 1 },
                { "10AR", 1 },
                { "15AR", 1 }
            };
            // Use the user ID instead of the username
            await achievementScript.UpdateUserAchievements(userId, userActions);
            Debug.Log("Target Counted");
        }
    }

    public async void Labanan()
    {
        string name = "Labanan sa Imus";
        await CheckFirestoreUserTarget(name);

        if (!isFound)
        {
            UpdateUserTarget(name);
            Dictionary<string, object> userActions = new Dictionary<string, object>
            {
                { "5AR", 1 },
                { "10AR", 1 },
                { "15AR", 1 }
            };
            // Use the user ID instead of the username
            await achievementScript.UpdateUserAchievements(userId, userActions);
            Debug.Log("Target Counted");
        }
    }

    public async void Bridge()
    {
        string name = "Bridge of Isabel";
        await CheckFirestoreUserTarget(name);

        if (!isFound)
        {
            UpdateUserTarget(name);
            Dictionary<string, object> userActions = new Dictionary<string, object>
            {
                { "5AR", 1 },
                { "10AR", 1 },
                { "15AR", 1 }
            };
            // Use the user ID instead of the username
            await achievementScript.UpdateUserAchievements(userId, userActions);
            Debug.Log("Target Counted");
        }
    }
}