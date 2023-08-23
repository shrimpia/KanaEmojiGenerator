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
    private readonly List<(string name, string glyph)> definition = new();

    private int fontSize = 52;
    private int imageSize = 64;
    private Color outlineColor = Color.Black;
    private Color innerColor = Color.White;
    
    public Generator()
    {
        LoadConfig();
        LoadDefinitions();
        font = LoadFont();
        options = new TextOptions(font)
        {
            Origin = new(imageSize / 2, imageSize / 2),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center,
        };
        encoder = new PngEncoder();
    }

    public async ValueTask GenerateAllAsync()
    {
        if (!Directory.Exists($"{AppContext.BaseDirectory}output"))
        {
            Directory.CreateDirectory($"{AppContext.BaseDirectory}output");
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
        using var image = new Image<Rgba32>(imageSize, imageSize);
        var glyph = TextBuilder.GenerateGlyphs(text, options);
        image.Mutate(c => c.Draw(outlineColor, 8, glyph).Fill(innerColor, glyph));
        await image.SaveAsync($"{AppContext.BaseDirectory}output/{name}.png", encoder);
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
        await File.WriteAllTextAsync($"{AppContext.BaseDirectory}output/meta.json", json);
    }

    private void LoadConfig()
    {
        var config = File.ReadAllLines($"{AppContext.BaseDirectory}config.ini");
        foreach (var line in config)
        {
            if (line.StartsWith('#')) continue;
            var equalPosition = line.IndexOf('=');
            if (equalPosition < 0) throw new InvalidOperationException("config: syntax error");
            var key = line[..equalPosition];
            var value = line[(equalPosition + 1)..];
            switch (key)
            {
                case "fontSize":
                    fontSize = int.Parse(value);
                    break;
                case "imageSize":
                    imageSize = int.Parse(value);
                    break;
                case "outlineColor":
                {
                    var c = System.Drawing.ColorTranslator.FromHtml(value);
                    outlineColor = Color.FromRgb(c.R, c.G, c.B);
                    break;
                }
                case "innerColor":
                {
                    var c = System.Drawing.ColorTranslator.FromHtml(value);
                    innerColor = Color.FromRgb(c.R, c.G, c.B);
                    break;
                }
                default:
                    throw new InvalidOperationException($"config: invalid key \"{key}\"");
            }
        }

    }

    private void LoadDefinitions()
    {
        var defs = File.ReadAllLines($"{AppContext.BaseDirectory}definitions.csv");
        foreach (var data in defs)
        {
            var record = data.Split(',');
            definition.Add((record[0], record[1]));
        }
    }

    private Font LoadFont()
    {
        var collection = new FontCollection();
        var family = collection.Add($"{AppContext.BaseDirectory}font.ttf");
        return new Font(family, fontSize, FontStyle.Regular);
    }
}