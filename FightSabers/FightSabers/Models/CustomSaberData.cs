using System.Reflection;
using CustomSaber.Utilities;
using UnityEngine;

namespace FightSabers.Models
{
    public class CustomSaberData
    {
        public string          FileName        { get; }
        public AssetBundle     AssetBundle     { get; }
        public SaberDescriptor SaberDescriptor { get; }
        public GameObject      Sabers          { get; }

        public CustomSaberData(string resourceName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                    AssetBundle = AssetBundle.LoadFromStream(stream);
                Sabers = AssetBundle.LoadAsset<GameObject>("_CustomSaber");
                SaberDescriptor = Sabers.GetComponent<SaberDescriptor>();

                if (SaberDescriptor.CoverImage == null)
                    SaberDescriptor.CoverImage = Utils.GetDefaultCoverImage();
            }
            catch
            {
                Logger.log.Warn($"Something went wrong getting the AssetBundle for '{resourceName}'!");

                SaberDescriptor = new SaberDescriptor {
                    SaberName = "Invalid Saber (Delete it)",
                    AuthorName = FileName,
                    CoverImage = Utils.GetErrorCoverImage()
                };

                FileName = "DefaultSabers";
            }
        }
    }
}