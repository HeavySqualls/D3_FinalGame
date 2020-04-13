using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitySpeedupTrigger : MonoBehaviour
{
    public SilentCity silentCity;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            silentCity.IncreaseSpeed();
        }
    }
}
