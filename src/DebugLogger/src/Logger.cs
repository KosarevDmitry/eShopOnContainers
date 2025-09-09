using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace DebugLogger;

public static class Logger
{
    private static TextWriter? _output;
  //  private static bool _isInit;

    /// <summary>
    /// Disable
    /// </summary>
    public static bool IsDisable { get; set; }


    /// <summary>
    /// DefaultFolder
    /// </summary>
    public static string DefaultFolder => @"D:\temp\kslogs";

    private static readonly object s_lock = new object();

    static Logger()=> Init();
    
    /// <summary>
    /// Log
    /// </summary>
    /// <param name="message"></param>
    /// <param name="filepath"></param>
    /// <param name="memberName"></param>
    /// <param name="sourceLineNumber"></param>
    public static void Log(string message, [CallerFilePath] string filepath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int sourceLineNumber = 0
    ) => WriteLine(message, filepath, memberName, sourceLineNumber);
      

    public static void Question(string message, [CallerFilePath] string filepath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int sourceLineNumber = 0
    ) => WriteLine( message, filepath, memberName, sourceLineNumber,":question:");
    

    public static void Attention(string message, [CallerFilePath] string filepath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int sourceLineNumber = 0
    ) => WriteLine( message, filepath, memberName, sourceLineNumber,":bulb:");
    

    public static void Snippet(string message, [CallerFilePath] string filepath = "",
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int sourceLineNumber = 0
    ) => WriteLine( message, filepath, memberName, sourceLineNumber,":point_right:");
      

    private static void WriteLine(string message, string filepath, string memberName,
        int sourceLineNumber, string emoii=null)
    {
        if (IsDisable) return;
        var stamp = DateTimeOffset.Now.ToString("ss:FFFF", DateTimeFormatInfo.InvariantInfo);
        emoii= emoii!=null?" " +emoii:"";
        
        _output?.WriteLine(
            $"1.{stamp}{emoii} {message} -- [{Path.GetFileName(filepath)}]({filepath}) {memberName}:{sourceLineNumber}");
    }

    private static void Init()
    {
        //return xunitRunner if a call was from unittest
        var fullname = Assembly.GetEntryAssembly()?.FullName;
        if (fullname == null)
        {
            fullname = Path.GetRandomFileName();
        }

        int i = fullname.IndexOf(',');
        var name = fullname.Substring(0, i != -1 ? i : fullname.Length);
       
        // it seems better to keep all attempts
        var path = Path.Combine(DefaultFolder,
            $"{name}-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss", DateTimeFormatInfo.InvariantInfo)}.md");

        Stream outputStream =  System.IO.File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
        outputStream.Seek(0, SeekOrigin.End);
        
        var  encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        _output = new StreamWriter(outputStream, encoding) { AutoFlush = true };
        _output.WriteLine("Start:" + DateTime.Now);
         
        //Console.SetOut(_output);
     
    }

    /// <summary>
    /// Flush memory
    /// </summary>
    public static void Flush()
    {
        lock (s_lock)
        {
            if (_output != null)
            {
                _output.Flush();
                _output.Close();
                _output.Dispose();
            }
        }
    }
}