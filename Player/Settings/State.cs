using System.Collections.Generic;
using Player.Services;

namespace Player.Settings
{
    public class State
    {
        private static readonly State instance = new State();

        static State()
        {
        }

        private State()
        {
        }

        public static State Instance
        {
            get
            {
                return instance;
            }
        }

        public List<PlayerWindow> WindowsStates = new List<PlayerWindow>();
    }
}