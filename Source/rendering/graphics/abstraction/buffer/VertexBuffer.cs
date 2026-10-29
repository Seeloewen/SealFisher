using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SealFisher.Rendering.Graphics.Abstraction.Buffer
{
    public class VertexBuffer
    {
        public ComPtr<ID3D11Buffer> buffer = default;
        private InputLayout inputlayout;
        private List<float> vertices;
        private uint vertexAmount;
        const uint bytes = 1024;

        public VertexBuffer(InputLayout layout)
        {
            inputlayout = layout;
            vertices = new();

            BufferDesc bufferDesc = new BufferDesc()
            {
                ByteWidth = bytes,
                Usage = Usage.Dynamic,
                BindFlags = (uint)BindFlag.VertexBuffer,
                CPUAccessFlags = (uint)CpuAccessFlag.Write
            };

            Renderer.device.CreateBuffer(ref bufferDesc, ref SilkMarshal.NullRef<SubresourceData>(), ref buffer);
        }

        public void Put(float x, float y, float r, float g, float b, float a)
        {
            //Check if the current amount of vertices in the buffer plus one more vertice would exceed the buffer length
            if (vertices.Count * sizeof(float) + 7 * sizeof(float) > bytes)
            {
                vertexAmount = 0;
                Flush(); //This might not be right, but I dont care enough to check it (famous last words)
            }

            vertices.Add(x);
            vertices.Add(y);
            vertices.Add(0.0f);
            vertices.Add(r);
            vertices.Add(g);
            vertices.Add(b);
            vertices.Add(a);

            vertexAmount++;
        }

        public void Put(float x, float y, float u, float v)
        {
            //Check if the current amount of vertices in the buffer plus one more vertice would exceed the buffer length
            if (vertices.Count * sizeof(float) + 5 * sizeof(float) > bytes)
            {
                vertexAmount = 0;
                Flush(); //This might not be right, but I dont care enough to check it (famous last words)
            }

            vertices.Add(x);
            vertices.Add(y);
            vertices.Add(0.0f);
            vertices.Add(u);
            vertices.Add(v);

            vertexAmount++;
        }

        public unsafe void Use()
        {
            MappedSubresource mappedSubresource = default;
            Renderer.deviceContext.Map(buffer, 0, Map.WriteDiscard, 0, ref mappedSubresource);
            Marshal.Copy(vertices.ToArray(), 0, (nint)mappedSubresource.PData, vertices.Count);
            Renderer.deviceContext.Unmap(buffer, 0);

            uint stride = inputlayout.GetSize();
            uint offset = 0;
            Renderer.deviceContext.IASetInputLayout(inputlayout.instance);
            Renderer.deviceContext.IASetVertexBuffers(0, 1, ref buffer, ref stride, ref offset);

        }

        public void Flush()
        {
            //Draw the vertices currently in the buffer and clear the buffer
            uint i = GetVertexCount();
            Renderer.deviceContext.Draw(i, 0);
            vertices.Clear();
            vertexAmount = 0;
        }

        public uint GetVertexCount()
        {
            return vertexAmount;
        }
    }
}
