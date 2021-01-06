public delegate void CallBack();
public delegate void CallBack<T>(T arg);
public delegate void CallBack<T, K>(T arg0, K arg1);
public delegate void CallBack<T, K ,V>(T arg0, K arg1, V arg2);

public enum EventType
{
    TIPS, // 产生tips
    BATTLEINFO, // 发送战斗信息
    /// <summary>
    /// 导表错误
    /// </summary>
    CFGERROR,
    /// <summary>
    /// 清理战斗记录
    /// </summary>
    CLEAR_BATTLE_INFO,
    /// <summary>
    /// 游戏 触发 事件
    /// </summary>
    EVENT, 
    /// <summary>
    /// 怪物死亡
    /// </summary>
    MONSTERDIE, // 失败
    /// <summary>
    /// 主角死亡
    /// </summary>
    PLAYERDIE, 
    /// <summary>
    /// 添加任务
    /// </summary>
    ADD_TASK,
    /// <summary>
    /// 完成任务
    /// </summary>
    FINISH_TASK,
    /// <summary>
    /// 清空任务
    /// </summary>
    CLEAR_TASK,
}