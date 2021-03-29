using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMEditor.Editor;
using SMEditor;
using System.Drawing;
using g3;
using System.Threading;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Buffer = SlimDX.Direct3D11.Buffer;
using BepuPhysics.Collidables;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.IO;
using Pfim;
using System.Drawing.Imaging;

namespace SMEditor.Editor
{

    public class TerrainChunk
    {
        public const int numXVerts = 64;
        int xPos, yPos;
        public int arrayX, arrayY;
        public DMesh3 dMesh;
        public DMeshAABBTree3 dMeshAABB;

        //Ctor
        public TerrainChunk(int _arrX, int _arrY, int terrainSize)
        {
            //this can be done here because it wont ever need to be modified.
            List<int> inds = new List<int>();

            #region Init
            xPos = _arrX * (numXVerts - 1);
            yPos = _arrY * (numXVerts - 1);
            arrayX = _arrX;
            arrayY = _arrY;

            dMesh = new DMesh3(MeshComponents.VertexNormals | MeshComponents.VertexColors);

            int indexTracker = 0;
            for (int i = 0; i < numXVerts; i++)
            {
                for (int j = 0; j < numXVerts; j++)
                {
                    Vector3d v = new Vector3d((float)i + xPos, 0, (float)j + yPos);
                    //vertices.Add(new BasicVertex(Convert.ToV3(v), new Vector3(0, 0, 0), new Vector3(0, 1, 0)));
                    dMesh.AppendVertex(new NewVertexInfo(v, new Vector3f(0, 1, 0)));

                    //fancy uv stuff.
                    float uvu, uvv;
                    uvu = ((1f / numXVerts) * i);
                    uvv = 1 - ((1f / numXVerts) * j);

                    Vertex vert = new Vertex()
                    {
                        x = (float)v.x,
                        y = (float)v.y,
                        z = (float)v.z,
                        u = uvu,
                        v = uvv,
                        n1 = 0.0f,
                        n2 = 1.0f,
                        n3 = 0.0f,
                        index = indexTracker
                    };
                    vertices.Add(vert);
                    vertNeedsCollisionUpdate.Add(false);

                    indexTracker++;
                }
            }
            for (int j = 0; j < numXVerts - 1; j++)
            {
                for (int i = 0; i < numXVerts - 1; i++)
                {
                    int i1, i2, i3, i4, i5, i6;

                    int row1 = i * numXVerts;
                    int row2 = (i + 1) * numXVerts;

                    i1 = row1 + j;
                    i2 = row1 + j + 1;
                    i3 = row2 + j;

                    i4 = row2 + j + 1;
                    i5 = row2 + j;
                    i6 = row1 + j + 1;

                    if (i == numXVerts - 1 && j == numXVerts - 1) Console.Write(i4 + " " + i5 + " " + i6);

                    dMesh.AppendTriangle(i1, i2, i3);
                    dMesh.AppendTriangle(i4, i5, i6);
                    inds.Add(i1);
                    inds.Add(i2);
                    inds.Add(i3);
                    inds.Add(i4);
                    inds.Add(i5);
                    inds.Add(i6);
                }
            }
            dMesh.EnableVertexNormals(new Vector3f(0, 1, 0));

            dMeshAABB = new DMeshAABBTree3(dMesh);
            dMeshAABB.Build();
            #endregion
            //
            //
            InitRendering(inds);
        }

        //Rendering
        BufferDescription vbd, ibd;
        public int indexCount = 0;
        public Buffer vb;
        public Buffer ib;
        public VertexBufferBinding vbind;
        public Vector3 location = new Vector3(0, 0, 0);
        public Vector3 scale = new Vector3(1, 1, 1);
        //
        [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 36)]
        public class Vertex
        {
            public float x, y, z, u, v, n1, n2, n3;
            public int index;
        }
        public List<Vertex> vertices                 = new List<Vertex>();
        public List<bool>   vertNeedsCollisionUpdate = new List<bool>();
        //
        private void InitRendering(List<int> inds)
        {
            indexCount = inds.Count();
            DataStream vd = new DataStream(GetBlittableVertexData(), true, true);
            vd.Position = 0;
            DataStream id = new DataStream(inds.ToArray(), true, true);
            id.Position = 0;

            vbd = new BufferDescription
            {
                Usage = ResourceUsage.Default,
                SizeInBytes = Marshal.SizeOf(new Vertex()) * (numXVerts * numXVerts),
                BindFlags = BindFlags.VertexBuffer
            };

            vb = new Buffer(Renderer.viewport.Device, vd, vbd);
            vbind = new VertexBufferBinding(vb, Marshal.SizeOf(new Vertex()), 0);
            
            {
                ibd = new BufferDescription(
                sizeof(uint) * indexCount,
                ResourceUsage.Dynamic,
                BindFlags.IndexBuffer,
                CpuAccessFlags.Write,
                ResourceOptionFlags.None, 0);
            }
            ib = new Buffer(Renderer.viewport.Device, id, ibd);

            vd.Dispose();
            id.Dispose();

            splatBuffer = new Buffer(Renderer.viewport.Device, new BufferDescription() {
                Usage = ResourceUsage.Default,
                SizeInBytes = numXVerts * numXVerts * 16,
                BindFlags = BindFlags.ConstantBuffer,
            });

            //init the first alpha layer to be fully opaque.
            for (int i = 0; i < numXVerts * numXVerts * 16; i+=16)
            {
                alphas[i + 0] = 100;
                alphas[i + 1] = 23;
                alphas[i + 2] = 255;
            }

            //init the buffer for the first time.
            UpdateSplatGPUBuffer();
        }
        public void Draw()
        {
            //apply shader
            Terrain.renderPass.Use();
            
            //set splat buffer
            Renderer.viewport.Device.ImmediateContext.PixelShader.SetConstantBuffer(splatBuffer, 1);

            //apply transform
            Renderer.mainCamera.SetModelMatrix(Matrix.Transformation(new Vector3(0, 0, 0),
                Quaternion.Identity, new Vector3(1, 1, 1), new Vector3(0, 0, 0), Quaternion.Identity, new Vector3(0, 0, 0)));

            //draw
            Renderer.viewport.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, vbind);
            Renderer.viewport.Device.ImmediateContext.InputAssembler.SetIndexBuffer(ib, Format.R32_SInt, 0);
            Renderer.viewport.Device.ImmediateContext.DrawIndexed(indexCount, 0, 0);
        }
        //
        //Splatting
        private byte[] alphas = new byte[numXVerts * numXVerts * 16];
        Buffer splatBuffer;

        //Updates
        public void UpdateVisual()
        {
            DataStream vd = new DataStream(GetBlittableVertexData(), true, true);
            vd.Position = 0;

            Renderer.viewport.Context.UpdateSubresource(new DataBox(0, 0, vd), vb, 0);
            vd.Dispose();
        }
        private void UpdateSplatGPUBuffer()
        {
            var db0 = new DataStream(alphas, true, true);
            Renderer.viewport.Context.UpdateSubresource(new DataBox(0, 0, db0), splatBuffer, 0);
            db0.Position = 0;
            db0.Dispose();
        }
        public void UpdateCollision()
        {
            for (int i = 0; i < numXVerts * numXVerts; i++)
            {
                if (vertNeedsCollisionUpdate[i])
                {
                    dMesh.SetVertex(i, new Vector3d(vertices[i].x, vertices[i].y, vertices[i].z));
                    vertNeedsCollisionUpdate[i] = false;
                }
            }
            dMeshAABB.Build();
        }


        //Because C# is dumb:
        [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 36)]
        public struct VertexBlitable
        {
            public float x, y, z, u, v, n1, n2, n3;
            public int index;
        }
        private VertexBlitable[] GetBlittableVertexData()
        {
            List<VertexBlitable> vl = new List<VertexBlitable>(numXVerts * numXVerts);

            foreach (Vertex v in vertices)
            {
                vl.Add(new VertexBlitable()
                {
                    x = v.x,
                    y = v.y,
                    z = v.z,
                    u = v.u,
                    v = v.v,
                    n1 = v.n1,
                    n2 = v.n2,
                    n3 = v.n3,
                    index = v.index
                });
            }
            return vl.ToArray();
        }
    }




    public class Terrain
    {
        public int chunksPerAxis;
        public static RenderPass renderPass = new RenderPass("terrain", FillMode.Solid, new[]
            {
                new InputElement("POSITION_IN", 0, Format.R32G32B32_Float, 0, 0),
                new InputElement("UV_IN", 0, Format.R32G32_Float, 12, 0),
                new InputElement("NORMAL_IN", 0, Format.R32G32B32_Float, 20, 0),
                new InputElement("INDEX_IN", 0, Format.R32_SInt, 32, 0)
            }, false);
        public TerrainChunk[,] terrainChunks;
        private bool[,] terrainChunkNeedsAABBUpdate;
        private bool[,] terrainChunkNeedsVisualUpdate;
        public TerrainTexture texture;

        //Ctor
        public Terrain(int _chunksPerAxis)
        {
            chunksPerAxis = _chunksPerAxis;
            terrainChunks = new TerrainChunk[_chunksPerAxis, _chunksPerAxis];
            terrainChunkNeedsAABBUpdate = new bool[_chunksPerAxis, _chunksPerAxis];
            terrainChunkNeedsVisualUpdate = new bool[_chunksPerAxis, _chunksPerAxis];
            for (int x = 0; x < _chunksPerAxis; x++)
            {
                for (int y = 0; y < _chunksPerAxis; y++)
                {
                    terrainChunks[x, y] = new TerrainChunk(x, y, _chunksPerAxis);
                    terrainChunkNeedsAABBUpdate[x, y] = false;
                    terrainChunkNeedsVisualUpdate[x, y] = false;
                }
            }

            texture = new TerrainTexture();
        }
        
        //Gets
        public class TerrainIndexMapping { public int index, x, y; public TerrainIndexMapping(int _i, int _x, int _y) { index = _i; x = _x; y = _y; } }
        public List<TerrainIndexMapping> GetVertsInRadius(Vector3 position, double radius)
        {
            List<TerrainIndexMapping> tim = new List<TerrainIndexMapping>();

            Vector3d v3d = Convert.ToV3d(position);
            AxisAlignedBox3d pointBox = new AxisAlignedBox3d(v3d, radius);
            pointBox.Min += new Vector3d(0, -1000f, 0); pointBox.Max += new Vector3d(0, 1000f, 0); //adjust box to always find neighboring chunks.

            foreach (TerrainChunk t in terrainChunks)
            {
                if (pointBox.Intersects(t.dMeshAABB.Bounds))
                {
                    foreach (int v in t.dMesh.VertexIndices())
                    {
                        if (Vector3.Distance(Convert.ToV3(t.dMesh.GetVertex(v)), position) <= radius)
                        {
                            tim.Add(new TerrainIndexMapping(v, t.arrayX, t.arrayY));
                        }
                    }
                }
            }
            return tim;
            #region old
            //List<int> verts = new List<int>();

            //int halfRad = (int)Math.Round(rad / 2F) + 1;

            ////get all verts in radius
            //for (int x = 0; x < halfRad * 2; x++) 
            //{
            //    for (int y = 0; y < halfRad * 2; y++)
            //    {
            //        verts.Add(vID + x + ((size + 1) * y));
            //        verts.Add(vID - x + ((size + 1) * y));

            //        verts.Add(vID + x + ((size + 1) * -y));
            //        verts.Add(vID - x + ((size + 1) * -y));
            //    }
            //}

            //List<int> finalVerts = new List<int>();
            ////clean verts for any out-of-bounds entries, or verts that are outside the radius.
            //foreach(int i in verts)
            //{
            //    if (i >= 0 && i <= size * size && Vector3.Distance(vertices[i].position, vertices[vID].position) < rad)
            //    {
            //        finalVerts.Add(i);
            //    }
            //}

            ////use Distinct() to remove duplicates, just in case.
            //return finalVerts.Distinct().ToList();
            #endregion
        }
        public Vector3d GetHitLocationFromRay(Ray3d ray)
        {
            Vector3d vout = new Vector3d(0, 0, 0);

            int currHitTri = -1;
            IntrRay3Triangle3 hitInfo;
            int x = 0, y = 0;

            foreach (TerrainChunk t in terrainChunks)
            {
                currHitTri = t.dMeshAABB.FindNearestHitTriangle(ray);
                if (currHitTri != -1)
                {
                    x = t.arrayX;
                    y = t.arrayY;
                    break;
                }
            }

            if (currHitTri != -1)
            {
                hitInfo = MeshQueries.TriangleIntersection(terrainChunks[x, y].dMesh, currHitTri, ray);
                vout = hitInfo.Ray.PointAt(hitInfo.RayParameter);
            }

            return vout;
        }

        //Edits
        public enum EditMode { Add, Set }
        public void EditVertexHeight(TerrainIndexMapping map, float height, EditMode mode)
        {
            float y = terrainChunks[map.x, map.y].vertices[map.index].y;
            if (mode == EditMode.Add) y += height;
            if (mode == EditMode.Set) y = height;

            terrainChunks[map.x, map.y].vertices[map.index].y = y;
            terrainChunks[map.x, map.y].vertNeedsCollisionUpdate[map.index] = true;

            terrainChunkNeedsVisualUpdate[map.x, map.y] = true;
            terrainChunkNeedsAABBUpdate[map.x, map.y] = true;
        }

        //Updates
        public void UpdateVisual()
        {
            for (int x = 0; x < chunksPerAxis; x++)
            {
                for (int y = 0; y < chunksPerAxis; y++)
                {
                    if (terrainChunkNeedsVisualUpdate[x, y])
                    {
                        terrainChunks[x, y].UpdateVisual();
                        terrainChunkNeedsVisualUpdate[x, y] = false;
                    }
                }
            }
        }
        public void UpdateCollisionModel()
        {
            for (int x = 0; x < chunksPerAxis; x++)
            {
                for (int y = 0; y < chunksPerAxis; y++)
                {
                    if (terrainChunkNeedsAABBUpdate[x, y])
                    {
                        terrainChunks[x, y].UpdateCollision();
                        terrainChunkNeedsAABBUpdate[x, y] = false;
                    }
                }
            }
        }

        //Rendering
        public void Draw()
        {
            foreach (TerrainChunk t in terrainChunks)
            {
                t.Draw();
            }
        }

        //Dtor
        public void Dispose()
        {
        }
    };

    public class TerrainTexture
    {
        Texture2D texArray;
        DataRectangle[] textureData = new DataRectangle[16];
        int[] layerOrderMapping = new int[16];
        public TerrainTexture()
        {
            for(int i = 0; i < 16; i++)
            {
                textureData[i] = new DataRectangle(1024 * 4, new DataStream(1024 * 4 * 1024, true, true));
            }

            LoadTextureIntoSlot(0, @"C:\Program Files (x86)\Steam\steamapps\common\HaloWarsDE\Extract\art\terrain\harvest\snowtrail_01_df.ddx.dds");
            LoadTextureIntoSlot(1, @"C:\Program Files (x86)\Steam\steamapps\common\HaloWarsDE\Extract\art\terrain\harvest\glassing_02_df.ddx.dds");
        }

        public void LoadTextureIntoSlot(int slot, string path)
        {
            textureData[slot] = new DataRectangle(1024 * 4, LoadDDS(path));

            Texture2DDescription d = new Texture2DDescription()
            {
                ArraySize = 16,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Height = 1024,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Immutable,
                Width = 1024,
            };
            texArray = new Texture2D(Renderer.viewport.Device, d, textureData);

            ShaderResourceViewDescription srvd = new ShaderResourceViewDescription
            {
                Dimension = ShaderResourceViewDimension.Texture2DArray,
                MipLevels = 1,
                MostDetailedMip = 0,
                FirstArraySlice = 0,
                ArraySize = 16
            };
            ShaderResourceView v = new ShaderResourceView(Renderer.viewport.Device, texArray, srvd);
            Renderer.viewport.Device.ImmediateContext.PixelShader.SetShaderResource(v, 0);
        }
        private DataStream LoadDDS(string path)
        {
            var img = Pfim.Pfim.FromFile(path);
            if (img.Width != 1024 || img.Height != 1024) throw new Exception("Terrain textures must be 1024x1024px.");
            return new DataStream(img.Data, true, true);
        }
    }
}
