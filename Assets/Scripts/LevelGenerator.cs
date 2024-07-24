using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private SpriteShapeController spriteShapeController;
    [SerializeField] private Transform playerReference;

    [Header("Terrain Generation Settings")]
    [SerializeField, Range(3, 100)] private int initialLevelLength = 50;
    [SerializeField, Range(1f, 50f)] private float xMultiplier = 2f;
    [SerializeField, Range(1f, 50f)] private float yMultiplier = 2f;
    [SerializeField, Range(0f, 1f)] private float curveSmoothness = 0.5f;
    [SerializeField] private float noiseStep = 0.5f;
    [SerializeField] private float bottomDepth = 10f;

    [Header("Dynamic Generation Settings")]
    [SerializeField] private float playerDistanceThreshold = 10f;
    [SerializeField] private int newSegmentsPerGeneration = 10;

    [SerializeField] private int currentLevelLength;
    private Vector3 lastGeneratedPosition;
    private bool isGeneratingNewSegment;

    private void Start()
    {
        currentLevelLength = initialLevelLength;
        GenerateInitialTerrain();
    }

    private void Update()
    {
        CheckAndGenerateNewSegment();
    }

    private void GenerateInitialTerrain()
    {
        spriteShapeController.spline.Clear();

        for (int i = 0; i < currentLevelLength; i++)
        {
            AddTerrainPoint(i);
        }

        AddBottomPoints();
    }

    private void AddTerrainPoint(int index)
    {
        Vector3 position = CalculatePointPosition(index);
        spriteShapeController.spline.InsertPointAt(index, position);

        if (index > 0)
        {
            SetPointTangents(index);
        }

        lastGeneratedPosition = position;
    }

    private Vector3 CalculatePointPosition(int index)
    {
        if (index < 2)
        {
            return transform.position + Vector3.right * index * xMultiplier;
        }
        return transform.position + new Vector3(index * xMultiplier, Mathf.PerlinNoise(0, index * noiseStep) * yMultiplier);
    }

    private void SetPointTangents(int index)
    {
        spriteShapeController.spline.SetTangentMode(index, ShapeTangentMode.Continuous);
        float tangent = xMultiplier * curveSmoothness;
        spriteShapeController.spline.SetLeftTangent(index, Vector3.left * tangent);
        spriteShapeController.spline.SetRightTangent(index, Vector3.right * tangent);
    }

    private void AddBottomPoints()
    {
        Vector3 bottomRight = new Vector3(lastGeneratedPosition.x, transform.position.y - bottomDepth);
        Vector3 bottomLeft = new Vector3(transform.position.x, transform.position.y - bottomDepth);

        spriteShapeController.spline.InsertPointAt(currentLevelLength, bottomRight);
        spriteShapeController.spline.InsertPointAt(currentLevelLength + 1, bottomLeft);
    }

    private void CheckAndGenerateNewSegment()
    {
        if (isGeneratingNewSegment)
        {
            return;
        }

        float playerDistance = Vector3.Distance(playerReference.position, spriteShapeController.spline.GetPosition(currentLevelLength - 2));

        if (playerDistance < playerDistanceThreshold)
        {
            StartCoroutine(GenerateNewSegment());
        }
    }

    private IEnumerator GenerateNewSegment()
    {
        isGeneratingNewSegment = true;

        int newSegmentEnd = currentLevelLength + newSegmentsPerGeneration;

        for (int i = currentLevelLength; i < newSegmentEnd; i++)
        {
            AddTerrainPoint(i);
            yield return null; // Spread the generation over multiple frames
        }

        currentLevelLength = newSegmentEnd;
        UpdateBottomPoints();

        isGeneratingNewSegment = false;
    }

    private void UpdateBottomPoints()
    {
        spriteShapeController.spline.RemovePointAt(currentLevelLength);
        spriteShapeController.spline.RemovePointAt(currentLevelLength);
        AddBottomPoints();
    }

    [ContextMenu("Remove Bhai")]
    private void RemoveAPoint()
    {
        spriteShapeController.spline.RemovePointAt(0);

        currentLevelLength -= 1;

        Vector3 bottomLeft = new Vector3(transform.position.x, transform.position.y - bottomDepth);

        spriteShapeController.spline.InsertPointAt(currentLevelLength + 1, bottomLeft);
    }

    private void OnDrawGizmos()
    {
        if (spriteShapeController != null && spriteShapeController.spline.GetPointCount() > 2)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(spriteShapeController.spline.GetPosition(currentLevelLength - 2), Vector3.one * 2);
        }
    }
}

#region

#endregion
