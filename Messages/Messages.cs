using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Messages : MonoBehaviour
{
    string userId, receiver, username;
    public TMP_Dropdown friendDropdown;
    private List<string> friendsList = new List<string>();
    public TMP_InputField message;
    public GameObject newMessagePanel;
    FirebaseFirestore db;

    private void Start()
    {
        // Check if the FirebaseController instance exists
        if (FirebaseController.Instance != null)
        {
            userId = FirebaseController.Instance.user.UserId;
            db = FirebaseFirestore.DefaultInstance;
            FetchUsername(userId);
            friendDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }

    public void SendNewMessage()
    {
        if (!friendsList.Contains(receiver))
        {
            Debug.LogError("You can only message your friends.");
            return;
        }

        string sender = username;
        string docId = sender + "_" + receiver;
        DocumentReference messageRef = db.Collection("messages").Document(docId);

        DateTime utcNow = DateTime.UtcNow;
        DateTime phTime;

        try
        {
            // Attempt to find the Philippine time zone
            TimeZoneInfo phTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
            phTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, phTimeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            // If the time zone is not found, create a manual time zone with an 8-hour offset from UTC
            TimeSpan offset = new TimeSpan(8, 0, 0);
            phTime = utcNow.Add(offset);
        }

        // Create a Firestore Timestamp from the DateTime
        Timestamp phTimestamp = Timestamp.FromDateTime(phTime);

        Dictionary<string, object> messageData = new Dictionary<string, object>
        {
            { "sender", sender },
            { "receiver", receiver},
            { "message", message.text },
            { "timestamp", phTimestamp }
        };
        messageRef.SetAsync(messageData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("Message sent");
                newMessagePanel.SetActive(false);
                FetchFriendListByUsername(username);
                message.text = "";

            }
            else
            {
                Debug.LogError("Failed to send message: " + task.Exception);
            }
        });
    }

    public void FetchUsername(string userId)
    {
        // First, fetch the user document to get the username
        DocumentReference userRef = db.Collection("users").Document(userId);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(userTask =>
        {
            if (userTask.IsCompletedSuccessfully)
            {
                DocumentSnapshot userSnapshot = userTask.Result;
                if (userSnapshot.Exists)
                {
                    username = userSnapshot.GetValue<string>("username");
                    if (!string.IsNullOrEmpty(username))
                    {
                        Debug.Log("Username found for user ID: " + username);
                    }
                    else
                    {
                        Debug.LogError("Username not found for user ID: " + userId);
                    }
                }
                else
                {
                    Debug.LogError("User document does not exist for user ID: " + userId);
                }
            }
            else
            {
                Debug.LogError("Failed to fetch user document: " + userTask.Exception);
            }
        });
    }

    private void FetchFriendListByUsername(string username)
    {
        // Clear the friendsList before populating it with new friends
        friendsList.Clear();

        // Fetch friends where user1 matches the username
        db.Collection("friendRelationships")
          .WhereEqualTo("status", "friends")
          .WhereEqualTo("user1", username)
          .GetSnapshotAsync()
          .ContinueWithOnMainThread(task =>
          {
              if (task.IsCompletedSuccessfully)
              {
                  QuerySnapshot snapshot = task.Result;
                  foreach (DocumentSnapshot document in snapshot.Documents)
                  {
                      string user2 = document.GetValue<string>("user2");
                      if (!friendsList.Contains(user2))
                      {
                          friendsList.Add(user2);
                      }
                  }
              }
              else
              {
                  Debug.LogError("Failed to fetch friends: " + task.Exception);
              }

              // Fetch friends where user2 matches the username
              db.Collection("friendRelationships")
                .WhereEqualTo("status", "friends")
                .WhereEqualTo("user2", username)
                .GetSnapshotAsync()
                .ContinueWithOnMainThread(innerTask =>
                {
                    if (innerTask.IsCompletedSuccessfully)
                    {
                        QuerySnapshot innerSnapshot = innerTask.Result;
                        foreach (DocumentSnapshot document in innerSnapshot.Documents)
                        {
                            string user1 = document.GetValue<string>("user1");
                            if (!friendsList.Contains(user1))
                            {
                                friendsList.Add(user1);
                            }
                        }

                        // Once friends are fetched, check for messages
                        CheckForMessages(username);
                    }
                    else
                    {
                        Debug.LogError("Failed to fetch friends: " + innerTask.Exception);
                    }
                });
          });
    }

    private void CheckForMessages(string username)
    {
        List<string> friendsWithoutMessages = new List<string>();
        int processedFriendsCount = 0;

        // Loop through the friendsList to check for messages
        foreach (string friend in friendsList)
        {
            // Check if there are no messages between the user and the friend
            bool noMessages = true;

            // Check for messages sent by the user to the friend
            db.Collection("messages")
              .WhereEqualTo("sender", username)
              .WhereEqualTo("receiver", friend)
              .GetSnapshotAsync()
              .ContinueWithOnMainThread(task =>
              {
                  if (task.IsCompletedSuccessfully && task.Result.Count > 0)
                  {
                      noMessages = false;
                  }

                  // Check for messages sent by the friend to the user
                  db.Collection("messages")
                    .WhereEqualTo("sender", friend)
                    .WhereEqualTo("receiver", username)
                    .GetSnapshotAsync()
                    .ContinueWithOnMainThread(innerTask =>
                    {
                        if (innerTask.IsCompletedSuccessfully && innerTask.Result.Count > 0)
                        {
                            noMessages = false;
                        }

                        // If no messages were found, add the friend to the list
                        if (noMessages)
                        {
                            friendsWithoutMessages.Add(friend);
                        }

                        // Increment the processed friends counter
                        processedFriendsCount++;

                        // If all friends have been processed, populate the dropdown
                        if (processedFriendsCount == friendsList.Count)
                        {
                            PopulateDropdown(friendsWithoutMessages);
                        }
                    });
              });
        }
    }

    private void PopulateDropdown(List<string> friendsWithoutMessages)
    {
        friendsWithoutMessages.Insert(0, "");
        friendDropdown.ClearOptions();
        friendDropdown.AddOptions(friendsWithoutMessages);
    }

    public void NewMessageButton()
    {
        // Toggle the newMessagePanel
        newMessagePanel.SetActive(!newMessagePanel.activeSelf);
        FetchFriendListByUsername(username);
    }

    void OnDropdownValueChanged(int index)
    {
        // Get the selected value from the options list using the index
        receiver = friendDropdown.options[index].text;

        // Now you can use the selected value as needed
        Debug.Log("Selected value: " + receiver);
    }
}