using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType
{
    SHIELD
}

public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerUpType _powerUpType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            switch (_powerUpType)
            {
                case PowerUpType.SHIELD:
                    playerController.OnShieldPickedUp();
                    break;
                default:
                    break;
            }
        }
    }
}
