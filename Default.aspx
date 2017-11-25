<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Src="~/Controls/UserControl/FileUploader.ascx" TagPrefix="uc1" TagName="FileUploader" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <uc1:FileUploader runat="server" ID="FileUploader" FileKey="logo" Wid="800" Hgt="800" />
            <asp:Button ID="btnSave" runat="server" Text="Upload Image" OnClick="btnSave_Click" />
            <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Size="Small" />&nbsp;<br />
            <asp:Image ID="imgID" runat="server" />
        </div>
    </form>
</body>
</html>
