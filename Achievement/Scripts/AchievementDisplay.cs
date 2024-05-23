using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public class AchievementDisplay : MonoBehaviour
{
    FirebaseFirestore db;

    void Start()
    {
        // Check if the FirebaseController instance exists
        if (FirebaseController.Instance != null)
        {
            db = FirebaseFirestore.DefaultInstance;
            DisplayUserAchievements(FirebaseController.Instance.user.UserId);
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }

    public void DisplayUserAchievements(string userId)
    {
        DocumentReference userRef = db.Collection("users").Document(userId);

        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to fetch user data: " + task.Exception);
                return;
            }

            DocumentSnapshot userSnapshot = task.Result;
            if (!userSnapshot.Exists)
            {
                Debug.LogWarning("User not found.");
                return;
            }

            // Fetch user points
            int userPoints = userSnapshot.GetValue<int>("points");
            Debug.Log($"User Points: {userPoints}");

            // Fetch user achievements
            CollectionReference userAchievementsRef = userRef.Collection("userAchievements");
            userAchievementsRef.GetSnapshotAsync().ContinueWithOnMainThread(userAchievementsTask =>
            {
                if (userAchievementsTask.IsFaulted)
                {
                    Debug.LogError("Failed to fetch user achievements: " + userAchievementsTask.Exception);
                    return;
                }

                List<DocumentSnapshot> userAchievementsSnapshots = userAchievementsTask.Result.Documents.ToList();
                foreach (var userAchievementSnapshot in userAchievementsSnapshots)
                {
                    UserAchievement userAchievement = userAchievementSnapshot.ConvertTo<UserAchievement>();
                    string achievementId = userAchievementSnapshot.Id;

                    // Fetch the corresponding achievement details
                    DocumentReference achievementRef = db.Collection("achievements").Document(achievementId);
                    achievementRef.GetSnapshotAsync().ContinueWithOnMainThread(achievementTask =>
                    {
                        if (achievementTask.IsFaulted)
                        {
                            Debug.LogError("Failed to fetch achievement details: " + achievementTask.Exception);
                            return;
                        }

                        DocumentSnapshot achievementSnapshot = achievementTask.Result;
                        if (achievementSnapshot.Exists)
                        {
                            Achievement achievement = achievementSnapshot.ConvertTo<Achievement>();

                            // Display achievement data
                            DisplayAchievementData(userPoints, achievement, userAchievement.Progress, userAchievement.Achieved);
                        }
                    });
                }
            });
        });
    }

    void DisplayAchievementData(int userPoints, Achievement achievement, int progress, bool achieved)
    {
        // Display the data
        Debug.Log($"User Points: {userPoints}");
        Debug.Log($"Achievement Name: {achievement.Name}");
        Debug.Log($"Description: {achievement.Description}");
        Debug.Log($"Achievement Points: {achievement.Points}");
        Debug.Log($"Progress: {progress}");
        Debug.Log($"Achieved: {achieved}");
    }

}