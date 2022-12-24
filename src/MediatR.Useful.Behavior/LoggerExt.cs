using Microsoft.Extensions.Logging;
using System;

namespace MediatR.Useful.Behavior;

public static partial class LoggerExt
{
    [LoggerMessage(
        EventId = 0,
        EventName = "CompileLog",
        Message = "{Message}")]
    public static partial void CompileLog(this ILogger logger, Exception exp,
        LogLevel level, string message);

    [LoggerMessage(
        EventId = 1,
        EventName = "CompileLogMessage",
        Message = "{Message}")]
    public static partial void CompileLogMessage(this ILogger logger,
        LogLevel level, string message);

    [LoggerMessage(
        EventId = 2,
        EventName = "CompileLogStr",
        Message = "{Message} {Param}")]
    public static partial void CompileLogStr(this ILogger logger, Exception exp,
        LogLevel level, string message, string param);

    [LoggerMessage(
        EventId = 3,
        EventName = "CompileLogStrStr",
        Message = "{Message} {Param} {Param2}")]
    public static partial void CompileLogStrStr(this ILogger logger, Exception exp,
        LogLevel level, string message, string param, string param2);

    [LoggerMessage(
        EventId = 4,
        EventName = "CompileLogObj",
        Message = "{Message} {Param}")]
    public static partial void CompileLogObj(this ILogger logger, Exception exp,
        LogLevel level, string message, object param);

    [LoggerMessage(
        EventId = 5,
        EventName = "CompileLogObjObj",
        Message = "{Message} {Param} {Param2}")]
    public static partial void CompileLogObjObj(this ILogger logger, Exception exp,
        LogLevel level, string message, object param, object param2);


    [LoggerMessage(
        EventId = 6,
        EventName = "CompileLogStrObj",
        Message = "{Message} {Param} {Param2}")]
    public static partial void CompileLogStrObj(this ILogger logger, Exception exp,
        LogLevel level, string message, string param, object param2);


    [LoggerMessage(
        EventId = 7,
        EventName = "CompileLogMessageStr",
        Message = "{Message} {Param}")]
    public static partial void CompileLogMessageStr(this ILogger logger,
        LogLevel level, string message, string param);



    [LoggerMessage(
        EventId = 8,
        EventName = "CompileLogStrObj",
        Message = "{Message} {Param} {Param2} {Param3} {Param4}")]
    public static partial void CompileLogStrObj(this ILogger logger,
        LogLevel level, string message, object param, object param2, object param3, object param4);
}