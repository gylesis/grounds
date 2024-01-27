using System;
using Dev.Scripts.PlayerLogic;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace Dev.Scripts.Items
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer[] _meshRenderer;

        private static readonly int CrackStrengthName = Shader.PropertyToID("_CrackStrength");
        private static readonly int EmissionStrengthName = Shader.PropertyToID("_EmissionStrength");

        private Action _onDestroy;

        public void Initialize(Item item)
        {
            _meshRenderer = GetComponentsInChildren<MeshRenderer>();
            TrySubscribeToHealth(item);
        }

        private void TrySubscribeToHealth(Item item)
        {
            Health health = item.Health;
            health.Changed.TakeUntilDestroy(this).Subscribe(UpdateView);
        }

        private void OnDestroy()
        {
            _onDestroy?.Invoke();
        }


        private void UpdateView(HealthChangedContext changedContext)
        {
            float currentHealth = changedContext.CurrentHealth;
            float maxHealth = changedContext.MaxHealth;

            var ratio = 1 - (currentHealth / maxHealth);
            var rescaledRatio = ratio * 2;
            var aaaaa = rescaledRatio * rescaledRatio;
            var crackStrength = rescaledRatio;

            Debug.Log($"{currentHealth}/{maxHealth}");

            foreach (var meshRenderer in _meshRenderer)
            {
                DOTween.Sequence()
                    .Append(meshRenderer.material.DOFloat(crackStrength * 2f, CrackStrengthName, 0.20f))
                    .Join(meshRenderer.material.DOFloat(3, EmissionStrengthName, 0.20f))
                    .Append(meshRenderer.material.DOFloat(crackStrength * 1f, CrackStrengthName, 0.80f))
                    .Join(meshRenderer.material.DOFloat(0, EmissionStrengthName, 0.20f));
            }
        }
    }
}