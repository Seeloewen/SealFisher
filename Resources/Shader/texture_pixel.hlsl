Texture2D tex0 : register(t0);
SamplerState samp0 : register(s0);

struct Input
{
    float4 pos : SV_Position;
    float2 uv : TEXCOORD0;
};

float4 Main(Input input) : SV_Target
{
    return tex0.Sample(samp0, input.uv);
}
    
