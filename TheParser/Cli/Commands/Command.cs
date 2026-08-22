namespace TheParser.Cli.Commands;

public abstract class CliCommand
{
    public abstract Resource CommandDescription { get; }
    public abstract CliCommandResult Run(ArgumentsProvider argumentsProvider);

    /// <summary>
    /// Command execution was considered a success. 
    /// </summary>
    protected static CliCommandResult Success() => CliCommandResult.Success();
    /// <summary>
    /// Command runner has detected an incorrect usage: missing or incorrect argument, etc.
    /// </summary>
    protected static CliCommandResult IncorrectUsage(string message) => CliCommandResult.IncorrectUsage(message);
}

public readonly struct CliCommandResult
{
    public const int SUCCESS = 0;
    public const int FAIL = 1;
    public const int INCORRECT_USAGE = 2;

    public int ResultCode { get; }
    public string? Description { get; }
    public Exception? Exception { get; }

    private CliCommandResult(int code, string? description, Exception? exception)
    {
        ResultCode = code;
        Description = description;
        Exception = exception;
    }

    public static CliCommandResult IncorrectUsage(string description) => new(INCORRECT_USAGE, description, null);

    public static CliCommandResult Fail(Exception ex, string? description = null) => new(FAIL, description, ex);

    public static CliCommandResult Success() => new(SUCCESS, null, null);

    public int AcknowledgeUser()
    {
        switch (ResultCode)
        {
            case FAIL:
                Console.Error.WriteLine(
                    $"Failed with an exception.\n{Exception!.GetType().FullName}\n\n\n{Exception.Message}"
                    );
                if (Description != null)
                    Console.Error.WriteLine(Description);
                break;
            case INCORRECT_USAGE:
                Console.Error.WriteLine($"Incorrect usage: {Description}\nSee parser help <command> for more information.");
                break;
            case SUCCESS:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(ResultCode), null, null);
        }

        return ResultCode;
    }
}