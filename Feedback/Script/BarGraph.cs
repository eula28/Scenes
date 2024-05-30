using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarGraph : MonoBehaviour
{
    public GameObject barPrefab; // Assign your bar prefab in the inspector
    public Transform barContainer; // Assign your panel (container for bars) in the inspector
    public Dictionary<int, int> ratingCounts;

    private List<GameObject> bars = new List<GameObject>();

    void Start()
    {
        CreateBars();
        UpdateBars();
    }

    void CreateBars()
    {
        // Assuming ratings range from 1 to 5
        for (int i = 1; i <= 5; i++)
        {
            GameObject bar = Instantiate(barPrefab, barContainer);
            bars.Add(bar);
        }
    }

    public void UpdateBars()
    {
        float containerHeight = barContainer.GetComponent<RectTransform>().rect.height;
        int maxRatingCount = 0;
        foreach (var ratingCount in ratingCounts.Values)
        {
            if (ratingCount > maxRatingCount)
                maxRatingCount = ratingCount;
        }

        for (int i = 0; i < bars.Count; i++)
        {
            int rating = i + 1; // Ratings are 1-based
            int ratingCount = ratingCounts.ContainsKey(rating) ? ratingCounts[rating] : 0;
            RectTransform rt = bars[i].GetComponent<RectTransform>();
            float height = (ratingCount / (float)maxRatingCount) * containerHeight;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
            rt.anchoredPosition = new Vector2(i * (rt.sizeDelta.x + 10), 0); // Adjust spacing between bars

            // Update bar label if you have one
            Text label = bars[i].GetComponentInChildren<Text>();
            if (label != null)
            {
                label.text = ratingCount.ToString();
            }
        }
    }

    // Method to set rating counts and update the graph
    public void SetRatingCounts(Dictionary<int, int> newRatingCounts)
    {
        ratingCounts = newRatingCounts;
        UpdateBars();
    }
}
