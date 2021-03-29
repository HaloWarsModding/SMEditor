using JeremyAnsel.DirectX.DXMath;
using JeremyAnsel.DirectX.DXMath.PackedVector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pfim;

class Program
{
    static void Main(string[] args)
    {
        //AAA();

        TextureTest();

        //byte[] xtd = File.ReadAllBytes(@"C:\Program Files (x86)\Steam\steamapps\common\HaloWarsDE\StumpyHWDEMod\SMEditorTests\scenario\skirmish\design\chasms\chasms_clean.xtd");
        //for (int i = 0; i < 589824; i++)
        //{
        //    int offset = i * 4;

        //    uint uix = BitConverter.ToUInt32(xtd, 9464 + offset) >> 20;
        //    uint uiy = BitConverter.ToUInt32(xtd, 9464 + offset) >> 10;
        //    uint uiz = BitConverter.ToUInt32(xtd, 9464 + offset) >> 0;

        //    uint kBitMask10 = (1 << 10) - 1;
        //    float kBitMask10Rcp = 1.0f / kBitMask10;

        //    uint xqq = uix & kBitMask10;
        //    float fxqq = xqq * kBitMask10Rcp;
        //    uint yqq = uiy & kBitMask10;
        //    float fyqq = yqq * kBitMask10Rcp;
        //    uint zqq = uiz & kBitMask10;
        //    float fzqq = zqq * kBitMask10Rcp;

        //    //Console.WriteLine();
        //    //Console.WriteLine(vy.X);
        //    //Console.WriteLine(vy.Y);
        //    //Console.WriteLine(vy.Z);

        //    //Console.WriteLine();
        //    //Console.WriteLine(vz.X);
        //    //Console.WriteLine(vz.Y);
        //    //Console.WriteLine(vz.Z);

        //    ////float rex = -0.01349051f;

        //    ////rex += 46;
        //    ////rex /= 96.6f;
        //    ////rex /= kBitMask10Rcp;
        //    ////Console.WriteLine(x + " " + rex);

        //    float fx = 0.4760508f;
        //    float fy = 0.518084f;
        //    float fz = 0.4760508f;

        //    //MAGIC SAUCE RIGHT HERE!
        //    //Multipy, then AND with 1023, then bitshift into position.
        //    uint newi = 0;
        //    newi |= ((uint)(fxqq * 1023) & 1023) << 20;
        //    newi |= ((uint)(fyqq * 1023) & 1023) << 10;
        //    newi |= ((uint)(fzqq * 1023) & 1023) << 0;

        //    //Console.WriteLine(uiz);
        //    //Console.WriteLine(newi);

        //    xtd[9464 + offset] = BitConverter.GetBytes(newi)[0];
        //    xtd[9465 + offset] = BitConverter.GetBytes(newi)[1];
        //    xtd[9466 + offset] = BitConverter.GetBytes(newi)[2];
        //    xtd[9467 + offset] = BitConverter.GetBytes(newi)[3];

        //    if (i % 10000 == 0) Console.WriteLine(i);
        //}
        //File.WriteAllBytes((@"C:\Program Files (x86)\Steam\steamapps\common\HaloWarsDE\StumpyHWDEMod\SMEditorTests\scenario\skirmish\design\chasms\chasms.xtd"), xtd);
    }
    static void TextureTest()
    {
        int[] shifts = { 8, 4, 0, 12 };
        byte[] xtd = File.ReadAllBytes(@"C:\Program Files (x86)\Steam\steamapps\common\HaloWarsDE\StumpyHWDEMod\SMEditorTests\scenario\skirmish\design\chasms\chasms_clean.xtt");
        //byte[] dxt1 = xtd.ToList().GetRange(2047016, 1179648).ToArray();









        //xtd[3552] = 0;
        //int offset = 1650596;
        int offset = 6300;
        for(int i = 0; i < 8192; i+=2)
        {
            xtd[i + offset + 0] = 0x00;
            xtd[i + offset + 1] = 0x00;
        }
        for (int i = 8192; i < 8192*2; i += 2)
        {
            xtd[i + offset + 0] = 0x0F;
            xtd[i + offset + 1] = 0x00;
        }
        File.WriteAllBytes(@"C:\Program Files (x86)\Steam\steamapps\common\HaloWarsDE\StumpyHWDEMod\SMEditorTests\scenario\skirmish\design\chasms\chasms.xtt", xtd);
        //byte[] xtd = File.ReadAllBytes(@"C:\Program Files (x86)\Steam\steamapps\common\HaloWarsDE\StumpyHWDEMod\SMEditorTests\scenario\skirmish\design\chasms.xtt");

        //Dds dds = Dxt1Dds.Create(dxt1, new PfimConfig());
        //Console.WriteLine(dds.Height);

        //Console.ReadKey();
    }
    static void AAA()
    {
        foreach (string p in Directory.GetFiles(@"C:\Program Files (x86)\Steam\steamapps\common\HaloWarsDE\Extract\art\terrain", "*", SearchOption.AllDirectories))
        {
            File.Move(p, p + ".dds");
        }
    }
}
