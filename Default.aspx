<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="NewsWebsite._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container" style="padding-top: 40px; padding-bottom: 50px;">
        <!-- Featured Article -->
        <div class="card mb-5" id="featuredArticle" style="position: relative; background: var(--bg-white);">
            <div class="row g-0">
                <div class="col-md-6" id="heroImageContainer" runat="server" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); min-height: 300px; display: flex; align-items: center; justify-content: center; position: relative; background-size: cover; background-position: center;">
                    <!-- Placeholder when no image -->
                    <div id="heroPlaceholder" runat="server" style="text-align: center; color: white; z-index: 1;">
                        <div style="font-size: 4rem;">📰</div>
                        <p style="margin-top: 20px;">Hình ảnh bài viết</p>
                    </div>
                    <!-- Navigation buttons for hero section -->
                    <button type="button" class="btn btn-light btn-sm" id="btnHeroPrev" runat="server" style="position: absolute; left: 10px; top: 50%; transform: translateY(-50%); opacity: 0.8; border-radius: 50%; width: 40px; height: 40px; font-size: 1.2rem; z-index: 10; box-shadow: 0 2px 5px rgba(0,0,0,0.3);" onserverclick="btnHeroPrev_Click">‹</button>
                    <button type="button" class="btn btn-light btn-sm" id="btnHeroNext" runat="server" style="position: absolute; right: 10px; top: 50%; transform: translateY(-50%); opacity: 0.8; border-radius: 50%; width: 40px; height: 40px; font-size: 1.2rem; z-index: 10; box-shadow: 0 2px 5px rgba(0,0,0,0.3);" onserverclick="btnHeroNext_Click">›</button>
                </div>
                <div class="col-md-6">
                    <div class="card-body p-5">
                        <span class="badge bg-secondary mb-3" id="featuredCategory" runat="server"></span>
                        <h2 class="card-title mb-4" id="featuredTitle" runat="server" style="font-size: 2rem; font-weight: 800; line-height: 1.3;"></h2>
                        <p class="card-text mb-4" id="featuredSummary" runat="server" style="font-size: 1.1rem; line-height: 1.8; color: var(--text-light);"></p>
                        <a class="btn btn-primary mt-3" id="featuredLink" runat="server" href="#" style="padding: 12px 32px; font-size: 1rem;"><i class="fas fa-arrow-right"></i> Đọc bài viết</a>
                        <!-- Hero article indicator -->
                        <div class="mt-3 text-center">
                            <small class="text-muted" id="heroIndicator" runat="server"></small>
                        </div>
                    </div>
                </div>
                <asp:Literal ID="litFeaturedImage" runat="server" />
            </div>
        </div>

        <!-- Latest News Grid -->
        <div class="mb-4">
            <h2 class="section-title">Tin tức mới nhất</h2>
        
        <asp:Repeater ID="rptNews" runat="server" OnItemDataBound="rptNews_ItemDataBound">
            <HeaderTemplate><div class="row"></HeaderTemplate>
            <ItemTemplate>
                <div class="col-md-4 mb-4">
                    <div class="card h-100 shadow-sm card-article" style="border: none;">
                        <asp:Literal ID="litImage" runat="server" />
                        <div class="card-body">
                            <span class="badge bg-secondary mb-2"><%# Eval("Category") ?? "News" %></span>
                            <h5 class="card-title"><%# Eval("Title") %></h5>
                            <p class="card-text text-muted"><%# Eval("Summary") %></p>
                        </div>
                        <div class="card-footer bg-white d-flex justify-content-between align-items-center">
                            <small class="text-muted"><%# Eval("Author") %> | <%# ((DateTime)Eval("CreatedAt")).ToLocalTime().ToString("dd/MM/yyyy") %></small>
                            <a class="text-primary" href='Details.aspx?id=<%# Eval("Id") %>' style="text-decoration: none;"><i class="fas fa-arrow-right"></i> Đọc thêm</a>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
            <FooterTemplate></div></FooterTemplate>
        </asp:Repeater>
        
        <!-- Pagination -->
        <div class="d-flex justify-content-between align-items-center mt-4">
            <asp:HyperLink ID="lnkPrev" runat="server" CssClass="btn btn-outline-secondary"><i class="fas fa-chevron-left"></i> Trang trước</asp:HyperLink>
            <asp:Label ID="lblPageInfo" runat="server" CssClass="text-muted"></asp:Label>
            <asp:HyperLink ID="lnkNext" runat="server" CssClass="btn btn-outline-secondary">Trang sau <i class="fas fa-chevron-right"></i></asp:HyperLink>
        </div>
    </div>
</asp:Content>


