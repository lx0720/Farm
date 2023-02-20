using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.Transition
{
    public class Teleport : MonoBehaviour
    {
        [SceneName]
        public string sceneToGo;
        public Vector3 positionToGo;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                EventCenter.CallTransitionEvent(sceneToGo, positionToGo);
            }
        }
    }
}