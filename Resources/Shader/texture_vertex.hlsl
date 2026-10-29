
struct Input
{
    float3 pos : POSITION;
    float2 uv : TEXCOORD0;
};

struct Output
{
    float4 pos : SV_Position;
    float2 uv : TEXCOORD0;
};

Output Main(Input input)
{
    Output o;
    o.pos = float4(input.pos, 1);
    o.uv = input.uv;
    return o;
}