using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChatBot.api.json;

public static class Json {
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
            obj = new T();
            HandleException();
        }
        catch (JsonSerializationException) {
            obj = new T();
            HandleException();
        }
        catch (JsonReaderException) {
            obj = new T();
            HandleException();
        }

        return;

        void HandleException() {
            var file = new FileInfo(filename);
            var directory = file.DirectoryName;
            if (directory == null) return;
            
            CreateOld(filename);
            WriteSafe(filename, directory, new T());
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