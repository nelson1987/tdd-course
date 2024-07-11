using AutoFixture;
using AutoFixture.AutoMoq;
using Manager.Api.Features.Accounts;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace Manager.Tests.BaseTests;

public class UnitTestsBase
{
    public readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    public Expression<Action<ILogger<AccountsController>>> LogMessage(LogLevel logLevel, string message)
    {
        return x => x.Log(
                        logLevel,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                        null,
                        It.IsAny<Func<It.IsAnyType, Exception, string>>());
    }
}