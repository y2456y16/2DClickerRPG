using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBrightnessController : MonoBehaviour
{
    private const float dayDuration = 60f; // 하루의 길이 (초 단위)
    private Color dayColor = Color.white; // 낮 색상
    private Color nightColor = Color.gray; // 밤 색상

    private SpriteRenderer spriteRenderer;
    private float time;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        // 시간 계산
        time += Time.deltaTime;
        float t = Mathf.PingPong(time / dayDuration, 1);

        // 색상 보간
        Color currentColor = Color.Lerp(nightColor, dayColor, t);
        spriteRenderer.color = currentColor;
    }
}