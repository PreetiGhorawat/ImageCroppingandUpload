<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ImageProcessor.aspx.cs" Inherits="ImageProcessor" %>

<%@ Register Assembly="DevExpress.Web.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Src="~/Controls/UserControl/ImageCrop.ascx" TagPrefix="uc1" TagName="ImageCrop" %>

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
            if (e.isValid) {
                document.getElementById("uploadedImage").src = "UploadedFiles/" + e.callbackData;
                document.getElementById("errorMsg").innerHTML = "";
                var field = 'wid';
                var url = window.location.href;
                var key = getQueryString('key', url);
                var windowName = "popConfirm" + key;
                if (url.indexOf('?' + field + '=') != -1) {
                    var p = window.parent;
                    var popup = p.window[windowName];
                    popup.PerformWindowCallback();
                    popup.Show();
                }
                else if (url.indexOf('&' + field + '=') != -1) {
                    var p = window.parent;
                    var popup = p.window[windowName];
                    popup.PerformWindowCallback();
                    popup.Show();
                }
            }
            else {
                document.getElementById("errorMsg").innerHTML = e.callbackData;
            }
            setElementVisible("uploadedImage", e.isValid);
        }
        function onImageLoad() {
            setElementVisible("dragZone", false);
        }
        function setElementVisible(elementId, visible) {
            document.getElementById(elementId).className = visible ? "" : "hidden";
        }
        var getQueryString = function (field, url) {
            var href = url ? url : window.location.href;
            var reg = new RegExp('[?&]' + field + '=([^&#]*)', 'i');
            var string = reg.exec(href);
            return string ? string[1] : null;
        };
    </script>
</head>
<body class="white-bg">
    <form id="form1" runat="server">
        <div class="img-wrap" visible="false" id="divImg" runat="server">
            <span class="close">
                <asp:Button ID="btnDelete" runat="server" Text="Remove" CssClass="btn btn-small btn-yellow" CommandName="2" OnClick="btnDelete_Click" /></span>
            <img id="newuploadedImage" runat="server" width="160" visible="false" src="#" alt=""/>
        </div>
        <%--        <img id="newuploadedImage" runat="server" src="#" alt="" width="160" visible="false" />--%>
        <div id="div" runat="server">
            <div id="externalDropZone" class="dropZoneExternal">
                <div id="errorMsg" style="color: red; font-size: 12px; width: 100%; background-color: #FFC0CB"></div>
                <div id="dragZone">
                    <span class="dragZoneText">Drag an image or click to select<br />
                        <span style="font-size: 12px">
                            (Max. file size: 400MB) <br/>(File format: jpg/png)
                            <asp:Literal ID="lblSize" runat="server" /></span></span>
                </div>
                <img id="uploadedImage" src="#" class="hidden" alt="" onload="onImageLoad()" width="160" />
                <div id="dropZone" class="hidden">
                    <span class="dropZoneText">Drag an image or click to select</span>
                </div>
            </div>
        </div>
        <dx:ASPxUploadControl ID="UploadControl" ClientInstanceName="image" runat="server" UploadMode="Auto" AutoStartUpload="True" Width="180"
            ShowProgressPanel="True" CssClass="uploadControl" DialogTriggerID="externalDropZone" OnFileUploadComplete="UploadControl_FileUploadComplete">
            <AdvancedModeSettings EnableDragAndDrop="True" EnableFileList="False" EnableMultiSelect="False" ExternalDropZoneID="externalDropZone" DropZoneText="" />
            <ValidationSettings MaxFileSize="409600" AllowedFileExtensions=".jpg, .jpeg, .png" ErrorStyle-CssClass="validationMessage" />
            <BrowseButton Text="Choose file" />
            <DropZoneStyle CssClass="uploadControlDropZone" />
            <ProgressBarStyle CssClass="uploadControlProgressBar" />
            <ClientSideEvents
                DropZoneEnter="function(s, e) { if(e.dropZone.id == 'externalDropZone') setElementVisible('dropZone', true); }"
                DropZoneLeave="function(s, e) { if(e.dropZone.id == 'externalDropZone') setElementVisible('dropZone', false); }"
                FileUploadComplete="onUploadControlFileUploadComplete"></ClientSideEvents>
        </dx:ASPxUploadControl>
    </form>
</body>
</html>
