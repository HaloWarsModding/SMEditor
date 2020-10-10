﻿using System;
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

        public int size;

        public DMesh3 dMesh;
        public DMeshAABBTree3 dMeshAABB;

        TerrainMesh visualMesh;

        public List<BasicVertex> vertices;
        public List<int> inds = new List<int>();
        public List<bool> vertexNeedsUpdate;


        public Terrain(int _size)
        {
            size = _size;

            dMesh = new DMesh3(MeshComponents.VertexColors);

            vertices = new List<BasicVertex>();
            vertexNeedsUpdate = new List<bool>();
            for (int i = 0; i <= size ; i++)
            {
                for (int j = 0; j <= size ; j++)
                {
                    Vector3d v = new Vector3d((float)i, 0, (float)j);
                    vertices.Add(new BasicVertex(Convert.ToV3(v), new Vector3(0, 0, 0)));
                    dMesh.AppendVertex(new NewVertexInfo(v, new Vector3f(0, 0, 0)));
                    vertexNeedsUpdate.Add(false);
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

                    inds.Add(row1 + i);
                    inds.Add(row1 + i + 1);
                    inds.Add(row2 + i + 1);

                    inds.Add(row1 + i);
                    inds.Add(row2 + i + 1);
                    inds.Add(row2 + i);
                }
            }
            
            dMeshAABB = new DMeshAABBTree3(dMesh);
            dMeshAABB.Build();
            Console.WriteLine(dMeshAABB.Bounds.Min + " | " + dMeshAABB.Bounds.Max);

            visualMesh = new TerrainMesh();
            visualMesh.Init(vertices, inds);
            Renderer.terrainMeshes.Add(visualMesh);
        }
        
        public void SetVertexPosition(int vID, Vector3d newPos)
        {
            vertices[vID] = new BasicVertex(vertices[vID].position + Convert.ToV3(newPos), vertices[vID].color);
            vertexNeedsUpdate[vID] = true;
        }


        public void UpdateCollisionModel()
        {
            for(int i = 0; i < vertices.Count; i++)
            {
                if(vertexNeedsUpdate[i])
                {
                    dMesh.SetVertex(i, Convert.ToV3d(vertices[i].position));
                    vertexNeedsUpdate[i] = false;
                }
            }
            dMeshAABB.Build();
        }
        public void UpdateVisual()
        {
            visualMesh.UpdateVertexData(vertices);
        }

        public List<int> GetVertsInRadius(int vID, double rad)
        {
            List<int> verts = new List<int>();

            int halfRad = (int)Math.Round(rad / 2F);
            Console.WriteLine(halfRad);

            for (int x = 0; x < halfRad * 2; x++) 
            {
                for (int y = 0; y < halfRad * 2; y++)
                {
                    verts.Add(vID + x + ((size + 1) * y));
                    verts.Add(vID - x + ((size + 1) * y));

                    verts.Add(vID + x + ((size + 1) * -y));
                    verts.Add(vID - x + ((size + 1) * -y));
                }
            }

            return verts.Distinct().ToList();
        }
    }
}
