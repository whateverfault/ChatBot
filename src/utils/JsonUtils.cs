using ChatBot.extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChatBot.utils;

public static class JsonUtils {
    private static readonly JsonSerializerSettings _options
        = new() {
                    Converters = new List<JsonConverter> { new StringEnumConverter() },
                    MissingMemberHandling = MissingMemberHandling.Error,
                    NullValueHandling = NullValueHandling.Include,
                    Formatting = Formatting.Indented
                };


    public static void Read<T>(string fileName, out T? obj) {
        using var sr = new StreamReader(fileName);
        obj = JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
    }

    public static bool TryRead<T>(string fileName, out T? obj) {
        obj = default;
        if (new FileInfo(fileName).IsLocked()) return false;
        if (!File.Exists(fileName)) return false;
        Read(fileName, out obj);
        return true;
    }

    public static void Write<T>(string fileName, T data) {
        var json = JsonConvert.SerializeObject(data, _options);
        File.WriteAllText(fileName, json);
    }

    public static void Clear(string fileName, string directory) {
        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }
        if (File.Exists(fileName)) {
            File.Delete(fileName);
        }
        File.Create(fileName).Close();
    }

    public static void WriteSafe<T>(string fileName, string directory, T data) {
        if (new FileInfo(fileName).IsLocked()) return;
        Clear(fileName, directory);
        Write(fileName, data);
    }
}