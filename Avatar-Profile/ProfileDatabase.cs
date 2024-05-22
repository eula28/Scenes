using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ProfileDatabase : ScriptableObject
{
    public Profile[] profile;

    public int profileCount
    {
        get
        {
            return profile.Length;
        }
    }
    public Profile GetCharacter(int index)
    {
        return profile[index];
    }
}
