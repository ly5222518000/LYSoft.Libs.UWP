namespace LYSoft.Libs.UWP;

public class ImageService {

    public static readonly DependencyProperty FileProperty = DependencyProperty.Register("File", typeof(string), typeof(ImageService), new(null));
    public static void SetFile(Image element, string value) {
        var file = GetFile(element);
        if (file != value) {
            element.SetValue(FileProperty, value);
            SetPage(element, 0);
            element.RenderFile();
        }
    }
    public static string GetFile(Image element) => (string)element.GetValue(FileProperty);

    public static readonly DependencyProperty PageProperty = DependencyProperty.Register("Page", typeof(int), typeof(ImageService), new(0));
    public static void SetPage(Image element, int value) {
        var page = GetPage(element);
        if (page != value) { 
            element.SetValue(PageProperty, value);
            element.RenderFile();
        }
    }
    public static int GetPage(Image element) => (int)element.GetValue(PageProperty);

    public static readonly DependencyProperty PageCountProperty = DependencyProperty.Register("PageCount", typeof(int), typeof(ImageService), new(0));
    public static void SetPageCount(Image element, int value) { }
    public static int GetPageCount(Image element) => (int)element.GetValue(PageCountProperty);

    public static readonly DependencyProperty FileFormatProperty = DependencyProperty.Register("FileFormat", typeof(ImageFileFormat), typeof(ImageService), new(ImageFileFormat.None));
    public static void SetFileFormat(Image element, ImageFileFormat value) { element.SetValue(FileFormatProperty, value); }
    public static ImageFileFormat GetFileFormat(Image element) => (ImageFileFormat)element.GetValue(FileFormatProperty);

}

public enum ImageFileFormat {
    None,
    PDF,
    PPT,
    TIFF,
}