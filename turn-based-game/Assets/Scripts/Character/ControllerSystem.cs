
 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterState { 

    Idel,
    Run,
    Walk,
    Command,
    Skill,
    Atk,
    CGAtk,
    Flinch,
    Dizz,

};

public class ControllerSystem : MonoBehaviour
{
    private ICharacter m_Character;

    private float moveSpeed = 10;
    public float atkDamageTime = 0;
    
    // 各个阶段的时间延迟
    public Dictionary<CharacterState, float> stateDelay = new Dictionary<CharacterState, float>();
     
    private int m_CurrMovePower = 0;
    public bool isMoving = false;
    public bool isRunning = false;
 
    // 移动目标点
    private Stack<Ceil> m_MoveCeilBuffer = new Stack<Ceil>();
    private List<Vector3> m_PathVecList = new List<Vector3>();
    private Ceil m_NextMoveCeil = null;
    private bool m_showLine = false; 

    // 遭遇后触发的事件
    public Action<ICharacter> encounterEvent;
    public Action endMoveEvent;
    public Transform effectPos;
    public Transform lookPos;
     
    private HUDCanvas m_Canvas;
    private LineRenderer m_Line;
    private Vector3 m_RunPos;

    private CharacterState m_CharacterState = CharacterState.Idel;
    private Animator m_Animator;
    private AnimatorStateInfo stateinfo;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        
        effectPos = transform.Find("EffectPos");
        lookPos = transform.Find("LookPos"); 
        m_Canvas = transform.Find("monster/Dummy_root").GetComponentInChildren<HUDCanvas>();
        Transform trans = transform.Find("LinePos");
        if(trans != null)
            m_Line = trans.GetComponent<LineRenderer>();
    }

    public void OnInit() {
        m_MoveCeilBuffer.Clear();
        m_NextMoveCeil = null;
        StopMoving();
    }
    public void ClearMoveBuffer() {
        m_MoveCeilBuffer.Clear();
        m_NextMoveCeil = null;
    }

    public void ShowValue(Value value)
    {
        m_Canvas.ShowValue(value);
    }
    public Transform GetValuePos() { return m_Canvas.GetValuePos(); }
    private void EndCurrState() {

        switch (m_CharacterState)
        {
            case CharacterState.Idel:
            case CharacterState.Skill: 
            case CharacterState.Atk: 
            case CharacterState.CGAtk: 
            case CharacterState.Flinch: 
            case CharacterState.Command:
                break;
            case CharacterState.Run:
                m_Animator.SetBool("Run",false);
                break;
            case CharacterState.Walk:
                m_Animator.SetBool("Walk", false);
                break;
            case CharacterState.Dizz:
                m_Animator.SetBool("Dizz", false);
                break;
            default:
                break;
        }
        m_CharacterState = CharacterState.Idel;
    }

    private void SwitchState(CharacterState state){
        EndCurrState();
        switch (state)
        {
            case CharacterState.Idel:
                m_Animator.SetBool("Run", false);
                m_Animator.SetBool("Walk", false);
                m_Animator.SetBool("Dizz", false);
                break;
            case CharacterState.Run:
                m_Animator.SetBool("Run",true);
                break;
            case CharacterState.Walk:
                m_Animator.SetBool("Walk", true);
                break;
            case CharacterState.Command:
                m_Animator.SetTrigger("Command");
                break;
            case CharacterState.Skill:
                m_Animator.SetTrigger("Skill");
                break;
            case CharacterState.Atk:
                m_Animator.SetTrigger("Atk");
                break;
            case CharacterState.CGAtk:
                m_Animator.SetTrigger("CGAtk");
                break;
            case CharacterState.Flinch:
                m_Animator.SetTrigger("Flinch");
                break;
            case CharacterState.Dizz:
                m_Animator.SetBool("Dizz", true);
                break;
            default:
                break;
        }
        m_CharacterState = state;
    }

    public HUDCanvas GetHUDCanvas() { return m_Canvas; }

    public Ceil GetNextMoveCeil() {
        return m_NextMoveCeil;
    }
    public void RegisterCharacter(ICharacter character) {
        this.m_Character = character;
        m_Canvas.RegisterName(m_Character.name);
    }

    public void RegisterMoveCeilBuffer(Stack<Ceil> moveCeils) {
         
        if (moveCeils == null || moveCeils.Count == 0) {  
            m_MoveCeilBuffer.Clear();
            if (isMoving && m_NextMoveCeil != null) m_MoveCeilBuffer.Push(m_NextMoveCeil);
            return;
        }

        Debug.Log(m_Character.name +"   注册 ： " + moveCeils.Count + " 目前 ： " + m_MoveCeilBuffer.Count);
        this.m_MoveCeilBuffer = moveCeils;
        if (isMoving == false) {
            m_NextMoveCeil = m_MoveCeilBuffer.Peek(); // 提前取出
        }

        if (m_Character.isEndMapRound == false && isMoving == false) {
            StartMoving();
        }
    }

    public void ShowPathLine(List<Vector3> pathList)
    {
        m_showLine = true;
        m_PathVecList = pathList;
         
        m_Line.gameObject.SetActive(true);

        m_PathVecList.Add(new Vector3(transform.position.x, 0, transform.position.z));
        m_Line.positionCount = m_PathVecList.Count;
        m_Line.SetPositions(pathList.ToArray());
    }

    public void ResetAndAddMoveCeilBuffer(Ceil moveCeil) {
        this.m_MoveCeilBuffer.Clear();
        this.m_MoveCeilBuffer.Push(moveCeil);
        if (m_NextMoveCeil == null)
        {
            m_NextMoveCeil = m_MoveCeilBuffer.Peek(); // 提前取出
        }

        if (m_Character.isEndMapRound == false && isMoving == false)
        {
            StartMoving();
        }
    }

    bool _flag = true;
    IEnumerator IEndStateCallBack(string name,Action action) {
        while (true) {
            stateinfo = m_Animator.GetCurrentAnimatorStateInfo(0); 
            if (stateinfo.IsName( name))
            { 
                _flag = false;
                yield return null; 
            }
            else if (_flag)
            { 
                yield return null;
            }
            else { 
                break;
            }
        } 
        action?.Invoke(); 
    }

    private void EndStateCallBack(string name, Action action) { 
        _flag = true;
        StartCoroutine(IEndStateCallBack(name,action));
    }


    public float Atk(ICharacter target,Action callBack) {

        // 原本朝向
        Vector3 dir = transform.forward;
        Vector3 pos = transform.position;

        Transform targetTrans = target.GetControllerSystem().transform;
        // 先移动到物体身边
        m_RunPos = targetTrans.position + targetTrans.forward * Config.ATK_DISTANCE; 

        Debug.Log(transform.position + "  :  " + target.gameObject.transform.position);
        endMoveEvent = () => {
            // 攻击
            SwitchState(CharacterState.Atk);
            EndStateCallBack("Atk", () =>
            {
                // 回来
                endMoveEvent = () =>
                {
                    AdjustDirection(dir);
                };
                StartRunning(pos);

            });
        };

        //ZTimerSvc.AddTask(Config.ATK_FLINCH_TIME, () => {
        //    target.GetControllerSystem().Flinch();
        //});
        StartRunning(m_RunPos);
        ZTimerSvc.AddTask(atkDamageTime, callBack); 
        return stateDelay[CharacterState.Atk];
    }
    
    
    public float Skill(Action callBack)
    {
        SwitchState(CharacterState.Skill);

        ZTimerSvc.AddTask(stateDelay[CharacterState.Skill], callBack);

        return stateDelay[CharacterState.Skill];
    }
    public float Command(Action callBack)
    {
        SwitchState(CharacterState.Command);

        ZTimerSvc.AddTask(stateDelay[CharacterState.Command], callBack);

        return stateDelay[CharacterState.Command];
    }
    public void Flinch()
    {
        SwitchState(CharacterState.Flinch);
    }

    public void Dizziness()
    {
        SwitchState(CharacterState.Dizz);
    }
    public void Idel()
    {
        SwitchState(CharacterState.Idel);
    }
    public void StartMoving() {
        
        if (m_NextMoveCeil == null || isMoving) return;

        isMoving = true;
        m_CurrMovePower = m_Character.actionPower;
        SwitchState(CharacterState.Run);
    }
    public void StopMoving() {
        isMoving = false; 
        SwitchState(CharacterState.Idel);
    }
    private void StartRunning(Vector3 pos)
    {
        m_RunPos = pos;
        isRunning = true;
        SwitchState(CharacterState.Run);
    }
    private void StopRunning()
    {
        isRunning = false;
        SwitchState(CharacterState.Idel);
    }

     
    private void AdjustDirection(Vector3 dir) {
        float angle = Vector3.Angle(transform.forward, dir); 
        if (angle > 0.1f)
        {
            int sign = Vector3.Cross(transform.forward, dir).y >= 0 ? 1 : -1;
            transform.Rotate(Vector3.up, Mathf.Abs(angle) * sign);
        }
    }

    public void DisableLine() {
        m_showLine = false;
        m_Line?.gameObject.SetActive(false);
    }

    private void TriggerEncounterEvent(Action<ICharacter,Action> action) {
        if (m_NextMoveCeil.isSelected && m_Character is Player) 
            m_NextMoveCeil.CancelSelected(); 
        StopMoving();
        DisableLine();
        --m_CurrMovePower;
        action(m_Character,()=> {
             
            //TODO 回调无效               
            // 检查nextCeil 是否与 m_MoveCeilBuffer 第一个相同 不同：则立即替换 
            m_NextMoveCeil = m_MoveCeilBuffer.Peek();
            if (m_CurrMovePower != 0 && m_MoveCeilBuffer.Count != 0)
                StartMoving();
            else
                m_Character.isEndMapRound = true;
        });
    }

    /*
     
    如果最后移动点是一个怪物或者事件
     
     */
    private void Move()
    { 

        Vector3 target = m_NextMoveCeil.Position;
        Vector3 dir = new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z);
        Vector3 curr = new Vector3( transform.position.x, 0, transform.position.z);
        AdjustDirection(dir);
        if (m_NextMoveCeil.Character != null)
        { 
            TriggerEncounterEvent(m_NextMoveCeil.Character.EncounterEvent);
        }
        else if (m_NextMoveCeil.Building != null)
        { 
            TriggerEncounterEvent(m_NextMoveCeil.Building.EncounterEvent);
        }
        else {  
            float distance = Vector3.Distance(curr, target); 
            if (distance <= 0.001f)
            {
                --m_CurrMovePower;

                if (m_MoveCeilBuffer.Peek() == m_NextMoveCeil) { 
                    Ceil tmp = m_MoveCeilBuffer.Pop();
                    Debug.Log("弹出：" + tmp  + "还剩: " + m_MoveCeilBuffer.Count);
                }
                // 注册人物
                m_NextMoveCeil.Character = m_Character;
                // 更新下一位置
                if (m_MoveCeilBuffer.Count != 0) m_NextMoveCeil = m_MoveCeilBuffer.Peek();
                else {　
                    if (m_NextMoveCeil.isSelected && m_Character is Player)
                        m_NextMoveCeil.CancelSelected();
                    m_NextMoveCeil = null;
                }
                
                // 查看是否回合结束
                if (m_MoveCeilBuffer.Count == 0 || m_CurrMovePower == 0) {
                    m_Character.isEndMapRound = true;
                    StopMoving(); 
                }
            }
            else {  
                Vector3 moveDir = dir.normalized * moveSpeed * Time.deltaTime; 
                if (distance <= moveDir.magnitude) { transform.position = new Vector3(target.x,transform.position.y,target.z); }
                else transform.Translate(moveDir,Space.World);
            } 
        }

    }

    private void Run() {
        float distance = Vector3.Distance(transform.position, m_RunPos);
        if (distance <= 0.001f)
        {
            StopRunning();
            endMoveEvent?.Invoke();
            endMoveEvent = null; 
        }
        else
        {
            Vector3 dir = new Vector3(m_RunPos.x - transform.position.x, 0, m_RunPos.z - transform.position.z);
            Vector3 moveDir = dir.normalized * moveSpeed * Time.deltaTime;
            AdjustDirection(moveDir); 
            if (distance <= moveDir.magnitude) { transform.position = new Vector3(m_RunPos.x, transform.position.y, m_RunPos.z); }
            else transform.Translate(moveDir, Space.World);
        }
    }

    private void LineUpdate() {
        int idx = m_Line.positionCount;
        if (idx <= 1) {
            DisableLine();
            return;
        }
        Vector3 vec = m_PathVecList[idx-2];
        Vector3 curr = new Vector3(transform.position.x, 0, transform.position.z);
        float distance = Vector3.Distance(curr, vec);
        if (distance <= 0.001f)
        {
            m_PathVecList.RemoveAt(idx - 2);
            m_Line.positionCount = m_PathVecList.Count;
            m_Line.SetPositions(m_PathVecList.ToArray());
        }
        else {
            m_PathVecList[idx - 1] = curr;
            m_Line.SetPosition(idx-1, curr);
        }
    }

    private void Update()
    {
        if (isMoving) Move();
        if (isRunning) Run();
        if (m_showLine) LineUpdate();
    }
}
