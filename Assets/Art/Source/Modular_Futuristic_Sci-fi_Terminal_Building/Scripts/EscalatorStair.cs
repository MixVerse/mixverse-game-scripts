using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscalatorStair : MonoBehaviour
{
    [SerializeField]
    float direction = 1;
    [SerializeField]
    float lerpDuration = 1;
    float timeElapsed;

    Vector3 origin;
    Vector3 target;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        target = origin + (transform.forward * 0.133f * transform.localScale.z + transform.up * 0.1f * transform.localScale.y) * 2.45f * direction;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeElapsed < lerpDuration)
        {
            transform.localPosition = Vector3.Lerp(origin, target, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
        }
        else
        {
            timeElapsed = 0;
        }
    }
}
