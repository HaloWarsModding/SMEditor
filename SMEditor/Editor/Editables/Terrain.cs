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
    public class TerrainChunk
    {
        //data
        Vector3 location;
        int pointsPerAxis;
        float pointSpacing;

        //world
        public DMesh3 dMesh;
        public DMeshAABBTree3 dMeshAABB;

        //visual
        TerrainMesh visualMesh;

        public TerrainChunk(float widthInWorldUnits, int _pointsPerAxis)
        {
            location = _location;
            pointsPerAxis = _pointsPerAxis;
            pointSpacing = widthInWorldUnits / pointsPerAxis;

            dMesh = new DMesh3(MeshComponents.VertexColors);

            InitWorld();

            visualMesh = new TerrainMesh();
            visualMesh.Init(visualVertices, visualIndices);
            visualMesh.location = location;
            Renderer.terrainMeshes.Add(visualMesh);

            dMeshAABB = new DMeshAABBTree3(dMesh);
            dMeshAABB.Build();
            Console.WriteLine(dMeshAABB.Bounds.Min + " | " + dMeshAABB.Bounds.Max);

            World.chunks.Add(this);
        }

        public void InitWorld()
        {
            for (int i = 0; i <= pointsPerAxis; i++)
            {
                for (int j = 0; j <= pointsPerAxis; j++)
                {
                    Vector3d visualVertex = new Vector3d(
                        (pointSpacing * i) + location.X,
                        0 + location.Y,
                        (pointSpacing * j) + location.Z);
                    
                    dMesh.AppendVertex(visualVertex);
                }
            }
            for (int i = 0; i < pointsPerAxis; i++)
            {
                for (int j = 0; j < pointsPerAxis; j++)
                {
                    int row1 = j * (pointsPerAxis + 1);
                    int row2 = (j + 1) * (pointsPerAxis + 1);

                    //tri 1
                    dMesh.AppendTriangle(row1 + i, row1 + i + 1, row2 + i + 1);
                    //tri 2
                    dMesh.AppendTriangle(row1 + i, row2 + i + 1, row2 + i);
                }
            }
            
        }
    }
}
