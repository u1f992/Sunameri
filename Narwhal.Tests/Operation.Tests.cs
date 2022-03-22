using Xunit;

using System.Text.Json;
using Narwhal;

public class Operation_Tests
{
    [Fact(DisplayName = "ToJson()は正しい結果を返す")]
    public void ToJson_should_return_expected_result()
    {
        Assert.Equal(@"{""message"":""a"",""interval"":100}", new Operation(KeyDown.A).ToString());
    }

    [Theory(DisplayName = "OperationをJSONからデシリアライズできる")]
    [InlineData('a', 100, @"{""message"":""a"",""interval"":100}")]
    [InlineData('m', 100, @"{""message"":""m"",""interval"":100}")]
    public void Operation_can_be_deserialize_from_JSON(char command, uint interval, string json)
    {
        var test = JsonSerializer.Deserialize<Operation>(json);
        Assert.NotNull(test);
        Assert.Equal(command, test?.Message);
        Assert.Equal(interval, test?.Interval);
    }

    [Theory(DisplayName = "Operationの配列をJSONからデシリアライズできる")]
    [InlineData('a', 'm', @"[{""message"":""a"",""interval"":100},{""message"":""m"",""interval"":100}]")]
    public void Operations_can_be_deserialize_from_JSON(char cmd1, char cmd2, string json)
    {
        var test = JsonSerializer.Deserialize<Operation[]>(json);
        Assert.NotNull(test);
        Assert.Equal(cmd1, test?[0].Message);
        Assert.Equal(cmd2, test?[1].Message);
    }
}