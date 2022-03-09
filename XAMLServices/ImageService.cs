namespace LYSoft.Libs.UWP;

public class ImageService {

    public static readonly DependencyProperty PdfFileProperty = DependencyProperty.Register("PdfFile", typeof(string), typeof(ImageService), new(null));
    public static void SetPdfFile(Image element, string value) {
        var file = GetPdfFile(element);
        if (file != value) {
            element.SetValue(PdfFileProperty, value);
            SetPdfPage(element, 0);
            element.RenderPdfFile();
        }
    }
    public static string GetPdfFile(Image element) => (string)element.GetValue(PdfFileProperty);

    public static readonly DependencyProperty PdfPageProperty = DependencyProperty.Register("PdfPage", typeof(int), typeof(ImageService), new(0));
    public static void SetPdfPage(Image element, int value) {
        element.SetValue(PdfPageProperty, value);
        element.RenderPdfFile();
    }
    public static int GetPdfPage(Image element) => (int)element.GetValue(PdfPageProperty);

    public static readonly DependencyProperty PdfPageCountProperty = DependencyProperty.Register("PdfPageCount", typeof(int), typeof(ImageService), new(0));
    public static void SetPdfPageCount(Image element, int value) { }
    public static int GetPdfPageCount(Image element) => (int)element.GetValue(PdfPageCountProperty);

}