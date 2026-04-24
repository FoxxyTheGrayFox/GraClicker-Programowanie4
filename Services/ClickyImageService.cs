using System.IO;
using System.Windows.Media.Imaging;

namespace ProjektClicker
{
    public class ClickyImageService
    {
        private static readonly Dictionary<ClickyStyle, string> StyleToFile = new()
        {
            { ClickyStyle.Normal,  "clicky.png" },
            { ClickyStyle.Hat,     "clickyhat.png" },
            { ClickyStyle.Monocle, "clickymonocle.png" }
        };

        public BitmapImage? GetImage(ClickyUpgrade clicky)
        {
            if (!clicky.Owned) return null;

            var file = StyleToFile.GetValueOrDefault(clicky.Style, "clicky.png");
            if (!File.Exists(file)) return null;

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(Path.GetFullPath(file));
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            image.EndInit();
            image.Freeze();
            return image;
        }
    }
}