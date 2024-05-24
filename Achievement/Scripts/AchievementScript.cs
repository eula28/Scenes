using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using System.Threading.Tasks;
using System;

public class AchievementScript
{
    public async Task UpdateUserAchievements(string userId, Dictionary<string, object> userActions)
    {
        // Check if the FirebaseController instance exists
        if (FirebaseController.Instance != null)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference userRef = db.Collection("users").Document(userId);

            try
            {
                // Fetch user data
                DocumentSnapshot userSnapshot = await userRef.GetSnapshotAsync();

                if (!userSnapshot.Exists)
                {
                    // If user document doesn't exist, create it with default data
                    await userRef.SetAsync(new Dictionary<string, object>
                    {
                        { "points", 0 } // Set default points to 0 or any other default values
                        // Add other default fields if needed
                    });
                }

                // Retrieve user data again after creating if it didn't exist before
                userSnapshot = await userRef.GetSnapshotAsync();

                if (userSnapshot.Exists)
                {
                    Dictionary<string, object> userData = userSnapshot.ToDictionary();
                    int currentPoints = userData.ContainsKey("points") ? (int)(long)userData["points"] : 0;

                    // Get userAchievements subcollection
                    CollectionReference userAchievementsRef = userRef.Collection("userAchievements");

                    // List to hold tasks for achievements and points update
                    List<Task> tasks = new List<Task>();

                    // Iterate through user actions
                    foreach (var action in userActions)
                    {
                        string achievementId = action.Key;
                        int actionCount = (int)action.Value;

                        // Check if achievement exists
                        DocumentReference achievementDocRef = userAchievementsRef.Document(achievementId);
                        DocumentSnapshot achievementSnapshot = await achievementDocRef.GetSnapshotAsync();

                        if (achievementSnapshot.Exists)
                        {
                            Dictionary<string, object> achievementData = achievementSnapshot.ToDictionary();

                            // Update progress
                            int currentProgress = achievementData.ContainsKey("progress") ? (int)(long)achievementData["progress"] : 0;
                            int newProgress = currentProgress + actionCount;

                            // Update or create achievement document
                            Dictionary<string, object> updateData = new Dictionary<string, object>
                            {
                                { "progress", newProgress }
                            };

                            // Check if criteria are met
                            if (CriteriaMet(achievementData, newProgress))
                            {
                                // Add points to the user and reset progress
                                int pointsToAdd = (int)(long)achievementData["points"];
                                int updatedPoints = currentPoints + pointsToAdd;

                                // Update user points in the database
                                Dictionary<string, object> userDataUpdate = new Dictionary<string, object>
                                {
                                    { "points", updatedPoints }
                                };

                                // Add task for updating user points
                                tasks.Add(userRef.SetAsync(userDataUpdate, SetOptions.MergeAll));

                                // Reset progress
                                updateData["progress"] = 0;
                            }

                            // Set achievement to true only if criteria are met
                            updateData["achieved"] = CriteriaMet(achievementData, newProgress);

                            // Add task for updating achievement
                            tasks.Add(achievementDocRef.SetAsync(updateData, SetOptions.MergeAll));
                        }
                        else
                        {
                            Debug.Log($"Achievement {achievementId} does not exist.");
                        }
                    }

                    // Wait for all tasks to complete
                    await Task.WhenAll(tasks);
                }
                else
                {
                    Debug.LogError("Failed to fetch or create user data.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error updating user achievements: " + ex);
            }
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }

    private bool CriteriaMet(Dictionary<string, object> achievementData, int progress)
    {
        if (!achievementData.ContainsKey("criteria")) return false;

        Dictionary<string, object> criteria = (Dictionary<string, object>)achievementData["criteria"];
        string type = criteria["type"].ToString();

        switch (type)
        {
            case "friend5":
                int requiredFriends5 = (int)criteria["count"];
                return progress >= requiredFriends5;
            case "friend10":
                int requiredFriends10 = (int)criteria["count"];
                return progress >= requiredFriends10;
            case "friend15":
                int requiredFriends15 = (int)criteria["count"];
                return progress >= requiredFriends15;
            default:
                Debug.LogError($"Unsupported criteria type: {type}");
                return false;
        }
    }

}
