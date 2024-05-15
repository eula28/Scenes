using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AvatarGender : MonoBehaviour
{

    public TextMeshProUGUI Month;
    public TextMeshProUGUI Date;
    public TextMeshProUGUI Year;
    public TextMeshProUGUI gender;
    public Button next;
    // Create a public variable to store the gender value
    public string genderValue;
    public static string val;
    public static int nbtn;
    public string gen;
    public string bday;

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


    }
    void Update()
    {
        gen = gender.text;
        bday = Month.text +"/"+ Date.text +"/"+ Year.text;
        checkValue();
    }
    void checkValue()
    {
        if (bday != "" && gen != "" && genderValue != "")
        {
            next.gameObject.SetActive(true);

        }
        else
        {
            next.gameObject.SetActive(false);
        }
    }

    // Define the MaleButton() method
    public void MaleButton()
    {
        genderValue = "Male-Avatar";
        val = genderValue;
    }

    // Define the FemaleButton() method
    public void FemaleButton()
    {
        genderValue = "Female-Avatar";
        val = genderValue;
    }
    public void TaskOnClick()
    {
        nbtn = 0;
        Debug.Log(nbtn);

    }
}