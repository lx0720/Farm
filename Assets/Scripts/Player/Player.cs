using System.Collections;
using System.Collections.Generic;
using Farm.Save;
using Farm.Tool;
using Farm.Input;
using UnityEngine;
using UnityEngine.SceneManagement;
using Farm.Transition;

public class Player : MonoBehaviour,ISaveLoad
{
    private Rigidbody2D rb;

    [SerializeField]private float speed;
    private float inputX;
    private float inputY;

    private Animator[] allAnimators;
    private bool movingState;
    public bool inputDisable;

    private float mouseInputX;
    private float mouseInputY;

    private Vector2 moveInput;
    private bool leftShift;
    private bool useTool;

    public float moveEndX;
    public float moveEndY;

    //StringToHash
    private int moveX;
    private int moveY;
    private int mouseX;
    private int mouseY;
    private int moveBool;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        allAnimators = GetComponentsInChildren<Animator>();
    }

    private void Start()
    { 
        StringToHash();
        RegisterInterface();
    }

    private void OnEnable()
    {
        /*EventCenter.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventCenter.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventCenter.MoveToPosition += OnMoveToPosition;
        EventCenter.MouseClickedEvent += OnMouseClickedEvent;
        EventCenter.UpdateGameStateEvent += OnUpdateGameStateEvent;*/
        EventManager.AddEventListener<int>(ConstString.StartNewGameEvent, OnStartNewGameEvent);
        EventManager.AddEventListener<GameScene, Vector3>(ConstString.TransitionSceneEvent, OnTransitionScene);

    }

    private void OnDisable()
    {
     /*   EventCenter.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventCenter.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventCenter.MoveToPosition -= OnMoveToPosition;
        EventCenter.MouseClickedEvent -= OnMouseClickedEvent;
        EventCenter.UpdateGameStateEvent -= OnUpdateGameStateEvent;*/

        EventManager.RemoveEventListener<int>(ConstString.StartNewGameEvent, OnStartNewGameEvent);
        EventManager.RemoveEventListener<GameScene, Vector3>(ConstString.TransitionSceneEvent, OnTransitionScene);
    }

    private void Update()
    {
        if (InputManager.Instance.GetCanInput())
            GetPlayerInput();
        else
            movingState = false;
        SwitchPlayerAnimation();
    }

    private void FixedUpdate()
    {
        if (InputManager.Instance.GetCanInput())
            PlayerMovement();
    }

    private void StringToHash()
    {
        moveX = Animator.StringToHash("InputX");
        moveY = Animator.StringToHash("InputY");
        mouseX = Animator.StringToHash("MouseX");
        mouseY = Animator.StringToHash("MouseY");
        moveBool = Animator.StringToHash("IsMoving");
    }

    private void OnTransitionScene(GameScene transitionScene,Vector3 transitionPosition)
    {
        transform.position = transitionPosition;
        moveEndX = 0;
        moveEndY = -1;
        SwitchPlayerAnimation();
    }
    private void OnStartNewGameEvent(int obj)
    {
        transform.position = Settings.GameStartPlayerPos;
    }

    private void OnUpdateGameStateEvent(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Gameplay:
                inputDisable = false;
                break;
            case GameState.Pause:
                inputDisable = true;
                break;
        }
    }

    private void OnMouseClickedEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        if (useTool)
            return;

        if (itemDetails.itemType != ItemType.Seed && itemDetails.itemType != ItemType.Commodity && itemDetails.itemType != ItemType.Furniture)
        {
            mouseInputX = mouseWorldPos.x - transform.position.x;
            mouseInputY = mouseWorldPos.y - (transform.position.y + 0.85f);

            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
                mouseY = 0;
            else
                mouseX = 0;

            StartCoroutine(UseToolRoutine(mouseWorldPos, itemDetails));
        }
        else
        {
            //EventCenter.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);
        }
    }

    private IEnumerator UseToolRoutine(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        useTool = true;
        yield return null;
        foreach (Animator anim in allAnimators)
        {
            anim.SetTrigger("UseTool");

            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);
        }
        yield return new WaitForSeconds(0.45f);
        //EventCenter.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);
        yield return new WaitForSeconds(0.25f);

        useTool = false;
        inputDisable = false;
    }

    private void OnMoveToPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    private void OnAfterSceneLoadedEvent()
    {
        inputDisable = false;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        inputDisable = true;
    }

    //输入处理
    private void GetPlayerInput()
    {
        moveInput = InputManager.Instance.GetMovement();
        leftShift = InputManager.Instance.GetLeftShift();
        if (moveInput.x != 0 && moveInput.y != 0)
        {
            moveInput *= 0.6f;
        }
        moveInput = leftShift == true ? moveInput * 0.5f : moveInput;
        movingState = moveInput != Vector2.zero;
    }
    //移动
    private void PlayerMovement()
    {
        rb.MovePosition(rb.position + moveInput * speed * Time.deltaTime);
    }
    //动画改变
    private void SwitchPlayerAnimation()
    {
        foreach (Animator anim in allAnimators)
        {
            anim.SetBool(moveBool,movingState);
            if (movingState)
            {
                anim.SetFloat(moveX,moveInput.x);
                anim.SetFloat(moveY,moveInput.y);
                moveEndX = moveInput.x;
                moveEndY = moveInput.y;
            }
            else
            {
                anim.SetFloat(mouseX, moveEndX);
                anim.SetFloat(mouseY, moveEndY);
            }
        }
    }

    public string GetGuid()=> GetComponent<Guid>().ModuleGuid;


    public GameSaveData SaveGameData()
    {
        GameSaveData gameSaveData = new GameSaveData();
        gameSaveData.characterPositionDict = new Dictionary<CharacterName,CharacterInfos>();
        CharacterInfos infos = new CharacterInfos(transform.position);
        infos.endDirection = new Vector2Int((int)moveEndX,(int)moveEndY);
        if(gameSaveData.characterPositionDict.ContainsKey(CharacterName.Herry))
        {
            gameSaveData.characterPositionDict[CharacterName.Herry] = infos;
        }
        else
        {
            gameSaveData.characterPositionDict.Add(CharacterName.Herry, infos);
        }
        gameSaveData.moduleName = "Player";
        return gameSaveData;
    }

    public void LoadGameData(GameSaveData gameSaveData)
    {

        Vector3Int position = gameSaveData.characterPositionDict[CharacterName.Herry].position;
        moveEndX = gameSaveData.characterPositionDict[CharacterName.Herry].endDirection.x;
        moveEndY = gameSaveData.characterPositionDict[CharacterName.Herry].endDirection.y;
        SwitchPlayerAnimation();
        transform.position = new Vector3(position.x,position.y,position.z);
    }

    public void RegisterInterface()
    {
        SaveLoadManager.Instance.RegisterInterface(this);
    }
}
