using System;
using System.Collections;
using System.Collections.Generic;
using Farm.AStar;
using Farm.Save;
using Farm.Tool;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class NPCMovement : MonoBehaviour
{
    public ScheduleDataList_SO scheduleData;
    private SortedSet<ScheduleDetails> scheduleSet;
    private ScheduleDetails currentSchedule;

    //临时存储信息
    private GameScene currentScene;
    private GameScene targetScene;
    private Vector3Int currentGridPosition;
    private Vector3Int tragetGridPosition;
    private Vector3Int nextGridPosition;
    private Vector3 nextWorldPosition;

    public GameScene StartScene { set => currentScene = value; }

    [Header("移动属性")]
    public float normalSpeed = 2f;
    private float minSpeed = 1;
    private float maxSpeed = 3;
    private Vector2 dir;
    public bool IsMoving { private set; get; }
    public bool Interactable { private set; get; }

    //Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;
    private Animator anim;
    private Grid gird;
    private Stack<MovementStep> movementSteps;
    private Coroutine npcMoveRoutine;

    private bool isInitialised;
    private bool npcMove;
    private bool sceneLoaded;
    public bool isFirstLoad;
    public bool canInteract { private set; get; }
    private Season currentSeason;
    //动画计时器
    private float animationBreakTime;
    private bool canPlayStopAnimaiton;
    private AnimationClip stopAnimationClip;
    public AnimationClip blankAnimationClip;
    private AnimatorOverrideController animOverride;

    private TimeSpan GameTime => GameTimeManager.Instance.GameSpanTime();

    public void SetCurrentScene(GameScene gameScene) { currentScene = gameScene; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        movementSteps = new Stack<MovementStep>();

        animOverride = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = animOverride;
        scheduleSet = new SortedSet<ScheduleDetails>();

        foreach (var schedule in scheduleData.scheduleList)
        {
            scheduleSet.Add(schedule);
        }
    }

    private void OnEnable()
    {
       /* EventCenter.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventCenter.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;

        EventCenter.GameMinuteEvent += OnGameMinuteEvent;
        EventCenter.EndGameEvent += OnEndGameEvent;*/
        //EventCenter.StartNewGameEvent += OnStartNewGameEvent;
        //EventManager.AddEventListener<int>(ConstString.StartNewGameEvent, OnStartNewGameEvent);
    }

    private void OnDisable()
    {
     /*   EventCenter.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventCenter.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;

        EventCenter.GameMinuteEvent -= OnGameMinuteEvent;
        EventCenter.EndGameEvent -= OnEndGameEvent;*/
        //EventCenter.StartNewGameEvent -= OnStartNewGameEvent;
        //EventManager.RemoveEventListener<int>(ConstString.StartNewGameEvent, OnStartNewGameEvent);
    }


    private void Start()
    {
        /*ISaveLoad saveable = this;
        saveable.RegisterSaveable();*/
    }

    private void Update()
    {
        if (sceneLoaded)
            SwitchAnimation();

        //计时器
        animationBreakTime -= Time.deltaTime;
        canPlayStopAnimaiton = animationBreakTime <= 0;
    }

    private void FixedUpdate()
    {
        if (sceneLoaded)
            Movement();
    }
    private void OnEndGameEvent()
    {
        sceneLoaded = false;
        npcMove = false;
        if (npcMoveRoutine != null)
            StopCoroutine(npcMoveRoutine);
    }

    private void OnStartNewGameEvent(int obj)
    {
        isInitialised = false;
        isFirstLoad = true;
    }

    private void OnGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        int time = (hour * 100) + minute;
        currentSeason = season;

        ScheduleDetails matchSchedule = null;
        foreach (var schedule in scheduleSet)
        {
            if (schedule.Time == time)
            {
                if (schedule.day != day && schedule.day != 0)
                    continue;
                if (schedule.season != season)
                    continue;
                matchSchedule = schedule;
            }
            else if (schedule.Time > time)
            {
                break;
            }
        }
        if (matchSchedule != null)
            BuildShortestPath(matchSchedule);
    }

    private void OnBeforeSceneUnloadEvent()
    {
        sceneLoaded = false;
    }

    private void OnAfterSceneLoadedEvent()
    {
        gird = FindObjectOfType<Grid>();
        UpdateNPCVisiableState();

        if (!isInitialised)
        {
            InitNPC();
            isInitialised = true;
        }

        sceneLoaded = true;

        if (!isFirstLoad)
        {
            currentGridPosition = gird.WorldToCell(transform.position);
            var schedule = new ScheduleDetails(0, 0, 0, 0, currentSeason, targetScene, (Vector2Int)tragetGridPosition, stopAnimationClip, Interactable);
            BuildShortestPath(schedule);
            isFirstLoad = true;
        }
    }

    private void UpdateNPCVisiableState()
    {
        if (currentScene == GameTools.StringToEnum(SceneManager.GetSceneAt(SceneManager.sceneCount - 1).name))
            SetActiveInScene();
        else
            SetInactiveInScene();
    }


    private void InitNPC()
    {
        targetScene = currentScene;

        currentGridPosition = gird.WorldToCell(transform.position);
        transform.position = new Vector3(currentGridPosition.x + Settings.gridCellSize / 2f, currentGridPosition.y + Settings.gridCellSize / 2f, 0);

        tragetGridPosition = currentGridPosition;
    }

    /// <summary>
    /// 主要移动方法
    /// </summary>
    private void Movement()
    {
        if (!npcMove)
        {
            if (movementSteps.Count > 0)
            {
                MovementStep step = movementSteps.Pop();

                currentScene = step.sceneName;

                UpdateNPCVisiableState();
                nextGridPosition = (Vector3Int)step.gridCoordinate;
                TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);

                MoveToGridPosition(nextGridPosition, stepTime);
            }
            else if (!IsMoving && canPlayStopAnimaiton)
            {
                StartCoroutine(SetStopAnimation());
            }
        }
    }

    private void MoveToGridPosition(Vector3Int gridPos, TimeSpan stepTime)
    {
        npcMoveRoutine = StartCoroutine(MoveRoutine(gridPos, stepTime));
    }

    private IEnumerator MoveRoutine(Vector3Int gridPos, TimeSpan stepTime)
    {
        npcMove = true;
        nextWorldPosition = GetWorldPostion(gridPos);

        if (stepTime > GameTime)
        {
            float timeToMove = (float)(stepTime.TotalSeconds - GameTime.TotalSeconds);
            float distance = Vector3.Distance(transform.position, nextWorldPosition);
            float speed = Mathf.Max(minSpeed, (distance / timeToMove / Settings.secondThreshold));

            if (speed <= maxSpeed)
            {
                while (Vector3.Distance(transform.position, nextWorldPosition) > Settings.pixelSize)
                {
                    dir = (nextWorldPosition - transform.position).normalized;

                    Vector2 posOffset = new Vector2(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime);
                    rb.MovePosition(rb.position + posOffset);
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        rb.position = nextWorldPosition;
        currentGridPosition = gridPos;
        nextGridPosition = currentGridPosition;

        npcMove = false;
    }


    /// <summary>
    /// 根据Schedule构建路径
    /// </summary>
    /// <param name="schedule"></param>
    public void BuildShortestPath(ScheduleDetails schedule)
    {
        movementSteps.Clear();
        currentSchedule = schedule;
        targetScene = schedule.targetScene;
        tragetGridPosition = (Vector3Int)schedule.targetGridPosition;
        stopAnimationClip = schedule.clipAtStop;
        Interactable = schedule.npcCanInteractable;
        if (schedule.targetScene == currentScene)
        {
            //AStar.Instance.BuildShortestPath(schedule.targetScene, (Vector2Int)currentGridPosition, schedule.targetGridPosition, movementSteps);
        }
        else if (schedule.targetScene != currentScene)
        {
            SceneRoute sceneRoute = NPCManager.Instance.GetSceneRoute(currentScene, schedule.targetScene);
            if (sceneRoute != null)
            {
                for (int i = 0; i < sceneRoute.scenePathList.Count; i++)
                {
                    Vector2Int fromPos, gotoPos;
                    ScenePath path = sceneRoute.scenePathList[i];

                    if (path.fromGridCell.x >= Settings.maxGridSize)
                    {
                        fromPos = (Vector2Int)currentGridPosition;
                    }
                    else
                    {
                        fromPos = path.fromGridCell;
                    }

                    if (path.gotoGridCell.x >= Settings.maxGridSize)
                    {
                        gotoPos = schedule.targetGridPosition;
                    }
                    else
                    {
                        gotoPos = path.gotoGridCell;
                    }

                    //AStar.Instance.BuildShortestPath(path.sceneName, fromPos, gotoPos, movementSteps);
                }
            }
        }

        if (movementSteps.Count > 1)
        {
            UpdateTimeOnPath();
        }
    }


    /// <summary>
    /// 更新路径上每一步的时间
    /// </summary>
    private void UpdateTimeOnPath()
    {
        MovementStep previousSetp = null;

        TimeSpan currentGameTime = GameTime;

        foreach (MovementStep step in movementSteps)
        {
            if (previousSetp == null)
                previousSetp = step;

            step.hour = currentGameTime.Hours;
            step.minute = currentGameTime.Minutes;
            step.second = currentGameTime.Seconds;

            TimeSpan gridMovementStepTime;

            if (MoveInDiagonal(step, previousSetp))
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / normalSpeed / Settings.secondThreshold));
            else
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.secondThreshold));
            currentGameTime = currentGameTime.Add(gridMovementStepTime);
            previousSetp = step;
        }
    }

    /// <summary>
    /// 判断是否走斜方向
    /// </summary>
    /// <param name="currentStep"></param>
    /// <param name="previousStep"></param>
    /// <returns></returns>
    private bool MoveInDiagonal(MovementStep currentStep, MovementStep previousStep)
    {
        return (currentStep.gridCoordinate.x != previousStep.gridCoordinate.x) && (currentStep.gridCoordinate.y != previousStep.gridCoordinate.y);
    }

    /// <summary>
    /// 网格坐标返回世界坐标中心
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    private Vector3 GetWorldPostion(Vector3Int gridPos)
    {
        Vector3 worldPos = gird.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize / 2f, worldPos.y + Settings.gridCellSize / 2);
    }


    private void SwitchAnimation()
    {
        IsMoving = transform.position != GetWorldPostion(tragetGridPosition);

        anim.SetBool("IsMoving", IsMoving);
        if (IsMoving)
        {
            anim.SetBool("Exit", true);
            anim.SetFloat("DirX", dir.x);
            anim.SetFloat("DirY", dir.y);
        }
        else
        {
            anim.SetBool("Exit", false);
        }
    }

    private IEnumerator SetStopAnimation()
    {
        anim.SetFloat("DirX", 0);
        anim.SetFloat("DirY", -1);

        animationBreakTime = Settings.animationBreakTime;
        if (stopAnimationClip != null)
        {
            animOverride[blankAnimationClip] = stopAnimationClip;
            anim.SetBool("EventAnimation", true);
            yield return null;
            anim.SetBool("EventAnimation", false);
        }
        else
        {
            animOverride[stopAnimationClip] = blankAnimationClip;
            anim.SetBool("EventAnimation", false);
        }
    }

    #region 设置NPC显示情况
    private void SetActiveInScene()
    {
        spriteRenderer.enabled = true;
        coll.enabled = true;
        canInteract = true;

        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void SetInactiveInScene()
    {
        spriteRenderer.enabled = false;
        coll.enabled = false;
        canInteract = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }
    #endregion

  

}
