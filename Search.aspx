<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="NewsWebsite.Search" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container" style="padding-top: 40px; padding-bottom: 50px;">
        <div class="mb-4">
            <h2 class="section-title mb-4">Tìm kiếm tin tức</h2>
            
            <div class="input-group mb-4" style="max-width: 700px;">
                <span class="input-group-text" style="background: white; border-right: none;"><i class="fas fa-search"></i></span>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Nhập từ khóa tìm kiếm..." AutoPostBack="false" style="border-left: none; border-right: none;" />
                <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
            </div>
            
            <asp:Label ID="lblMessage" runat="server" CssClass="text-muted mb-3" Visible="false"></asp:Label>
            
            <asp:Repeater ID="rptSearchResults" runat="server" OnItemDataBound="rptSearchResults_ItemDataBound">
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
            
            <asp:Panel ID="pnlNoResults" runat="server" Visible="false">
                <div class="alert alert-info text-center">
                    <h5><i class="fas fa-search"></i> Không tìm thấy kết quả</h5>
                    <p>Không có bài viết nào phù hợp với từ khóa bạn tìm kiếm.</p>
                </div>
            </asp:Panel>
        </div>
    </div>
    
    <script type="text/javascript">
        // Add icon to search button
        (function() {
            var searchButtonId = '<%= btnSearch.ClientID %>';
            setTimeout(function() {
                var searchButton = document.getElementById(searchButtonId);
                if (searchButton) {
                    var icon = document.createElement('i');
                    icon.className = 'fas fa-search';
                    icon.style.marginRight = '6px';
                    searchButton.insertBefore(icon, searchButton.firstChild);
                }
            }, 100);
        })();
        
        // Auto-search on typing
        (function() {
            var searchBoxId = '<%= txtSearch.ClientID %>';
            var searchButtonId = '<%= btnSearch.ClientID %>';
            var searchButtonUniqueId = '<%= btnSearch.UniqueID %>';
            
            setTimeout(function() {
                var searchBox = document.getElementById(searchBoxId);
                var searchButton = document.getElementById(searchButtonId);
                var searchTimer;
                
                if (searchBox) {
                    searchBox.addEventListener('keyup', function(e) {
                        clearTimeout(searchTimer);
                        
                        // Debounce search - wait 300ms after user stops typing
                        searchTimer = setTimeout(function() {
                            if (searchButton && typeof __doPostBack !== 'undefined') {
                                __doPostBack(searchButtonUniqueId, '');
                            } else if (searchButton) {
                                searchButton.click();
                            }
                        }, 300);
                    });
                    
                    // Also search on Enter key
                    searchBox.addEventListener('keypress', function(e) {
                        if (e.key === 'Enter') {
                            clearTimeout(searchTimer);
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


