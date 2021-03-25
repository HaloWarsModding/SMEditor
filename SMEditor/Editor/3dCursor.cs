using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX;
using g3;

namespace SMEditor.Editor
{
    public class _3dCursor
    {
        public enum Mode { YAligned, NonAligned }
        public Mode mode = Mode.YAligned;
        BasicMesh cursor;
        public Transform t = new Transform();
        public bool hitInfoExists;

        public _3dCursor()
        {
            cursor = new BasicMesh();
            cursor.Init(new List<BasicVertex>() {
new BasicVertex(new Vector3(0.000000F, 0.000000F, 0.000000F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(0.000000F, 0.000000F, 0.000000F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(0.000000F, 1.993729F, -1.000000F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(0.781832F, 1.993729F, -0.623490F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(0.974928F, 1.993729F, 0.222521F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(0.433884F, 1.993729F, 0.900969F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(-0.433884F, 1.993729F, 0.900969F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(-0.974928F, 1.993729F, 0.222521F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(-0.781832F, 1.993729F, -0.623490F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(-0.232554F, 2.165461F, 0.053079F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(-0.186494F, 2.165461F, -0.148724F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(-0.103496F, 2.165461F, 0.214912F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(0.103496F, 2.165461F, 0.214912F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(0.142345F, 5.531382F, 0.295583F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(-0.142345F, 5.531382F, 0.295583F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(0.186494F, 2.165461F, -0.148724F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(0.232554F, 2.165461F, 0.053079F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(0.000000F, 2.165461F, -0.238535F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(0.256496F, 5.531382F, -0.204548F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(0.000000F, 5.531382F, -0.328070F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(-0.256496F, 5.531382F, -0.204548F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(-0.319846F, 5.531382F, 0.073004F), new Vector3(0, 0, 0), new Vector3(0,0,0)),
new BasicVertex(new Vector3(0.319846F, 5.531382F, 0.073004F), new Vector3(0, 0, 0), new Vector3(0,0,0))
            }, new List<int>()
            {
1, 2, 3,
1, 3, 4,
1, 4, 5,
1, 5, 6,
1, 6, 7,
7, 10, 8,
1, 7, 8,
1, 8, 2,
11, 13, 14,
5, 11, 6,
4, 15, 16,
3, 17, 15,
8, 17, 2,
6, 9, 7,
5, 16, 12,
21, 14, 13,
15, 22, 16,
10, 19, 17,
9, 14, 21,
16, 13, 12,
15, 19, 18,
9, 20, 10,
7, 9, 10,
11, 12, 13,
5, 12, 11,
4, 3, 15,
3, 2, 17,
8, 10, 17,
6, 11, 9,
5, 4, 16,
22, 18, 13,
18, 19, 13,
19, 20, 13,
20, 21, 13,
15, 18, 22,
10, 20, 19,
9, 11, 14,
16, 22, 13,
15, 17, 19,
9, 21, 20,
            });
        }
        public void UpdatePositionOnTerrain()
        {
            hitInfoExists = false;

            var vx = (2 * Input.mouseAbsPosNormalized.X) / Camera.cbData.projMatrix.M11;
            var vy = (-2 * Input.mouseAbsPosNormalized.Y) / Camera.cbData.projMatrix.M22;

            var wInv = Matrix.Invert(Camera.cbData.viewMatrix);
            Vector3d pos = Convert.ToV3d(Vector3.TransformCoordinate(new Vector3(0, 0, 0), wInv));
            Vector3d dir = Convert.ToV3d(Vector3.TransformNormal(new Vector3(vx, vy, 1.0F), wInv));

            Ray3d ray = new Ray3d(pos, dir);
            t.position = Convert.ToV3(Editor.scenario.terrain.GetHitLocationFromRay(ray));
        }
        public void Draw()
        {
            Renderer.passes["cursor"].Use();

            Renderer.mainCamera.SetModelMatrix(Matrix.Transformation(new Vector3(0, 0, 0),
            Quaternion.Identity, new Vector3(.25F, .25F, .25F), new Vector3(0, 0, 0), Quaternion.Identity, t.position));

            cursor.Draw();
        }
    }
}
