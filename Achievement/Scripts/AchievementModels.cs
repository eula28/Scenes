using System.Collections.Generic;
using Firebase.Firestore;

[FirestoreData]
public class Achievement
{
    [FirestoreProperty]
    public string Name { get; set; }

    [FirestoreProperty]
    public string Description { get; set; }

    [FirestoreProperty]
    public Criteria Criteria { get; set; }

    [FirestoreProperty] 
    public int Points { get; set; }
}

[FirestoreData]
public class Criteria
{
    [FirestoreProperty]
    public string Type { get; set; }

    [FirestoreProperty]
    public int Count { get; set; }
}

[FirestoreData]
public class UserAchievement
{
    [FirestoreProperty]
    public int Progress { get; set; }

    [FirestoreProperty]
    public bool Achieved { get; set; } // New property to indicate achievement status
}
