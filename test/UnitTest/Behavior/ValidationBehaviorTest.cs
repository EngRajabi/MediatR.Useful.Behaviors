using FluentAssertions;
using FluentValidation;
using MediatR;
using MediatR.Useful.Behavior.Behavior;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTest.Model;
using Xunit;

namespace UnitTest.Behavior;

public sealed class ValidationBehaviorTest : TestBase
{
    private readonly Mock<RequestHandlerDelegate<GetUserPointCommandRes>> _behaviourDelegate;

    public ValidationBehaviorTest()
    {
        _behaviourDelegate = new Mock<RequestHandlerDelegate<GetUserPointCommandRes>>();
    }


    [Theory]
    [MemberData(nameof(TestCases))]
    public async Task CheckDistributeCache(GetUserPointCommandReq requestCommandRq,
        int countBehaviorCall, bool isExp, GetUserPointCommandRes result)
    {
        //Arrange
        var validators = new List<IValidator<GetUserPointCommandReq>>
        {
            new GetUserPointCommandReqValidator()
    };
        var behavior = new ValidationBehavior<GetUserPointCommandReq,
            GetUserPointCommandRes>(validators);

        _behaviourDelegate.Setup(r => r.Invoke())
                .ReturnsAsync(result);

        GetUserPointCommandRes handle = null;
        bool exp = false;
        try
        {
            //Act
            handle = await RunBehavior(behavior, requestCommandRq, _behaviourDelegate.Object);
        }
        catch
        {
            exp = true;
        }

        //Assert
        _behaviourDelegate.Verify(r => r.Invoke(), Times.Exactly(countBehaviorCall));
        exp.Should().Be(isExp);

        handle.Should().BeEquivalentTo(result);
    }


    public static IEnumerable<object[]> TestCases
    {
        get
        {
            yield return new object[]
            {
                //req
                new GetUserPointCommandReq{Amount = 0},
                //runbehvaior
                0,
                //is exception
                true,
                //result
                null
            };
            yield return new object[]
            {
                //req
                new GetUserPointCommandReq{Amount = 500},
                //runbehvaior
                0,
                //is exception
                true,
                //result
                null
            };
            yield return new object[]
            {
                //req
                new GetUserPointCommandReq{Amount = 500, UserId = "hi"},
                //runbehvaior
                1,
                //is exception
                false,
                //result
                new GetUserPointCommandRes
                {
                    Point = 40.0m
                }
            };
        }
    }
}
