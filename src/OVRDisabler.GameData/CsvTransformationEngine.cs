using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using OVRDisabler.Core.Models;
using OVRDisabler.Core.Utilities;

namespace OVRDisabler.GameData;

public interface ICsvTransformationEngine
{
    CsvAssetType IdentifyAssetType(string csvContent);
    CsvAnalysisResult Analyze(string filePath, DisablerOptions options);
    CsvTransformationResult Transform(string originalCsvContent, CsvAssetType assetType, DisablerOptions options);
}

public class CsvTransformationEngine : ICsvTransformationEngine
{
    private readonly ILogger<CsvTransformationEngine>? _logger;

    public CsvTransformationEngine(ILogger<CsvTransformationEngine>? logger = null)
    {
        _logger = logger;
    }

    public CsvAssetType IdentifyAssetType(string csvContent)
    {
        if (string.IsNullOrWhiteSpace(csvContent)) return CsvAssetType.Unknown;

        // Form & Morale tespiti
        if (csvContent.Contains("Very Good", StringComparison.OrdinalIgnoreCase) &&
            csvContent.Contains("Very Happy", StringComparison.OrdinalIgnoreCase) &&
            csvContent.Contains("Skill Weight", StringComparison.OrdinalIgnoreCase))
        {
            return CsvAssetType.FormMoraleBoosts;
        }

        // Match Fitness tespiti
        if (csvContent.Contains("Decent", StringComparison.OrdinalIgnoreCase) &&
            csvContent.Contains("Sharp", StringComparison.OrdinalIgnoreCase) &&
            csvContent.Contains("Acceleration", StringComparison.OrdinalIgnoreCase) &&
            csvContent.Contains("Sprint_Speed", StringComparison.OrdinalIgnoreCase))
        {
            return CsvAssetType.MatchFitnessData;
        }

        return CsvAssetType.Unknown;
    }

    public CsvAnalysisResult Analyze(string filePath, DisablerOptions options)
    {
        if (!File.Exists(filePath))
        {
            return new CsvAnalysisResult(
                FileName: Path.GetFileName(filePath),
                FileSizeBytes: 0,
                AssetType: CsvAssetType.Unknown,
                Sha256Hash: string.Empty,
                IsSchemaValid: false,
                SchemaValidationMessage: "Dosya bulunamadı.",
                TotalValuesChanged: 0,
                PreviewChanges: Array.Empty<CsvValueChange>()
            );
        }

        var fileInfo = new FileInfo(filePath);
        string content = File.ReadAllText(filePath, DetectEncoding(filePath));
        string hash = HashUtility.ComputeSha256(filePath);
        var assetType = IdentifyAssetType(content);

        if (assetType == CsvAssetType.Unknown)
        {
            return new CsvAnalysisResult(
                FileName: fileInfo.Name,
                FileSizeBytes: fileInfo.Length,
                AssetType: CsvAssetType.Unknown,
                Sha256Hash: hash,
                IsSchemaValid: false,
                SchemaValidationMessage: "Bilinmeyen veya desteklenmeyen CSV şeması.",
                TotalValuesChanged: 0,
                PreviewChanges: Array.Empty<CsvValueChange>()
            );
        }

        var result = Transform(content, assetType, options);

        return new CsvAnalysisResult(
            FileName: fileInfo.Name,
            FileSizeBytes: fileInfo.Length,
            AssetType: assetType,
            Sha256Hash: hash,
            IsSchemaValid: result.Success,
            SchemaValidationMessage: result.Success ? "Şema geçerli ve uyumlu." : result.Message,
            TotalValuesChanged: result.TotalValuesChanged,
            PreviewChanges: result.Changes
        );
    }

    public CsvTransformationResult Transform(string originalCsvContent, CsvAssetType assetType, DisablerOptions options)
    {
        if (string.IsNullOrWhiteSpace(originalCsvContent))
        {
            return new CsvTransformationResult(false, null, null, null, 0, Array.Empty<CsvValueChange>(), "Boş CSV içeriği.");
        }

        string originalHash = HashUtility.ComputeSha256(originalCsvContent, Encoding.UTF8);

        return assetType switch
        {
            CsvAssetType.FormMoraleBoosts => TransformFormMorale(originalCsvContent, options, originalHash),
            CsvAssetType.MatchFitnessData => TransformMatchFitness(originalCsvContent, options, originalHash),
            _ => new CsvTransformationResult(false, null, originalHash, null, 0, Array.Empty<CsvValueChange>(), "Desteklenmeyen CSV şeması.")
        };
    }

    private CsvTransformationResult TransformFormMorale(string content, DisablerOptions options, string originalHash)
    {
        var lineEnding = DetectLineEnding(content);
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var changes = new List<CsvValueChange>();
        var outputLines = new List<string>();

        string currentSection = "Unknown";
        // Form kategorileri: Very Good, Good, Neutral, Bad, Very Bad
        var formCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Very Good", "Good", "Neutral", "Bad", "Very Bad"
        };

        // Moral kategorileri: Complacent, Very Happy, Happy, Content, Sad, Very Sad
        var moraleCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Complacent", "Very Happy", "Happy", "Content", "Sad", "Very Sad"
        };

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#") || line.TrimStart().StartsWith("//"))
            {
                outputLines.Add(line);
                continue;
            }

            // Bölüm başlığı tespiti (örn: [Form], [Morale] veya sütun başlıkları)
            if (line.Contains("Form", StringComparison.OrdinalIgnoreCase) && line.Contains("Threshold", StringComparison.OrdinalIgnoreCase))
            {
                currentSection = "Form";
                outputLines.Add(line);
                continue;
            }
            if (line.Contains("Morale", StringComparison.OrdinalIgnoreCase) && line.Contains("Threshold", StringComparison.OrdinalIgnoreCase))
            {
                currentSection = "Morale";
                outputLines.Add(line);
                continue;
            }

            // Ağırlık satırları korunmalıdır! (Skill Weight, Physical Weight, Mental Weight)
            if (line.Contains("Weight", StringComparison.OrdinalIgnoreCase))
            {
                outputLines.Add(line);
                continue;
            }

            // CSV satırını ayrıştır
            var fields = ParseCsvLine(line);
            if (fields.Count > 0)
            {
                string firstCol = fields[0].Trim();
                bool isFormRow = formCategories.Contains(firstCol);
                bool isMoraleRow = moraleCategories.Contains(firstCol);

                if (isFormRow) currentSection = "Form";
                else if (isMoraleRow) currentSection = "Morale";

                bool shouldModify = (currentSection == "Form" && isFormRow && options.DisableForm) ||
                                     (currentSection == "Morale" && isMoraleRow && options.DisableMorale);

                if (shouldModify)
                {
                    // Format: Category, Threshold, SkillMod, PhysicalMod, MentalMod...
                    // İlk 2 sütun (Category, Threshold) korunur, kalan sayısal modifiye değerleri 0 yapılır!
                    for (int i = 2; i < fields.Count; i++)
                    {
                        string val = fields[i].Trim();
                        if (double.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double numVal))
                        {
                            if (numVal != 0)
                            {
                                changes.Add(new CsvValueChange(currentSection, firstCol, $"Col_{i}", val, "0"));
                                fields[i] = "0";
                            }
                        }
                    }
                    outputLines.Add(string.Join(",", fields));
                    continue;
                }
            }

            outputLines.Add(line);
        }

        string resultText = string.Join(lineEnding, outputLines);
        string newHash = HashUtility.ComputeSha256(resultText, Encoding.UTF8);

        return new CsvTransformationResult(
            Success: true,
            TransformedCsvContent: resultText,
            OriginalSha256: originalHash,
            TransformedSha256: newHash,
            TotalValuesChanged: changes.Count,
            Changes: changes,
            Message: $"Form/Moral dönüşümü tamamlandı. Toplam {changes.Count} değer sıfırlandı."
        );
    }

    private CsvTransformationResult TransformMatchFitness(string content, DisablerOptions options, string originalHash)
    {
        var lineEnding = DetectLineEnding(content);
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var changes = new List<CsvValueChange>();
        var outputLines = new List<string>();

        // Bilinen nitelikler (Attribute rows)
        var knownAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Acceleration", "Agility", "Aggression", "Balance", "Composure",
            "Interceptions", "Jumping", "Positioning", "Reactions", "Sprint_Speed",
            "Stamina", "Strength", "Vision"
        };

        // Bilinen fitness kategorileri / bölümleri
        // Abysmal, Unfit, Off-Par, Decent, Sharp
        // Korunacak bölümler: Injury (Yellow Injury: -30, Orange/Red: -80 vb.) ve Thresholds
        string currentCategory = "General";

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#") || line.TrimStart().StartsWith("//"))
            {
                outputLines.Add(line);
                continue;
            }

            // Kategori veya eşik başlık kontrolü
            if (line.Contains("Abysmal", StringComparison.OrdinalIgnoreCase)) currentCategory = "Abysmal";
            else if (line.Contains("Unfit", StringComparison.OrdinalIgnoreCase)) currentCategory = "Unfit";
            else if (line.Contains("Off-Par", StringComparison.OrdinalIgnoreCase)) currentCategory = "Off-Par";
            else if (line.Contains("Decent", StringComparison.OrdinalIgnoreCase)) currentCategory = "Decent";
            else if (line.Contains("Sharp", StringComparison.OrdinalIgnoreCase)) currentCategory = "Sharp";
            else if (line.Contains("Injury", StringComparison.OrdinalIgnoreCase)) currentCategory = "Injury";

            // Eğer eşik tanımları satırıysa veya Injury bölümüyse değiştirilmez!
            if (currentCategory.Equals("Injury", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("Threshold", StringComparison.OrdinalIgnoreCase))
            {
                outputLines.Add(line);
                continue;
            }

            var fields = ParseCsvLine(line);
            if (fields.Count > 1)
            {
                string firstCol = fields[0].Trim();
                if (knownAttributes.Contains(firstCol) && options.DisableMatchFitness)
                {
                    // Bu bir nitelik satırıdır. Konum sütunlarındaki (GK, CB, LB, RB, CDM, CM, CAM, LM, RM, LW, RW, ST)
                    // sayısal modifikasyon değerleri sıfırlanmalıdır.
                    for (int i = 1; i < fields.Count; i++)
                    {
                        string val = fields[i].Trim();
                        if (double.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double numVal))
                        {
                            if (numVal != 0)
                            {
                                changes.Add(new CsvValueChange(currentCategory, firstCol, $"Pos_{i}", val, "0"));
                                fields[i] = "0";
                            }
                        }
                    }
                    outputLines.Add(string.Join(",", fields));
                    continue;
                }
            }

            outputLines.Add(line);
        }

        string resultText = string.Join(lineEnding, outputLines);
        string newHash = HashUtility.ComputeSha256(resultText, Encoding.UTF8);

        return new CsvTransformationResult(
            Success: true,
            TransformedCsvContent: resultText,
            OriginalSha256: originalHash,
            TransformedSha256: newHash,
            TotalValuesChanged: changes.Count,
            Changes: changes,
            Message: $"Maç kondisyonu dönüşümü tamamlandı. Toplam {changes.Count} değer sıfırlandı."
        );
    }

    public static List<string> ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }
        fields.Add(sb.ToString());
        return fields;
    }

    public static string DetectLineEnding(string text)
    {
        if (text.Contains("\r\n")) return "\r\n";
        if (text.Contains("\r")) return "\r";
        return "\n";
    }

    public static Encoding DetectEncoding(string filename)
    {
        // BOM kontrolü
        var bom = new byte[4];
        using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
        {
            file.Read(bom, 0, 4);
        }

        if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
        if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32;
        if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode;
        if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode;
        if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return new UTF32Encoding(true, true);

        return Encoding.UTF8; // Varsayılan UTF-8
    }
}
