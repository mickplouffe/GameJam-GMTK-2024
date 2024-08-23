using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileController : MonoBehaviour
{
    public Vector2Int GridPosition { get; set; }
    public bool IsSpawnerTile { get; set; }
    [SerializeField] private EnemyEventChannel enemyEventChannel;
    [SerializeField] private GameManagerEventChannel gameManagerEventChannel;

    [SerializeField] private Color flashColor;
    [SerializeField] private float flashSpeed = 1.0f;
    [SerializeField] private Animator tileAnimator;

    private Color _originalTileColor;

    private Renderer _tileRenderer;

    private bool _isFlashing;
    private float _flashTimeElapsed;
    private float _flashDuration;
    private static readonly int Spawner = Animator.StringToHash("Spawner");

    private void OnEnable()
    {
        enemyEventChannel.OnWaveCompleted += HandleWaveCompleted;
        gameManagerEventChannel.OnGameRestart += HandleWaveCompleted;
    }

    private void OnDisable()
    {
        enemyEventChannel.OnWaveCompleted -= HandleWaveCompleted;
        gameManagerEventChannel.OnGameRestart -= HandleWaveCompleted;
    }

    private void Start()
    {
        _tileRenderer = GetComponentInChildren<Renderer>();
        _originalTileColor = _tileRenderer.materials[0].color;

        if (!tileAnimator)
            tileAnimator = GetComponentInChildren<Animator>();
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

    public void HandleTileFlashing(HexTile tile, float flashDuration)
    {
        if (tile.Q != GridPosition.x || tile.R != GridPosition.y)
            return;

        _isFlashing = true;
        _flashTimeElapsed = 0.0f;
        _flashDuration = flashDuration;
        
        tileAnimator.SetBool(Spawner, true);
        IsSpawnerTile = true;
        Debug.Log($"{tile.TileObject.name} Became a spawner tile");
    }
    
    private void HandleWaveCompleted()
    {
        if(tileAnimator != null)
            tileAnimator.SetBool(Spawner, false);
    }
    
}
