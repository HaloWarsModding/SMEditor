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
using SMEditor.Editor;

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

            passes.Add("terrain", new RenderPass("terrain", new[]
            {
                new InputElement("POSITION_IN", 0, Format.R32G32B32_Float, 0),
                new InputElement("COLOR_IN", 0, Format.R32G32B32_Float, 12, 0)
            }));

            //InitTerrain();
            //InitGrid();
            //InitboldVert();
        }

        public static Dictionary<string, RenderPass> passes = new Dictionary<string, RenderPass>();
        
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Draw functions -- called in interval set in MainWindow.cs.
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void Draw() { Draw(null, null); }
        public static void Draw(object o, System.Windows.Forms.PaintEventArgs e)
        {
            viewport.Clear(System.Drawing.Color.DarkGray);
            viewport.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            mainCamera.UpdateCameraBuffer();

            passes["terrain"].Use();
            foreach (TerrainMesh m in terrainMeshes) m.Draw();

            viewport.Present();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        
    }



    [StructLayout(LayoutKind.Sequential)]
    public struct BasicVertex
    {
        public BasicVertex(Vector3 v, Vector3 col) { position = v; color = col; }
        public Vector3 position;
        public Vector3 color;
    }
    class BasicMesh
    {
        public int indexCount = 0;
        public Buffer vb;
        public Buffer ib;
        public VertexBufferBinding vbind;
        public Vector3 location = new Vector3(0,0,0);
        public Vector3 scale = new Vector3(1,1,1);

        BufferDescription ibd;
        BufferDescription vbd;

        public BasicMesh()
        {
        }
        public virtual void Init(List<BasicVertex> vertices, List<int> indices)
        {
            DataStream vd = new DataStream(vertices.ToArray(), true, true); vd.Position = 0;
            DataStream id = new DataStream(indices.ToArray(), true, true); id.Position = 0;

            vbd = new BufferDescription(
            Marshal.SizeOf(new BasicVertex()) * vertices.Count,
            ResourceUsage.Dynamic,
            BindFlags.VertexBuffer,
            CpuAccessFlags.Write,
            ResourceOptionFlags.None, 0);
            vb = new Buffer(Renderer.viewport.Device, vbd);
            vbind = new VertexBufferBinding(vb, Marshal.SizeOf(new BasicVertex()), 0);

            //indices
            ibd = new BufferDescription(
            sizeof(uint) * indices.Count,
            ResourceUsage.Dynamic,
            BindFlags.IndexBuffer,
            CpuAccessFlags.Write,
            ResourceOptionFlags.None, 0);
            ib = new Buffer(Renderer.viewport.Device, id, ibd);

            indexCount = indices.Count;

            UpdateData(vertices, indices);
        }
        public virtual void Draw()
        {
            Renderer.viewport.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, vbind);
            Renderer.viewport.Device.ImmediateContext.InputAssembler.SetIndexBuffer(ib, Format.R32_SInt, 0);
            Renderer.viewport.Device.ImmediateContext.DrawIndexed(indexCount, 0, 0);
        }
        public virtual void UpdateData(List<BasicVertex> vertices, List<int> indices)
        {
            DataStream vd = new DataStream(vertices.ToArray(), true, true); vd.Position = 0;
            //DataStream id = new DataStream(indices.ToArray(), true, true); id.Position = 0;
            
            //ib = new Buffer(Renderer.viewport.Device, id, ibd);
            vb = new Buffer(Renderer.viewport.Device, vd, vbd);
            vbind = new VertexBufferBinding(vb, Marshal.SizeOf(new BasicVertex()), 0);

            //Renderer.viewport.Context.MapSubresource(vb, MapMode.WriteDiscard, SlimDX.Direct3D11.MapFlags.None);
            //Renderer.viewport.Context.UpdateSubresource(new DataBox((int)vd.Length, (int)vd.Length, vd), vb, 0);
            //Renderer.viewport.Context.UnmapSubresource(vb, 0);

            //Renderer.viewport.Context.MapSubresource(ib, MapMode.WriteDiscard, SlimDX.Direct3D11.MapFlags.None);
            //Renderer.viewport.Context.UpdateSubresource(new DataBox(0, 0, id), ib, 0);
            //Renderer.viewport.Context.UnmapSubresource(ib, 0);
        }
    }

    class TerrainMesh : BasicMesh
    {
        public static bool drawPoints = false;

        public TerrainMesh() { }
        public override void Init(List<BasicVertex> vertices, List<int> indices)
        {
            base.Init(vertices, indices);
        }
        public override void Draw()
        {
            Renderer.mainCamera.SetModelMatrix(Matrix.Transformation(new Vector3(0, 0, 0),
                Quaternion.Identity, new Vector3(1, 1, 1), new Vector3(0, 0, 0), Quaternion.Identity, new Vector3(0, 0, 0)));
            base.Draw();
        }
        public override void UpdateData(List<BasicVertex> vertices, List<int> indices)
        {
            base.UpdateData(vertices, indices);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GridVertex
    {
        public GridVertex(Vector3 pos, Vector3 col) { position = pos; color = col; }
        Vector3 position;
        Vector3 color;
    }
    class GridMesh
    {
        public Vector3 gridColor;
        private int indexCount = 0;
        private List<GridVertex> vertices = new List<GridVertex>();
        private List<uint> indices = new List<uint>();

        private Buffer vb;
        private Buffer ib;
        private VertexBufferBinding vbind;

        public GridMesh(string name, float gridSize, int numXVerts, float[][] heights, Vector3 color)
        {
            Init(name, gridSize, numXVerts, heights, color);
        }
        public GridMesh(string name, float gridSize, int numXVerts, float[][] heights, System.Drawing.Color color)
        {
            Init(name, gridSize, numXVerts, heights, new Vector3(color.R / 255F, color.G / 255F, color.B / 255F));
        }
        private void Init(string name, float gridSize, int numXVerts, float[][] heights, Vector3 color)
        {
            gridColor = color;

            ///////////////////////////////////
            // Construct grid
            ///////////////////////////////////
            float vertSpacing = gridSize / numXVerts;
            //Generate vertices
            for(int x = 0; x < numXVerts + 1; x++)
            {
                for(int z = 0; z < numXVerts + 1; z++)
                {
                    GridVertex v = new GridVertex(new Vector3(x * vertSpacing, 0f, z * vertSpacing), gridColor);
                    vertices.Add(v);
                    Console.WriteLine(x * vertSpacing + " | " + z * vertSpacing);
                }
            }
            //Generate indices
            for (int j = 0; j < numXVerts; ++j)
            {
                for (int i = 0; i < numXVerts; ++i)
                {
                    int row1 = j * (numXVerts + 1);
                    int row2 = (j + 1) * (numXVerts + 1);

                    // triangle 1
                    indices.Add((uint)(row1 + i));
                    indices.Add((uint)(row1 + i + 1));
                    indices.Add((uint)(row2 + i + 1));
                    // triangle 2
                    indices.Add((uint)(row1 + i));
                    indices.Add((uint)(row2 + i+1));
                    indices.Add((uint)(row2 + i));
                }
            }

            ///////////////////////////////////
            // Prep D3D stuff
            ///////////////////////////////////
            DataStream vstream = new DataStream(vertices.ToArray(), true, false); vstream.Position = 0;
            DataStream istream = new DataStream(indices.ToArray(), true, false); istream.Position = 0;


            var vbd = new BufferDescription(
            Marshal.SizeOf(new GridVertex()) * vertices.Count,
            ResourceUsage.Dynamic,
            BindFlags.VertexBuffer,
            CpuAccessFlags.Write,
            ResourceOptionFlags.None, 0);
            vb = new Buffer(Renderer.viewport.Device, vstream, vbd);
            vbind = new VertexBufferBinding(vb, Marshal.SizeOf(new GridVertex()), 0);

            //indices
            var ibd = new BufferDescription(
            sizeof(uint) * indices.Count,
            ResourceUsage.Dynamic,
            BindFlags.IndexBuffer,
            CpuAccessFlags.Write,
            ResourceOptionFlags.None, 0);
            ib = new Buffer(Renderer.viewport.Device, istream, ibd);

            indexCount = indices.Count;

            Renderer.gridMeshes.Add(this);

        }
        public void Draw()
        {
            Renderer.viewport.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, vbind);
            Renderer.viewport.Device.ImmediateContext.InputAssembler.SetIndexBuffer(ib, Format.R32_UInt, 0);
            Renderer.viewport.Device.ImmediateContext.DrawIndexed(indexCount, 0, 0);
        }
    }

    class RenderPass
    {
        public static List<BasicMesh> Meshes = new List<BasicMesh>();
        public static RasterizerState RS;
        static ShaderSignature Sig;
        public static VertexShader VS;
        public static PixelShader PS;
        public static GeometryShader GS_Tri;
        public static GeometryShader GS_Vert;
        public static InputLayout Inpl;
        public RenderPass(string name, InputElement[] inplElems) { Init(name, inplElems); }
        private static void Init(string name, InputElement[] inplElems)
        {
            //Shaders
            using (var b = ShaderBytecode.CompileFromFile("Shaders\\" + name + "Shader.fx", "vs", "vs_4_0", ShaderFlags.None, EffectFlags.None))
            {
                Sig = ShaderSignature.GetInputSignature(b);
                VS = new VertexShader(Renderer.viewport.Device, b);
            }
            using (var b = ShaderBytecode.CompileFromFile("Shaders\\" + name + "Shader.fx", "ps", "ps_4_0", ShaderFlags.None, EffectFlags.None))
            {
                PS = new PixelShader(Renderer.viewport.Device, b);
            }
            using (var b = ShaderBytecode.CompileFromFile("Shaders\\" + name + "Shader.fx", "gs", "gs_4_0", ShaderFlags.None, EffectFlags.None))
            {
                GS_Tri = new GeometryShader(Renderer.viewport.Device, b);
            }

            //InputLayout
            Inpl = new InputLayout(Renderer.viewport.Device, Sig, inplElems);

            //Rasterizer state
            var wireFrameDesc = new RasterizerStateDescription
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
                IsFrontCounterclockwise = false,
                IsDepthClipEnabled = true
            };
            RS = RasterizerState.FromDescription(Renderer.viewport.Device, wireFrameDesc);
        }
        public void Use()
        {
            Renderer.viewport.Device.ImmediateContext.Rasterizer.State = RS;
            Renderer.viewport.Device.ImmediateContext.VertexShader.Set(VS);
            Renderer.viewport.Device.ImmediateContext.PixelShader.Set(PS);
            Renderer.viewport.Device.ImmediateContext.InputAssembler.InputLayout = Inpl;
            Renderer.viewport.Device.ImmediateContext.GeometryShader.Set(GS_Tri);
        }
    }
}
