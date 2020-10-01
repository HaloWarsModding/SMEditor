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

        public enum Size { _256x256, _512x512, _1024x1024, _2048x2048};
        public enum SimResolution { _4x }

        public Size size;
        public SimResolution simRes;
    }

    public class HWDEScenario
    {
        TerrainChunk[,] chunks;
        public HWDEScenario(HWDEScenarioDescription desc)
        {
            int chunksPerAxis = 0;
            switch(desc.size)
            {
                case HWDEScenarioDescription.Size._256x256: {
                        chunksPerAxis = 4;
                        break;  }
                case HWDEScenarioDescription.Size._512x512: {
                        chunksPerAxis = 8;
                        break; }
                case HWDEScenarioDescription.Size._1024x1024: {
                        chunksPerAxis = 16;
                        break; }
                case HWDEScenarioDescription.Size._2048x2048: {
                        chunksPerAxis = 32;
                        break; }
            }
            
            for(int i = 0; i < chunksPerAxis; i++)
            {
                for (int j = 0; j < chunksPerAxis; j++)
                {
                    string s = "(" + i + " ," + j + ")";
                    Console.WriteLine(i + " | " + j);
                    chunks[i, j] = new TerrainChunk(s, 64, 64, new Vector3(64 * i, 0 , 64*j));
                }
            }
        }
    }
}
