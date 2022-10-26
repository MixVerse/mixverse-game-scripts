using System.Collections.Generic;
using UnityEngine;

namespace PvrEvent
{
    public enum EHandEvent
    {
        Right,
        Left
    }

    public class Pvr_UEventManager : MonoBehaviour
    {
        public static Pvr_UEventManager Instance { get; private set; }

        [SerializeField] List<Pvr_UEvent> list_events;

        Dictionary<EHandEvent, Pvr_UEvent> DicEvents;

        public GameObject GetObjUI(EHandEvent hand = EHandEvent.Right)
        {
            if (DicEvents.ContainsKey(hand))
            {
                return DicEvents[hand].Current;
            }
            return null;
        }

        #region 生命周期

        private void Awake()
        {
            Instance = this;

            DicEvents = new Dictionary<EHandEvent, Pvr_UEvent>();
            foreach (Pvr_UEvent @event in list_events)
            {
                if (@event)
                {
                    if (!DicEvents.ContainsKey(@event.handEvent))
                    {
                        DicEvents.Add(@event.handEvent, @event);
                    }
                }
            }
        }

        private void OnDisable()
        {
            if (Instance != null) Instance = null;
        } 

        #endregion
    } 
}
