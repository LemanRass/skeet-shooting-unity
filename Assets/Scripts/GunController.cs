using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class GunController : MonoBehaviour
    {
        private Animator _animator;
        
        public ParticleSystem muzzleParticles;
        public ParticleSystem sparkParticles;
        
        public int minSparkEmission = 1;
        public int maxSparkEmission = 7;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Fire()
        {
            _animator.Play("Fire");
                
            muzzleParticles.Emit (1);
            sparkParticles.Emit (Random.Range (minSparkEmission, maxSparkEmission));
        }
    }
}