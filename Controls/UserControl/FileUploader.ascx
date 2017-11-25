<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FileUploader.ascx.cs" Inherits="Controls_UserControl_FileUploader" %>

<%@ Register Assembly="DevExpress.Web.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Src="~/Controls/UserControl/ImageCrop.ascx" TagPrefix="uc1" TagName="ImageCrop" %>
<asp:Literal ID="lit" runat="server" />
<dx:ASPxPopupControl ID="popConfirm" runat="server" EnableHierarchyRecreation="false" ClientInstanceName="popConfirm" CloseAction="None" ShowCloseButton="false" Width="400px" OnWindowCallback="popConfirm_WindowCallback"
    PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="Middle" HeaderText="Crop Image" Modal="true">
    <ContentCollection>
        <dx:PopupControlContentControl runat="server">
            <asp:Literal ID="litCrop" runat="server" />
        </dx:PopupControlContentControl>
    </ContentCollection>
</dx:ASPxPopupControl>
