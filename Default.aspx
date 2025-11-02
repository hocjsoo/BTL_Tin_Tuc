<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="NewsManagement._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container" style="padding-top: 30px; padding-bottom: 30px;">
        <!-- Search Box -->
        <div class="mb-4">
            <div class="input-group" style="max-width: 600px; margin: 0 auto;">
                <asp:TextBox ID="txtSearchHome" runat="server" CssClass="form-control" placeholder="Tìm kiếm bài viết..." />
                <asp:Button ID="btnSearchHome" runat="server" Text="Tìm kiếm" CssClass="btn btn-primary" OnClick="btnSearchHome_Click" />
            </div>
        </div>
        
        <!-- Featured Article -->
        <div class="card mb-5 shadow-sm" id="featuredArticle" style="border: none; position: relative;">
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
                    <div class="card-body p-4">
                        <span class="badge bg-secondary mb-2" id="featuredCategory" runat="server"></span>
                        <h2 class="card-title mb-3" id="featuredTitle" runat="server"></h2>
                        <p class="card-text text-muted" id="featuredSummary" runat="server"></p>
                        <a class="btn btn-primary mt-3" id="featuredLink" runat="server" href="#">Đọc bài viết</a>
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
            <div class="d-flex align-items-center mb-4">
                <div style="width: 4px; height: 40px; background-color: #ff6b35; margin-right: 15px;"></div>
                <h2 class="mb-0">Tin tức mới nhất</h2>
        </div>
        
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
                            <a class="text-primary" href='Details.aspx?id=<%# Eval("Id") %>' style="text-decoration: none;">Đọc thêm →</a>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
            <FooterTemplate></div></FooterTemplate>
        </asp:Repeater>
        
        <!-- Pagination -->
        <div class="d-flex justify-content-between align-items-center mt-4">
            <asp:HyperLink ID="lnkPrev" runat="server" CssClass="btn btn-outline-secondary">« Trang trước</asp:HyperLink>
            <asp:Label ID="lblPageInfo" runat="server" CssClass="text-muted"></asp:Label>
            <asp:HyperLink ID="lnkNext" runat="server" CssClass="btn btn-outline-secondary">Trang sau »</asp:HyperLink>
        </div>
    </div>
    
    <script type="text/javascript">
        // Search on Enter key
        (function() {
            var searchBoxId = '<%= txtSearchHome.ClientID %>';
            var searchButtonId = '<%= btnSearchHome.ClientID %>';
            
            setTimeout(function() {
                var searchBox = document.getElementById(searchBoxId);
                var searchButton = document.getElementById(searchButtonId);
                
                if (searchBox) {
                    searchBox.addEventListener('keypress', function(e) {
                        if (e.key === 'Enter') {
                            e.preventDefault();
                            if (searchButton) {
                                searchButton.click();
                            }
                        }
                    });
                }
            }, 100);
        })();
    </script>
</asp:Content>


