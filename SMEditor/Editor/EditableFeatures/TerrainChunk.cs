using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX;

namespace SMEditor.Editor
{
    public class TerrainChunk
    {
        TerrainMesh mesh;
        private List<TerrainVertex> vertices = new List<TerrainVertex>();
        private List<uint> indices = new List<uint>();

        public TerrainChunk(string name, float gridSize, int numXVerts, Vector3 position)
        {
            ///////////////////////////////////
            // Construct grid
            ///////////////////////////////////
            float vertSpacing = gridSize / numXVerts;
            //Generate vertices
            for (int x = 0; x < numXVerts + 1; x++)
            {
                for (int z = 0; z < numXVerts + 1; z++)
                {
                    TerrainVertex v = new TerrainVertex(new Vector3(x * vertSpacing, 0f, z * vertSpacing));
                    vertices.Add(v);
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
                    indices.Add((uint)(row2 + i + 1));
                    indices.Add((uint)(row2 + i));
                }
            }

            mesh = new TerrainMesh(vertices, indices);
        }
    }
}
