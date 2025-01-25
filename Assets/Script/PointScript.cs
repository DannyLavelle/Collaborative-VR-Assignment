using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PointScript : MonoBehaviour
{
    //public static PointScript Instance;
    public TMP_Text pointScore;
    public int points = 0;

    private void Awake()
    {
        //Instance = this;
    }

    public int IncrementPoints()
    {
        points++;
        pointScore.text = points.ToString();

        return points;
    }

}
