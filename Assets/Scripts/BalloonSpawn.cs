using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BalloonSpawn : MonoBehaviour
{
    [SerializeField] private Balloon _balloonPrefab;
    [SerializeField] private float _spawnYOffset = 1f;
    [SerializeField] private float _balloonSpawnInterval = 3;
    [SerializeField] private int _maxBalloons = 8;

    private Rigidbody _rb;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        StartCoroutine(BalloonSpawnRoutine());
    }

    private void Spawn()
    {
        var offset = _spawnYOffset * Vector3.up;
        var balloon = Instantiate(_balloonPrefab,
            transform.position + offset, Quaternion.identity);
        balloon.SetConnectedBody(_rb);
        balloon.Active = true;
    }

    private IEnumerator BalloonSpawnRoutine()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(_balloonSpawnInterval);
        }
    }
}
