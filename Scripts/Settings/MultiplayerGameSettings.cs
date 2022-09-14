namespace Settings
{
    class MultiplayerGameSettings : BaseSettingsContainer
    {
        public StringSetting Map = new StringSetting("The City");
        public StringSetting Name = new StringSetting("[FF8000]PTBot[000000]", maxLength: 100);
        public StringSetting Password = new StringSetting(string.Empty, maxLength: 100);
        public IntSetting MaxPlayers = new IntSetting(99, minValue: 0, maxValue: 255);
        public IntSetting MaxTime = new IntSetting(999999, minValue: 1, maxValue: 100000);
        public IntSetting Difficulty = new IntSetting((int)GameDifficulty.Abnormal);
    }
}