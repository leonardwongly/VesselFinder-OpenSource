<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VesselFinder._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-sm">
           <h3>VesselFinder</h3>
            <br />
            <asp:Button ID="btnProcess" runat="server" OnClick="btnProcess_Click" CssClass="btn btn-primary" Text="Process" />
            <br /><br />
            <asp:Label ID="lblMsg" runat="server" Font-Bold="true" Font-Size="Large"></asp:Label>
            <asp:Label ID="lblTxt" runat="server" Visible="false"></asp:Label>
        </div>
        
    </div>

</asp:Content>
