using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    static public class Algos
    {   
        public static string IsChecked(int filter) => filter == 0 ? "checked" : "";
    }
}