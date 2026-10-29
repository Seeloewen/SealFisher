using SealFisher.Rendering.Graphics.Abstraction.Shader;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using System.Collections.Generic;
using System.Windows;

namespace SealFisher.Rendering.Graphics.Abstraction.Buffer
{
    public class InputLayout
    {
        private List<InputElementDesc> inputLayoutInfos;
        public ComPtr<ID3D11InputLayout> instance;
        private uint size;

        public InputLayout()
        {
            inputLayoutInfos = new List<InputElementDesc>();
        }

        public unsafe void AddInfoElement(string semanticName, Format format, uint size)
        {
            //Create input element description from input
            InputElementDesc element = new InputElementDesc()
            {
                SemanticName = (byte*)SilkMarshal.StringToMemory(semanticName),
                SemanticIndex = 0,
                Format = format,
                AlignedByteOffset = this.size,
                InputSlotClass = InputClassification.PerVertexData,
                InstanceDataStepRate = 0
            };

            inputLayoutInfos.Add(element);
            this.size += size;
        }

        public unsafe int Create(IShader shader)
        {
            InputElementDesc[] layoutArray = inputLayoutInfos.ToArray();
            int i = Renderer.device.CreateInputLayout(in layoutArray[0], (uint)layoutArray.Length, shader.shaderBlob.GetBufferPointer(), shader.shaderBlob.GetBufferSize(), ref instance);

            if (i != 0)
            {
                MessageBox.Show("Failed to create Input Layout for Shader");
                return i;
            }

            return 0;
        }

        public uint GetSize()
        {
            return size;
        }
    }
}
