using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ZverseSceneObject : NetworkBehaviour
{
    /*
    [SyncVar(hook = nameof(OnChangeEquipment))]
    public EquippedItem equippedItem;

    public GameObject ballPrefab;
    public GameObject boxPrefab;
    public GameObject cylinderPrefab;

    void OnChangeEquipment(EquippedItem oldEquippedItem, EquippedItem newEquippedItem)
    {
        StartCoroutine(ChangeEquipment(newEquippedItem));
    }


    IEnumerator ChangeEquipment(EquippedItem newEquippedItem)
    {
        while (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
            yield return null;
        }

        SetEquippedItem(newEquippedItem);
    }


    public void SetEquippedItem(EquippedItem newEquippedItem)
    {
        switch (newEquippedItem)
        {
            case EquippedItem.ball:
                Instantiate(ballPrefab, transform);
                break;
            case EquippedItem.box:
                Instantiate(boxPrefab, transform);
                break;
            case EquippedItem.cylinder:
                Instantiate(cylinderPrefab, transform);
                break;
        }
    }
    */
}
