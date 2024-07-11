using AutoFixture;
using AutoFixture.AutoMoq;

namespace Manager.Tests.BaseTests;

public class UnitTestsBase
{
    public readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
}