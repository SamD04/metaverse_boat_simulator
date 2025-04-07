using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveScript : MonoBehaviour
{
    public static waveScript instance;

    public float amplitude = 0.5f;
    public float length = 2f;
    public float speed = 0.2f;

    private float offset = 0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        offset += Time.deltaTime * speed;
    }

    public float GetWaveHeight(float _x)
    {
        return amplitude * Mathf.Sin(_x / length + offset);
    }
}
