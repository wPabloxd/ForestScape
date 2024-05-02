using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Tracer : MonoBehaviour
{
    [SerializeField] float lifeTime = 0.3f;
    LineRenderer lineRenderer;
    Tween scallingTween;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    private void Start()
    {
        scallingTween = DOTween.To(() => lineRenderer.widthMultiplier, (x) => lineRenderer.widthMultiplier = x, 0, lifeTime);
        Destroy(gameObject, lifeTime);
    }
    public void Init(Vector3 startPosition, Vector3 endPosition)
    {
        Vector3[] positions = new Vector3[2] { startPosition, endPosition };
        lineRenderer.SetPositions(positions);
    }
    private void OnDestroy()
    {
        scallingTween.Kill();
    }
}