<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ImageProcessorShowName.aspx.cs" Inherits="ImageProcessorShowName" %>

<%@ Register Assembly="DevExpress.Web.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="adminAsset/css/bootstrap.min.css" rel="stylesheet" />
    <link href="font-awesome/css/font-awesome.css" rel="stylesheet" />

    <link href="adminAsset/css/animate.css" rel="stylesheet" />
    <link href="adminAsset/css/style.css" rel="stylesheet" />
    <link href="adminAsset/css/DragDrop.css" rel="Stylesheet" />
    <script type="text/javascript">
        function onUploadControlFileUploadComplete(s, e) {
            if (e.isValid)
                document.getElementById("FileName").innerHTML = "File selected, please proceed";
            else
                document.getElementById("FileName").innerHTML = "";
        }
    </script>
</head>
<body class="white-bg">
    <form id="form1" runat="server">
        <dx:ASPxUploadControl ID="UploadControl" ClientInstanceName="image" runat="server" UploadMode="Auto" AutoStartUpload="True" Width="180"
            ShowProgressPanel="True" CssClass="uploadControl" OnFileUploadComplete="UploadControl_FileUploadComplete">
            <AdvancedModeSettings EnableFileList="False" EnableMultiSelect="False" />
            <ValidationSettings MaxFileSize="409600" AllowedFileExtensions=".jpg, .jpeg, .png" ErrorStyle-CssClass="validationMessage" />
            <BrowseButton Text="Choose file" />
            <ProgressBarStyle CssClass="uploadControlProgressBar" />
            <ClientSideEvents
                FileUploadComplete="onUploadControlFileUploadComplete"></ClientSideEvents>
        </dx:ASPxUploadControl>
        <div class="small-font" style="color: red;" id="FileName"></div>
    </form>
</body>
</html>
