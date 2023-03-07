using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Farm.Dialogue
{
    [RequireComponent(typeof(NPCMovement))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class DialogueController : MonoBehaviour
    {
        private NPCMovement npc => GetComponent<NPCMovement>();
        [SerializeField,Header("对话完成执行的事件")]private UnityEvent OnFinishEvent;
        [SerializeField,Header("对话列表")]private List<DialoguePiece> dialogueList = new List<DialoguePiece>();

        private Stack<DialoguePiece> dailogueStack;

        private bool canTalk;
        private bool isTalking;
        private GameObject uiSign;
        private void Awake()
        {
            uiSign = transform.GetChild(1).gameObject;
            FillDialogueStack();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canTalk = !npc.IsMoving && npc.Interactable;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canTalk = false;
            }
        }

        private void Update()
        {
            uiSign.SetActive(canTalk);

            if (canTalk & Input.GetKeyDown(KeyCode.Space) && !isTalking && npc.canInteract)
            {
                StartCoroutine(DailogueRoutine());
            }
        }




        /// <summary>
        /// 构建对话堆栈
        /// </summary>
        private void FillDialogueStack()
        {
            dailogueStack = new Stack<DialoguePiece>();
            for (int i = dialogueList.Count - 1; i > -1; i--)
            {
                dialogueList[i].isDone = false;
                dailogueStack.Push(dialogueList[i]);
            }
        }

        private IEnumerator DailogueRoutine()
        {
            isTalking = true;
            if (dailogueStack.TryPop(out DialoguePiece result))
            {
                //传到UI显示对话
                EventCenter.CallShowDialogueEvent(result);
                EventCenter.CallUpdateGameStateEvent(GameState.Pause);
                yield return new WaitUntil(() => result.isDone);
                isTalking = false;
            }
            else
            {
                EventCenter.CallUpdateGameStateEvent(GameState.Gameplay);
                EventCenter.CallShowDialogueEvent(null);
                FillDialogueStack();
                isTalking = false;

                if (OnFinishEvent != null)
                {
                    OnFinishEvent.Invoke();
                    canTalk = false;
                }
            }
        }
    }
}