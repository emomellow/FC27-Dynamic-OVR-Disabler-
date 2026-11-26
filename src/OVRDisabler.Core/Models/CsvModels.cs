namespace OVRDisabler.Core.Models;

public enum CsvAssetType
{
    Unknown,
    FormMoraleBoosts,
    MatchFitnessData
}

public record CsvValueChange(
    string Section,
    string RowIdentifier,
    string ColumnIdentifier,
    string OriginalValue,
    string NewValue
);

public record CsvAnalysisResult(
    string FileName,
    long FileSizeBytes,
    CsvAssetType AssetType,
    string Sha256Hash,
    bool IsSchemaValid,
    string SchemaValidationMessage,
    int TotalValuesChanged,
    IReadOnlyList<CsvValueChange> PreviewChanges
);

public record CsvTransformationResult(
    bool Success,
    string? TransformedCsvContent,
    string? OriginalSha256,
    string? TransformedSha256,
    int TotalValuesChanged,
    IReadOnlyList<CsvValueChange> Changes,
    string Message
);
