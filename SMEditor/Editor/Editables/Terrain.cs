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

namespace SMEditor.Editor
{
    public class TerrainChunk
    {
        int xPos, yPos, size;
        public int arrayX, arrayY;

        public DMesh3 dMesh;
        public DMeshAABBTree3 dMeshAABB;

        public TerrainChunk(int _arrX, int _arrY)
        {
            //this can be done here because it wont ever need to be modified.
            List<int> inds = new List<int>();

            #region Init
            size = 64;
            xPos = _arrX * size;
            yPos = _arrY * size;
            arrayX = _arrX;
            arrayY = _arrY;

            dMesh = new DMesh3(MeshComponents.VertexNormals | MeshComponents.VertexColors);
            
            for (int i = 0; i <= size; i++)
            {
                for (int j = 0; j <= size; j++)
                {
                    Vector3d v = new Vector3d((float)i + xPos, 0, (float)j + yPos);
                    //vertices.Add(new BasicVertex(Convert.ToV3(v), new Vector3(0, 0, 0), new Vector3(0, 1, 0)));
                    dMesh.AppendVertex(new NewVertexInfo(v, new Vector3f(0, 0, 0)));

                    visualVerts.Add(Convert.ToV3(v));
                    vertNeedsCollisionUpdate.Add(false);
                }
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int row1 = j * (size + 1);
                    int row2 = (j + 1) * (size + 1);
                    //tri 1
                    dMesh.AppendTriangle(row1 + i, row1 + i + 1, row2 + i + 1);
                    //tri 2
                    dMesh.AppendTriangle(row1 + i, row2 + i + 1, row2 + i);
                    
                    inds.Add(row1 + i);
                    inds.Add(row1 + i + 1);
                    inds.Add(row2 + i + 1);

                    inds.Add(row1 + i);
                    inds.Add(row2 + i + 1);
                    inds.Add(row2 + i);
                }
            }
            dMesh.EnableVertexNormals(new Vector3f(0, 1, 0));

            dMeshAABB = new DMeshAABBTree3(dMesh);
            dMeshAABB.Build();
            #endregion





            #region Init Rendering

            indexCount = inds.Count();
            DataStream vd = new DataStream(GetFloatsFromDMesh(), true, true); vd.Position = 0;
            DataStream id = new DataStream(inds.ToArray(), true, true); id.Position = 0;

            vbd = new BufferDescription
            {
                Usage = ResourceUsage.Default,
                SizeInBytes = 4 * dMesh.VerticesBuffer.Count(),
                BindFlags = BindFlags.VertexBuffer
            };

            vb = new Buffer(Renderer.viewport.Device, vd, vbd);
            vbind = new VertexBufferBinding(vb, 12, 0);

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
            #endregion
        }

        #region Rendering
        BufferDescription vbd, ibd;
        public int indexCount = 0;
        public Buffer vb;
        public Buffer ib;
        public VertexBufferBinding vbind;
        public Vector3 location = new Vector3(0, 0, 0);
        public Vector3 scale = new Vector3(1, 1, 1);

        public List<Vector3> visualVerts = new List<Vector3>();
        public List<bool> vertNeedsCollisionUpdate = new List<bool>();

        public void Draw()
        {
            Terrain.renderPass.Use();

            Renderer.mainCamera.SetModelMatrix(Matrix.Transformation(new Vector3(0, 0, 0),
                Quaternion.Identity, new Vector3(1, 1, 1), new Vector3(0, 0, 0), Quaternion.Identity, new Vector3(0, 0, 0)));

            Renderer.viewport.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, vbind);
            Renderer.viewport.Device.ImmediateContext.InputAssembler.SetIndexBuffer(ib, Format.R32_SInt, 0);
            Renderer.viewport.Device.ImmediateContext.DrawIndexed(indexCount, 0, 0);
        }
        public void UpdateVisual()
        {
            DataStream vd = new DataStream(visualVerts.ToArray(), true, true); vd.Position = 0;
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

        private float[] GetFloatsFromDMesh()
        {
            return dMesh.VerticesBuffer.ToList().ConvertAll(x => (float)(double)x).ToArray();
        }
        #endregion
    }




    public class Terrain
    {
        public int chunksPerAxis;
        public static RenderPass renderPass;
        public TerrainChunk[,] terrainChunks;
        private bool[,] terrainChunkNeedsAABBUpdate;
        private bool[,] terrainChunkNeedsVisualUpdate;


        public Terrain(int _chunksPerAxis)
        {
            chunksPerAxis = _chunksPerAxis;
            terrainChunks = new TerrainChunk[_chunksPerAxis, _chunksPerAxis];
            terrainChunkNeedsAABBUpdate = new bool[_chunksPerAxis, _chunksPerAxis];
            terrainChunkNeedsVisualUpdate = new bool[_chunksPerAxis, _chunksPerAxis];
            for (int x = 0; x < _chunksPerAxis; x++)
            {
                for(int y = 0; y < _chunksPerAxis; y++)
                {
                    terrainChunks[x, y] = new TerrainChunk(x, y);
                    terrainChunkNeedsAABBUpdate[x, y] = false;
                    terrainChunkNeedsVisualUpdate[x, y] = false;
                }
            }
        }

        public Vector3d GetHitLocationFromRay(Ray3d ray)
        {
            Vector3d vout = new Vector3d(0,0,0);

            int currHitTri = -1;
            IntrRay3Triangle3 hitInfo;
            int x = 0, y = 0;

            foreach(TerrainChunk t in terrainChunks)
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

        //TODO: REMOVE!
        public void UpdateLighting()
        {
            //SMMeshNormals.QuickCompute(dMesh);
            //for(int i = 0; i < vertices.Count; i++)
            //{
            //    if (vertexNeedsLightingUpdate[i])
            //    {
            //        vertices[i] = new BasicVertex(vertices[i].position, vertices[i].color, Convert.ToV3(dMesh.GetVertexNormal(i)));
            //        vertexNeedsLightingUpdate[i] = false;
            //    }
            //}
        }
        public void UpdateNormalFast(int vID)
        {
            //List<int> t = new List<int>();
            //dMesh.GetVtxTriangles(vID, t, false);
            //foreach (int triI in t)
            //{
            //    Index3i tri = dMesh.GetTriangle(triI);
            //    Vector3d va = Convert.ToV3d(vertices[tri.a].position);
            //    Vector3d vb = Convert.ToV3d(vertices[tri.b].position);
            //    Vector3d vc = Convert.ToV3d(vertices[tri.c].position);
            //    Vector3d N = MathUtil.Normal(ref va, ref vb, ref vc);
            //    double a = MathUtil.Area(ref va, ref vb, ref vc);
            //    vertices[vID] = new BasicVertex(vertices[vID].position, vertices[vID].color, Convert.ToV3(a * N));
            //}
        }
        public void UpdateNormalsFinal()
        {
            //MeshNormals.QuickCompute(dMesh);
            //for(int i = 0; i < vertices.Count; i++)
            //{
            //    vertices[i] = new BasicVertex(vertices[i].position, vertices[i].color, Convert.ToV3(dMesh.GetVertexNormal(i)));
            //}
            //UpdateVisual();
        }

        public class TerrainIndexMapping { public int index, x, y; public TerrainIndexMapping(int _i, int _x, int _y) { index = _i; x = _x; y = _y; } }
        public List<TerrainIndexMapping> GetVertsInRadius(Vector3 position, double radius)
        {
            List<TerrainIndexMapping> tim = new List<TerrainIndexMapping>();

            Vector3d v3d = Convert.ToV3d(position);
            AxisAlignedBox3d pointBox = new AxisAlignedBox3d(v3d, radius);
            int vID = -1;

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
        public enum EditMode { Add, Set }
        public void EditVertexHeight(TerrainIndexMapping map, float height, EditMode mode)
        {
            Vector3 v = terrainChunks[map.x, map.y].visualVerts[map.index];
            if (mode == EditMode.Add) v.Y += height;
            if (mode == EditMode.Set) v.Y = height;

            //UpdateNormalFast(vID);
            terrainChunks[map.x, map.y].visualVerts[map.index] = v;
            terrainChunks[map.x, map.y].vertNeedsCollisionUpdate[map.index] = true;

            terrainChunkNeedsVisualUpdate[map.x, map.y] = true;
            terrainChunkNeedsAABBUpdate[map.x, map.y] = true;

            //vertexNeedsCollisionUpdate[vID] = true;
            //vertexNeedsLightingUpdate[vID] = true;
        }

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

        public void Dispose()
        {
        }

        public void Draw()
        {
            foreach(TerrainChunk t in terrainChunks)
            {
                t.Draw();
            }
        }
        
    }
}
