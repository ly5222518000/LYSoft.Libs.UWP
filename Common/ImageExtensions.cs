namespace LYSoft.Libs.UWP;

public static partial class Common {
    public static void RenderPdfFile(this Image image) {
        var path = (string)image.GetValue(ImageService.PdfFileProperty);
        var page = (int)image.GetValue(ImageService.PdfPageProperty);
        image.RenderPdfFileAsync(path, page);
    }

    public static async void RenderPdfFileAsync(this Image image, string path, int page) {
        StorageFile file;
        try {
            file = await StorageFile.GetFileFromApplicationUriAsync(new(path));
        } catch {
            try {
                file = await StorageFile.GetFileFromPathAsync(path);
            } catch {
                return;
            }
        }
        var pdf = await PdfDocument.LoadFromFileAsync(file);
        var pagecount = (int)pdf.PageCount;
        image.SetValue(ImageService.PdfPageCountProperty, pagecount);
        if (page < 0) { page = 0; }
        if (page >= pagecount) { page = pagecount - 1; }
        var pdfpage = pdf.GetPage((uint)page);
        var stream = new InMemoryRandomAccessStream();
        await pdfpage.RenderToStreamAsync(stream, new() { DestinationWidth = 1920 });
        var src = new BitmapImage();
        image.Source = src;
        await src.SetSourceAsync(stream);
    }
}