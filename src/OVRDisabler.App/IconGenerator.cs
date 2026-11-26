using System.IO;
using System.Windows.Media.Imaging;

namespace OVRDisabler.App;

public static class IconGenerator
{
    public static void EnsureIcon(string iconPath)
    {
        if (File.Exists(iconPath)) return;
        var dir = Path.GetDirectoryName(iconPath);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

        // Basit 32x32 ICO dosyası oluştur
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        // ICONDIR
        bw.Write((short)0); // reserved
        bw.Write((short)1); // icon type
        bw.Write((short)1); // count = 1
        // ICONDIRENTRY
        bw.Write((byte)32); // width
        bw.Write((byte)32); // height
        bw.Write((byte)0);  // color count
        bw.Write((byte)0);  // reserved
        bw.Write((short)1); // color planes
        bw.Write((short)32); // bpp
        int bmpSize = 40 + (32 * 32 * 4) + (32 * 4); // header + xor + and mask
        bw.Write(bmpSize);
        bw.Write(22); // offset

        // BITMAPINFOHEADER
        bw.Write(40); // header size
        bw.Write(32); // width
        bw.Write(64); // height (double for icon)
        bw.Write((short)1); // planes
        bw.Write((short)32); // bit count
        bw.Write(0); // compression
        bw.Write(32 * 32 * 4); // image size
        bw.Write(0); // x pixels per meter
        bw.Write(0); // y pixels per meter
        bw.Write(0); // colors used
        bw.Write(0); // colors important

        // Pixel data (Teal color #00E5CC)
        for (int i = 0; i < 32 * 32; i++)
        {
            bw.Write((byte)0xCC); // B
            bw.Write((byte)0xE5); // G
            bw.Write((byte)0x00); // R
            bw.Write((byte)0xFF); // A
        }

        // AND mask (all zeros for transparent)
        for (int i = 0; i < 32 * 4; i++)
        {
            bw.Write((byte)0);
        }

        File.WriteAllBytes(iconPath, ms.ToArray());
    }
}
