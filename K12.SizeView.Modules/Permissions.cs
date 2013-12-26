using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.SizeView.Modules
{
    class Permissions
    {
        public static string 系統容量檢視 { get { return "K12.SizeView.Modules.SizeViewFrom"; } }

        public static bool 系統容量檢視權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[系統容量檢視].Executable;
            }
        }
    }
}
