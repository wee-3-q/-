using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insertion_back_loss.serve
{
    public class glob
    {
        private static glob _instance;
        public static glob Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new glob();
                }
                return _instance;
            }
        }

        public int Counter { get; set; }
        public string AppName { get; set; } = "My Application";

        private glob() { } // 私有构造函数，防止外部实例化  
    }
}
