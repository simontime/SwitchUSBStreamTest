using System.IO;
using System.Linq;
using static SwitchUSBStreamTest.UsbIO;

namespace SwitchUSBStreamTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var Context = new UsbCtx())
            {
                string LastTarg = "Images/A.dds", TargFile;
                ulong Key = 1;
                while (true)
                {
                    if (File.Exists(TargFile = $"Images/{(InputKeys)Key}.dds"))
                    {
                        WriteBuf(Context, File.ReadAllBytes(TargFile).Skip(0x80).ToArray());
                        LastTarg = TargFile;
                    }
                    else WriteBuf(Context, File.ReadAllBytes(LastTarg).Skip(0x80).ToArray());

                    var Pkg = ReadPkg(Context);

                    if (Pkg.HeldKeys != 0) Key = Pkg.HeldKeys;
                }
            }
        }
    }
}