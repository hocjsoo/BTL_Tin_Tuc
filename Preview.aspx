<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Preview.aspx.cs" Inherits="NewsWebsite.Preview" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
            <div class="alert alert-danger">
                <h4>Không tìm thấy bài viết</h4>
                <p>Bài viết không tồn tại hoặc bạn không có quyền xem.</p>
                <a href="ManageNews.aspx" class="btn btn-secondary">← Quay lại</a>
            </div>
        </asp:Panel>
        
        <asp:Panel ID="pnlPreview" runat="server" Visible="false">
            <div class="alert alert-info">
                <strong>👁️ CHẾ ĐỘ XEM TRƯỚC</strong> - Đây là cách bài viết sẽ hiển thị khi được xuất bản.
            </div>
            
            <div class="card shadow-sm">
                <div class="card-body">
                    <div class="text-center mb-4">
                        <h1 id="previewTitle" runat="server"></h1>
                        <div class="text-muted mt-2">
                            <span id="previewCategory" runat="server"></span>
                            <span class="mx-2">•</span>
                            <span id="previewAuthor" runat="server"></span>
                            <span class="mx-2">•</span>
                            <span id="previewDate" runat="server"></span>
                        </div>
                    </div>
                    
                    <div class="text-center mb-4" id="previewImageContainer" runat="server" visible="false">
                        <img id="previewImage" runat="server" src="" class="img-fluid rounded" style="max-height: 500px;" />
                    </div>
                    
                    <div class="mb-4">
                        <p class="lead" id="previewSummary" runat="server"></p>
                    </div>
                    
                    <div id="previewContent" runat="server" style="line-height: 1.8; white-space: pre-wrap;"></div>
                </div>
            </div>
            
            <div class="mt-4 text-center">
                <a id="backLink" href="#" class="btn btn-secondary me-2">← Quay lại chỉnh sửa</a>
                <a href="ManageNews.aspx" class="btn btn-primary">Quản lý tin tức</a>
            </div>
        </asp:Panel>
    </div>
</asp:Content>

