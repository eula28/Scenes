using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class AvatarGender : MonoBehaviour
{
    private ColorBlock colors;
    private ColorBlock maleDefaultColors;
    private ColorBlock femaleDefaultColors;
    private Button currentlySelectedButton;

    public TextMeshProUGUI Month;
    public TMP_InputField Date;
    public TMP_InputField Year;
    public TextMeshProUGUI gender;
    public Button next;
    // Create a public variable to store the gender value
    public string genderValue;
    public static string val;
    public static int nbtn;
    public string gen;
    public string bday;
    public string month;
    public string date;
    public string year;
    public int modelnum;

    // Get a reference to the buttons in the Unity inspector
    public Button maleButton;
    public Button femaleButton;

    // Use the Start() method to set up the button click events
    void Start()
    {
        // Set up the button click event for maleButton
        maleButton.onClick.AddListener(MaleButton);

        // Set up the button click event for femaleButton
        femaleButton.onClick.AddListener(FemaleButton);
        next.onClick.AddListener(TaskOnClick);

        // Store default colors
        maleDefaultColors = maleButton.colors;
        femaleDefaultColors = femaleButton.colors;

        // Ensure no button is selected initially
        currentlySelectedButton = null;
    }

    void Update()
    {
        gen = gender.text;

        checkValue();

        if (currentlySelectedButton != null)
        {
            ColorBlock colors = currentlySelectedButton.colors;
            colors.normalColor = colors.selectedColor; // Ensuring the selected color remains as normal color
            currentlySelectedButton.colors = colors;
        }
    }

    void checkValue()
    {
        month = Month.text;
        date = Date.text;
        year = Year.text;

        if (month != "" && date != "" && year != "" && gen != "" && genderValue != "" && IsValidAge(year, month, date))
        {
            bday = Month.text + "/" + Date.text + "/" + Year.text;
            next.gameObject.SetActive(true);
        }
        else
        {
            next.gameObject.SetActive(false);
        }
    }

    // Method to validate age
    bool IsValidAge(string year, string month, string day)
    {
        if (int.TryParse(year, out int birthYear) && int.TryParse(month, out int birthMonth) && int.TryParse(day, out int birthDay))
        {
            DateTime birthDate = new DateTime(birthYear, birthMonth, birthDay);
            DateTime currentDate = DateTime.Now;
            TimeSpan ageSpan = currentDate - birthDate;
            double ageInYears = ageSpan.TotalDays / 365.25;

            return ageInYears >= 10;
        }
        return false;
    }

    // Define the MaleButton() method
    public void MaleButton()
    {
        SetSelectedButton(maleButton);
        genderValue = "Male-Avatar";
        val = genderValue;
        modelnum = 0;
    }

    // Define the FemaleButton() method
    public void FemaleButton()
    {
        SetSelectedButton(femaleButton);
        genderValue = "Female-Avatar";
        val = genderValue;
        modelnum = 5;
    }

    private void SetSelectedButton(Button selectedButton)
    {
        // If there is a currently selected button, revert it to default colors
        if (currentlySelectedButton != null)
        {
            if (currentlySelectedButton == maleButton)
            {
                maleButton.colors = maleDefaultColors;
            }
            else if (currentlySelectedButton == femaleButton)
            {
                femaleButton.colors = femaleDefaultColors;
            }
        }

        // Set the new selected button's color
        ColorBlock colors = selectedButton.colors;
        colors.selectedColor = Color.white;
        selectedButton.colors = colors;

        // Update the currently selected button reference
        currentlySelectedButton = selectedButton;
    }

    public void TaskOnClick()
    {
        nbtn = 0;
        Debug.Log(nbtn);
    }

    public void updateUserDoc()
    {
        if (FirebaseController.Instance != null)
        {
            Dictionary<string, object> userUpdate = new Dictionary<string, object>
            {
                { "gender model", val},
                { "bday", bday },
                { "gender", gen },
                { "model number", modelnum}
            };

            string userId = FirebaseController.Instance.auth.CurrentUser.UserId;
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference userUpdateDocRef = db.Collection("users").Document(userId);

            userUpdateDocRef.UpdateAsync(userUpdate).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to update user data: " + task.Exception);
                    return;
                }
                else
                {
                    SceneManager.LoadScene("Account");
                    Debug.Log("User data updated successfully for user: " + FirebaseController.Instance.auth.CurrentUser.UserId);
                }
            });
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }
}
