using UnityEngine;

namespace Effects
{
    public class LightFlasher : MonoBehaviour
    {
        [Header("Material Affect Data")] public float emissionChangeRate;
        public float minEmissionAmount;
        public float maxEmissionAmount;

        [Header("Material Data")] public int materialIndex = 1;
        public Color emissionColor;

        private bool _flashEmissionActive;
        private bool _isEmissionIncreasing;
        private Material _targetMaterial;
        private float _currentEmissionValue;

        private static readonly int BloomColorMiddleParam = Shader.PropertyToID("_BloomColorMiddle");

        #region Unity Functions

        private void Start()
        {
            _targetMaterial = GetComponent<MeshRenderer>().materials[materialIndex];
            _currentEmissionValue = minEmissionAmount;

            _targetMaterial.SetColor(BloomColorMiddleParam, emissionColor * minEmissionAmount);
        }

        private void Update()
        {
            if (!_flashEmissionActive)
            {
                return;
            }

            UpdateEmission();
        }

        #endregion

        #region External Functions

        public void ActiveFlashEmission()
        {
            _flashEmissionActive = true;
            _isEmissionIncreasing = true;
        }

        #endregion

        #region Utility Functions

        private void UpdateEmission()
        {
            if (_isEmissionIncreasing)
            {
                _currentEmissionValue += emissionChangeRate * Time.deltaTime;
                if (_currentEmissionValue > maxEmissionAmount)
                {
                    _isEmissionIncreasing = false;
                    _currentEmissionValue = maxEmissionAmount;
                }
            }
            else
            {
                _currentEmissionValue -= emissionChangeRate * Time.deltaTime;
                if (_currentEmissionValue < minEmissionAmount)
                {
                    _isEmissionIncreasing = false;
                    _flashEmissionActive = false;
                    _currentEmissionValue = minEmissionAmount;
                }
            }

            _targetMaterial.SetColor(BloomColorMiddleParam, emissionColor * _currentEmissionValue);
        }

        #endregion
    }
}