using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TASBoard.Models
{
    public class Workspace
    {
        public ObservableCollection<KeyObject> AllKeys;
        public Workspace()
        {
            AllKeys = new();
        }

        public void AddKey(string keyStyle, string keyName)
        {
            AllKeys.Add(new KeyObject(keyStyle, keyName, displayKeyDown));
        }

        public void AddKey(KeyObject key)
        {
            AllKeys.Add(key);
        }

        public void SetDisplayKeyDown(bool value)
        {
            foreach (var key in AllKeys)
            {
                key.SetKeyDown(value);
            }
            displayKeyDown = value;
        }

        private bool displayKeyDown = false;
    }
}
