using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Layer Setting")]
    [SerializeField] private float[] _layerSpeeds = new float[7];
    [SerializeField] private GameObject[] _layers = new GameObject[7];

    private float[] startPos = new float[7];
    private float _backgroundBoundX;
    private float _backgroundSizeX;

    void Start()
    {
        //Layer 0 is background
        _backgroundSizeX = _layers[0].transform.localScale.x;
        _backgroundBoundX = _layers[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        for (int i = 0; i < 5; i++)
        {
            startPos[i] = this.transform.position.x;
        }
    }

    void Update()
    {
        for (int i = 0; i < 5; i++)
        {
            float temp = (this.transform.position.x * (1 - _layerSpeeds[i]));

            float distance = this.transform.position.x * _layerSpeeds[i];

            _layers[i].transform.position = new Vector2(startPos[i] + distance, this.transform.position.y);

            if (temp > startPos[i] + _backgroundBoundX * _backgroundSizeX)
            {
                startPos[i] += _backgroundBoundX * _backgroundSizeX;
            }
            else if (temp < startPos[i] - _backgroundBoundX * _backgroundSizeX)
            {
                startPos[i] -= _backgroundBoundX * _backgroundSizeX;
            }
        }
    }
}
