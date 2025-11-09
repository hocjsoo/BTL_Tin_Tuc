<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="ManageUsers.aspx.cs" Inherits="NewsWebsite.ManageUsers" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Quản lý người dùng</h2>
    <asp:Panel ID="pnlAuth" runat="server" Visible="false">
        <div class="alert alert-warning">Vui lòng <a href="Login.aspx">đăng nhập</a> với quyền Admin để truy cập.</div>
    </asp:Panel>
    <asp:Panel ID="pnlMain" runat="server" Visible="false">
        <asp:Label ID="lblInfo" runat="server" CssClass="text-muted"></asp:Label>
        <hr />
        <asp:Label ID="lblMsg" runat="server" CssClass="text-success"></asp:Label>
        
        <!-- Button to show Add User Form -->
        <div class="mb-3">
            <asp:Button ID="btnShowAddForm" runat="server" Text="+ Thêm người dùng mới" 
                CssClass="btn btn-primary" OnClick="btnShowAddForm_Click" />
        </div>
        
        <!-- Add New User Form -->
        <asp:Panel ID="pnlAddUserForm" runat="server" Visible="false">
        <div class="card shadow-sm mb-4">
            <div class="card-header bg-primary text-white">
                <h5 class="mb-0">Thêm tài khoản người dùng mới</h5>
            </div>
            <div class="card-body">
                <div class="row mb-3">
                    <div class="col-md-3">
                        <label class="form-label fw-bold">Họ tên: <span class="text-danger">*</span></label>
                    </div>
                    <div class="col-md-9">
                        <asp:TextBox ID="txtNewFullName" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvNewFullName" runat="server" 
                            ControlToValidate="txtNewFullName" ErrorMessage="Họ tên không được để trống" 
                            CssClass="text-danger small" Display="Dynamic" ValidationGroup="AddUser" />
                    </div>
                </div>
                
                <div class="row mb-3">
                    <div class="col-md-3">
                        <label class="form-label fw-bold">Email: <span class="text-danger">*</span></label>
                    </div>
                    <div class="col-md-9">
                        <asp:TextBox ID="txtNewEmail" runat="server" CssClass="form-control" TextMode="Email" />
                        <asp:RequiredFieldValidator ID="rfvNewEmail" runat="server" 
                            ControlToValidate="txtNewEmail" ErrorMessage="Email không được để trống" 
                            CssClass="text-danger small" Display="Dynamic" ValidationGroup="AddUser" />
                        <asp:RegularExpressionValidator ID="revNewEmail" runat="server" 
                            ControlToValidate="txtNewEmail" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                            ErrorMessage="Email không hợp lệ" CssClass="text-danger small" Display="Dynamic" ValidationGroup="AddUser" />
                    </div>
                </div>
                
                <div class="row mb-3">
                    <div class="col-md-3">
                        <label class="form-label fw-bold">Mật khẩu: <span class="text-danger">*</span></label>
                    </div>
                    <div class="col-md-9">
                        <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvNewPassword" runat="server" 
                            ControlToValidate="txtNewPassword" ErrorMessage="Mật khẩu không được để trống" 
                            CssClass="text-danger small" Display="Dynamic" ValidationGroup="AddUser" />
                        <small class="text-muted">Tối thiểu 6 ký tự</small>
                    </div>
                </div>
                
                <div class="row mb-3">
                    <div class="col-md-3">
                        <label class="form-label fw-bold">Vai trò: <span class="text-danger">*</span></label>
                    </div>
                    <div class="col-md-9">
                        <asp:DropDownList ID="ddlNewRole" runat="server" CssClass="form-control">
                            <asp:ListItem Value="Viewer" Text="Người xem" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="Editor" Text="Biên tập viên"></asp:ListItem>
                            <asp:ListItem Value="Admin" Text="Quản trị viên"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                
                <div class="row mb-3">
                    <div class="col-md-3">
                        <label class="form-label fw-bold">Giới tính:</label>
                    </div>
                    <div class="col-md-9">
                        <asp:DropDownList ID="ddlNewGender" runat="server" CssClass="form-control">
                            <asp:ListItem Value="" Text="-- Chọn giới tính --"></asp:ListItem>
                            <asp:ListItem Value="Male" Text="Nam"></asp:ListItem>
                            <asp:ListItem Value="Female" Text="Nữ"></asp:ListItem>
                            <asp:ListItem Value="Other" Text="Khác"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                
                <div class="row mb-3">
                    <div class="col-md-3">
                        <label class="form-label fw-bold">Ngày sinh:</label>
                    </div>
                    <div class="col-md-9">
                        <asp:TextBox ID="txtNewDateOfBirth" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                </div>
                
                <div class="d-flex gap-2">
                    <asp:Button ID="btnAddUser" runat="server" Text="Thêm người dùng" 
                        CssClass="btn btn-primary" OnClick="btnAddUser_Click" ValidationGroup="AddUser" />
                    <asp:Button ID="btnCancelAdd" runat="server" Text="Hủy" 
                        CssClass="btn btn-secondary" OnClick="btnCancelAdd_Click" CausesValidation="false" />
                </div>
            </div>
        </div>
        </asp:Panel>
        
        <hr />
        <h5 class="mb-3">Danh sách người dùng</h5>
        
        <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" CssClass="table table-striped"
            DataKeyNames="Id" AllowPaging="true" PageSize="10"
            OnPageIndexChanging="gvUsers_PageIndexChanging"
            OnRowDataBound="gvUsers_RowDataBound"
            OnRowEditing="gvUsers_RowEditing"
            OnRowUpdating="gvUsers_RowUpdating"
            OnRowCancelingEdit="gvUsers_RowCancelingEdit"
            OnRowDeleting="gvUsers_RowDeleting">
            <Columns>
                <asp:BoundField DataField="FullName" HeaderText="Họ tên" ReadOnly="true" />
                <asp:BoundField DataField="Email" HeaderText="Email" ReadOnly="true" />
                <asp:TemplateField HeaderText="Vai trò">
                    <ItemTemplate>
                        <asp:Label ID="lblRole" runat="server" Text='<%# Eval("Role") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control">
                            <asp:ListItem Value="Viewer" Text="Người xem"></asp:ListItem>
                            <asp:ListItem Value="Editor" Text="Biên tập viên"></asp:ListItem>
                            <asp:ListItem Value="Admin" Text="Quản trị viên"></asp:ListItem>
                        </asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="CreatedAt" HeaderText="Ngày tạo" DataFormatString="{0:dd/MM/yyyy HH:mm}" ReadOnly="true" />
                <asp:CommandField ShowEditButton="true" EditText="Sửa quyền" UpdateText="Lưu" CancelText="Hủy" />
                <asp:CommandField ShowDeleteButton="true" DeleteText="Xóa" />
            </Columns>
        </asp:GridView>
        
        <div class="mb-3 mt-4">
            <a href="Admin.aspx" class="btn btn-secondary">← Quay lại quản trị</a>
        </div>
    </asp:Panel>
</asp:Content>

