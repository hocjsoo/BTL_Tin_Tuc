<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Details.aspx.cs" Inherits="NewsManagement.Details" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container" style="padding-top: 40px; padding-bottom: 40px;">
        <asp:Panel ID="pnlDetails" runat="server">
            <div class="row justify-content-center">
                <div class="col-md-8">
                    <span class="badge bg-secondary mb-3" id="detailCategory" runat="server">Công nghệ</span>
                    <h1 class="mb-4" style="font-weight: bold; color: #2c3e50;"><asp:Label ID="lblTitle" runat="server" /></h1>
                    
                    <div class="d-flex align-items-center text-muted mb-4">
                        <span style="margin-right: 20px;">👤 <asp:Label ID="lblAuthor" runat="server" /></span>
                        <span>📅 <asp:Label ID="lblDate" runat="server" /></span>
                    </div>
                    
                    <!-- Hero Image -->
                    <asp:Literal ID="litImage" runat="server" />
                    
                    <p class="lead text-muted mb-4"><asp:Label ID="lblSummary" runat="server" /></p>
                    
                    <div class="article-content" style="line-height: 1.8; color: #495057; font-size: 1.1rem;">
                        <asp:Literal ID="litContent" runat="server" />
                    </div>
                    
                    <div class="mt-5">
                        <a class="btn btn-outline-secondary" href="Default.aspx">« Quay lại trang chủ</a>
                    </div>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
            <div class="row justify-content-center">
                <div class="col-md-6">
                    <div class="alert alert-warning text-center">
                        <h4>Không tìm thấy bài viết</h4>
                        <p>Bài viết bạn đang tìm kiếm không tồn tại hoặc đã bị xóa.</p>
                        <a class="btn btn-primary" href="Default.aspx">Quay lại trang chủ</a>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>


