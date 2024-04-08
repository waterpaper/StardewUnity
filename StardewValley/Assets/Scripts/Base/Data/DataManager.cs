using WATP.Data;

namespace WATP
{
    public partial class DataManager
    {
        private Config config;
        private TableData tableData;
        private AtlasContainer atlasContainer;
        private Preferences preferences;


        public Config Config { get => config; }
        public TableData TableData { get => tableData; }
        public AtlasContainer AtlasContainer { get => atlasContainer; }
        public Preferences Preferences { get => preferences; }

        public void Init()
        {
            config = new Config();
            preferences = new Preferences();
            tableData = new TableData();
            atlasContainer = new AtlasContainer();
            preferences.Load();
        }
    }
}