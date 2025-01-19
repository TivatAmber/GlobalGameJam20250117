using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreUI: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        private void Update()
        {
            text.text = "Score: " + GameManager.Instance.Score;
        }
    }
}