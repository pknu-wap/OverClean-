using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSortingZoneScript : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name.Contains("Player"))
        {
            other.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, 0.1f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.name.Contains("Player"))
        {
            other.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, -2f);
        }
    }
}
