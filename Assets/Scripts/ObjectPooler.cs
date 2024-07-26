using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [Header("Object Pooler Settings: ")]
    [SerializeField] private List<GameObject> _pooledGroundObstacles = new List<GameObject>();
    [SerializeField] private GameObject _groundObstacle;

    [SerializeField] private List<GameObject> _pooledAirObstacle = new List<GameObject>();
    [SerializeField] private GameObject _airObstacle;

    [SerializeField] private int _poolAmount;

    [Header("Object Pooler Dependencies: ")]
    [SerializeField] private Transform _obstacleHolder;

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
}
