using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        string err = "";
        string id = "";
        if (Session["File"] != null)
        {
            Dictionary<string, string> files = new Dictionary<string, string>();
            files = ((Dictionary<string, string>)Session["File"]);
            try
            {
                if (files.ContainsKey("photo"))
                {
                    string filePath = files["photo"];
                    string FolderName = Tools.GetConfigValue(Tools.KeyVariables.S3Root) + "profile";
                    id = Tools.SaveImage(filePath, FolderName, out err);
                }
                else
                {
                    if (!imgID.Visible)
                    {
                        lblMsg.Text = "Please select your profile picture.";
                        return;
                    }
                }
            }
            catch { }
            Session.Remove("File");
        }
    }
}