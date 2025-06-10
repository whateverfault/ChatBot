namespace ChatBot.extensions;

public static class FileExtension {
    public static bool IsLocked(this FileInfo file) {
        FileStream? stream = null;
        try {
            stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        catch (IOException) {
            return true;
        }
        finally {
            stream?.Close();
        }
        return false;
    }
}