using System.Collections.Generic;

    [System.Serializable]
    public class LevelSaveData
    {
        public int currentLevelIndex;
        public int currentLevelNo;
        public List<int> levelIndicesToRepeat;

        public LevelSaveData()
        {
            currentLevelIndex = 0;
            currentLevelNo = 1;
            levelIndicesToRepeat = new List<int>();
  
        }
    }