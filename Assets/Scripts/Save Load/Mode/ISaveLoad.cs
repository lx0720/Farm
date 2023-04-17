using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farm.Save
{
    public interface ISaveLoad
    {
        public string GetGuid();
        public GameSaveData SaveGameData();
        public void LoadGameData(GameSaveData gameSaveData);
        public void RegisterInterface();
    }
}
