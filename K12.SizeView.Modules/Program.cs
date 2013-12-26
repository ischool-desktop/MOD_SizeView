using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FISCA;
using FISCA.Presentation;
using FISCA.Permission;

namespace K12.SizeView.Modules
{
    public class Program
    {
        [MainMethod("K12.SizeView.Modules")]
        static public void Main()
        {
            MenuButton odr = FISCA.Presentation.MotherForm.StartMenu["系統容量檢視"];
            odr.Enable = Permissions.系統容量檢視權限;
            odr.Image = Properties.Resources.gain_zoom_64;
            odr.Click += delegate
            {
                SizeView sv = new SizeView();
                sv.ShowDialog();
            };

            Catalog detail = RoleAclSource.Instance["系統"]["功能按鈕"];
            detail.Add(new ReportFeature(Permissions.系統容量檢視, "系統容量檢視"));
        }
    }
}
