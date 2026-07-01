using UnityEngine;

[CreateAssetMenu(fileName = "UserProfileData", menuName = "ScriptableObjects/UserProfileData")]
public class UserProfileData : ScriptableObject
{
    public string userName = "Unknown";
    public int userLevel = 1;

    public void UpdateProfile(string newName)
    {
        userName = newName;
    }
}