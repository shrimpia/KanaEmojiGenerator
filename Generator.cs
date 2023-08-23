using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;

namespace KanaEmojiGenerator;

public partial class Generator
{
    private readonly Font font;
    private readonly TextOptions options;
    private readonly PngEncoder encoder;
    private readonly Color outlineColor = Color.FromRgb(185, 62, 67);
    private readonly Color innerColor = Color.White;
    
    public Generator()
    {
        font = LoadFont();
        options = new TextOptions(font)
        {
            Origin = new(32, 32),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center,
        };
        encoder = new PngEncoder();
    }

    public async ValueTask GenerateAllAsync()
    {
        if (!Directory.Exists("output"))
        {
            Directory.CreateDirectory("output");
        }

        foreach (var (name, glyph) in definition)
        {
            await GenerateFontImageAsync(glyph, name);
            Console.Write(glyph);
        }

        await GenerateMetaJsonAsync();

        Console.WriteLine("\n完了");
    }

    public async ValueTask GenerateFontImageAsync(string text, string name)
    {
        var size = TextMeasurer.Measure(text, options);
        using var image = new Image<Rgba32>(64, 64);
        var glyph = TextBuilder.GenerateGlyphs(text, options);
        image.Mutate(c => c.Draw(outlineColor, 8, glyph).Fill(innerColor, glyph));
        await image.SaveAsync($"output/{name}.png", encoder);
    }

    public async ValueTask GenerateMetaJsonAsync()
    {
        const string template = """
                                {{
                                   "metaVersion": 2,
                                   "host": "",
                                   "exportedAt": "",
                                   "emojis": [
                                     {0}
                                   ]
                                }}
                                """;
        const string template2 = """
                        {{
                            "fileName": "{0}.png",
                            "downloaded": true,
                            "emoji": {{
                                "name": "{0}",
                                "category": null,
                                "aliases": ["{1}"],
                                "license": null,
                                "localOnly": false,
                                "isSensitive": false
                            }}
                        }}
                        """;
        var emojis = string.Join(',', definition.Select(d => string.Format(template2, d.name, d.glyph)));
        var json = string.Format(template, emojis);
        await File.WriteAllTextAsync("output/meta.json", json);
    }

    private Font LoadFont()
    {
        var collection = new FontCollection();
        var family = collection.Add("./setofont.ttf");
        return new Font(family, 52, FontStyle.Regular);
    }
}