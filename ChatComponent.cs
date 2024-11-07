using System.Web;

namespace ServerOverflow; 

/// <summary>
/// A Minecraft chat component
/// </summary>
public class ChatComponent {
    public List<ChatComponent>? Extra { get; set; }
    public bool Strikethrough { get; set; }
    public bool Underlined { get; set; }
    public bool Obfuscated { get; set; }
    public string? Color { get; set; }
    public string? Text { get; set; }
    public bool Italic { get; set; }
    public bool Bold { get; set; }
    
    /// <summary>
    /// Color code to CSS color dictionary
    /// </summary>
    private static readonly Dictionary<string, string> _mapping = new() {
        { "black", "#000000" },
        { "dark_blue", "#0000AA" },
        { "dark_green", "#00AA00" },
        { "dark_aqua", "#00AAAA" },
        { "dark_red", "#AA0000" },
        { "dark_purple", "#AA00AA" },
        { "gold", "#FFAA00" },
        { "gray", "#AAAAAA" },
        { "dark_gray", "#555555" },
        { "blue", "#5555FF" },
        { "green", "#55FF55" },
        { "aqua", "#55FFFF" },
        { "red", "#FF5555" },
        { "light_purple", "#FF55FF" },
        { "yellow", "#FFFF55" },
        { "white", "#FFFFFF" },
        { "reset", "#FFFFFF" }
    };

    /// <summary>
    /// Converts this chat components to HTML
    /// </summary>
    /// <param name="clean">Strip out decorations</param>
    /// <returns>Converted HTML</returns>
    public string ToHtml(bool clean = false) {
        if (Extra == null) return GetComponentHtml(clean);
        return Extra.Aggregate(GetComponentHtml(clean), 
            (str, i) => str + i.ToHtml());
    }

    /// <summary>
    /// Converts this chat component to HTML
    /// </summary>
    /// <param name="clean">Strip out decorations</param>
    /// <returns>Converted HTML</returns>
    private string GetComponentHtml(bool clean) {
        Text ??= ""; Color ??= "#FFFFFF";
        if (Text.Contains('§') || Text.Contains('&'))
            return ColorEncoding.ToHtml(Text, clean);
        var enc = HttpUtility.HtmlEncode(Text);
        if (clean) return $"<a>{enc}</a>"; var code = "#FFFFFF";
        if (_mapping.TryGetValue(Color, out var value)) code = value;
        var output = $"<a style=\"color: {code};\">{enc}</a>";
        if (Obfuscated) output = $"<obf>{output}</obf>";
        if (Strikethrough) output = $"<s>{output}</s>";
        if (Underlined) output = $"<u>{output}</u>";
        if (Italic) output = $"<em>{output}</em>";
        if (Bold) output = $"<b>{output}</b>";
        return output;
    }
}