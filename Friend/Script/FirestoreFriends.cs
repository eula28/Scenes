using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;
using System;
using Firebase.Extensions;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class FirestoreFriends : MonoBehaviour
{
    AchievementScript achievementScript = new AchievementScript();
    public Transform friendsListParent, friendRequestsListParent, searchFriendListParent;
    public GameObject friendPrefab, requestPrefab, searchPrefab, friendPanel, invitesPanel, searchPanel;
    public TextMeshProUGUI friendCountText, requestCountText;
    public ProfileDatabase profileDB;
    string userId;
    string username;
    public TMP_InputField searchReceiverUsername;
 

    private async void Start()
    {
        await InitializeUser();
    }

    async System.Threading.Tasks.Task InitializeUser()
    {
        if (FirebaseController.Instance != null)
        {
            userId = FirebaseController.Instance.auth.CurrentUser.UserId;
            await GetUsername(userId, username =>
            {
                this.username = username;
                if (string.IsNullOrEmpty(username))
                {
                    Debug.LogError("Failed to get username.");
                }
                else
                {
                    OpenFriendPanel();
                }
            });
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }

    async System.Threading.Tasks.Task GetUsername(string userId, Action<string> callback)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        await db.Collection("users").Document(userId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error getting username: " + task.Exception);
                callback(null);
                return;
            }

            DocumentSnapshot document = task.Result;
            if (document.Exists)
            {
                string username = document.GetValue<string>("username");
                callback(username);
            }
            else
            {
                Debug.LogError("No such document!");
                callback(null);
            }
        });
    }

    void PerformFirestoreOperation(Action<FirebaseFirestore, string> operation)
    {
        if (!string.IsNullOrEmpty(username))
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            operation(db, username);
        }
        else
        {
            Debug.LogError("Username is not initialized.");
        }
    }

    public void SendFriendRequest(string friendUsername)
    {
        PerformFirestoreOperation(async (db, requesterUsername) =>
        {
            DocumentReference friendRequestRef = db.Collection("friendRequests").Document($"{requesterUsername}_{friendUsername}");
            await friendRequestRef.SetAsync(new Dictionary<string, object>
            {
                { "requester", requesterUsername },
                { "receiver", friendUsername },
                { "status", "pending" }
            }).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error sending friend request: " + task.Exception);
                }
                else
                {
                    Debug.Log("Friend request sent successfully.");
                    // Remove the prefab from the UI
                    Transform itemToRemove = searchFriendListParent.Find(friendUsername);
                    if (itemToRemove != null)
                    {
                        Destroy(itemToRemove.gameObject);
                    }
                }
            });
        });
    }

    public void AcceptFriendRequest(string requesterUsername)
    {
        PerformFirestoreOperation(async (db, receiverUsername) =>
        {
            // Get the user ID of the requester
            string requesterUserId = await GetUserIdByUsername(db, requesterUsername);

            DocumentReference friendRequestRef = db.Collection("friendRequests").Document($"{requesterUsername}_{receiverUsername}");
            await friendRequestRef.DeleteAsync().ContinueWithOnMainThread(async task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error accepting friend request: " + task.Exception);
                }
                else
                {
                    await CreateFriendRelationship(db, requesterUsername, receiverUsername);
                    await CreateFriendRelationship(db, receiverUsername, requesterUsername);
                    Dictionary<string, object> userActions = new Dictionary<string, object>
                {
                    { "5Friends", 1 },
                    { "10Friends", 1 },
                    { "15Friends", 1 }
                };
                    // Use the user ID instead of the username
                    await achievementScript.UpdateUserAchievements(requesterUserId, userActions);
                    await achievementScript.UpdateUserAchievements(userId, userActions);

                    Debug.Log("Friend request accepted.");
                    // Remove the prefab from the UI
                    Transform itemToRemove = friendRequestsListParent.Find(requesterUsername);
                    if (itemToRemove != null)
                    {
                        Destroy(itemToRemove.gameObject);
                    }
                }
            });
        });
    }

    // Method to get the user ID by username
    private async Task<string> GetUserIdByUsername(FirebaseFirestore db, string username)
    {
        try
        {
            var snapshot = await db.Collection("users").WhereEqualTo("username", username).GetSnapshotAsync();

            if (snapshot.Documents.Count() > 0)
            {
                var userDoc = snapshot.Documents.First();
                return userDoc.Id;
            }
            else
            {
                Debug.LogError($"User with username '{username}' not found.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error getting user ID by username: {ex.Message}");
            return null;
        }
    }

    public void RejectFriendRequest(string requesterUsername)
    {
        PerformFirestoreOperation(async (db, receiverUsername) =>
        {
            DocumentReference friendRequestRef = db.Collection("friendRequests").Document($"{requesterUsername}_{receiverUsername}");
            await friendRequestRef.DeleteAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error rejecting friend request: " + task.Exception);
                }
                else
                {
                    Debug.Log("Friend request rejected.");
                    // Remove the prefab from the UI
                    Transform itemToRemove = friendRequestsListParent.Find(requesterUsername);
                    if (itemToRemove != null)
                    {
                        Destroy(itemToRemove.gameObject);
                    }
                }
            });
        });
    }

    async System.Threading.Tasks.Task CreateFriendRelationship(FirebaseFirestore db, string user1, string user2)
    {
        DocumentReference friendRef = db.Collection("friendRelationships").Document($"{user1}_{user2}");
        await friendRef.SetAsync(new Dictionary<string, object>
        {
            { "user1", user1 },
            { "user2", user2 },
            { "status", "friends" }
        });
    }

    public void DisplayFriends()
    {
        PerformFirestoreOperation(async (db, currentUsername) =>
        {
            try
            {
                var snapshot = await db.Collection("friendRelationships")
                    .WhereEqualTo("user1", currentUsername)
                    .WhereEqualTo("status", "friends")
                    .GetSnapshotAsync();

                // Convert documents to a list and count them
                List<DocumentSnapshot> documents = snapshot.Documents.ToList();
                int friendCount = documents.Count;
                friendCountText.text = "Friends (" + friendCount.ToString() + ")";

                // Clear previous entries
                ClearUIList(friendsListParent);

                foreach (DocumentSnapshot document in documents)
                {
                    string friendUsername = document.GetValue<string>("user2");
                    AddFriendToUI(friendsListParent, friendUsername, "friend");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error getting friends: " + ex);
            }
        });
    }

    public void DisplayFriendRequests()
    {
        PerformFirestoreOperation(async (db, currentUsername) =>
        {
            try
            {
                var snapshot = await db.Collection("friendRequests")
                    .WhereEqualTo("receiver", currentUsername)
                    .WhereEqualTo("status", "pending")
                    .GetSnapshotAsync();

                // Convert documents to a list and count them
                List<DocumentSnapshot> documents = snapshot.Documents.ToList();
                int requestCount = documents.Count;
                requestCountText.text = "Invites (" + requestCount.ToString() + ")";

                // Clear previous entries
                ClearUIList(friendRequestsListParent);

                foreach (DocumentSnapshot document in documents)
                {
                    string requesterUsername = document.GetValue<string>("requester");
                    AddFriendToUI(friendRequestsListParent, requesterUsername, "request");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error getting friend requests: " + ex);
            }
        });
    }

    void AddFriendToUI(Transform parent, string friendUsername, string prefabType)
    {
        PerformFirestoreOperation(async (db, currentUsername) =>
        {
            try
            {
                var querySnapshot = await db.Collection("users")
                    .WhereEqualTo("username", friendUsername)
                    .GetSnapshotAsync();

                if (!querySnapshot.Documents.Any())
                {
                    Debug.LogError("User document not found for username: " + friendUsername);
                    return;
                }

                DocumentSnapshot userDocument = querySnapshot.Documents.FirstOrDefault();
                int modelNumber = userDocument.GetValue<int>("model number");
                int points = userDocument.GetValue<int>("points");

                GameObject friendItem = null;

                if (prefabType == "friend")
                {
                    friendItem = Instantiate(friendPrefab, parent);
                }
                else if (prefabType == "request")
                {
                    friendItem = Instantiate(requestPrefab, parent);
                    Button acceptButton = friendItem.transform.Find("AcceptButton")?.GetComponent<Button>();
                    Button rejectButton = friendItem.transform.Find("RejectButton")?.GetComponent<Button>();

                    if (acceptButton != null)
                        acceptButton.onClick.AddListener(() => AcceptFriendRequest(friendUsername));
                    else
                        Debug.LogError("AcceptButton not found in requestPrefab.");

                    if (rejectButton != null)
                        rejectButton.onClick.AddListener(() => RejectFriendRequest(friendUsername));
                    else
                        Debug.LogError("RejectButton not found in requestPrefab.");
                }
                else if (prefabType == "search")
                {
                    friendItem = Instantiate(searchPrefab, parent);
                    Button addButton = friendItem.transform.Find("AddButton")?.GetComponent<Button>();

                    if (addButton != null)
                        addButton.onClick.AddListener(() => SendFriendRequest(friendUsername));
                    else
                        Debug.LogError("AddButton not found in searchPrefab.");
                }

                if (friendItem != null)
                {
                    friendItem.name = friendUsername;
                    friendItem.transform.localScale = Vector3.one; // Ensure the scale is (1, 1, 1)
                    RectTransform rectTransform = friendItem.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(200, 50); // Set to desired size
                    TMP_Text[] textFields = friendItem.GetComponentsInChildren<TMP_Text>();
                    Image[] profileImageComponents = friendItem.GetComponentsInChildren<Image>();
                    // Handle different prefab types with different numbers of TMP_Text components
                    if (prefabType == "friend" && textFields.Length >= 2)
                    {
                        textFields[0].text = friendUsername;
                        textFields[1].text = points.ToString();
                        // add profile picture using modelNumber

                        // Add profile picture using modelNumber
                        if (modelNumber >= 0 && modelNumber < profileDB.profileCount)
                        {
                            Sprite profileSprite = profileDB.GetCharacter(modelNumber).profileSprite;
                           

                            if (profileImageComponents != null && profileImageComponents.Length > 0)
                            {
                                // Assuming we want the first Image component found
                                Image profileImageComponent = profileImageComponents[2];

                                if (profileImageComponent != null)
                                {
                                    profileImageComponent.sprite = profileSprite;
                                }
                            }
                        }


                    }
                    else if (prefabType == "request" && textFields.Length >= 1)
                    {
                        textFields[0].text = friendUsername;
                        // add profile picture using modelNumber

                        // Add profile picture using modelNumber
                        if (modelNumber >= 0 && modelNumber < profileDB.profileCount)
                        {
                            Sprite profileSprite = profileDB.GetCharacter(modelNumber).profileSprite;


                            if (profileImageComponents != null && profileImageComponents.Length > 0)
                            {
                                // Assuming we want the first Image component found
                                Image profileImageComponent = profileImageComponents[2];

                                if (profileImageComponent != null)
                                {
                                    profileImageComponent.sprite = profileSprite;
                                }
                            }
                        }
                    }
                    else if (prefabType == "search" && textFields.Length >= 1)
                    {
                        textFields[0].text = friendUsername;
                        // add profile picture using modelNumber
                        // Add profile picture using modelNumber
                        if (modelNumber >= 0 && modelNumber < profileDB.profileCount)
                        {
                            Sprite profileSprite = profileDB.GetCharacter(modelNumber).profileSprite;


                            if (profileImageComponents != null && profileImageComponents.Length > 0)
                            {
                                // Assuming we want the first Image component found
                                Image profileImageComponent = profileImageComponents[2];

                                if (profileImageComponent != null)
                                {
                                    profileImageComponent.sprite = profileSprite;
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Insufficient TMP_Text components found in friendItemPrefab.");
                    }

                    Debug.Log("Friend added to UI: " + friendUsername);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error adding friend to UI: " + ex);
            }
        });
    }

    void ClearUIList(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    public void SearchUsers()
    {
        string searchTerm = searchReceiverUsername.text.Trim();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            PerformFirestoreOperation(async (db, currentUsername) =>
            {
                try
                {
                    var querySnapshot = await db.Collection("users")
                        .WhereEqualTo("username", searchTerm)
                        .WhereNotEqualTo("username", currentUsername)
                        .GetSnapshotAsync();

                    ClearUIList(friendsListParent);

                    foreach (DocumentSnapshot document in querySnapshot.Documents)
                    {
                        string foundUsername = document.GetValue<string>("username");

                        var friendRequestSnapshot = await db.Collection("friendRequests")
                            .WhereEqualTo("requester", currentUsername)
                            .WhereEqualTo("receiver", foundUsername)
                            .WhereEqualTo("status", "pending")
                            .GetSnapshotAsync();

                        if (friendRequestSnapshot.Documents.Count() == 0)
                        {
                            AddFriendToUI(searchFriendListParent, foundUsername, "search");
                            searchReceiverUsername.text = "";
                            Debug.Log("User Found!." + foundUsername);
                        }
                        else
                        {
                            Debug.Log("Friend request already sent.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error searching users: " + ex);
                }
            });
        }
    }

    public void OpenFriendPanel()
    {
        friendPanel.SetActive(true);
        invitesPanel.SetActive(false);
        searchPanel.SetActive(false);
        DisplayFriends();
    }

    public void OpenInvitesPanel()
    {
        friendPanel.SetActive(false);
        invitesPanel.SetActive(true);
        searchPanel.SetActive(false);
        DisplayFriendRequests();
    }

    public void OpenSearchPanel()
    {
        friendPanel.SetActive(false);
        invitesPanel.SetActive(false);
        searchPanel.SetActive(true);
    }

    public void OpenAccount()
    {
        SceneManager.LoadScene("Account");
    }
}
