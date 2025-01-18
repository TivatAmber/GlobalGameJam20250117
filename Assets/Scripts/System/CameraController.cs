using UnityEngine;

namespace System
{
    public class CameraController : MonoBehaviour
    {
        public float shakeDuration = 0.5f;
        public float shakeAmount = 0.7f;
        public float decreaseFactor = 1.0f;
    
        private Vector3 _originalPosition;
        private float _currentShakeDuration = 0f;
    
        private void Start()
        {
            _originalPosition = transform.position;
        }
    
        private void Update()
        {
            if (_currentShakeDuration > 0)
            {
                transform.position = _originalPosition + UnityEngine.Random.insideUnitSphere * shakeAmount;
                _currentShakeDuration -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                _currentShakeDuration = 0f;
                transform.position = _originalPosition;
            }
        }
    
        public void StartShake()
        {
            _originalPosition = transform.position;
            _currentShakeDuration = shakeDuration;
        }
    }
}