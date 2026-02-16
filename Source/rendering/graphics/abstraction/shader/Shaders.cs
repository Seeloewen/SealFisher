using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealFisher.rendering.graphics.abstraction.shader
{
    public static class Shaders
    {
        //TO BE REWORKED WITH ACTUAL SHADER FILES INSTEAD OF THIS
        public static string pixelShader = @"struct Input
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
}";
        public static string vertexShader = @"struct Input
{
    float3 position : POSITION;
    float4 color : COLOR0;
};

struct Output
{
    float4 position : SV_Position;
    float4 color : COLOR0;
};

Output Main(Input input)
{
    Output output;
    output.position = float4(input.position, 1.0);
    output.color = input.color;
    return output;
}";
    }
}
