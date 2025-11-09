<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Admin.aspx.cs" Inherits="NewsWebsite.Admin" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Quản trị hệ thống</h2>
        <asp:Panel ID="pnlAuth" runat="server" Visible="false">
            <div class="alert alert-warning">Vui lòng <a href="Login.aspx">đăng nhập</a> để truy cập.</div>
        </asp:Panel>
        <asp:Panel ID="pnlMain" runat="server" Visible="false">
            <asp:Label ID="lblInfo" runat="server" CssClass="text-muted"></asp:Label>
            <hr />
            
            <!-- Navigation Cards -->
            <div class="row mb-4">
                <div class="col-md-4 mb-3">
                    <div class="card h-100 shadow-sm">
                        <div class="card-body text-center">
                            <div style="font-size: 3rem; margin-bottom: 10px;">📝</div>
                            <h5 class="card-title">Quản lý Tin tức</h5>
                            <p class="card-text text-muted">Thêm, sửa, xóa bài viết</p>
                            <a href="ManageNews.aspx" class="btn btn-primary">Quản lý</a>
                        </div>
                    </div>
                </div>
                <asp:Panel ID="pnlAdminCard" runat="server" Visible="false">
                    <div class="col-md-4 mb-3">
                        <div class="card h-100 shadow-sm">
                            <div class="card-body text-center">
                                <div style="font-size: 3rem; margin-bottom: 10px;">👥</div>
                                <h5 class="card-title">Quản lý Người dùng</h5>
                                <p class="card-text text-muted">Quản lý tài khoản và phân quyền</p>
                                <a href="ManageUsers.aspx" class="btn btn-info">Quản lý</a>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <div class="col-md-4 mb-3">
                    <div class="card h-100 shadow-sm">
                        <div class="card-body text-center">
                            <div style="font-size: 3rem; margin-bottom: 10px;">⚙️</div>
                            <h5 class="card-title">Tài khoản của tôi</h5>
                            <p class="card-text text-muted">Quản lý thông tin cá nhân</p>
                            <a href="MyAccount.aspx" class="btn btn-success">Xem chi tiết</a>
                        </div>
                    </div>
                </div>
            </div>

            <div class="mb-3 mt-4">
                <a href="Default.aspx" class="btn btn-secondary">← Xem trang chủ</a>
            </div>
    </asp:Panel>
</asp:Content>


