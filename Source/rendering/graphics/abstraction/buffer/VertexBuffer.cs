using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SealFisher.Rendering.Graphics.Abstraction.Buffer
{
    public class VertexBuffer
    {
        public ComPtr<ID3D11Buffer> buffer = default;
        private InputLayout inputlayout;
        private Action onFull; //Draws the buffer when it's full, provided by the renderer that owns it
        private List<float> vertices = new();
        private uint vertexAmount;
        private uint maxVertices;
        const uint bytes = 64 * 1024;

        public VertexBuffer(InputLayout layout, Action onFull)
        {
            inputlayout = layout;
            this.onFull = onFull;

            //Round down to whole triangles, so a flush never splits a triangle
            maxVertices = bytes / layout.GetSize();
            maxVertices -= maxVertices % 3;

            BufferDesc bufferDesc = new BufferDesc()
            {
                ByteWidth = bytes,
                Usage = Usage.Dynamic,
                BindFlags = (uint)BindFlag.VertexBuffer,
                CPUAccessFlags = (uint)CpuAccessFlag.Write
            };

            Renderer.device.CreateBuffer(ref bufferDesc, ref SilkMarshal.NullRef<SubresourceData>(), ref buffer);
        }

        private void EnsureSpace()
        {
            //Draw everything collected so far if the buffer is full
            if (vertexAmount >= maxVertices) onFull();
        }

        public void Put(float x, float y, float r, float g, float b, float a)
        {
            EnsureSpace();

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
            EnsureSpace();

            vertices.Add(x);
            vertices.Add(y);
            vertices.Add(0.0f);
            vertices.Add(u);
            vertices.Add(v);

            vertexAmount++;
        }

        public unsafe void Use()
        {
            if (vertices.Count > 0)
            {
                MappedSubresource mappedSubresource = default;
                Renderer.deviceContext.Map(buffer, 0, Map.WriteDiscard, 0, ref mappedSubresource);
                Marshal.Copy(vertices.ToArray(), 0, (nint)mappedSubresource.PData, vertices.Count);
                Renderer.deviceContext.Unmap(buffer, 0);
            }

            uint stride = inputlayout.GetSize();
            uint offset = 0;
            Renderer.deviceContext.IASetInputLayout(inputlayout.instance);
            Renderer.deviceContext.IASetVertexBuffers(0, 1, ref buffer, ref stride, ref offset);
        }

        public void Flush()
        {
            //Draw the vertices currently in the buffer and clear the buffer
            if (vertexAmount > 0) Renderer.deviceContext.Draw(vertexAmount, 0);
            vertices.Clear();
            vertexAmount = 0;
        }

        public uint GetVertexCount() => vertexAmount;
    }
}