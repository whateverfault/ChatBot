using System.Text;

namespace ChatBot.bot.shared.handlers;

public static class IoHandler {
    private const string CONSOLE_CLEAR_SEQUENCE = "\x1b[2J\x1b[3J\x1b[H";
    
    private static readonly object _lock = new object(); 
    
    public static Encoding InputEncoding {
        get {
            lock (_lock) {
                return Console.InputEncoding;
            }
        }
        set {
            lock (_lock) {
                Console.InputEncoding = value;
            }
        }
    }
    
    public static Encoding OutputEncoding {
        get {
            lock (_lock) {
                return Console.OutputEncoding;
            }
        }
        set {
            lock (_lock) {
                Console.OutputEncoding = value;
            }
        }
    }

    public static bool KeyAvailable {
        get {
            lock (_lock) {
                return Console.KeyAvailable;
            }
        }
    }
    
    
    public static void WriteLine(string message) {
        lock (_lock) {
            Console.WriteLine(message);
        }
    }
    
    public static void Write(string message) {
        lock (_lock) {
            Console.Write(message);
        }
    }
    
    public static void ClearWriteLine(string message) {
        lock (_lock) {
            ClearUnsynced();
            Console.WriteLine(message);
        }
    }
    
    public static void ClearWrite(string message) {
        lock (_lock) {
            ClearUnsynced();
            Console.Write(message);
        }
    }
    
    public static void Clear() {
        lock (_lock) {
            ClearUnsynced();
        }
    }
    
    public static string? ReadLine(string? message = null) {
        lock (_lock) {
            Console.Write(message);
            return Console.ReadLine();
        }
    }

    public static ConsoleKeyInfo ReadKey(bool intercept = false) {
        lock (_lock) {
            return Console.ReadKey(intercept);
        }
    }

    private static void ClearUnsynced() {
        Console.Write(CONSOLE_CLEAR_SEQUENCE);
    }
}