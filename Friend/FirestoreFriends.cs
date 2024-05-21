using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FirestoreFriends : MonoBehaviour
{
    string userId;
    string username;
    public TMP_Text receiverUsername;
    public TMP_InputField searchReceiverUsername;

    private void Start()
    {
        GetUsername();
    }

    public void GetUsername()
    {
        if (FirebaseController.Instance != null)
        {
            userId = FirebaseController.Instance.auth.CurrentUser.UserId;
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            db.Collection("users").Document(userId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error getting username: " + task.Exception);
                    return;
                }

                DocumentSnapshot document = task.Result;
                if (document.Exists)
                {
                    string username = document.GetValue<string>("username");

                }
                else
                {
                    Debug.LogError("No such document!");
                }
            });
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
        
    }

    public void SendFriendRequest()
    {
        if (FirebaseController.Instance != null)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference friendRequestRef = db.Collection("friendRequests").Document($"{username}_{receiverUsername.text}");
            friendRequestRef.SetAsync(new Dictionary<string, object>
            {
                { "requester", username },
                { "receiver", receiverUsername.text },
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
                }
            });
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }

    public void AcceptFriendRequest()
    {
        if (FirebaseController.Instance != null)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference friendRequestRef = db.Collection("friendRequests").Document($"{username}_{receiverUsername.text}");
            friendRequestRef.UpdateAsync("status", "accepted").ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error accepting friend request: " + task.Exception);
                }
                else
                {
                    // Create friend relationship
                    DocumentReference friendRef1 = db.Collection("friendRelationships").Document($"{username}_{receiverUsername.text}");
                    friendRef1.SetAsync(new Dictionary<string, object>
            {
                { "user1", username },
                { "user2", receiverUsername.text },
                { "status", "friends" }
            });

                    DocumentReference friendRef2 = db.Collection("friendRelationships").Document($"{receiverUsername.text}_{username}");
                    friendRef2.SetAsync(new Dictionary<string, object>
            {
                { "user1", receiverUsername.text},
                { "user2", username },
                { "status", "friends" }
            });

                    Debug.Log("Friend request accepted.");
                }
            });
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }

    public void DisplayFriends()
    {

        if (FirebaseController.Instance != null)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            db.Collection("friendRelationships").WhereEqualTo("user1", username).WhereEqualTo("status", "friends").GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error getting friends: " + task.Exception);
                    return;
                }

                foreach (DocumentSnapshot document in task.Result.Documents)
                {
                    Debug.Log("Friend: " + document.GetValue<string>("user2"));
                }
            });
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }

    public void DisplayFriendRequests()
    {
        if (FirebaseController.Instance != null)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            db.Collection("friendRequests").WhereEqualTo("receiver", username).WhereEqualTo("status", "pending").GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error getting friend requests: " + task.Exception);
                    return;
                }

                foreach (DocumentSnapshot document in task.Result.Documents)
                {
                    Debug.Log("Friend Request from: " + document.GetValue<string>("requester"));
                }
            });
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }

}
