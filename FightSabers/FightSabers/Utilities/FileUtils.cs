using System.IO;
using System.Reflection;

namespace FightSabers.Utilities
{
    public static class FileUtils
    {
        public static void WriteResourceToFile(string resourceName, string filePath, FileMode mode = FileMode.Create, FileAccess access = FileAccess.Write)
        {
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var file = new FileStream(filePath, mode, access))
                    resource?.CopyTo(file);
            }
        }
    }
}
