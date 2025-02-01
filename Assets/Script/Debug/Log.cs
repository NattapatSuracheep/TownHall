using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class Log
{
    private static Action<string> onLogInvoke;
    private static Action<string> onWarningInvoke;
    private static Action<string> onErrorInvoke;

    public static void AddLogInvoke(LogType type, Action<string> listener)
    {
        switch (type)
        {
            case LogType.Log:
                onLogInvoke += listener;
                break;
            case LogType.Warning:
                onWarningInvoke += listener;
                break;
            case LogType.Error:
                onErrorInvoke += listener;
                break;
            default:
                onLogInvoke += listener;
                break;
        }
    }

    public static void RemoveLogInvoke(LogType type, Action<string> listener)
    {
        switch (type)
        {
            case LogType.Log:
                onLogInvoke -= listener;
                break;
            case LogType.Warning:
                onWarningInvoke -= listener;
                break;
            case LogType.Error:
                onErrorInvoke -= listener;
                break;
            default:
                onLogInvoke -= listener;
                break;
        }
    }

    public static void Array<T, T1>(Dictionary<T, T1> dictionary, [CallerMemberName] string methodName = "", [CallerFilePath] string file = "")
    {
        Array(dictionary.ToArray(), methodName, file);
    }

    public static void Array<T>(List<T> list, [CallerMemberName] string methodName = "", [CallerFilePath] string file = "")
    {
        Array(list.ToArray(), methodName, file);
    }

    public static void Array<T>(T[] array, [CallerMemberName] string methodName = "", [CallerFilePath] string file = "")
    {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        return;
#endif

        // Check if the array is null or empty
        if (array == null || array.Length == 0)
        {
            Warning("Array / List / Dictionary is null or empty.");
            return;
        }

        // Build a string to represent the array contents
        string arrayContents = "";

        for (int i = 0; i < array.Length; i++)
        {
            arrayContents += array[i].ToString();

            if (i < array.Length - 1) // Add a comma if it's not the last element
                arrayContents += ", ";
        }

        // Log the array contents
        Logging(arrayContents, methodName, file);
    }

    public static void Variable(string name, object message, [CallerMemberName] string methodName = "", [CallerFilePath] string file = "")
    {
        Logging($"{name} : {message}", methodName, file);
    }

    public static void Logging(object message, [CallerMemberName] string methodName = "", [CallerFilePath] string file = "")
    {
        Logger(LogType.Log, message, methodName, file);
    }

    public static void Warning(object message, [CallerMemberName] string methodName = "", [CallerFilePath] string file = "")
    {
        Logger(LogType.Warning, message, methodName, file);
    }

    public static void Error(object message, [CallerMemberName] string methodName = "", [CallerFilePath] string file = "")
    {
        Logger(LogType.Error, message, methodName, file);
    }

    public static void Call([CallerMemberName] string methodName = "", [CallerFilePath] string file = "")
    {
        var filePath = file.Split('\\');
        var fileName = filePath.Last();

        var message = $"<color=#CDFF47>{fileName} - {methodName}</color>";

        Debug.Log(message);
    }

    private static void Logger(LogType logType, object message, string methodName, string file)
    {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        return;
#endif

        var filePath = file.Split('\\');
        var fileName = filePath.Last();

        var prefix = $"[<color=#78fbff>{fileName}</color>/<color=#e678ff>{methodName}</color>]";

        switch (logType)
        {
            case LogType.Log:
                var s = $"{prefix}: {message}";
                Debug.Log(s);

                onLogInvoke?.Invoke(s);
                break;
            case LogType.Warning:
                s = $"{prefix}: <color=#ffc847>{message}</color>";
                Debug.LogWarning(s);

                onWarningInvoke?.Invoke(s);
                break;
            case LogType.Error:
                s = $"{prefix}: <color=#ff4747>{message}</color>";
                Debug.LogError(s);

                onErrorInvoke?.Invoke(s);
                break;
            default:
                s = $"{prefix}: {message}";
                Debug.Log(s);

                onLogInvoke?.Invoke(s);
                break;
        }
    }
}