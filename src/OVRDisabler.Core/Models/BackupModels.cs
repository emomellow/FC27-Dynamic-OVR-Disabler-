namespace OVRDisabler.Core.Models;

public record BackupManifest(
    string BackupId,
    DateTime Timestamp,
    string OriginalFilePath,
    string BackupFilePath,
    string Sha256Original,
    long FileSize,
    string Description
);

public record BackupResult(
    bool Success,
    BackupManifest? Manifest,
    string Message
);

public record RestoreResult(
    bool Success,
    string RestoredFilePath,
    string Message
);
