﻿using System;
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
            InitTerrain();
            InitGrid();
            InitboldVert();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Terrain specifics
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static List<BasicMesh> terrainMeshes = new List<BasicMesh>();
        public static RasterizerState terrainRS;
        static ShaderSignature terrainSig;
        public static VertexShader terrainVS;
        public static PixelShader terrainPS;
        public static GeometryShader terrainGS_Tri;
        public static GeometryShader terrainGS_Vert;
        public static InputLayout terrainInpl;
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
            using (var b = ShaderBytecode.CompileFromFile("Shaders\\terrainShader.fx", "gsTri", "gs_4_0", ShaderFlags.None, EffectFlags.None))
            {
                terrainGS_Tri = new GeometryShader(viewport.Device, b);
            }
            using (var b = ShaderBytecode.CompileFromFile("Shaders\\terrainShader.fx", "gsVert", "gs_4_0", ShaderFlags.None, EffectFlags.None))
            {
                terrainGS_Vert = new GeometryShader(viewport.Device, b);
            }

            //InputLayout
            var vertDesc = new[]
            {
                new InputElement("POSITION_IN", 0, Format.R32G32B32_Float, 0),
                new InputElement("COLOR_IN", 0, Format.R32G32B32_Float, 12, 0)
            };
            terrainInpl = new InputLayout(viewport.Device, terrainSig, vertDesc);

            //Rasterizer state
            var wireFrameDesc = new RasterizerStateDescription
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
                IsFrontCounterclockwise = false,
                IsDepthClipEnabled = true
            };
            terrainRS = RasterizerState.FromDescription(viewport.Device, wireFrameDesc);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // BoldVertex specifics
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static List<BasicMesh> boldVertMeshes = new List<BasicMesh>();
        public static RasterizerState boldVertRS;
        static ShaderSignature boldVertSig;
        public static VertexShader boldVertVS;
        public static PixelShader boldVertPS;
        public static InputLayout boldVertInpl;
        private static void InitboldVert()
        {
            //Shaders
            using (var b = ShaderBytecode.CompileFromFile("Shaders\\boldVertShader.fx", "vs", "vs_4_0", ShaderFlags.None, EffectFlags.None))
            {
                boldVertSig = ShaderSignature.GetInputSignature(b);
                boldVertVS = new VertexShader(viewport.Device, b);
            }
            using (var b = ShaderBytecode.CompileFromFile("Shaders\\boldVertShader.fx", "ps", "ps_4_0", ShaderFlags.None, EffectFlags.None))
            {
                boldVertPS = new PixelShader(viewport.Device, b);
            }

            //InputLayout
            var vertDesc = new[]
            {
                new InputElement("POSITION_IN", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
                new InputElement("COLOR_IN", 0, Format.R32G32B32_Float, 12, 0, InputClassification.PerVertexData, 0),
                new InputElement("INST_POS", 0, Format.R32G32B32_Float, 24, 1, InputClassification.PerInstanceData, 0),
            };
            boldVertInpl = new InputLayout(viewport.Device, boldVertSig, vertDesc);

            //Rasterizer state
            var wireFrameDesc = new RasterizerStateDescription
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
                IsFrontCounterclockwise = false,
                IsDepthClipEnabled = true
            };
            boldVertRS = RasterizerState.FromDescription(viewport.Device, wireFrameDesc);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Grid specifics
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static List<GridMesh> gridMeshes = new List<GridMesh>();
        static RasterizerState gridRS;
        static ShaderSignature gridSig;
        static VertexShader gridVS;
        static PixelShader gridPS;
        static InputLayout gridInpl;
        private static void InitGrid()
        {
            //Shaders
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
            var vertDesc = new[]
            {
                new InputElement("POSITION_IN", 0, Format.R32G32B32_Float, 0),
                new InputElement("COLOR_IN", 0, Format.R32G32B32_Float, 12, 0)
            };
            gridInpl = new InputLayout(viewport.Device, gridSig, vertDesc);

            //Rasterizer state
            var wireFrameDesc = new RasterizerStateDescription
            {
                FillMode = FillMode.Wireframe,
                CullMode = CullMode.None,
                IsFrontCounterclockwise = false,
                IsDepthClipEnabled = true
            };
            gridRS = RasterizerState.FromDescription(viewport.Device, wireFrameDesc);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Draw functions -- called in interval set in MainWindow.cs.
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void Draw() { Draw(null, null); }
        public static void Draw(object o, System.Windows.Forms.PaintEventArgs e)
        {
            viewport.Clear(System.Drawing.Color.DarkGray);
            viewport.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            mainCamera.UpdateCameraBuffer();

            // Terrain
            viewport.Device.ImmediateContext.Rasterizer.State = terrainRS;
            viewport.Device.ImmediateContext.VertexShader.Set(terrainVS);
            viewport.Device.ImmediateContext.PixelShader.Set(terrainPS);
            viewport.Device.ImmediateContext.InputAssembler.InputLayout = terrainInpl;

            viewport.Device.ImmediateContext.GeometryShader.Set(terrainGS_Tri);
            foreach (BasicMesh m in terrainMeshes) m.Draw();

            if (TerrainMesh.drawPoints) {
                viewport.Device.ImmediateContext.GeometryShader.Set(terrainGS_Vert);
                foreach (BasicMesh m in terrainMeshes) m.Draw();
            }

            // Grid
            viewport.Device.ImmediateContext.Rasterizer.State = gridRS;
            viewport.Device.ImmediateContext.VertexShader.Set(gridVS);
            viewport.Device.ImmediateContext.PixelShader.Set(gridPS);
            viewport.Device.ImmediateContext.InputAssembler.InputLayout = gridInpl;
            foreach (GridMesh m in gridMeshes)
            {
                m.Draw();
            }

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

        public BasicMesh()
        {
        }
        public virtual void Init(List<BasicVertex> vertices, List<uint> indices)
        {
            DataStream vstream = new DataStream(vertices.ToArray(), true, false); vstream.Position = 0;
            DataStream istream = new DataStream(indices.ToArray(), true, false); istream.Position = 0;

            var vbd = new BufferDescription(
            Marshal.SizeOf(new BasicVertex()) * vertices.Count,
            ResourceUsage.Dynamic,
            BindFlags.VertexBuffer,
            CpuAccessFlags.Write,
            ResourceOptionFlags.None, 0);
            vb = new Buffer(Renderer.viewport.Device, vstream, vbd);
            vbind = new VertexBufferBinding(vb, Marshal.SizeOf(new BasicVertex()), 0);

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
        public virtual void Draw()
        {
            Renderer.mainCamera.SetModelMatrix(Matrix.Transformation(
                new Vector3(0, 0, 0), Quaternion.Identity,
                new Vector3(1, 1, 1), new Vector3(0, 0, 0), 
                Quaternion.Identity, location));

            Renderer.viewport.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, vbind);
            Renderer.viewport.Device.ImmediateContext.InputAssembler.SetIndexBuffer(ib, Format.R32_UInt, 0);
            Renderer.viewport.Device.ImmediateContext.DrawIndexed(indexCount, 0, 0);
        }
        public void UpdateData(List<BasicVertex> vertices, List<uint> indices)
        {


        }
    }

    class TerrainMesh : BasicMesh
    {
        public static bool drawPoints = false;

        BasicMesh vertexMesh;
        Buffer instanceData;
        VertexBufferBinding instVBind;
        int instanceCount = 0;

        public TerrainMesh() { }
        public override void Init(List<BasicVertex> vertices, List<uint> indices)
        {
            //Set up vertex mesh
            Vector3 color = new Vector3(1, 0, 0);
            Vector3 v = new Vector3(0, 0, 0);
            List<BasicVertex> bvs = new List<BasicVertex>()
            {
            new BasicVertex(new Vector3(-0.5F + v.X, -0.5F + v.Y, 0.5F + v.Z), color),
            new BasicVertex(new Vector3(0.5F + v.X, -0.5F + v.Y, 0.5F + v.Z), color),
            new BasicVertex(new Vector3(0.5F + v.X, 0.5F + v.Y, 0.5F + v.Z), color),
            new BasicVertex(new Vector3(-0.5F + v.X, 0.5F + v.Y, 0.5F + v.Z), color),

            new BasicVertex(new Vector3(-0.5F + v.X, -0.5F + v.Y, -0.5F + v.Z), color),
            new BasicVertex(new Vector3(0.5F + v.X, -0.5F + v.Y, -0.5F + v.Z), color),
            new BasicVertex(new Vector3(0.5F + v.X, 0.5F + v.Y, -0.5F + v.Z), color),
            new BasicVertex(new Vector3(-0.5F + v.X, 0.5F + v.Y, -0.5F + v.Z), color)
            };
            List<uint> inds = new List<uint>()
            {
        // front
		0, 1, 2,
        2, 3, 0,
		// right
		1, 5, 6,
        6, 2, 1,
		// back
		7, 6, 5,
        5, 4, 7,
		// left
		4, 0, 3,
        3, 7, 4,
		// bottom
		4, 5, 1,
        1, 0, 4,
		// top
		3, 2, 6,
        6, 7, 3
            };
            vertexMesh = new BasicMesh();
            vertexMesh.Init(bvs, inds);
            
            var db = new DataStream((sizeof(float) * 3) * vertices.Count, true, true);
            foreach(BasicVertex bv in vertices)
            {
                db.Write(bv.position);
            }
            db.Position = 0;
            instanceData = new Buffer(Renderer.viewport.Device, db,
            new BufferDescription (
                (sizeof(float) * 3) * vertices.Count,
                ResourceUsage.Dynamic,
                BindFlags.VertexBuffer,
                CpuAccessFlags.Write,
                ResourceOptionFlags.None,
                0));
            
            instanceCount = vertices.Count;

            instVBind = new VertexBufferBinding(instanceData, 12, 0);

            //Set up main mesh
            base.Init(vertices, indices);
        }
        public override void Draw()
        {
            base.Draw();
            
            if(drawPoints)
            {
                Renderer.viewport.Device.ImmediateContext.Rasterizer.State = Renderer.boldVertRS;
                Renderer.viewport.Device.ImmediateContext.VertexShader.Set(Renderer.boldVertVS);
                Renderer.viewport.Device.ImmediateContext.PixelShader.Set(Renderer.boldVertPS);
                Renderer.viewport.Device.ImmediateContext.InputAssembler.InputLayout = Renderer.boldVertInpl;

                Renderer.viewport.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, vertexMesh.vbind);
                Renderer.viewport.Device.ImmediateContext.InputAssembler.SetVertexBuffers(1, instVBind);

                Renderer.viewport.Device.ImmediateContext.InputAssembler.SetIndexBuffer(vertexMesh.ib, Format.R32_UInt, 0);
                Renderer.viewport.Device.ImmediateContext.DrawIndexedInstanced(36, 2, 0, 0, 0);
                ///


            }
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
}
