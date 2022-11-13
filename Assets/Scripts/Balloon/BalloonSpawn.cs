using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class BalloonSpawn : MonoBehaviour
{
    [SerializeField] private Balloon _balloonPrefab;
    [SerializeField] private float _spawnYOffset = 1f;
    [SerializeField] private float _balloonSpawnInterval = 3;
    [SerializeField] private int _maxBalloons = 12;

    private List<Balloon> _balloons = new List<Balloon>();

    private Rigidbody _rb;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        StartCoroutine(BalloonSpawnRoutine());
    }

    private void Spawn()
    {
        var randomPoint = Random.insideUnitCircle;
        var offset = _spawnYOffset * Vector3.up
                     + 0.05f * new Vector3(randomPoint.x, 0f, randomPoint.y);
        
        var balloon = Instantiate(_balloonPrefab,
            transform.position + offset, Quaternion.identity);
        _balloons.Add(balloon);
        balloon.SetConnectedBody(_rb);
        balloon.Active = true;
    }

    private IEnumerator BalloonSpawnRoutine()
    {
        while (true)
        {
            if (_balloons.Count >= _maxBalloons)
                yield return null;
            else
            {
                Spawn();
                yield return new WaitForSeconds(_balloonSpawnInterval);
            }
        }
    }
}
