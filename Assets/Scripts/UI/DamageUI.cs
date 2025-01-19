using System;
using Entity.Components;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DamageUI: MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private DisplacementHandler targetDisplacementHandler;

        private void Update()
        {
            image.fillAmount = targetDisplacementHandler.ChaosValue / 100.0f;
        }
    }
}