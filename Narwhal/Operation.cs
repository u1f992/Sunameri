using System.Text.Json.Serialization;

namespace Narwhal;

/// <summary>
/// Represents the operation, which has a message to be sent and the interval between the next message.
/// </summary>
public class Operation
{
    /// <summary>
    /// Default value of Interval. (milliseconds)
    /// </summary>
    /// <value>100</value>
    public static uint DefaultInterval { get; } = 100;

    /// <summary>
    /// A message to be sent.
    /// </summary>
    [JsonPropertyName("message")] public char Message { get; set; }
    /// <summary>
    /// The interval between the next message.
    /// </summary>
    [JsonPropertyName("interval")] public uint Interval { get; set; }

    /// <summary>
    /// Initialize the object with <see cref="Special.Empty"/>.<br/>
    /// Use <see cref="Operation.DefaultInterval"/> as the default interval.
    /// </summary>
    public Operation() : this(Special.Empty, Operation.DefaultInterval) { }
    /// <summary>
    /// Initialize the object with a message.<br/>
    /// Use <see cref="Operation.DefaultInterval"/> as the default interval.
    /// </summary>
    /// <param name="message"></param>
    public Operation(char message) : this(message, Operation.DefaultInterval) { }
    /// <summary>
    /// Initialize the object with a tuple of message and interval.
    /// </summary>
    /// <param name="operation"></param>
    public Operation((char message, uint interval) operation) : this(operation.message, operation.interval) { }
    /// <summary>
    /// Initialize the object with a message and interval.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="interval"></param>
    public Operation(char message, uint interval)
    {
        Message = message;
        Interval = interval;
    }

    public string ToJson() { return "{\"message\":\"" + Message + "\",\"interval\":" + Interval + "}"; }
}
