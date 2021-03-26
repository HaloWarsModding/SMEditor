using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMEditor.Editor
{
    public enum HWDEScenarioSize { small512, small768, medium1024, medium1536, large2048 }
    public class HWDEScenarioData
    {
        private HWDEScenarioSize size;
        private int numXChunks = 0;

        public Terrain terrain;

        public HWDEScenarioData(HWDEScenarioSize _size)
        {
            size = _size;
            switch(_size)
            {
                case HWDEScenarioSize.small512:
                    terrain = new Terrain(8);
                    numXChunks = 8;
                    break;
                case HWDEScenarioSize.small768:
                    terrain = new Terrain(12);
                    numXChunks = 12;
                    break;
                case HWDEScenarioSize.medium1024:
                    terrain = new Terrain(16);
                    numXChunks = 16;
                    break;
                case HWDEScenarioSize.medium1536:
                    terrain = new Terrain(24);
                    numXChunks = 24;
                    break;
                case HWDEScenarioSize.large2048:
                    terrain = new Terrain(32);
                    numXChunks = 32;
                    break;
            }
        }

        public void Dispose()
        {
            terrain.Dispose();
        }
    }
}
