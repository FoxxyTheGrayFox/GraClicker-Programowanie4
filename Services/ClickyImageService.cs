using System.IO;
using System.Windows.Media.Imaging;
/*TODO: pls help czemu alpha nie działa?*/
namespace ProjektClicker
{
    public class ClickyImageService
    {
        public static BitmapImage? GetImage(ClickyUpgrade clicky)
        {
            if (!clicky.Owned) // tylko jeśli clicky jest kupiony
                return null;
                
            string file = clicky.Style switch
            {
                ClickyStyle.Normal => "clicky.png",
                ClickyStyle.Hat => "clickyhat.png",
                ClickyStyle.Monocle => "clickymonocle.png",
                _ => "clicky.png",
            };
            if (!File.Exists(file))
                return null;

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