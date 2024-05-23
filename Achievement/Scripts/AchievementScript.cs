using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;

public class AchievementScript : MonoBehaviour
{
    public void UpdateUserAchievements(string userId, Dictionary<string, object> userActions)
    {
        // Check if the FirebaseController instance exists
        if (FirebaseController.Instance != null)
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
                Dictionary<string, object> userData = userSnapshot.ToDictionary();

                // Get userAchievements subcollection
                CollectionReference userAchievementsRef = userRef.Collection("userAchievements");

                // Iterate through user actions
                foreach (var action in userActions)
                {
                    string achievementId = action.Key;
                    int actionCount = (int)action.Value;

                    // Check if achievement exists
                    DocumentReference achievementDocRef = userAchievementsRef.Document(achievementId);
                    achievementDocRef.GetSnapshotAsync().ContinueWithOnMainThread(achievementTask =>
                    {
                        if (achievementTask.Result.Exists)
                        {
                            Achievement achievement = achievementTask.Result.ConvertTo<Achievement>();

                            // Check if achievement criteria are met
                            if (achievement.Criteria != null && actionCount >= achievement.Criteria.Count)
                            {
                                Dictionary<string, object> achievementData = new Dictionary<string, object>
                                {
                                    { "achieved", true },
                                    { "progress", actionCount }
                                };

                                // Update or create achievement document
                                achievementDocRef.SetAsync(achievementData, SetOptions.MergeAll);
                            }
                        }
                    });
                }
            });
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }
}
