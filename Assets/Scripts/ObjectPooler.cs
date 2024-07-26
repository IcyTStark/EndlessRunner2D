using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [Header("Obstacle Pooler Settings: ")]
    [SerializeField] private List<GameObject> _pooledGroundObstacles = new List<GameObject>();
    [SerializeField] private GameObject _groundObstacle;

    [SerializeField] private List<GameObject> _pooledAirObstacle = new List<GameObject>();
    [SerializeField] private GameObject _airObstacle;

    [SerializeField] private int _poolAmount;

    [Header("PowerUp Pooler Settings: ")]
    [SerializeField] private List<GameObject> _pooledPowerUp = new List<GameObject>();
    [SerializeField] private GameObject _pooledPowerUpObject;

    [SerializeField] private int _poolPowerUpAmount;

    [Header("Object Pooler Dependencies: ")]
    [SerializeField] private Transform _obstacleHolder;
    [SerializeField] private Transform _powerUpHolder;

    private void Start()
    {
        GameObject temp;
        for (int i = 0; i < _poolAmount; i++)
        {
            temp = Instantiate(_groundObstacle, _obstacleHolder);
            temp.SetActive(false);
            _pooledGroundObstacles.Add(temp);
        }

        for (int i = 0; i < _poolAmount; i++)
        {
            temp = Instantiate(_airObstacle, _obstacleHolder);
            temp.SetActive(false);
            _pooledAirObstacle.Add(temp);
        }

        for (int i = 0; i < _poolPowerUpAmount; i++)
        {
            temp = Instantiate(_pooledPowerUpObject, _powerUpHolder);
            temp.SetActive(false);
            _pooledPowerUp.Add(temp);
        }
    }

    public GameObject GetGroundPooledObject()
    {
        for (int i = 0; i < _poolAmount; i++)
        {
            if (!_pooledGroundObstacles[i].activeInHierarchy)
            {
                return _pooledGroundObstacles[i];
            }
        }
        return null;
    }

    public GameObject GetAirPooledObject()
    {
        for (int i = 0; i < _poolAmount; i++)
        {
            if (!_pooledAirObstacle[i].activeInHierarchy)
            {
                return _pooledAirObstacle[i];
            }
        }
        return null;
    }

    public GameObject GetPowerUpPooledObject()
    {
        for (int i = 0; i < _poolPowerUpAmount; i++)
        {
            if (!_pooledPowerUp[i].activeInHierarchy)
            {
                return _pooledPowerUp[i];
            }
        }
        return null;
    }
}
