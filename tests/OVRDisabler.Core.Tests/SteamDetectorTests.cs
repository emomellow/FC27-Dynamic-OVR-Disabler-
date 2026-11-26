using OVRDisabler.Steam;
using Xunit;

namespace OVRDisabler.Core.Tests;

public class SteamDetectorTests
{
    [Fact]
    public void ExtractBuildIdFromManifest_ParsesCorrectly()
    {
        string tempManifest = Path.GetTempFileName();
        try
        {
            string content = @"""AppState""
{
    ""appid""        ""4080220""
    ""name""         ""EA SPORTS FC 27""
    ""buildid""      ""26789738""
}";
            File.WriteAllText(tempManifest, content);
            var buildId = SteamGameDetector.ExtractBuildIdFromManifest(tempManifest);

            Assert.Equal("26789738", buildId);
        }
        finally
        {
            if (File.Exists(tempManifest)) File.Delete(tempManifest);
        }
    }

    [Fact]
    public void ParseLibraryFoldersVdf_CollectsPaths()
    {
        string tempVdf = Path.GetTempFileName();
        string dummyDir = Path.Combine(Path.GetTempPath(), "SteamLibTest_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dummyDir);

        try
        {
            string escaped = dummyDir.Replace(@"\", @"\\");
            string content = $@"""libraryfolders""
{{
    ""0""
    {{
        ""path""        ""{escaped}""
    }}
}}";
            File.WriteAllText(tempVdf, content);

            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            SteamGameDetector.ParseLibraryFoldersVdf(tempVdf, set);

            Assert.Contains(Path.GetFullPath(dummyDir), set);
        }
        finally
        {
            if (File.Exists(tempVdf)) File.Delete(tempVdf);
            if (Directory.Exists(dummyDir)) Directory.Delete(dummyDir);
        }
    }
}
