using Farm.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{
    [SerializeField]private GameScene transitionScene;
    [SerializeField]private Vector3 transitionPosition;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            EventManager.InvokeEventListener(ConstString.TransitionSceneEvent, transitionScene, transitionPosition);
        }
    }

}
