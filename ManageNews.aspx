<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="ManageNews.aspx.cs" Inherits="NewsWebsite.ManageNews" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Quản lý Tin tức</h2>
        <asp:Panel ID="pnlAuth" runat="server" Visible="false">
            <div class="alert alert-warning">Vui lòng <a href="Login.aspx">đăng nhập</a> để truy cập.</div>
        </asp:Panel>
        <asp:Panel ID="pnlMain" runat="server" Visible="false">
            <asp:Label ID="lblInfo" runat="server" CssClass="text-muted"></asp:Label>
            <hr />
            <asp:Label ID="lblMsg" runat="server" CssClass="text-success"></asp:Label>
            
            <!-- Tabs -->
            <ul class="nav nav-tabs mb-3" role="tablist">
                <li class="nav-item" role="presentation">
                    <button class="nav-link active" id="tab-articles" data-bs-toggle="tab" data-bs-target="#panel-articles" type="button" role="tab" aria-controls="panel-articles" aria-selected="true">📰 Quản lý bài viết</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button class="nav-link" id="tab-categories" data-bs-toggle="tab" data-bs-target="#panel-categories" type="button" role="tab" aria-controls="panel-categories" aria-selected="false">📂 Quản lý chuyên mục</button>
                </li>
            </ul>
            
            <!-- Tab Panels -->
            <div class="tab-content">
                <!-- Articles Panel -->
                <div class="tab-pane fade show active" id="panel-articles" role="tabpanel" aria-labelledby="tab-articles">
                    <div class="d-flex justify-content-between align-items-center mb-3 flex-wrap">
                        <h4 class="mb-2 mb-md-0">Danh sách bài viết</h4>
                <asp:Panel ID="pnlEditButtons" runat="server">
                    <a href="AddEditNews.aspx" class="btn btn-primary">➕ Thêm bài viết mới</a>
                </asp:Panel>
            </div>
            
                    <div class="table-responsive">
            <asp:GridView ID="gvNews" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-hover"
                DataKeyNames="Id" AllowPaging="true" PageSize="10"
                OnPageIndexChanging="gvNews_PageIndexChanging"
                OnRowDataBound="gvNews_RowDataBound"
                OnRowDeleting="gvNews_RowDeleting" OnSelectedIndexChanged="gvNews_SelectedIndexChanged">
                <Columns>
                    <asp:BoundField DataField="Title" HeaderText="Tiêu đề" />
                    <asp:BoundField DataField="Category" HeaderText="Chuyên mục" />
                    <asp:BoundField DataField="Author" HeaderText="Tác giả" />
                    <asp:TemplateField HeaderText="Trạng thái">
                        <ItemTemplate>
                            <asp:Label ID="lblStatus" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CreatedAt" HeaderText="Ngày tạo" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:CommandField ShowSelectButton="true" SelectText="Sửa" />
                    <asp:CommandField ShowDeleteButton="true" DeleteText="Xóa" />
                </Columns>
            </asp:GridView>
                    </div>
                </div>
                
                <!-- Categories Panel -->
                <div class="tab-pane fade" id="panel-categories" role="tabpanel" aria-labelledby="tab-categories">
                    <div class="d-flex justify-content-between align-items-center mb-3 flex-wrap">
                        <h4 class="mb-2 mb-md-0">Danh sách chuyên mục</h4>
                        <asp:Panel ID="pnlCategoryButtons" runat="server">
                            <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#categoryModal" onclick="clearCategoryForm(); return false;">➕ Thêm chuyên mục mới</button>
                        </asp:Panel>
                    </div>
                    
                    <div class="table-responsive">
                    <asp:GridView ID="gvCategories" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-hover"
                        DataKeyNames="Id" AllowPaging="true" PageSize="10"
                        OnPageIndexChanging="gvCategories_PageIndexChanging"
                        OnRowDataBound="gvCategories_RowDataBound"
                        OnRowDeleting="gvCategories_RowDeleting" OnRowEditing="gvCategories_RowEditing">
                        <Columns>
                            <asp:BoundField DataField="Name" HeaderText="Tên chuyên mục" />
                            <asp:BoundField DataField="Description" HeaderText="Mô tả" />
                            <asp:BoundField DataField="CreatedAt" HeaderText="Ngày tạo" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                            <asp:CommandField ShowEditButton="true" EditText="Sửa" />
                            <asp:CommandField ShowDeleteButton="true" DeleteText="Xóa" />
                        </Columns>
                    </asp:GridView>
                    </div>
                </div>
            </div>

            <div class="mb-3 mt-4">
                <a href="Admin.aspx" class="btn btn-secondary">← Quay lại quản trị</a>
            </div>
        </asp:Panel>
        
        <!-- Category Modal -->
        <div class="modal fade" id="categoryModal" tabindex="-1" aria-labelledby="categoryModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="categoryModalLabel">Thêm/Sửa chuyên mục</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <asp:HiddenField ID="hfCategoryId" runat="server" />
                        <div class="mb-3">
                            <label class="form-label">Tên chuyên mục <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtCategoryName" runat="server" CssClass="form-control" MaxLength="100" />
                            <asp:RequiredFieldValidator ID="rfvCategoryName" runat="server" 
                                ControlToValidate="txtCategoryName" ErrorMessage="Tên chuyên mục không được để trống" 
                                CssClass="text-danger small" Display="Dynamic" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Mô tả</label>
                            <asp:TextBox ID="txtCategoryDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="500" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
                        <asp:Button ID="btnSaveCategory" runat="server" Text="Lưu" CssClass="btn btn-primary" OnClick="btnSaveCategory_Click" />
                    </div>
                </div>
            </div>
        </div>
        
        <script type="text/javascript">
            function clearCategoryForm() {
                document.getElementById('<%= hfCategoryId.ClientID %>').value = '';
                document.getElementById('<%= txtCategoryName.ClientID %>').value = '';
                document.getElementById('<%= txtCategoryDescription.ClientID %>').value = '';
                document.getElementById('categoryModalLabel').textContent = 'Thêm chuyên mục mới';
            }
            
            function editCategory(id, name, description) {
                document.getElementById('<%= hfCategoryId.ClientID %>').value = id;
                document.getElementById('<%= txtCategoryName.ClientID %>').value = name;
                document.getElementById('<%= txtCategoryDescription.ClientID %>').value = description || '';
                document.getElementById('categoryModalLabel').textContent = 'Sửa chuyên mục';
                var modal = new bootstrap.Modal(document.getElementById('categoryModal'));
                modal.show();
            }
        </script>
    </div>
</asp:Content>

