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

namespace SMEditor.Editor
{
    static class Renderer
    {
        public static Camera mainCamera;
        public static SlimDX.Windows.D3D11Control viewport;
        public static void Init()
        {
            viewport = Program.mainWindow.d3D11Control;
            mainCamera = new Camera();
            InitDrawCategories();
            InitDrawCategories();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Terrain specifics
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static RasterizerState terrainRS, gridRS;
        static ShaderSignature terrainSig, gridSig;
        static VertexShader terrainVS, gridVS;
        static PixelShader terrainPS, gridPS;
        static InputLayout terrainInpl, gridInpl;
        private static void InitDrawCategories()
        {
            #region Terrain
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
                new InputElement("POSITION_IN", 0, Format.R32G32B32_Float, 0,  0),
                new InputElement("COLOR_IN", 0, Format.R32G32B32_Float, 12, 0),
                new InputElement("UV_IN", 0, Format.R32G32B32_Float, 0),
            };
            terrainInpl = new InputLayout(viewport.Device, terrainSig, vertDesc);

            //Rasterizer state
            var terrainDesc = new RasterizerStateDescription
            {
                FillMode = FillMode.Wireframe,
                CullMode = CullMode.None,
                IsFrontCounterclockwise = false,
                IsDepthClipEnabled = true
            };
            terrainRS = RasterizerState.FromDescription(viewport.Device, terrainDesc);
            #endregion
            #region Grid
            using (var b = ShaderBytecode.CompileFromFile("Shaders\\gridShader.fx", "vs", "vs_4_0", ShaderFlags.None, EffectFlags.None))
            {
                gridSig = ShaderSignature.GetInputSignature(b);
                gridVS = new VertexShader(viewport.Device, b);
            }
            using (var b = ShaderBytecode.CompileFromFile("Shaders\\gridShader.fx", "ps", "ps_4_0", ShaderFlags.None, EffectFlags.None))
            {
                gridPS = new PixelShader(viewport.Device, b);
            }

            //InputLayout
            var gvertDesc = new[]
{
                new InputElement("POSITION_IN", 0, Format.R32G32B32_Float, 0,  0),
                new InputElement("COLOR_IN", 0, Format.R32G32B32_Float, 12, 0),
                new InputElement("UV_IN", 0, Format.R32G32B32_Float, 0),
            };
            gridInpl = new InputLayout(viewport.Device, gridSig, vertDesc);

            //Rasterizer state
            var gridDesc = new RasterizerStateDescription
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
                IsFrontCounterclockwise = false,
                IsDepthClipEnabled = true
            };
            gridRS = RasterizerState.FromDescription(viewport.Device, gridDesc);
            #endregion
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Draw functions -- called in interval set in MainWindow.cs.
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static List<MeshBase> terrainMeshes = new List<MeshBase>();
        public static List<MeshBase> gridMeshes = new List<MeshBase>();
        public static void Draw() { Draw(null, null); }
        public static void Draw(object o, System.Windows.Forms.PaintEventArgs e)
        {
            viewport.Clear(System.Drawing.Color.DarkGray);
            viewport.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            // Terrain
            viewport.Device.ImmediateContext.Rasterizer.State = terrainRS;
            viewport.Device.ImmediateContext.VertexShader.Set(terrainVS);
            viewport.Device.ImmediateContext.PixelShader.Set(terrainPS);
            viewport.Device.ImmediateContext.InputAssembler.InputLayout = terrainInpl;
            mainCamera.UpdateCameraBuffer();
            foreach (MeshBase m in terrainMeshes)
            {
                m.Draw();
            }

            viewport.Device.ImmediateContext.Rasterizer.State = gridRS;
            viewport.Device.ImmediateContext.VertexShader.Set(gridVS);
            viewport.Device.ImmediateContext.PixelShader.Set(gridPS);
            viewport.Device.ImmediateContext.InputAssembler.InputLayout = gridInpl;
            mainCamera.UpdateCameraBuffer();
            foreach (MeshBase m in gridMeshes)
            {
                m.Draw();
            }

            viewport.Present();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }



    [StructLayout(LayoutKind.Sequential)]
    public struct UniversalVertex
    {
        public UniversalVertex(Vector3 pos, Vector3 col, Vector2 uv) { position = pos; color = col; UV = uv; }
        Vector3 position;
        Vector3 color;
        Vector2 UV;
    }
    public enum MeshType { ColorGrid, ColorSolid, TexturedSolid }
    public class MeshBase
    {
        private int indexCount = 0;
        private Buffer vb;
        private Buffer ib;
        private VertexBufferBinding vbind;

        public void Init(List<UniversalVertex> vertices, List<uint> indices)
        {
            #region old
            /////////////////////////////////////
            //// Construct grid
            /////////////////////////////////////
            //float vertSpacing = gridSize / numXVerts;
            ////Generate vertices
            //for (int x = 0; x < numXVerts + 1; x++)
            //{
            //    for (int z = 0; z < numXVerts + 1; z++)
            //    {
            //        TerrainVertex v = new TerrainVertex(new Vector3(x * vertSpacing, 0f, z * vertSpacing));
            //        vertices.Add(v);
            //    }
            //}
            ////Generate indices
            //for (int j = 0; j < numXVerts; ++j)
            //{
            //    for (int i = 0; i < numXVerts; ++i)
            //    {
            //        int row1 = j * (numXVerts + 1);
            //        int row2 = (j + 1) * (numXVerts + 1);

            //        // triangle 1
            //        indices.Add((uint)(row1 + i));
            //        indices.Add((uint)(row1 + i + 1));
            //        indices.Add((uint)(row2 + i + 1));
            //        // triangle 2
            //        indices.Add((uint)(row1 + i));
            //        indices.Add((uint)(row2 + i + 1));
            //        indices.Add((uint)(row2 + i));
            //    }
            //}
            #endregion

            DataStream vstream = new DataStream(vertices.ToArray(), true, false); vstream.Position = 0;
            DataStream istream = new DataStream(indices.ToArray(), true, false); istream.Position = 0;

            var vbd = new BufferDescription(
            Marshal.SizeOf(new UniversalVertex()) * vertices.Count,
            ResourceUsage.Dynamic,
            BindFlags.VertexBuffer,
            CpuAccessFlags.Write,
            ResourceOptionFlags.None, 0);
            vb = new Buffer(Renderer.viewport.Device, vstream, vbd);
            vbind = new VertexBufferBinding(vb, Marshal.SizeOf(new UniversalVertex()), 0);

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
        public void UpdateVertex(int x, int y, Vector3 v)
        {


        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TerrainVertex
    {
        public TerrainVertex(Vector3 pos)
        {
            position = pos;
        }
        public Vector3 position;
    }
    public class TerrainMesh : MeshBase
    {
        public TerrainMesh(List<TerrainVertex> vertices, List<uint> indices)
        {
            List<UniversalVertex> univVs = new List<UniversalVertex>();
            foreach(TerrainVertex v in vertices)
            {
                UniversalVertex univV = new UniversalVertex(
                    v.position,
                    new Vector3(0, 0, 0),
                    new Vector2(0, 0));
                univVs.Add(univV);
                Init(univVs, indices);
                Renderer.terrainMeshes.Add(this);
            }
        }
    }
}
