using System.Collections.Generic;

namespace ValheimAthletics.Util
{
    /**
     * <summary>This class provides functionality to translate strings into <seealso cref="Localization"/> selected language.</summary>
     **/
    public sealed class Localizer
    {
        /**
         * <summary>Singleton instance</summary>
         **/
        private static readonly Localizer _instance = new Localizer();

        /**
         * <summary>The selected language</summary>
         **/
        private string Language;

        /**
         * <summary>The dictionary which holds translations as key-value pairs</summary>
         **/
        private Dictionary<string, string> translationCache = new Dictionary<string, string>();

        /**
         * No external instance creation allowed.
         **/
        private Localizer()
        {
            this.Language = Localization.instance.GetSelectedLanguage();
        }

        /**
         * <summary>Retrieve the one and only instance of <see cref="Localizer"/></summary>
         **/
        public static Localizer Instance
        {
            get { return _instance;  }
        }

        /**
         * <summary>Translate a given string into selected language.</summary>
         **/
        public string Translate(string str)
        {
            return Translate(str, this.Language);
        }

        /**
         * <summary>Translate a given string into given language.</summary>
         **/
        public string Translate(string str, string language)
        {
            if (!this.translationCache.ContainsKey(str))
            {
                this.translationCache = Data.GetTranslationsFor(language);
            }

            if(this.translationCache.ContainsKey(str))
            {
                return (string)this.translationCache[str]; 
            }

            Log.warn("No translation found for '" + str + "' in " + language);
            return str;
        }
    }
}
