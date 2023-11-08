void GaussianBlur_float(UnityTexture2D Texture, float2 UV, float Blur, UnitySamplerState Sampler, out float4 Out)
{
    const float TwoPi = 6.28318530718;

    float Directions = 10.0;
    float Quality = 3.0;

    float4 Color = Texture.Sample(Sampler, UV);

    float stepSize = TwoPi / Directions;
    for (float d = 0.0; d < TwoPi; d += stepSize)
    {
        for (float i = 1.0 / Quality; i <= 1.001; i += 1.0 / Quality)
        {
            float2 offset = float2(_MainTex_TexelSize.x * cos(d), _MainTex_TexelSize.y * sin(d)) * Blur * i;
            Out += Texture.Sample(Sampler, UV + offset); 
        }
    }

    Out /= Quality * Directions + 1.0;

}