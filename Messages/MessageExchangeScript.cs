using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class MessageExchangeScript : MonoBehaviour
{
    string userId, username;
    FirebaseFirestore db;
    public TMP_InputField message;
    public Transform messageContainer;
    public GameObject sentMessagePrefab;
    public GameObject receivedMessagePrefab;
    public TMP_Text usernameHead;
    public string friendUsername;

    private IDisposable messageListener;

    async void Start()
    {
        if (FirebaseController.Instance != null)
        {
            userId = FirebaseController.Instance.user.UserId;
            db = FirebaseFirestore.DefaultInstance;
            await FetchUsername(userId);
            Debug.Log("Username fetched in Start: " + username);
            friendUsername = PlayerPrefs.GetString("FriendUsername");
            usernameHead.text = friendUsername;
            await FetchLatestMessagesAndPopulate();
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }

    private async Task FetchUsername(string userId)
    {
        DocumentReference userRef = db.Collection("users").Document(userId);
        DocumentSnapshot userSnapshot = await userRef.GetSnapshotAsync();
        if (userSnapshot.Exists)
        {
            username = userSnapshot.GetValue<string>("username");
            Debug.Log("Username found: " + username);
        }
        else
        {
            Debug.LogError("User document does not exist.");
        }
    }

    public void toMessages()
    {
        SceneManager.LoadScene("Messages");
    }

    public void SendMessage()
    {
        string sender = username;
        string docId = Guid.NewGuid().ToString();
        DocumentReference messageRef = db.Collection("messages").Document(docId);
        Dictionary<string, object> messageData = new Dictionary<string, object>
        {
            { "sender", sender },
            { "receiver", friendUsername},
            { "message", message.text },
            { "timestamp", Timestamp.GetCurrentTimestamp() }
        };
        messageRef.SetAsync(messageData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("Message sent");
                message.text = "";
                _ = FetchLatestMessagesAndPopulate();
            }
            else
            {
                Debug.LogError("Failed to send message: " + task.Exception);
            }
        });
    }

    private async Task FetchLatestMessagesAndPopulate()
    {
        if (string.IsNullOrEmpty(friendUsername))
        {
            Debug.LogError("Friend username is not set.");
            return;
        }

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username is not set.");
            return;
        }

        // Clear the message container
        foreach (Transform child in messageContainer)
        {
            Destroy(child.gameObject);
        }

        List<MessageData> messages = new List<MessageData>();

        try
        {
            // Query for messages where the user is the sender and the friend is the receiver
            var sentMessagesQuery = db.Collection("messages")
                .WhereEqualTo("sender", username)
                .WhereEqualTo("receiver", friendUsername)
                .OrderBy("timestamp"); // Ensure correct ordering

            // Query for messages where the user is the receiver and the friend is the sender
            var receivedMessagesQuery = db.Collection("messages")
                .WhereEqualTo("sender", friendUsername)
                .WhereEqualTo("receiver", username)
                .OrderBy("timestamp"); // Ensure correct ordering

            // Execute both queries concurrently
            var sentMessagesTask = sentMessagesQuery.GetSnapshotAsync();
            var receivedMessagesTask = receivedMessagesQuery.GetSnapshotAsync();

            await Task.WhenAll(sentMessagesTask, receivedMessagesTask);

            // Process sent messages
            var sentMessagesSnapshot = sentMessagesTask.Result;
            if (sentMessagesSnapshot != null && sentMessagesSnapshot.Count > 0)
            {
                foreach (var doc in sentMessagesSnapshot.Documents)
                {
                    string message = doc.GetValue<string>("message");
                    Timestamp timestamp = doc.GetValue<Timestamp>("timestamp");
                    messages.Add(new MessageData
                    {
                        Message = message,
                        Timestamp = timestamp.ToDateTime(),
                        IsSentMessage = true // Message sent by the user
                    });
                }
            }

            // Process received messages
            var receivedMessagesSnapshot = receivedMessagesTask.Result;
            if (receivedMessagesSnapshot != null && receivedMessagesSnapshot.Count > 0)
            {
                foreach (var doc in receivedMessagesSnapshot.Documents)
                {
                    string message = doc.GetValue<string>("message");
                    Timestamp timestamp = doc.GetValue<Timestamp>("timestamp");
                    messages.Add(new MessageData
                    {
                        Message = message,
                        Timestamp = timestamp.ToDateTime(),
                        IsSentMessage = false // Message received by the user
                    });
                }
            }

            // Sort messages by timestamp
            messages.Sort((x, y) => x.Timestamp.CompareTo(y.Timestamp));
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to fetch and populate messages: " + ex);
        }

        InstantiateMessagePrefabs(messages);
    }


    private void InstantiateMessagePrefabs(List<MessageData> messages)
    {
        foreach (var messageData in messages)
        {
            GameObject newMessagePrefab = messageData.IsSentMessage
                ? Instantiate(sentMessagePrefab, messageContainer)
                : Instantiate(receivedMessagePrefab, messageContainer);

            TextMeshProUGUI messageText = newMessagePrefab.transform.Find("Message").GetComponent<TextMeshProUGUI>();
            if (messageText != null)
            {
                messageText.text = messageData.Message;
            }
            else
            {
                Debug.LogError("Message Text component not found in prefab.");
            }

            TextMeshProUGUI timestampText = newMessagePrefab.transform.Find("Timestamp").GetComponent<TextMeshProUGUI>();
            if (timestampText != null)
            {
                timestampText.text = messageData.Timestamp.ToString();
            }
            else
            {
                Debug.LogError("Timestamp Text component not found in prefab.");
            }
        }
    }

    private void StartMessageListener()
    {
        CollectionReference messagesCollectionRef = db.Collection("messages");

        messageListener = messagesCollectionRef
            .WhereEqualTo("receiver", friendUsername)
            .WhereEqualTo("sender", username)
            .Listen(async snapshot =>
            {
                // Whenever a new message is received, fetch and populate the messages
                await FetchLatestMessagesAndPopulate();
            });
    }

    private void OnDisable()
    {
        if (messageListener != null)
        {
            messageListener.Dispose();
        }
    }

    private class MessageData
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsSentMessage { get; set; }
    }
}

