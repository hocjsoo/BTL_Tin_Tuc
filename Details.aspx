<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Details.aspx.cs" Inherits="NewsWebsite.Details" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container" style="padding-top: 50px; padding-bottom: 60px;">
        <asp:Panel ID="pnlDetails" runat="server">
            <div class="row justify-content-center">
                <div class="col-md-9">
                    <span class="badge bg-secondary mb-4" id="detailCategory" runat="server">Công nghệ</span>
                    <h1 class="mb-4" style="font-weight: 800; color: var(--text-dark); font-size: 2.5rem; line-height: 1.2;"><asp:Label ID="lblTitle" runat="server" /></h1>
                    
                    <div class="d-flex align-items-center mb-5" style="color: var(--text-medium); font-size: 0.95rem;">
                        <span style="margin-right: 24px; font-weight: 600;"><i class="fas fa-user"></i> <asp:Label ID="lblAuthor" runat="server" /></span>
                        <span style="font-weight: 600;"><i class="fas fa-calendar-alt"></i> <asp:Label ID="lblDate" runat="server" /></span>
                    </div>
                    
                    <!-- Hero Image -->
                    <div class="mb-5" style="border-radius: 20px; overflow: hidden; box-shadow: var(--shadow-lg);">
                    <asp:Literal ID="litImage" runat="server" />
                    </div>
                    
                    <p class="lead mb-5" style="font-size: 1.3rem; line-height: 1.8; color: var(--text-medium); font-weight: 500;"><asp:Label ID="lblSummary" runat="server" /></p>
                    
                    <div class="article-content mb-5" style="line-height: 1.9; color: var(--text-dark); font-size: 1.15rem; font-weight: 400;">
                        <asp:Literal ID="litContent" runat="server" />
                    </div>
                    
                    <div class="mt-5 mb-5">
                        <a class="btn btn-outline-secondary" href="Default.aspx" style="padding: 12px 28px; font-weight: 600;"><i class="fas fa-arrow-left"></i> Quay lại trang chủ</a>
                    </div>
                    
                    <!-- Related Articles Section -->
                    <div class="mt-5 pt-5" style="border-top: 2px solid var(--border-color);">
                        <h3 class="section-title" style="font-size: 1.75rem;">Bài viết liên quan</h3>
                        <asp:Repeater ID="rptRelatedArticles" runat="server" OnItemDataBound="rptRelatedArticles_ItemDataBound">
                            <HeaderTemplate><div class="row"></HeaderTemplate>
                            <ItemTemplate>
                                <div class="col-md-4 mb-4">
                                    <div class="card h-100 shadow-sm card-article" style="border: none; transition: transform 0.2s;">
                                        <asp:Literal ID="litRelatedImage" runat="server" />
                                        <div class="card-body">
                                            <span class="badge bg-secondary mb-2"><%# Eval("Category") ?? "News" %></span>
                                            <h5 class="card-title"><%# Eval("Title") ?? "Không có tiêu đề" %></h5>
                                            <p class="card-text text-muted" style="font-size: 0.9rem;"><%# Eval("Summary") ?? "" %></p>
                                        </div>
                                        <div class="card-footer bg-white d-flex justify-content-between align-items-center">
                                            <small class="text-muted">
                                                <%# Eval("Author") ?? "Không rõ" %> | 
                                                <%# ((DateTime)Eval("CreatedAt")) != DateTime.MinValue 
                                                    ? ((DateTime)Eval("CreatedAt")).ToLocalTime().ToString("dd/MM/yyyy") 
                                                    : "N/A" %>
                                            </small>
                                            <asp:HyperLink ID="lnkRelatedArticle" runat="server" CssClass="text-primary" NavigateUrl='<%# Eval("Id") != null ? "Details.aspx?id=" + Eval("Id").ToString() : "#" %>' style="text-decoration: none;"><i class="fas fa-arrow-right"></i> Đọc thêm</asp:HyperLink>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                            <FooterTemplate></div></FooterTemplate>
                        </asp:Repeater>
                        <asp:Label ID="lblNoRelatedArticles" runat="server" Text="Không có bài viết liên quan." CssClass="text-muted" Visible="false"></asp:Label>
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
                        <a class="btn btn-primary" href="Default.aspx"><i class="fas fa-arrow-left"></i> Quay lại trang chủ</a>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>


