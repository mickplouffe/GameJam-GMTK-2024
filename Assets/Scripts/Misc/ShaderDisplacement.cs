using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderDisplacement : MonoBehaviour
{
    public Texture2D noiseTexture;
    Material material;
    public float noiseScale = 1.0f;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // Sample the noise texture based on object position or other criteria
        Vector2 uv = new Vector2(transform.position.x, transform.position.z) * noiseScale;
        float height = SampleNoiseTexture(noiseTexture, uv);

        // Pass the sampled height as a uniform to the shader
        material.SetFloat("_HeightScale", height);
    }

    float SampleNoiseTexture(Texture2D texture, Vector2 uv)
    {
        int x = Mathf.Clamp((int)(uv.x * texture.width), 0, texture.width - 1);
        int y = Mathf.Clamp((int)(uv.y * texture.height), 0, texture.height - 1);

        Color pixelColor = texture.GetPixel(x, y);
        return pixelColor.grayscale;  // Assuming you want grayscale height
    }
}
