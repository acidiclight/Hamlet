using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamlet
{
    public class GameData
    {
        public int StartingSceneState { get; set; }
        public SceneState[] SceneStates { get; set; }
    }

    public class SceneState
    {
        public int Act { get; set; }
        public int Scene { get; set; }
        public int CorrectChoice { get; set; }
        public string Prompt { get; set; }
        public string[] Choices { get; set; }
        public string[] Responses { get; set; }
    }
}
