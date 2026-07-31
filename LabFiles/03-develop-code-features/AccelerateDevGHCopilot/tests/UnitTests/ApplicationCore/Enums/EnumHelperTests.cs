using System.ComponentModel;
using Library.ApplicationCore.Enums;

namespace Library.UnitTests.ApplicationCore.Enums;

public class EnumHelperTests
{
    [Fact]
    public void GetDescription_ReturnsDescription_WhenAttributeExists()
    {
        // Arrange
        var value = SampleStatus.Active;

        // Act
        var description = EnumHelper.GetDescription(value);

        // Assert
        Assert.Equal("Active state", description);
    }

    [Fact]
    public void GetDescription_ReturnsEnumName_WhenAttributeIsMissing()
    {
        // Arrange
        var value = SampleStatus.Inactive;

        // Act
        var description = EnumHelper.GetDescription(value);

        // Assert
        Assert.Equal(nameof(SampleStatus.Inactive), description);
    }

    private enum SampleStatus
    {
        [Description("Active state")]
        Active,

        Inactive
    }
}
