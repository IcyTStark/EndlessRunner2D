using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField] public List<GameObject> _pooledObstacles = new List<GameObject>();
    [SerializeField] public GameObject _obstaclesToPool;
    public int _poolAmount;

    private void Start()
    {
        GameObject temp;
        for (int i = 0; i < _poolAmount; i++)
        {
            temp = Instantiate(_obstaclesToPool);
            temp.SetActive(false);
            _pooledObstacles.Add(temp);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < _poolAmount; i++)
        {
            if (!_pooledObstacles[i].activeInHierarchy)
            {
                return _pooledObstacles[i];
            }
        }
        return null;
    }
}
