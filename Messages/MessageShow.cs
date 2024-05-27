using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Firestore;
using TMPro;
using Unity.VisualScripting;
using Firebase.Extensions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MessageShow : MonoBehaviour
{
    string userId, friendUsernameToPass, username;
    FirebaseFirestore db;
    public Transform messageContainer;
    List<string> friendsList = new List<string>(); // Ensure this is populated with friends' usernames
    public GameObject messagePrefab;
    public ProfileDatabase profileDB; // Assuming you have a ProfileDatabase script or component
    public MessageExchangeScript targetScript;

    // Start is called before the first frame update
    void Start()
    {
        // Check if the FirebaseController instance exists
        if (FirebaseController.Instance != null)
        {
            userId = FirebaseController.Instance.user.UserId;
            db = FirebaseFirestore.DefaultInstance;
            FetchUsername(userId);
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }

    }

    private async Task FetchLatestMessagesAndPopulate(string username)
    {
        // Clear the message container
        foreach (Transform child in messageContainer)
        {
            Destroy(child.gameObject);
        }

        List<MessageData> messagesList = new List<MessageData>();

        try
        {
            var tasks = friendsList.Select(async friend =>
            {
                // Fetch latest message where the user is the sender
                Query senderMessagesQuery = db.Collection("messages")
                    .WhereEqualTo("sender", username)
                    .WhereEqualTo("receiver", friend)
                    .OrderBy("timestamp") // Fetch latest message first
                    .Limit(1);

                var senderMessageSnapshot = await senderMessagesQuery.GetSnapshotAsync();

                // Fetch latest message where the user is the receiver
                Query receiverMessagesQuery = db.Collection("messages")
                    .WhereEqualTo("receiver", username)
                    .WhereEqualTo("sender", friend)
                    .OrderBy("timestamp") // Fetch latest message first
                    .Limit(1);

                var receiverMessageSnapshot = await receiverMessagesQuery.GetSnapshotAsync();

                // Check the latest message between sender and receiver
                DocumentSnapshot latestMessageDoc = null;
                if (senderMessageSnapshot != null && senderMessageSnapshot.Count > 0)
                {
                    latestMessageDoc = senderMessageSnapshot.Documents.First();
                }
                if (receiverMessageSnapshot != null && receiverMessageSnapshot.Count > 0)
                {
                    var latestReceiverMessageDoc = receiverMessageSnapshot.Documents.First();
                    if (latestMessageDoc == null || latestReceiverMessageDoc.GetValue<Timestamp>("timestamp") > latestMessageDoc.GetValue<Timestamp>("timestamp"))
                    {
                        latestMessageDoc = latestReceiverMessageDoc;
                    }
                }

                // If there is a latest message, add it to the list
                if (latestMessageDoc != null)
                {
                    string latestMessage = latestMessageDoc.GetValue<string>("message");
                    Timestamp timestamp = latestMessageDoc.GetValue<Timestamp>("timestamp");

                    var userSnapshot = await db.Collection("users")
                        .WhereEqualTo("username", friend)
                        .Limit(1)
                        .GetSnapshotAsync();

                    var userDoc = userSnapshot.Documents.FirstOrDefault();
                    if (userDoc != null && userDoc.Exists)
                    {
                        int modelNumber = userDoc.GetValue<int>("model number");
                        messagesList.Add(new MessageData
                        {
                            FriendName = friend,
                            LatestMessage = latestMessage,
                            Timestamp = timestamp.ToDateTime(),
                            ModelNumber = modelNumber
                        });
                    }
                    else
                    {
                        Debug.LogError("User document does not exist for username: " + friend);
                    }
                }
                else
                {
                    Debug.LogWarning("No messages found for friend: " + friend);
                }
            });

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to fetch and populate messages: " + ex);
        }

        // Sort the combined list of messages by timestamp
        messagesList.Sort((x, y) => y.Timestamp.CompareTo(x.Timestamp));
        // Instantiate message prefabs with the combined list
        InstantiateMessagePrefabs(messagesList);
    }

    private void InstantiateMessagePrefabs(List<MessageData> messagesList)
    {
        foreach (var messageData in messagesList)
        {
            GameObject newMessagePrefab = Instantiate(messagePrefab, messageContainer);
            newMessagePrefab.transform.Find("FriendName").GetComponent<TextMeshProUGUI>().text = messageData.FriendName;
            newMessagePrefab.transform.Find("LatestMessage").GetComponent<TextMeshProUGUI>().text = messageData.LatestMessage;
            newMessagePrefab.transform.Find("Timestamp").GetComponent<TextMeshProUGUI>().text = messageData.Timestamp.ToString();

            // Set profile picture
            int modelNumber = messageData.ModelNumber;
            if (modelNumber >= 0 && modelNumber < profileDB.profileCount)
            {
                Sprite profileSprite = profileDB.GetCharacter(modelNumber).profileSprite;
                Image profileImageComponent = newMessagePrefab.transform.Find("ProfilePicture").GetComponent<Image>();
                profileImageComponent.sprite = profileSprite;
            }

            Button button = newMessagePrefab.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    PlayerPrefs.SetString("FriendUsername", messageData.FriendName);
                    SceneManager.LoadScene("MessageExchange");
                });
            }
        }
    }

    private void StartMessageListener()
    {
        CollectionReference messagesCollectionRef = db.Collection("messages");

        // Start listening to changes in the messages collection
        messageListener = messagesCollectionRef
            .WhereIn("receiver", friendsList)
            .WhereEqualTo("sender", username)  // Listen for messages sent by the user
            .Listen(snapshot =>
            {
                // Whenever a new message is received, fetch and populate the messages
                _ = FetchLatestMessagesAndPopulate(username);
            });
    }

    private void FetchUsername(string userId)
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
                        FetchFriendListByUsername(username);
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

                        // Once friends are fetched, start listening for messages
                        StartMessageListener();
                    }
                    else
                    {
                        Debug.LogError("Failed to fetch friends: " + innerTask.Exception);
                    }
                });
          });
    }

    private IDisposable messageListener;

    private class MessageData
    {
        public string FriendName { get; set; }
        public string LatestMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public int ModelNumber { get; set; }
    }
}
