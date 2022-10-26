using UnityEngine;
using UnityEngine.Events;
using Mirror;

/// <summary>
/// 能量条（血条 蓝条等）的父类
/// </summary>
public abstract class Energy : NetworkBehaviour
{
    //当前值，保持在 min 和 max 范围内
    [SyncVar] int _current = 0;
    public int current
    {
        get { return Mathf.Min(_current, max); }
        set
        {
            bool emptyBefore = _current == 0;
            _current = Mathf.Clamp(value, 0, max);
            if (_current == 0 && !emptyBefore) onEmpty.Invoke();
        }
    }

    //最大值，与多种因素有关 buff，药剂等
    public abstract int max { get; }

    //能量条数值回复速率
    public abstract int recoveryRate { get; }

    //不能在死亡状态下回复能量，这用于判断是否死亡
    public Health health;

    //是否满能量生成
    public bool spawnFull = true;

    [Header("Events")]
    public UnityEvent onEmpty;

    public override void OnStartServer()
    {
        // set full energy on start if needed
        if (spawnFull) current = max;

        // recovery every second
        InvokeRepeating(nameof(Recover), 1, 1);
    }

    // 能量百分比
    public float Percent() =>
        (current != 0 && max != 0) ? (float)current / (float)max : 0;

    //每秒回复能量
    [Server]
    public void Recover()
    {
        if (enabled && health.current > 0)
            current += recoveryRate;
    }
}