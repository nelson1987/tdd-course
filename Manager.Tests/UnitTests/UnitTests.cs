using AutoFixture;
using AutoFixture.AutoMoq;

namespace Manager.Tests.UnitTests;

public class UnitTests
{
    public readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
}
