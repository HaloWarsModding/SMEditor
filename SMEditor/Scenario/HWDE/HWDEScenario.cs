using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX;
using SMEditor;
using SMEditor.Editor;

namespace SMEditor.Scenario.HWDE
{
    public struct HWDEScenarioDescription
    {
        public HWDEScenarioDescription(Size s, SimResolution sr)
        {
            size = s;
            simRes = sr;
        }

        public enum Size { _256x256, _512x512, _1024x1024, _2048x2048 };
        public enum SimResolution { _4x }

        public Size size;
        public SimResolution simRes;
    }
    public class HWDEScenario
    {
        private TerrainChunk[,] terrainChunks;
        public HWDEScenario(HWDEScenarioDescription desc)
        {
            int chunksPerAxis;
            if (desc.size == HWDEScenarioDescription.Size._256x256) chunksPerAxis = 4;
            else if (desc.size == HWDEScenarioDescription.Size._512x512) chunksPerAxis = 8;
            else if (desc.size == HWDEScenarioDescription.Size._1024x1024) chunksPerAxis = 16;
            else if (desc.size == HWDEScenarioDescription.Size._2048x2048) chunksPerAxis = 32;
            else return;

            terrainChunks = new TerrainChunk[chunksPerAxis, chunksPerAxis];
            for(int i = 0; i < chunksPerAxis; i++)
            {
                for(int j = 0; j < chunksPerAxis; j++)
                {
                    terrainChunks[i, j] = new TerrainChunk(64, 64, new Vector3(100,300,100), new Vector3(i*64, 0, j*64));
                }
            }

        }
    }
}
