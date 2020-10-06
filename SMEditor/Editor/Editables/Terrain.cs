using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMEditor.Editor;
using SMEditor;
using SlimDX;
using System.Drawing;
using g3;

namespace SMEditor.Editor
{
    public class Terrain
    {
        //data
        int size;

        //world
        public DMesh3 dMesh;
        public DMeshAABBTree3 dMeshAABB;

        //visual
        TerrainMesh visualMesh;

        public Terrain(int _size)
        {
            size = _size;

            dMesh = new DMesh3(MeshComponents.VertexColors);

            for (int i = 0; i <= size + 1; i++)
            {
                for (int j = 0; j <= size + 1; j++)
                {
                    Vector3d v = new Vector3d((float)i, 0, (float)j);

                    dMesh.AppendVertex(new NewVertexInfo(v, new Vector3f(0,0,1)));
                }
            }
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    int row1 = j * (size + 1);
                    int row2 = (j + 1) * (size + 1);
                    //tri 1
                    dMesh.AppendTriangle(row1 + i, row1 + i + 1, row2 + i + 1);
                    //tri 2
                    dMesh.AppendTriangle(row1 + i, row2 + i + 1, row2 + i);
                }
            }
            
            dMeshAABB = new DMeshAABBTree3(dMesh);
            dMeshAABB.Build();
            Console.WriteLine(dMeshAABB.Bounds.Min + " | " + dMeshAABB.Bounds.Max);
            

            UpdateVisual();
        }
        
        public void UpdateVisual()
        {
            List<BasicVertex> bvs = new List<BasicVertex>();
            List<int> inds = new List<int>();

            for (int i = 0; i < dMesh.MaxVertexID; i++)
            {
                Vector3d pd = dMesh.GetVertex(i);
                Vector3 p = new Vector3((float)pd.x, (float)pd.y, (float)pd.z);
                Vector3d cd = dMesh.GetVertexColor(i);
                Vector3 c = new Vector3((float)cd.x, (float)cd.y, (float)cd.z);
                BasicVertex bv = new BasicVertex(p, c);
                bvs.Add(bv);
            }

            Console.WriteLine(dMesh.MaxVertexID);

            int triCnt = dMesh.Triangles().Count();
            List<Index3i> i3s = dMesh.Triangles().ToList();

            for (int i = 0; i < triCnt; i++)
            {
                inds.Add(i3s[i].a);
                inds.Add(i3s[i].b);
                inds.Add(i3s[i].c);
            }

            Console.WriteLine(dMesh.Triangles().Count());

            visualMesh = new TerrainMesh();
            visualMesh.Init(bvs, inds);
            Renderer.terrainMeshes.Add(visualMesh);
            
        }
    }
}
