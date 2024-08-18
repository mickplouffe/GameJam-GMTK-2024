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

    private bool _fleshStarted;

    private void OnEnable()
    {
        enemyEventChennl.OnTileFlashing += HandleTileFlashing;
    }

    private void OnDisable()
    {
        enemyEventChennl.OnTileFlashing -= HandleTileFlashing;
    }

    private void Start()
    {
        _tileRenderer = GetComponent<Renderer>();
        _originalTileColor = _tileRenderer.material.color;
    }

    private void Update()
    {
        if (!_fleshStarted)
            return;

        _tileRenderer.material.color = Color.Lerp(_originalTileColor, flashColor, Mathf.Sin(Time.time * flashSpeed));
    }

    private void HandleTileFlashing(HexTileController tile, float flashDuration)
    {
        if (!Equals(tile))
            return;

        StartCoroutine(FlashTile(flashDuration));
    }

    private IEnumerator FlashTile(float flashDuration)
    {
        _fleshStarted = true;

        yield return new WaitForSeconds(flashDuration);

        _fleshStarted = false;
        _tileRenderer.material.color = _originalTileColor;

    }
}
