using System.Collections.Generic;
using UnityEngine;

public class EffectManager:MonoBehaviour
{
    public static EffectManager Instance;
    private Transform currentEffectsObject; private Transform currentEffectsParent; 
    private List<ParticleSystem> effects;

    private void Awake()
    {
        if (Instance == null) 
            Instance = this; 
        effects = new List<ParticleSystem>();
    }
    
    public void PlayOneShot(ParticleSystem particleSystem, Vector3 position)
    {
        if (particleSystem == null) return;
            
        var effect = Instantiate(particleSystem, position, Quaternion.identity);
        effect.Play();

        var duration = effect.main.duration + effect.main.startLifetime.constantMax;
        effect.gameObject.AddComponent<Disposable>().lifetime = duration;
    }
}
