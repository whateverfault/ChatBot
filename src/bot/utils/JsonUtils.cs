using ChatBot.bot.shared;
using ChatBot.bot.shared.Handlers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChatBot.bot.utils;

public static class JsonUtils {
    private static readonly JsonSerializerSettings _options
        = new JsonSerializerSettings {
                                         Converters = new List<JsonConverter> { new StringEnumConverter(), },
                                         MissingMemberHandling = MissingMemberHandling.Error,
                                         NullValueHandling = NullValueHandling.Include,
                                         Formatting = Formatting.Indented,
                                     };


    public static void CreateOld(string filename) {
        if (!File.Exists(filename)) {
            return;
        }
        
        using var sr = new StreamReader(filename);
        
        var json = sr.ReadToEnd();
        var oldPath = $"{filename}.old";
        
        if (string.IsNullOrEmpty(json)) {
            return;
        }
        
        if (File.Exists(oldPath)) {
            File.Delete(oldPath);
        }
        
        File.Copy(filename, oldPath);
    }
    
    private static void Read<T>(string filename, out T? obj) where T : new() {
        obj = default;

        try {
            using var sr = new StreamReader(filename);
            var json = sr.ReadToEnd();
            
            obj = JsonConvert.DeserializeObject<T>(json, _options);
            if (obj == null) {
                throw new NullReferenceException();
            }
        }
        catch (NullReferenceException) {
            HandleException();
        }
        catch (JsonSerializationException) {
            HandleException();
        }

        return;

        void HandleException() {
            CreateOld(filename);
            WriteSafe(filename, Directories.ServiceDirectory, new T());
            
            ErrorHandler.LogErrorMessageAndPrint(ErrorCode.SaveFileCorrupted, $"An error occured while reading one of your save files.\nDefault settings are restored.\nOld save file can be found at: {filename}.old\n \nPress Enter To Continue...");
        } 
    }

    public static bool TryRead<T>(string fileName, out T? obj) where T : new() {
        obj = default;
        if (!File.Exists(fileName)) return false;
        
        Read(fileName, out obj);
        return true;
    }

    private static void Write<T>(string fileName, T data) {
        var json = JsonConvert.SerializeObject(data, _options);
        File.WriteAllText(fileName, json);
    }

    private static void Clear(string fileName) {
        if (File.Exists(fileName)) {
            File.Delete(fileName);
        }

        File.Create(fileName).Close();
    }

    public static void WriteSafe<T>(string fileName, string directory, T data) {
        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }

        Clear(fileName);
        Write(fileName, data);
    }
}