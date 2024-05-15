using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject star1, star2, star3, star4, star5;
    public TMP_InputField feedbackEmail, feedbackLocation, feedbackComment;
    public int feedbackRating = 0;
    
    public void starOne() {
        GameObject.Find("star1").GetComponent<Renderer>().material.color = new Color(103, 203, 71);
        feedbackRating = 1;
        Debug.Log("Star Rated: " + feedbackRating);
    }

    public void starTwo()
    {
        GameObject.Find("star1").GetComponent<Renderer>().material.color = new Color(103, 203, 71);
        GameObject.Find("star2").GetComponent<Renderer>().material.color = new Color(103, 203, 71);
        feedbackRating = 2;
        Debug.Log("Star Rated: " + feedbackRating);
    }

    public void starThree()
    {
        GameObject.Find("star1").GetComponent<Renderer>().material.color = new Color(103, 203, 71);
        GameObject.Find("star2").GetComponent<Renderer>().material.color = new Color(103, 203, 71);
        GameObject.Find("star3").GetComponent<Renderer>().material.color = new Color(103, 203, 71);
        feedbackRating = 3;
        Debug.Log("Star Rated: " + feedbackRating);
    }

    public void starFour()
    {
        GameObject.Find("star1").GetComponent<Renderer>().material.color = new Color(103, 203, 71);
        GameObject.Find("star2").GetComponent<Renderer>().material.color = new Color(103, 203, 71);
        GameObject.Find("star3").GetComponent<Renderer>().material.color = new Color(103, 203, 71);
        GameObject.Find("star4").GetComponent<Renderer>().material.color = new Color(103, 203, 71);
        feedbackRating = 4;
        Debug.Log("Star Rated: " + feedbackRating);
    }
    public void starFive()
    {
        GameObject.Find("star1").GetComponent<Renderer>().material.color = new Color(103, 203, 71);
        GameObject.Find("star2").GetComponent<Renderer>().material.color = new Color(103, 203, 71);
        GameObject.Find("star3").GetComponent<Renderer>().material.color = new Color(103, 203, 71);
        GameObject.Find("star4").GetComponent<Renderer>().material.color = new Color(103, 203, 71);
        GameObject.Find("star5").GetComponent<Renderer>().material.color = new Color(103, 203, 71);
        feedbackRating = 5;
        Debug.Log("Star Rated: " + feedbackRating);
    }
}
