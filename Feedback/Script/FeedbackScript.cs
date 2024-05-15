using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public Button[] stars;
    public TMP_InputField feedbackEmail, feedbackLocation, feedbackComment;
    public int feedbackRating = 0;

    void Start()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            int index = i + 1;
            stars[i].onClick.AddListener(() => StarClicked(index));
        }
    }

    private Color STAR_COLOR = Color.green;


    public void StarClicked(int rating)
    {
        feedbackRating = rating;
        for (int i = 0; i < stars.Length; i++)
        {
            Image starImage = stars[i].GetComponent<Image>();
            Color color = i < rating ? STAR_COLOR : Color.white;
            starImage.color = color;
        }
        Debug.Log("Star Rated: " + feedbackRating);
    }
}