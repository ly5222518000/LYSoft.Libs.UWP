namespace LYSoft.Libs.UWP;

public static partial class Common {
    public static void RenderFile(this Image image) {
        var page = ImageService.GetPage(image);
        var path = ImageService.GetFile(image);
        var format = ImageService.GetFileFormat(image);
        switch (format) {
            case ImageFileFormat.None:
                break;
            case ImageFileFormat.PDF:
                image.RenderPdfFileAsync(path, page);
                break;
            case ImageFileFormat.PPT:
                image.RenderPptFileAsync(path, page);
                break;
            case ImageFileFormat.TIFF:
                break;
            default:
                break;
        }
    }

    public static async void RenderPptFileAsync(this Image image, string path, int page) {
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
        image.SetValue(ImageService.PageCountProperty, pagecount);
        if (page < 0) { page = 0; }
        if (page >= pagecount) { page = pagecount - 1; }
        var pdfpage = pdf.GetPage((uint)page);
        var stream = new InMemoryRandomAccessStream();
        await pdfpage.RenderToStreamAsync(stream, new() { DestinationWidth = 1920 });
        var src = new BitmapImage();
        image.Source = src;
        await src.SetSourceAsync(stream);
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
        image.SetValue(ImageService.PageCountProperty, pagecount);
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