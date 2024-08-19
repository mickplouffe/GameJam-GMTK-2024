using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileController : MonoBehaviour
{
    public Vector2Int GridPosition { get; set; }

    [SerializeField] private EnemyEventChennl enemyEventChennl;
    [SerializeField] private Color flashColor;
    [SerializeField] private float flashSpeed;

    private Color _originalTileColor;

    private Renderer _tileRenderer;

    private bool _isFlashing;
    private float _flashTimeElapsed;
    private float _flashDuration;
    private void OnEnable()
    {
        enemyEventChennl.OnWaveStart += HandleTileFlashing;
    }

    private void OnDisable()
    {
        enemyEventChennl.OnWaveStart -= HandleTileFlashing;
    }

    private void Start()
    {
        _tileRenderer = GetComponentInChildren<Renderer>();
        _originalTileColor = _tileRenderer.materials[0].color;
    }

    private void Update()
    {
        if (!_isFlashing)
            return;

        _flashTimeElapsed += Time.deltaTime;

        // Calculate the flashing effect
        float t = Mathf.Sin(Time.time * flashSpeed);
        _tileRenderer.materials[0].color = Color.Lerp(_originalTileColor, flashColor, t);

        // Stop flashing after the duration
        if (_flashTimeElapsed < _flashDuration) 
            return;
        
        _isFlashing = false;
        _tileRenderer.materials[0].color = _originalTileColor;
    }

    private void HandleTileFlashing(HexTileController tile, float flashDuration)
    {
        if (!Equals(tile))
            return;

        _isFlashing = true;
        _flashTimeElapsed = 0.0f;
        _flashDuration = flashDuration;
    }
    
}
