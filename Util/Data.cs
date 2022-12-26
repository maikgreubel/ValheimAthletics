using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ValheimAthletics.Util
{
    /**
     * <summary>This class provides wrapper method to retrieve and store various data.</summary>
     **/
    public static class Data
    {
        public static string ModId;
        public static string Folder;

        /**
         * <summary>Retrieve the path to mod data file storage as string.</summary>
         **/
        public static string GetModDataPath(this PlayerProfile profile) => Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Auto), "ModData", Data.ModId, "char_" + profile.GetFilename());

        /**
         * <summary>Reterieve arbitrary data from <see cref="PlayerProfile"/>.</summary>
         **/
        public static TData LoadModData<TData>(this PlayerProfile profile) where TData : new() => !File.Exists(profile.GetModDataPath()) ? new TData() : JsonUtility.FromJson<TData>(File.ReadAllText(profile.GetModDataPath()));

        /**
         * <summary>Save arbitrary data to mod file storage for particular <see cref="PlayerProfile"/>.</summary>
         **/
        public static void SaveModData<TData>(this PlayerProfile profile, TData data)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(profile.GetModDataPath()));
            File.WriteAllText(profile.GetModDataPath(), JsonUtility.ToJson((object)data));
        }

        /**
         * <summary>Load a texture from assets directory using given file name identified by <paramref name="path"/> into a <see cref="Texture2D"/> object.</summary>
         **/
        public static Texture2D LoadTextureFromAssets(string path)
        {
            byte[] numArray = File.ReadAllBytes(Path.Combine(Data.Folder, "assets", path));
            Texture2D texture = new Texture2D(1, 1);
            ImageConversion.LoadImage(texture, numArray);
            return texture;
        }

        /**
         * <summary>Load translations for given <paramref name="language"/> and return it as <see cref="Dictionary{TKey, TValue}"/>.</summary>
         **/
        public static Dictionary<string,string> GetTranslationsFor(string language)
        {
            try
            {
                string jsonText = File.ReadAllText(Path.Combine(Data.Folder, "assets", language + ".lng"));
                Dictionary<string, string> translations = JsonUtility.FromJson<Serialization<string,string>>(jsonText).ToDictionary();
                foreach (KeyValuePair<string,string> de in translations)
                {
                    AccessTools.Method(typeof(Localization), "AddWord", null, null).Invoke(Localization.instance, new object[2] { de.Key, de.Value });
                }
                return translations;
            }
            catch(Exception e)
            {
                Log.error(e.Message);
                return new Dictionary<string, string>();
            }
        }
    }
}
