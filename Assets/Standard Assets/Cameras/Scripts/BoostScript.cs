using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class BoostScript : MonoBehaviour {
    private ParticleSystem m_ParticleSystem;
    [SerializeField]
    private Transform CarTransform;
    // Use this for initialization
    void Start () {
        m_ParticleSystem = GetComponent<ParticleSystem>();
        var em = m_ParticleSystem.emission;
            em.enabled = false;
        m_ParticleSystem.Play();
    }
	
	// Update is called once per frame
	void Update ()
    {
        /*float boost = CrossPlatformInputManager.GetAxis("Boost");
        if(boost > 0)
        {
            var em = m_ParticleSystem.emission;
            em.enabled = true;
        }
        else
        {
            var em = m_ParticleSystem.emission;
            em.enabled = false;
        }*/
    }
}
