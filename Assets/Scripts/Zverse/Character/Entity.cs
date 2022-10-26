using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Mirror;
using TMPro;

[Serializable] public class UnityEventEntity : UnityEvent<Entity> { }
[Serializable] public class UnityEventEntityInt : UnityEvent<Entity, int> { }

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))] // kinematic, only needed for OnTrigger
[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public abstract partial class Entity : NetworkBehaviour
{
    [Header("基本组件")]
    public Movement movement;
    public Animator animator;
    public new Collider collider;
    public AudioSource audioSource;


    [Header("金币")]
    [SyncVar, SerializeField] long _gold = 0;
    public long gold { get { return _gold; } set { _gold = Math.Max(value, 0); } }

    [Header("速度")]
    [SerializeField] protected float _speed = 5;

    [Header("实体头顶显示的名称")]
    public TextMeshPro stunnedOverlay;

    [Header("Events")]
    public UnityEventEntity onAggro;
    public UnityEvent onSelect; // called when clicking it the first time
    public UnityEvent onInteract; // called when clicking it the second time

    public override void OnStartServer()
    {

    }

    protected virtual void Start()
    {
        if (!isClient) animator.enabled = false;
    }

    // server function to check which entities need to be updated.
    // monsters, npcs etc. don't have to be updated if no player is around
    // checking observers is enough, because lonely players have at least
    // themselves as observers, so players will always be updated
    // and dead monsters will respawn immediately in the first update call
    // even if we didn't update them in a long time (because of the 'end'
    // times)
    // -> update only if:
    //    - has observers
    //    - if the entity is hidden, otherwise it would never be updated again
    //      because it would never get new observers
    // -> can be overwritten if necessary (e.g. pets might be too far from
    //    observers but should still be updated to run to owner)
    // => only call this on server. client should always update!
    [Server]
    public virtual bool IsWorthUpdating()
    {
        return netIdentity.observers.Count > 0 ||
               IsHidden();
    }

    // entity logic will be implemented with a finite state machine
    // -> we should react to every state and to every event for correctness
    // -> we keep it functional for simplicity
    // note: can still use LateUpdate for Updates that should happen in any case
    void Update()
    {


    }

    public virtual void OnDrawGizmos()
    {

    }

    // visibility //////////////////////////////////////////////////////////////
    // hide an entity
    // note: using SetActive won't work because its not synced and it would
    //       cause inactive objects to not receive any info anymore
    // note: this won't be visible on the server as it always sees everything.
    [Server]
    public void Hide() => netIdentity.visible = Visibility.ForceHidden;

    [Server]
    public void Show() => netIdentity.visible = Visibility.Default;

    // is the entity currently hidden?
    // note: usually the server is the only one who uses forceHidden, the
    //       client usually doesn't know about it and simply doesn't see the
    //       GameObject.
    public bool IsHidden() => netIdentity.visible == Visibility.ForceHidden;

    public float VisRange() => ((SpatialHashingInterestManagement)NetworkServer.aoi).visRange;



    // selection & interaction /////////////////////////////////////////////////
    // use Unity's OnMouseDown function. no need for raycasts.
    void OnMouseDown()
    {
        
    }

    protected virtual void OnSelect() { }
    protected abstract void OnInteract();

    // ontrigger ///////////////////////////////////////////////////////////////
    // protected so that inheriting classes can use OnTrigger too, while also
    // calling those here via base.OnTriggerEnter/Exit
    protected virtual void OnTriggerEnter(Collider col)
    {
        // check if trigger first to avoid GetComponent tests for environment
       // if (col.isTrigger && col.GetComponent<SafeZone>())
        //    inSafeZone = true;
    }

    protected virtual void OnTriggerExit(Collider col)
    {
        // check if trigger first to avoid GetComponent tests for environment
       // if (col.isTrigger && col.GetComponent<SafeZone>())
       //     inSafeZone = false;
    }
}
