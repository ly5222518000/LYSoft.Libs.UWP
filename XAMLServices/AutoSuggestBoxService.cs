namespace LYSoft.Libs.UWP;

public class AutoSuggestBoxService {
    public static readonly DependencyProperty SuggestionTextsProperty = DependencyProperty.Register("SuggestionTexts", typeof(ICollection<string>), typeof(AutoSuggestBoxService), new(null));
    public static void SetSuggestionTexts(AutoSuggestBox element, ICollection<string> value) {
        element.SetValue(SuggestionTextsProperty, value);
        element.ItemsSource = value;
        element.SuggestionChosen += (s, e) => { s.Text = e.SelectedItem.ToString(); };
        element.TextChanged += (s, e) => {
            var text = s.Text.ToLower();
            var items = value.Where(x => x.Contains(text));
            if (!items.Any()) { items = items.Append("无建议"); }
            s.ItemsSource = items;
        };
    }
    public static ICollection<string> GetSuggestionTexts(AutoSuggestBox element) => (ICollection<string>)element.GetValue(SuggestionTextsProperty);
}