using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;
using System;
using Firebase.Extensions;
using System.Linq;
using UnityEngine.SceneManagement;

public class FirestoreFriends : MonoBehaviour
{
    public Transform friendsListParent, friendRequestsListParent, searchFriendListParent;
    public GameObject friendPrefab, requestPrefab, searchPrefab, friendPanel, invitesPanel, searchPanel;

    string userId;
    string username;
    public TMP_InputField searchReceiverUsername;

    private void Start()
    {
        InitializeUser();
    }

    void InitializeUser()
    {
        if (FirebaseController.Instance != null)
        {
            userId = FirebaseController.Instance.auth.CurrentUser.UserId;
            GetUsername(userId, username =>
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

    void GetUsername(string userId, Action<string> callback)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        db.Collection("users").Document(userId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
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
        PerformFirestoreOperation((db, requesterUsername) =>
        {
            DocumentReference friendRequestRef = db.Collection("friendRequests").Document($"{requesterUsername}_{friendUsername}");
            friendRequestRef.SetAsync(new Dictionary<string, object>
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
        PerformFirestoreOperation((db, receiverUsername) =>
        {
            DocumentReference friendRequestRef = db.Collection("friendRequests").Document($"{requesterUsername}_{receiverUsername}");
            friendRequestRef.DeleteAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error accepting friend request: " + task.Exception);
                }
                else
                {
                    CreateFriendRelationship(db, requesterUsername, receiverUsername);
                    CreateFriendRelationship(db, receiverUsername, requesterUsername);
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


    public void RejectFriendRequest(string requesterUsername)
    {
        PerformFirestoreOperation((db, receiverUsername) =>
        {
            DocumentReference friendRequestRef = db.Collection("friendRequests").Document($"{requesterUsername}_{receiverUsername}");
            friendRequestRef.DeleteAsync().ContinueWithOnMainThread(task =>
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


    void CreateFriendRelationship(FirebaseFirestore db, string user1, string user2)
    {
        DocumentReference friendRef = db.Collection("friendRelationships").Document($"{user1}_{user2}");
        friendRef.SetAsync(new Dictionary<string, object>
        {
            { "user1", user1 },
            { "user2", user2 },
            { "status", "friends" }
        });
    }

    public void DisplayFriends()
    {
        PerformFirestoreOperation((db, currentUsername) =>
        {
            db.Collection("friendRelationships").WhereEqualTo("user1", currentUsername).WhereEqualTo("status", "friends").GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error getting friends: " + task.Exception);
                    return;
                }

                ClearUIList(friendsListParent); // Clear previous entries

                foreach (DocumentSnapshot document in task.Result.Documents)
                {
                    string friendUsername = document.GetValue<string>("user2");
                    AddFriendToUI(friendsListParent, friendUsername, "friend");
                }
            });
        });
    }

    public void DisplayFriendRequests()
    {
        PerformFirestoreOperation((db, currentUsername) =>
        {
            db.Collection("friendRequests").WhereEqualTo("receiver", currentUsername).WhereEqualTo("status", "pending").GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error getting friend requests: " + task.Exception);
                    return;
                }

                ClearUIList(friendRequestsListParent); // Clear previous entries

                foreach (DocumentSnapshot document in task.Result.Documents)
                {
                    string requesterUsername = document.GetValue<string>("requester");
                    AddFriendToUI(friendRequestsListParent, requesterUsername, "request");
                }
            });
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

                    // Handle different prefab types with different numbers of TMP_Text components
                    if (prefabType == "friend" && textFields.Length >= 2)
                    {
                        textFields[0].text = friendUsername;
                        textFields[1].text = points.ToString();
                        // add profile picture using modelNumber
                    }
                    else if (prefabType == "request" && textFields.Length >= 1)
                    {
                        textFields[0].text = friendUsername;
                        // add profile picture using modelNumber
                    }
                    else if (prefabType == "search" && textFields.Length >= 1)
                    {
                        textFields[0].text = friendUsername;
                        // add profile picture using modelNumber
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
