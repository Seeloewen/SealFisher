struct Input
{
    float4 position : SV_Position0;
    float4 color : COLOR0;
};

struct Output
{
    float4 color : SV_Target0;
};

Output Main(Input input)
{
    Output output = (Output) 0;
    output.color = input.color; //transforms to RGBA from rgba with a always being 1.0
    return output;
}