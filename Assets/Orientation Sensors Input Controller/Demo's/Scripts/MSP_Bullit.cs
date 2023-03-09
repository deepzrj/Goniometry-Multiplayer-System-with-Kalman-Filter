using UnityEngine;
using System.Collections;

namespace MSP_Input.Demo
{
    public class MSP_Bullit : MonoBehaviour
    {
        public AudioClip explosionSound;
        public float autoDestructAfterSeconds = 10f;

        //====================================================================

        void Start()
        {
            // make sure the gameObject destroy's after a while
            Destroy(gameObject, autoDestructAfterSeconds);
        }

        //====================================================================

        void OnCollisionEnter(Collision collisionInfo)
        {
            // say "kaboom!"
            gameObject.GetComponent<AudioSource>().PlayOneShot(explosionSound);
            // Destroy rigidbody, collider and halo
            Destroy(gameObject.GetComponent<Rigidbody>());
            Destroy(gameObject.GetComponent<SphereCollider>());
            Destroy(gameObject.GetComponent("Halo"));
            // destroy the gameobject after explosion sound has completed
            Destroy(gameObject, explosionSound.length);
        }

        //====================================================================

    }
}