using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTex : MonoBehaviour
{
    public float scrollX = 0.5f; //Horizontal scrolling speed
    public float scrollY = 0.0f; //Vertical scrolling speed

    Vector2[] materialOffsets; //Initial material offsets

    private void Start()
    {
        //Saves the initial material offsets
        materialOffsets = new Vector2[GetComponent<MeshRenderer>().materials.Length];
        for (int i = 0; i < GetComponent<MeshRenderer>().materials.Length; i++)
        {
            materialOffsets[i] = GetComponent<MeshRenderer>().materials[i].GetTextureOffset("_EmissiveColorMap");
        }
    }

    private void Update()
    {
        float offsetX = Time.time * scrollX; //Sets the realtime scrolling horizontal speed
        float offsetY = Time.time * scrollY; //Sets the realtime vertical scrolling speed

        //Sets the scrolling offset for each material in the object
        for (int i = 0; i < GetComponent<MeshRenderer>().materials.Length; i++)
        {
            GetComponent<MeshRenderer>().materials[i].SetTextureOffset("_EmissiveColorMap", new Vector2(offsetX, offsetY) + materialOffsets[i]);
        }
    }
}
