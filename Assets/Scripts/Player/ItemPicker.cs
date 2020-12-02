using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPicker : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D info) 
    {
        if(info.name == "Player")

        Destroy(gameObject);
    }
}
