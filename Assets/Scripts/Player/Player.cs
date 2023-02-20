using System.Collections;
using System.Collections.Generic;
using Farm.Save;
using UnityEngine;
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField]private float speed;
    private float inputX;
    private float inputY;
    private Vector2 movementInput;

    private Animator[] animators;
    private bool isMoving;
    public bool inputDisable;

    //使用工具动画
    private float mouseX;
    private float mouseY;
    private bool useTool;

/*    private int inputX;
    private int inputY;*/

    public string GUID => GetComponent<DataGUID>().guid;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
        inputDisable = true;
    }

    private void Start()
    {
       /* ISaveable saveable = this;
        saveable.RegisterSaveable();*/
    }

    private void OnEnable()
    {
        EventCenter.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventCenter.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventCenter.MoveToPosition += OnMoveToPosition;
        EventCenter.MouseClickedEvent += OnMouseClickedEvent;
        EventCenter.UpdateGameStateEvent += OnUpdateGameStateEvent;
        EventCenter.StartNewGameEvent += OnStartNewGameEvent;
        EventCenter.EndGameEvent += OnEndGameEvent;
    }

    private void OnDisable()
    {
        EventCenter.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventCenter.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventCenter.MoveToPosition -= OnMoveToPosition;
        EventCenter.MouseClickedEvent -= OnMouseClickedEvent;
        EventCenter.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventCenter.StartNewGameEvent -= OnStartNewGameEvent;
        EventCenter.EndGameEvent -= OnEndGameEvent;
    }

    private void Update()
    {
        if (!inputDisable)
            PlayerInput();
        else
            isMoving = false;
        SwitchAnimation();
    }

    private void FixedUpdate()
    {
        if (!inputDisable)
            Movement();
    }

    private void OnStartNewGameEvent(int obj)
    {
        inputDisable = false;
        transform.position = Settings.playerStartPos;
    }

    private void OnEndGameEvent()
    {
        inputDisable = true;
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

        //执行动画
        if (itemDetails.itemType != ItemType.Seed && itemDetails.itemType != ItemType.Commodity && itemDetails.itemType != ItemType.Furniture)
        {
            mouseX = mouseWorldPos.x - transform.position.x;
            mouseY = mouseWorldPos.y - (transform.position.y + 0.85f);

            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
                mouseY = 0;
            else
                mouseX = 0;

            StartCoroutine(UseToolRoutine(mouseWorldPos, itemDetails));
        }
        else
        {
            EventCenter.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);
        }
    }

    private IEnumerator UseToolRoutine(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        useTool = true;
        inputDisable = true;
        yield return null;
        foreach (var anim in animators)
        {
            anim.SetTrigger("UseTool");
            //人物的面朝方向
            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);
        }
        yield return new WaitForSeconds(0.45f);
        EventCenter.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);
        yield return new WaitForSeconds(0.25f);
        //等待动画结束
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


    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (inputX != 0 && inputY != 0)
        {
            inputX = inputX * 0.6f;
            inputY = inputY * 0.6f;
        }

        //走路状态速度
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX = inputX * 0.5f;
            inputY = inputY * 0.5f;
        }
        movementInput = new Vector2(inputX, inputY);

        isMoving = movementInput != Vector2.zero;
    }

    private void Movement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }

    private void SwitchAnimation()
    {
        foreach (var anim in animators)
        {
            anim.SetBool("IsMoving", isMoving);
            anim.SetFloat("MouseX", mouseX);
            anim.SetFloat("MouseY", mouseY);

            if (isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
        }
    }

    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.characterPosDict = new Dictionary<string, SerializableVector3>();
        saveData.characterPosDict.Add(this.name, new SerializableVector3(transform.position));

        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        var targetPosition = saveData.characterPosDict[this.name].ToVector3();

        transform.position = targetPosition;
    }
}
