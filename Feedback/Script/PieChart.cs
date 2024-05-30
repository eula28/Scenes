using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChart : MonoBehaviour
{
    public Image[] imagesPieChart;
    public float[] values;

    // Start is called before the first frame update
    void Start()
    {
        if (values != null && values.Length > 0)
        {
            SetValues(values);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetValues(float[] valuesToSet)
    {
        // Convert int array to float array if necessary
        float[] floatValues = new float[valuesToSet.Length];
        for (int i = 0; i < valuesToSet.Length; i++)
        {
            floatValues[i] = (float)valuesToSet[i];
        }

        float totalValues = 0;
        for (int i = 0; i < imagesPieChart.Length; i++)
        {
            totalValues += FindPercentage(floatValues, i);
            imagesPieChart[i].fillAmount = totalValues;
        }
    }

    private float FindPercentage(float[] valuesToSet, int index)
    {
        float totalAmount = 0;
        for (int i = 0; i < valuesToSet.Length; i++)
        {
            totalAmount += valuesToSet[i];
        }
        return valuesToSet[index] / totalAmount;
    }
}
