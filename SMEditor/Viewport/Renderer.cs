using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Buffer = SlimDX.Direct3D11.Buffer;
using Device = SlimDX.Direct3D11.Device;

namespace SMEditor
{
    static class Renderer
    {
        public static Camera mainCamera;
        public static SlimDX.Windows.D3D11Control viewport;
        public static void Init()
        {
            viewport = Program.mainWindow.d3D11Control;
            mainCamera = new Camera();
            InitTerrain();
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Terrain specifics.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static ShaderSignature terrainSig;
        static VertexShader terrainVS;
        static PixelShader terrainPS;
        static InputLayout terrainInpl;
        private static void InitTerrain()
        {
            //Shaders
            using (var b = ShaderBytecode.CompileFromFile("Shaders\\terrainShader.fx", "vs", "vs_4_0", ShaderFlags.None, EffectFlags.None))
            {
                terrainSig = ShaderSignature.GetInputSignature(b);
                terrainVS = new VertexShader(viewport.Device, b);
            }
            using (var b = ShaderBytecode.CompileFromFile("Shaders\\terrainShader.fx", "ps", "ps_4_0", ShaderFlags.None, EffectFlags.None))
            {
                terrainPS = new PixelShader(viewport.Device, b);
            }

            //InputLayout
            var vertDesc = new[]
{
                new InputElement("POSITION_IN", 0, Format.R32G32B32_Float, 0)
            };
            terrainInpl = new InputLayout(viewport.Device, terrainSig, vertDesc);
        }
        
        private static Dictionary<string, TerrainMesh> terrainMeshes = new Dictionary<string, TerrainMesh>();
        public static void CreateTerrainMesh(string name, List<TerrainVertex> vertices, List<uint> indices)
        {
            TerrainMesh m = new TerrainMesh();
            m.Init(vertices, indices);
            terrainMeshes.Add(name, m);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Draw functions -- called every frame by WinForms control.
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void Draw() { Draw(null, null); }
        public static void Draw(object o, System.Windows.Forms.PaintEventArgs e)
        {
            viewport.Clear(System.Drawing.Color.CornflowerBlue);
            viewport.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            viewport.Device.ImmediateContext.VertexShader.Set(terrainVS);
            viewport.Device.ImmediateContext.PixelShader.Set(terrainPS);
            viewport.Device.ImmediateContext.InputAssembler.InputLayout = terrainInpl;
            
            foreach (TerrainMesh m in terrainMeshes.Values)
            {
                m.Draw();
            }

            viewport.Present();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TerrainVertex
    {
        public TerrainVertex(Vector3 v) { position = v; }
        Vector3 position;
    }
    class TerrainMesh
    {
        private int indexCount = 0;

        private Buffer vb;
        private Buffer ib;
        private VertexBufferBinding vbind;
        public void Init(List<TerrainVertex> vertices, List<uint> indices)
        {
            DataStream vstream = new DataStream(vertices.ToArray(), true, false); vstream.Position = 0;
            DataStream istream = new DataStream(indices.ToArray(), true, false); istream.Position = 0;

            Console.WriteLine(vertices.Count);

            var vbd = new BufferDescription(
            sizeof(float) * 3 * vertices.Count,
            ResourceUsage.Dynamic,
            BindFlags.VertexBuffer,
            CpuAccessFlags.Write,
            ResourceOptionFlags.None, 0);
            vb = new Buffer(Renderer.viewport.Device, vstream, vbd);
            vbind = new VertexBufferBinding(vb, 12, 0);

            //indices
            var ibd = new BufferDescription(
            sizeof(uint) * indices.Count,
            ResourceUsage.Dynamic,
            BindFlags.IndexBuffer,
            CpuAccessFlags.Write,
            ResourceOptionFlags.None, 0);
            ib = new Buffer(Renderer.viewport.Device, istream, ibd);

            indexCount = indices.Count;
        }
        public void Draw()
        {
            Renderer.viewport.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, vbind);
            Renderer.viewport.Device.ImmediateContext.InputAssembler.SetIndexBuffer(ib, Format.R32_UInt, 0);
            Renderer.viewport.Device.ImmediateContext.DrawIndexed(indexCount, 0, 0);
        }
        public void UpdateData(List<TerrainVertex> vertices, List<uint> indices)
        {


        }
    }
}
