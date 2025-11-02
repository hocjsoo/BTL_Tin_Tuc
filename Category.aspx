<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Category.aspx.cs" Inherits="NewsManagement.Category" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container" style="padding-top: 30px; padding-bottom: 30px;">
        <div class="d-flex align-items-center mb-4">
            <div style="width: 4px; height: 40px; background-color: #ff6b35; margin-right: 15px;"></div>
            <h2 class="mb-0">| Chuyên mục: <asp:Label ID="lblCategoryName" runat="server" /></h2>
        </div>
        
        <asp:Repeater ID="rptNews" runat="server" OnItemDataBound="rptNews_ItemDataBound">
            <HeaderTemplate><div class="row"></HeaderTemplate>
            <ItemTemplate>
                <div class="col-md-6 mb-4">
                    <div class="card shadow-sm card-article" style="border: none;">
                        <asp:Literal ID="litImage" runat="server" />
                        <div class="card-body">
                            <span class="badge bg-secondary mb-2"><%# Eval("Category") %></span>
                            <h5 class="card-title"><%# Eval("Title") %></h5>
                            <p class="card-text text-muted"><%# Eval("Summary") %></p>
                        </div>
                        <div class="card-footer bg-white d-flex justify-content-between align-items-center">
                            <small class="text-muted"><%# Eval("Author") %> | <%# ((DateTime)Eval("CreatedAt")).ToLocalTime().ToString("dd/MM/yyyy") %></small>
                            <a class="text-primary" href='Details.aspx?id=<%# Eval("Id") %>' style="text-decoration: none;">Đọc thêm →</a>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
            <FooterTemplate></div></FooterTemplate>
        </asp:Repeater>
        
        <div id="pnlEmpty" runat="server" visible="false" class="text-center py-5">
            <p class="text-muted">Chưa có bài viết nào trong chuyên mục này.</p>
            <a href="Default.aspx" class="btn btn-primary">Quay lại trang chủ</a>
        </div>
    </div>
</asp:Content>

