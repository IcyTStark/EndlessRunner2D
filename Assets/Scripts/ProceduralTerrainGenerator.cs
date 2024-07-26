using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class ProceduralTerrainGenerator : MonoBehaviour
{
    [SerializeField] private SpriteShapeController _spriteShapeController;
    [SerializeField] private Transform _playerReference;

    [Header("Terrain Generation Settings")]
    [SerializeField, Range(3, 100)] private int _initialLevelLength = 50;
    [SerializeField, Range(1f, 50f)] private float _xMultiplier = 2f;
    [SerializeField, Range(1f, 50f)] private float _yMultiplier = 2f;
    [SerializeField, Range(0f, 1f)] private float _curveSmoothness = 0.5f;
    [SerializeField] private float _noiseStep = 0.5f;
    [SerializeField] private float _bottomDepth = 10f;

    [Header("Dynamic Generation Settings")]
    [SerializeField] private float _playerDistanceThreshold = 10f;
    [SerializeField] private int _newSegmentsPerGeneration = 10;

    [SerializeField] private int _currentLevelLength;
    private Vector3 _lastGeneratedPosition;
    private bool _isGeneratingNewSegment;

    private int _pointsToDeletePerFrame = 5;
    private int firstTerrainPointIndex = 0;

    private int _newPointPositionIndexValue = 0;

    [Header("Obstacle Controllers: ")]
    [SerializeField] private ObjectPooler _objectPooler;
    [SerializeField] private List<int> _randomSpawnIndex;
    [SerializeField] private List<GameObject> _activeObstacles;

    [Header("PowerUp Controllers: ")]
    [SerializeField] private int _randomSpawnIndexForPowerUps;
    [SerializeField] private List<GameObject> _activePowerUps;

    private void Start()
    {
        Initialize();
    }

    private void Initialize(Action onGenerated = null)
    {
        _currentLevelLength = _initialLevelLength;

        GenerateInitialTerrain();

        GenerateObstacles();

        onGenerated?.Invoke();
    }

    private void Update()
    {
        if (GameManager.Instance.HasGameStarted)
        {
            CheckAndGenerateNewSegment();
        }
    }

    private void GenerateInitialTerrain(Action onGenerated = null)
    {
        _spriteShapeController.spline.Clear();

        for (int i = 0; i < _currentLevelLength; i++)
        {
            AddTerrainPoint(i);
        }

        AddBottomPoints();
    }

    private void AddTerrainPoint(int index, bool isFromNewGeneration = false)
    {
        Vector3 position = CalculatePointPosition(index, isFromNewGeneration);

        _spriteShapeController.spline.InsertPointAt(index, position);

        if (index > 0)
        {
            SetPointTangents(index);
        }

        _lastGeneratedPosition = position;
    }

    private Vector3 CalculatePointPosition(int index, bool isFromNewGeneration)
    {
        index = isFromNewGeneration ? (index + _newPointPositionIndexValue) : index;

        if (index < 2)
        {
            return transform.position + Vector3.right * index * _xMultiplier;
        }
        return transform.position + new Vector3(index * _xMultiplier, Mathf.PerlinNoise(0, index * _noiseStep) * _yMultiplier);
    }

    private void SetPointTangents(int index)
    {
        _spriteShapeController.spline.SetTangentMode(index, ShapeTangentMode.Continuous);
        float tangent = _xMultiplier * _curveSmoothness;
        _spriteShapeController.spline.SetLeftTangent(index, Vector3.left * tangent);
        _spriteShapeController.spline.SetRightTangent(index, Vector3.right * tangent);
    }

    private void AddBottomPoints()
    {
        Vector3 bottomRight = new Vector3(_lastGeneratedPosition.x, transform.position.y - _bottomDepth);
        Vector3 bottomLeft = new Vector3(transform.position.x, transform.position.y - _bottomDepth);

        _spriteShapeController.spline.InsertPointAt(_currentLevelLength, bottomRight);
        _spriteShapeController.spline.InsertPointAt(_currentLevelLength + 1, bottomLeft);
    }

    private void CheckAndGenerateNewSegment()
    {
        if (_isGeneratingNewSegment)
        {
            return;
        }

        if (_playerReference != null)
        {
            float playerDistance = Vector3.Distance(_playerReference.position, _spriteShapeController.spline.GetPosition(_currentLevelLength - 2));

            if (playerDistance < _playerDistanceThreshold)
            {
                RemovePreviousPoints();

                StartCoroutine(GenerateNewSegment());
            }
        }
    }

    private IEnumerator GenerateNewSegment()
    {
        _isGeneratingNewSegment = true;

        int newSegmentEnd = _currentLevelLength + _newSegmentsPerGeneration;

        for (int i = _currentLevelLength; i < newSegmentEnd; i++)
        {
            AddTerrainPoint(i, true);
            yield return null;
        }

        _currentLevelLength = newSegmentEnd;
        UpdateBottomPoints();

        _isGeneratingNewSegment = false;

        GenerateObstacles();

        GeneratePowerUps();
    }

    private void UpdateBottomPoints()
    {
        _spriteShapeController.spline.RemovePointAt(_currentLevelLength + 1);
        _spriteShapeController.spline.RemovePointAt(_currentLevelLength);

        Vector3 firstPoint = _spriteShapeController.spline.GetPosition(firstTerrainPointIndex);
        Vector3 lastPoint = _spriteShapeController.spline.GetPosition(_currentLevelLength - 1);

        Vector3 bottomLeft = new Vector3(firstPoint.x, transform.position.y - _bottomDepth);
        Vector3 bottomRight = new Vector3(lastPoint.x, transform.position.y - _bottomDepth);

        _spriteShapeController.spline.InsertPointAt(_currentLevelLength, bottomRight);
        _spriteShapeController.spline.InsertPointAt(_currentLevelLength + 1, bottomLeft);
    }

    public void RemovePreviousPoints()
    {
        _pointsToDeletePerFrame = _currentLevelLength - 3;

        for (int i = 0; i < _pointsToDeletePerFrame; i++)
        {
            _spriteShapeController.spline.RemovePointAt(0);
        }

        _currentLevelLength -= _pointsToDeletePerFrame;

        _newPointPositionIndexValue += _pointsToDeletePerFrame;

        _spriteShapeController.spline.RemovePointAt(_currentLevelLength + 1);

        Vector3 bottomLeft = new Vector3(_spriteShapeController.spline.GetPosition(0).x, transform.position.y - _bottomDepth);

        _spriteShapeController.spline.InsertPointAt(_currentLevelLength + 1, bottomLeft);
    }

    [ContextMenu("Test")]
    private void GenerateObstacles()
    {
        if (_activeObstacles.Count >= 6)
        {
            RemovePreviousObstacles();
        }

        _randomSpawnIndex = GenerateUniqueRandomNumbers();

        for (int i = 0; i < _randomSpawnIndex.Count; i++)
        {
            bool isGroundObstacle = GetRandomZeroOrOne() == 1 ? true : false;

            GameObject rock = isGroundObstacle ? _objectPooler.GetGroundPooledObject() : _objectPooler.GetAirPooledObject();

            if(rock != null)
            {
                rock.transform.position = _spriteShapeController.spline.GetPosition(_randomSpawnIndex[i]);
            }

            rock.gameObject.SetActive(true);

            _activeObstacles.Add(rock);
        } 
    }

    private void GeneratePowerUps()
    {
        if (_activePowerUps.Count >= 1)
        {
            RemovePreviousPowerUps();
        }

        int initialPoints = 2;
        int bottomPoints = 2;
        int splineCount = _spriteShapeController.spline.GetPointCount() - bottomPoints;

        do
        {
            _randomSpawnIndexForPowerUps = UnityEngine.Random.Range(initialPoints, splineCount);
        } while (_randomSpawnIndex.Contains(_randomSpawnIndexForPowerUps));

        GameObject powerUp = _objectPooler.GetPowerUpPooledObject();

        if (powerUp != null)
        {
            powerUp.transform.position = _spriteShapeController.spline.GetPosition(_randomSpawnIndexForPowerUps);
        }

        powerUp.gameObject.SetActive(true);

        _activePowerUps.Add(powerUp);
    }

    public int GetRandomZeroOrOne()
    {
        return UnityEngine.Random.Range(0, 2);
    }

    private void RemovePreviousObstacles(bool removeAllObstacles = false)
    {
        int numberOfObstaclesToRemove = removeAllObstacles ? _activeObstacles.Count : 3;

        for (int i = 0; i < numberOfObstaclesToRemove; i++)
        {
            _activeObstacles[i].gameObject.SetActive(false);
        }

        if (removeAllObstacles)
        {
            _activeObstacles.Clear();
        }
        else
        {
            _activeObstacles.RemoveRange(0, numberOfObstaclesToRemove);
        }

        if (_randomSpawnIndex.Count > 0)
        {
            _randomSpawnIndex.Clear();
        }
    }

    private void RemovePreviousPowerUps(bool removeAllObstacles = false)
    {
        int numberOfPowerUpsToRemove = removeAllObstacles ? _activePowerUps.Count : 1;

        for (int i = 0; i < numberOfPowerUpsToRemove; i++)
        {
            _activePowerUps[i].gameObject.SetActive(false);
        }

        if (removeAllObstacles)
        {
            _activePowerUps.Clear();
        }
        else
        {
            _activePowerUps.RemoveRange(0, numberOfPowerUpsToRemove);
        }
    }

    private List<int> GenerateUniqueRandomNumbers()
    {
        HashSet<int> numbers = new HashSet<int>();

        int initialPoints = 2;
        int bottomPoints = 2;
        int splineCount = _spriteShapeController.spline.GetPointCount() - bottomPoints;

        while (numbers.Count < 3)
        {
            int randomNumber = UnityEngine.Random.Range(initialPoints, splineCount);
            numbers.Add(randomNumber);
        }

        return numbers.ToList();
    }

    private void OnDrawGizmos()
    {
        if (_spriteShapeController != null && _spriteShapeController.spline.GetPointCount() > 2)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(_spriteShapeController.spline.GetPosition(_currentLevelLength - 2), Vector3.one * 2);
        }
    }

    public void OnRetry(Action onGenerated)
    {
        //Reset all the values
        _lastGeneratedPosition = Vector3.zero;

        RemovePreviousObstacles(true);

        _newPointPositionIndexValue = 0;

        Initialize(onGenerated);
    }
}

#region

#endregion
