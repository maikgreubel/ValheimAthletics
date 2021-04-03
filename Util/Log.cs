using BepInEx.Logging;

namespace ValheimAthletics.Util
{
    internal class Log
    {
        internal static ManualLogSource Logging;

        /**
         * <summary>Log messages on debug level.</summary>
         **/
        public static void debug(string str) => Log.Logging.LogDebug(str);

        /**
         * <summary>Log messages on info level.</summary>
         **/
        public static void info(string str) => Log.Logging.LogInfo(str);

        /**
         * <summary>Log messages on message level.</summary>
         **/
        public static void message(string str) => Log.Logging.LogMessage(str);

        /**
         * <summary>Log messages on warning level.</summary>
         **/
        public static void warn(string str) => Log.Logging.LogWarning(str);

        /**
         * <summary>Log messages on error level.</summary>
         **/
        public static void error(string str) => Log.Logging.LogError(str);

        /**
         * <summary>Log messages on fatal error level.</summary>
         **/
        public static void fatal(string str) => Log.Logging.LogFatal(str);
    }
}
