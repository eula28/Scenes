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

            // Get reference to userAchievements collection for the user
            CollectionReference userAchievementsRef = db.Collection("users").Document(userId).Collection("userAchievements");

            foreach (var action in userActions)
            {
                // Get reference to the specific userAchievement document
                DocumentReference userAchievementDocRef = userAchievementsRef.Document(action.Key);

                // Get the current achievement data
                DocumentSnapshot snapshot = await userAchievementDocRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    Dictionary<string, object> achievementData = snapshot.ToDictionary();

                    // Update progress
                    int progress = achievementData.ContainsKey("progress") ? Convert.ToInt32(achievementData["progress"]) : 0;
                    int criteria = achievementData.ContainsKey("criteria") ? Convert.ToInt32(achievementData["criteria"]) : 0;
                    progress += (int)action.Value;

                    // Check if progress meets criteria
                    if (progress >= criteria && !(bool)achievementData["achieved"])
                    {
                        // Update achievement status to achieved
                        achievementData["achieved"] = true;
                        // Add points to the user
                        int points = achievementData.ContainsKey("points") ? Convert.ToInt32(achievementData["points"]) : 0;
                        await db.Collection("users").Document(userId).UpdateAsync(new Dictionary<string, object>
                        {
                            { "points", FieldValue.Increment(points) },
                            { "task achieved", FieldValue.Increment(1)}
                        });
                    }

                    // Update progress in userAchievements collection
                    achievementData["progress"] = progress;
                    await userAchievementDocRef.UpdateAsync(achievementData);
                }
                else
                {
                    Console.WriteLine($"Achievement {action.Key} does not exist for user {userId}.");
                }
            }
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }
}
