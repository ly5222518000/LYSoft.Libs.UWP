namespace LYSoft.Libs.UWP;

public class RichEditBoxService {

    public static readonly DependencyProperty RtfFileProperty = DependencyProperty.Register("RtfFile", typeof(string), typeof(RichEditBoxService), new(null));
    public static void SetRtfFile(RichEditBox element, string value) { 
        element.SetValue(RtfFileProperty, value);
        if (string.IsNullOrWhiteSpace(value)) { return; }
        StorageFile file = null;
        try { file = Common.RunSync(() => StorageFile.GetFileFromPathAsync(value).AsTask()); } catch { }
        if (file == null) {
            try { file = Common.RunSync(() => StorageFile.GetFileFromApplicationUriAsync(new(value)).AsTask()); } catch { }
        }
        try {
            var stream = Common.RunSync(() => file.OpenAsync(FileAccessMode.Read).AsTask());
            element.Document.LoadFromStream(TextSetOptions.FormatRtf, stream); 
        } catch { }
    }
    public static string GetRtfFile(RichEditBox element) => (string)element.GetValue(RtfFileProperty);

}