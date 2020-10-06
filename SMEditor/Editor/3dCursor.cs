using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX;

namespace SMEditor.Editor
{
    class _3dCursor
    {
        BasicMesh cursor;

        public _3dCursor()
        {
            cursor = new BasicMesh();
            cursor.Init(new List<BasicVertex>() {
new BasicVertex(new Vector3( -0.000000F, 0.000000F, 0.000000F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( 0.000000F, 1.993729F ,-1.000000F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( 0.781832F, 1.993729F, -0.623490F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( 0.974928F, 1.993729F, 0.222521F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( 0.433884F, 1.993729F, 0.900969F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( -0.433884F ,1.993729F, 0.900969F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( -0.974928F, 1.993729F, 0.222521F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( -0.781832F ,1.993729F ,-0.623490F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( 0.186494F ,2.165461F,-0.148724F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( -0.000000F, 2.165461F ,-0.238535F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( 0.232554F, 2.165461F ,0.053079F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( 0.103496F, 2.165461F, 0.214912F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( -0.103496F ,2.165461F, 0.214912F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( -0.232554F, 2.165461F, 0.053079F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( -0.186494F, 2.165461F ,-0.148724F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( 0.256496F, 5.531382F, -0.204549F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( -0.000000F, 5.531382F, -0.328071F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( 0.319846F, 5.531382F, 0.073003F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( 0.142345F, 5.531382F, 0.295582F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( -0.142345F, 5.531382F, 0.295582F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( -0.319846F, 5.531382F, 0.073003F), new Vector3(0,0,0)),
new BasicVertex(new Vector3( -0.256496F, 5.531382F, -0.204549F), new Vector3(0,0,0))
            }, new List<int>()
            {
1,1,1,2,2,1,3,3,1,
1,4,2,3,3,2,4,5,2,
1,6,3,4,5,3,5,7,3,
1,8,4,5,7,4,6,9,4,
1,10,5,6,9,5,7,11,5,
8,12,6,7,11,6,14,13,6,15,14,6,
1,15,7,7,11,7,8,12,7,
1,16,8,8,12,8,2,17,8,
13,18,9,12,19,9,19,20,9,20,21,9,
6,9,10,5,7,10,12,19,10,13,18,10,
4,5,11,3,3,11,9,22,11,11,23,11,
3,3,12,2,2,12,10,24,12,9,22,12,
2,17,13,8,12,13,15,14,13,10,25,13,
7,11,14,6,9,14,13,18,14,14,13,14,
5,7,15,4,5,15,11,23,15,12,19,15,
16,26,16,17,27,16,22,28,16,21,29,16,20,30,16,19,31,16,18,32,16,
11,23,17,9,22,17,16,33,17,18,34,17,
10,25,18,15,14,18,22,35,18,17,36,18,
14,13,19,13,18,19,20,21,19,21,37,19,
12,19,20,11,23,20,18,34,20,19,20,20,
9,22,21,10,24,21,17,38,21,16,33,21,
15,14,22,14,13,22,21,37,22,22,35,22
            });
        }
    }
}
