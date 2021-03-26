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

namespace SMEditor.Editor
{

    public class TerrainChunk
    {
        public const int size = 64;
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
            xPos = _arrX * (size - 1);
            yPos = _arrY * (size - 1);
            arrayX = _arrX;
            arrayY = _arrY;

            dMesh = new DMesh3(MeshComponents.VertexNormals | MeshComponents.VertexColors);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Vector3d v = new Vector3d((float)i + xPos, 0, (float)j + yPos);
                    //vertices.Add(new BasicVertex(Convert.ToV3(v), new Vector3(0, 0, 0), new Vector3(0, 1, 0)));
                    dMesh.AppendVertex(new NewVertexInfo(v, new Vector3f(0, 1, 0)));

                    //fancy uv stuff. automatically sets the uvs so the texture is spreat over the entire terrain.
                    float uvu, uvv;
                    uvu =     ((((1f / size) * i) / terrainSize) + ((1f / terrainSize) * arrayX));
                    uvv = 1 - ((((1f / size) * j) / terrainSize) + ((1f / terrainSize) * arrayY));
                    if (i==0 && j==0)Console.Write(uvu + " " + uvv);
                    visualUVs.Add(new Vector2(uvu, uvv));

                    visualVerts.Add(Convert.ToV3(v));
                    visualNormals.Add(new Vector3(0, 1, 0));
                    vertNeedsCollisionUpdate.Add(false);
                }
            }
            for (int j = 0; j < size - 1; j++)
            {
                for (int i = 0; i < size - 1; i++)
                {
                    int i1, i2, i3, i4, i5, i6;

                    int row1 = i * size;
                    int row2 = (i + 1) * size;

                    i1 = row1 + j;
                    i2 = row1 + j + 1;
                    i3 = row2 + j;

                    i4 = row2 + j;
                    i5 = row2 + j + 1;
                    i6 = row1 + j + 1;

                    if (i == size - 1 && j == size - 1) Console.Write(i4 + " " + i5 + " " + i6);

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
        public List<Vector3> visualVerts = new List<Vector3>();
        public List<Vector3> visualNormals = new List<Vector3>();
        private List<Vector2> visualUVs = new List<Vector2>();
        public List<bool> vertNeedsCollisionUpdate = new List<bool>();
        //
        private void InitRendering(List<int> inds)
        {
            indexCount = inds.Count();
            DataStream vd = new DataStream(PackedVertexData(), true, true);
            vd.Position = 0;
            DataStream id = new DataStream(inds.ToArray(), true, true);
            id.Position = 0;

            vbd = new BufferDescription
            {
                Usage = ResourceUsage.Default,
                SizeInBytes = sizeof(float) * 8 * (size * size),
                BindFlags = BindFlags.VertexBuffer
            };

            vb = new Buffer(Renderer.viewport.Device, vd, vbd);
            vbind = new VertexBufferBinding(vb, sizeof(float) * 8, 0);

            //indices
            ibd = new BufferDescription(
            sizeof(uint) * indexCount,
            ResourceUsage.Dynamic,
            BindFlags.IndexBuffer,
            CpuAccessFlags.Write,
            ResourceOptionFlags.None, 0);
            ib = new Buffer(Renderer.viewport.Device, id, ibd);

            vd.Dispose();
            id.Dispose();
        }
        public void Draw()
        {
            Terrain.renderPass.Use();

            Renderer.mainCamera.SetModelMatrix(Matrix.Transformation(new Vector3(0, 0, 0),
                Quaternion.Identity, new Vector3(1, 1, 1), new Vector3(0, 0, 0), Quaternion.Identity, new Vector3(0, 0, 0)));

            Renderer.viewport.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, vbind);
            Renderer.viewport.Device.ImmediateContext.InputAssembler.SetIndexBuffer(ib, Format.R32_SInt, 0);
            Renderer.viewport.Device.ImmediateContext.DrawIndexed(indexCount, 0, 0);
        }
        
        //Updates
        public void UpdateVisual()
        {
            DataStream vd = new DataStream(PackedVertexData(), true, true);
            vd.Position = 0;

            Renderer.viewport.Context.UpdateSubresource(new DataBox(0, 0, vd), vb, 0);
            vd.Dispose();
        }
        public void UpdateCollision()
        {
            for (int i = 0; i < size * size; i++)
            {
                if (vertNeedsCollisionUpdate[i])
                {
                    dMesh.SetVertex(i, Convert.ToV3d(visualVerts[i]));
                    vertNeedsCollisionUpdate[i] = false;
                }
            }
            dMeshAABB.Build();
        }

        //Gets
        private float[] PackedVertexData()
        {
            List<float> f = new List<float>(visualVerts.Count * 8);
            for (int i = 0; i < visualVerts.Count; i++)
            {
                f.Add(visualVerts[i].X);
                f.Add(visualVerts[i].Y);
                f.Add(visualVerts[i].Z);
                f.Add(visualUVs[i].X);
                f.Add(visualUVs[i].Y);
                f.Add(visualNormals[i].X);
                f.Add(visualNormals[i].Y);
                f.Add(visualNormals[i].Z);
            }
            return f.ToArray();
        }
    }




    public class Terrain
    {
        public int chunksPerAxis;
        public static RenderPass renderPass = new RenderPass("terrain", FillMode.Solid, new[]
            {
                new InputElement("POSITION_IN", 0, Format.R32G32B32_Float, 0, 0),
                new InputElement("UV_IN", 0, Format.R32G32_Float, 12, 0),
                new InputElement("NORMAL_IN", 0, Format.R32G32B32_Float, 20, 0)
            });
        public TerrainChunk[,] terrainChunks;
        private bool[,] terrainChunkNeedsAABBUpdate;
        private bool[,] terrainChunkNeedsVisualUpdate;
        
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

            InitRendering();
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
            Vector3 v = terrainChunks[map.x, map.y].visualVerts[map.index];
            if (mode == EditMode.Add) v.Y += height;
            if (mode == EditMode.Set) v.Y = height;

            terrainChunks[map.x, map.y].visualVerts[map.index] = v;
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
        //  Texutring
        //      Splatting

        //      Texture representation
        private void InitRendering()
        {
            ShaderResourceView view = ShaderResourceView.FromFile(Renderer.viewport.Device, "thumbs/sizechart.png");
            Renderer.viewport.Device.ImmediateContext.PixelShader.SetShaderResource(view, 0);
        }
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
    }
}
