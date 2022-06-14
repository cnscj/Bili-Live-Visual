namespace XLibrary
{
    // 程序集无法使用#if UNITY_EDITOR等宏，提供一下静态类来代替
    public static class EditorEnvironment
    {
        static private bool m_isEngineEditor = false;
        static private bool m_isDesignEditor = false;
        static private bool m_isArtEditor = false;
        static private bool m_isGame = true;

        public static bool isEngineEditor
        {
            get
            {
                return m_isEngineEditor;
            }
        }

        public static bool isDesignEditor
        {
            get
            {
                return m_isDesignEditor;
            }
        }

        public static bool isArtEditor
        {
            get
            {
                return m_isArtEditor;
            }
        }

        public static bool isGame
        {
            get
            {
                return m_isGame;
            }
        }

        public static void SetIsEngineEditor()
        {
            m_isEngineEditor = true;
            m_isDesignEditor = false;
            m_isArtEditor = false;
            m_isGame = false;
        }


        public static void SetIsDesignEditor()
        {
            m_isEngineEditor = false;
            m_isDesignEditor = true;
            m_isArtEditor = false;
            m_isGame = false;
        }

        public static void SetIsArtEditor()
        {
            m_isEngineEditor = false;
            m_isDesignEditor = false;
            m_isArtEditor = true;
            m_isGame = false;
        }
    }
}
