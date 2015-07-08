using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace MC2UE
{
    class FastPixelEditor : IDisposable
    {
        Bitmap bitmap;
        Rectangle source;
        BitmapData bitmapData;
        IntPtr nativePixelPointer;
        const int bytesPerPixel = 4 * sizeof(byte);

        public FastPixelEditor(Bitmap bitmap) : this(bitmap, new Rectangle(Point.Empty, bitmap.Size)) { }

        public FastPixelEditor(Bitmap bitmap, Rectangle source)
        {
            this.bitmap = bitmap;
            this.source = source;
            bitmapData = this.bitmap.LockBits(source, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            nativePixelPointer = bitmapData.Scan0;
        }

        private int GetOffset(int x, int y) { return y * Math.Abs(bitmapData.Stride) + x * bytesPerPixel; }

        public void SetPixel(int x, int y, Color color) { Marshal.WriteInt32(nativePixelPointer, GetOffset(x, y), color.ToArgb()); }

        public Color GetPixel(int x, int y) { return Color.FromArgb(Marshal.ReadInt32(nativePixelPointer, GetOffset(x, y))); }

        public void Dispose() { bitmap.UnlockBits(bitmapData); }
    }
}
