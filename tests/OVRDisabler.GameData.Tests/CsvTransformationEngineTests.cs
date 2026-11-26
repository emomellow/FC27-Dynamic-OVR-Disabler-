using OVRDisabler.Core.Models;
using OVRDisabler.GameData;
using Xunit;

namespace OVRDisabler.GameData.Tests;

public class CsvTransformationEngineTests
{
    private readonly CsvTransformationEngine _engine = new();

    private const string SampleFormMoraleCsv =
@"# DynamicOverall_FormMorale_Boosts
Category,Threshold,SkillWeight,PhysicalWeight,MentalWeight
Skill Weight,50,,,
Physical Weight,30,,,
Mental Weight,20,,,
Form,Threshold,SkillMod,PhysicalMod,MentalMod
Very Good,85,5,4,3
Good,70,3,2,1
Neutral,50,0,0,0
Bad,30,-3,-2,-1
Very Bad,15,-5,-4,-3
Morale,Threshold,SkillMod,PhysicalMod,MentalMod
Very Happy,85,4,3,2
Happy,70,2,1,1
Content,50,0,0,0
Sad,30,-2,-1,-1
Very Sad,15,-4,-3,-2";

    private const string SampleMatchFitnessCsv =
@"# DynamicOverall_MatchFitnessData
Category,Threshold
Abysmal,0
Unfit,25
Off-Par,45
Decent,65
Sharp,85
Injury,Penalty
Yellow Injury,-30
Orange/Red Injury,-80
Decent,Acceleration,Agility,Aggression,Sprint_Speed
Acceleration,3,2,1,4
Sprint_Speed,4,3,2,5";

    [Fact]
    public void IdentifyAssetType_CorrectlyIdentifiesSchemas()
    {
        Assert.Equal(CsvAssetType.FormMoraleBoosts, _engine.IdentifyAssetType(SampleFormMoraleCsv));
        Assert.Equal(CsvAssetType.MatchFitnessData, _engine.IdentifyAssetType(SampleMatchFitnessCsv));
        Assert.Equal(CsvAssetType.Unknown, _engine.IdentifyAssetType("Unknown,Header\n1,2"));
    }

    [Fact]
    public void TransformFormMorale_ZeroesOutFormAndMorale_PreservesWeightsAndThresholds()
    {
        var options = new DisablerOptions(DisableForm: true, DisableMorale: true, DisableMatchFitness: false);
        var result = _engine.Transform(SampleFormMoraleCsv, CsvAssetType.FormMoraleBoosts, options);

        Assert.True(result.Success);
        Assert.True(result.TotalValuesChanged > 0);

        string transformed = result.TransformedCsvContent!;

        // Ağırlıkların korunduğunu doğrula
        Assert.Contains("Skill Weight,50", transformed);
        Assert.Contains("Physical Weight,30", transformed);
        Assert.Contains("Mental Weight,20", transformed);

        // Eşiklerin korunduğunu doğrula
        Assert.Contains("Very Good,85,0,0,0", transformed);
        Assert.Contains("Very Bad,15,0,0,0", transformed);
        Assert.Contains("Very Happy,85,0,0,0", transformed);
        Assert.Contains("Very Sad,15,0,0,0", transformed);

        // Orijinal bonus ve cezaların kalmadığını doğrula
        Assert.DoesNotContain("Very Good,85,5,4,3", transformed);
        Assert.DoesNotContain("Very Bad,15,-5,-4,-3", transformed);
    }

    [Fact]
    public void TransformFormOnly_PreservesMoraleValues()
    {
        var options = new DisablerOptions(DisableForm: true, DisableMorale: false, DisableMatchFitness: false);
        var result = _engine.Transform(SampleFormMoraleCsv, CsvAssetType.FormMoraleBoosts, options);

        Assert.True(result.Success);
        string transformed = result.TransformedCsvContent!;

        // Form sıfırlandı
        Assert.Contains("Very Good,85,0,0,0", transformed);
        // Moral aynen korundu
        Assert.Contains("Very Happy,85,4,3,2", transformed);
    }

    [Fact]
    public void TransformMatchFitness_PreservesInjuryPenaltiesAndThresholds()
    {
        var options = new DisablerOptions(DisableForm: false, DisableMorale: false, DisableMatchFitness: true);
        var result = _engine.Transform(SampleMatchFitnessCsv, CsvAssetType.MatchFitnessData, options);

        Assert.True(result.Success);
        string transformed = result.TransformedCsvContent!;

        // Eşiklerin korunduğunu doğrula
        Assert.Contains("Abysmal,0", transformed);
        Assert.Contains("Decent,65", transformed);
        Assert.Contains("Sharp,85", transformed);

        // Sakatlık cezalarının (Yellow: -30, Orange/Red: -80) KORUNDUĞUNU doğrula!
        Assert.Contains("Yellow Injury,-30", transformed);
        Assert.Contains("Orange/Red Injury,-80", transformed);

        // Nitelik modifikatörlerinin sıfırlandığını doğrula
        Assert.Contains("Acceleration,0,0,0,0", transformed);
        Assert.Contains("Sprint_Speed,0,0,0,0", transformed);
    }

    [Fact]
    public void Transformation_IsIdempotent()
    {
        var options = new DisablerOptions(DisableForm: true, DisableMorale: true, DisableMatchFitness: true);
        var firstResult = _engine.Transform(SampleFormMoraleCsv, CsvAssetType.FormMoraleBoosts, options);
        var secondResult = _engine.Transform(firstResult.TransformedCsvContent!, CsvAssetType.FormMoraleBoosts, options);

        Assert.Equal(firstResult.TransformedCsvContent, secondResult.TransformedCsvContent);
        Assert.Equal(0, secondResult.TotalValuesChanged);
    }
}
