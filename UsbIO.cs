using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;

namespace SwitchUSBStreamTest
{
    internal class UsbIO
    {
        public struct InputPkg
        {
            public ulong HeldKeys;
            public int LJoyX;
            public int LJoyY;
            public int RJoyX;
            public int RJoyY;
        };

        public enum InputKeys : ulong
        {
            A     = 1,
            B     = 1 << 1,
            X     = 1 << 2,
            Y     = 1 << 3,
            LS    = 1 << 4,
            RS    = 1 << 5,
            L     = 1 << 6,
            R     = 1 << 7,
            ZL    = 1 << 8,
            ZR    = 1 << 9,
            Plus  = 1 << 10,
            Minus = 1 << 11,
            Left  = 1 << 12,
            Up    = 1 << 13,
            Right = 1 << 14,
            Down  = 1 << 15
        }

        public class UsbCtx : IDisposable
        {
            public UsbDeviceFinder Find;
            public UsbDevice Nx;
            public UsbEndpointWriter Write;
            public UsbEndpointReader Read;
            public bool isSet;

            public void Dispose()
            {
                Read.Dispose();
                Write.Dispose();
                Nx.Close();
            }
        }

        public static void SetCtx(UsbCtx ctx)
        {
            ctx.Find  = new UsbDeviceFinder(0x57e, 0x3000);
            ctx.Nx    = UsbDevice.OpenUsbDevice(ctx.Find);
            ctx.Write = ctx.Nx.OpenEndpointWriter(WriteEndpointID.Ep01);
            ctx.Read  = ctx.Nx.OpenEndpointReader(ReadEndpointID.Ep01);
            ctx.isSet = true;
        }

        public static int WriteBuf(UsbCtx ctx, byte[] buffer)
        {
            if (!ctx.isSet)
                SetCtx(ctx);

            ctx.Write.Write(buffer, 1000, out int len);

            return len;
        }

        public static InputPkg ReadPkg(UsbCtx ctx)
        {
            if (!ctx.isSet)
                SetCtx(ctx);

            var buf = new byte[24];
            var pkg = new InputPkg();

            ctx.Read.Read(buf, 1000, out int len);

            pkg.HeldKeys = BitConverter.ToUInt64(buf, 0);
            pkg.LJoyX    = BitConverter.ToInt32(buf, 8);
            pkg.LJoyY    = BitConverter.ToInt32(buf, 12);
            pkg.RJoyX    = BitConverter.ToInt32(buf, 16);
            pkg.RJoyY    = BitConverter.ToInt32(buf, 20);

            return pkg;
        }
    }
}