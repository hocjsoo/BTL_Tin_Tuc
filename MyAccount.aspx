<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="MyAccount.aspx.cs" Inherits="NewsWebsite.MyAccount" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Quản lý tài khoản</h2>
        <asp:Panel ID="pnlAuth" runat="server" Visible="false">
            <div class="alert alert-warning">Vui lòng <a href="Login.aspx">đăng nhập</a> để truy cập.</div>
        </asp:Panel>
        <asp:Panel ID="pnlMain" runat="server" Visible="false">
            <asp:Label ID="lblInfo" runat="server" CssClass="text-muted"></asp:Label>
            <hr />
            <asp:Label ID="lblMsg" runat="server" CssClass="text-success"></asp:Label>
            
            <div class="card shadow-sm mt-4">
                <div class="card-body">
                    <h4 class="card-title mb-4">Thông tin tài khoản</h4>
                    
                    <div class="row mb-3">
                        <div class="col-md-3">
                            <label class="form-label fw-bold">Họ tên:</label>
                        </div>
                        <div class="col-md-9">
                            <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvFullName" runat="server" 
                                ControlToValidate="txtFullName" ErrorMessage="Họ tên không được để trống" 
                                CssClass="text-danger small" Display="Dynamic" ValidationGroup="UpdateAccount" />
                        </div>
                    </div>
                    
                    <div class="row mb-3">
                        <div class="col-md-3">
                            <label class="form-label fw-bold">Email:</label>
                        </div>
                        <div class="col-md-9">
                            <asp:Label ID="lblEmail" runat="server" CssClass="form-control-plaintext fw-bold"></asp:Label>
                            <small class="text-muted">Email không thể thay đổi</small>
                        </div>
                    </div>
                    
                    <div class="row mb-3">
                        <div class="col-md-3">
                            <label class="form-label fw-bold">Vai trò:</label>
                        </div>
                        <div class="col-md-9">
                            <asp:Label ID="lblRole" runat="server" CssClass="form-control-plaintext fw-bold"></asp:Label>
                            <small class="text-muted">Vai trò chỉ có thể thay đổi bởi quản trị viên</small>
                        </div>
                    </div>
                    
                    <div class="row mb-3">
                        <div class="col-md-3">
                            <label class="form-label fw-bold">Ngày tạo:</label>
                        </div>
                        <div class="col-md-9">
                            <asp:Label ID="lblCreatedAt" runat="server" CssClass="form-control-plaintext"></asp:Label>
                        </div>
                    </div>
                    
                    <!-- Gender and DateOfBirth - Hidden for Admin -->
                    <asp:Panel ID="pnlPersonalInfo" runat="server" Visible="false">
                        <hr class="my-4" />
                        <h5 class="mb-3">Thông tin cá nhân</h5>
                        
                        <div class="row mb-3">
                            <div class="col-md-3">
                                <label class="form-label fw-bold">Giới tính:</label>
                            </div>
                            <div class="col-md-9">
                                <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control">
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
                                <asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                <small class="text-muted">Định dạng: DD/MM/YYYY</small>
                            </div>
                        </div>
                    </asp:Panel>
                    
                    <hr class="my-4" />
                    
                    <h5 class="mb-3">Đổi mật khẩu</h5>
                    <div class="row mb-3">
                        <div class="col-md-3">
                            <label class="form-label fw-bold">Mật khẩu hiện tại:</label>
                        </div>
                        <div class="col-md-9">
                            <asp:TextBox ID="txtCurrentPassword" runat="server" TextMode="Password" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvCurrentPassword" runat="server" 
                                ControlToValidate="txtCurrentPassword" ErrorMessage="Nhập mật khẩu hiện tại" 
                                CssClass="text-danger small" Display="Dynamic" ValidationGroup="ChangePassword" />
                        </div>
                    </div>
                    
                    <div class="row mb-3">
                        <div class="col-md-3">
                            <label class="form-label fw-bold">Mật khẩu mới:</label>
                        </div>
                        <div class="col-md-9">
                            <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvNewPassword" runat="server" 
                                ControlToValidate="txtNewPassword" ErrorMessage="Nhập mật khẩu mới" 
                                CssClass="text-danger small" Display="Dynamic" ValidationGroup="ChangePassword" />
                        </div>
                    </div>
                    
                    <div class="row mb-4">
                        <div class="col-md-3">
                            <label class="form-label fw-bold">Xác nhận mật khẩu mới:</label>
                        </div>
                        <div class="col-md-9">
                            <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" 
                                ControlToValidate="txtConfirmPassword" ErrorMessage="Xác nhận mật khẩu mới" 
                                CssClass="text-danger small" Display="Dynamic" ValidationGroup="ChangePassword" />
                            <asp:CompareValidator ID="cvPassword" runat="server" 
                                ControlToValidate="txtConfirmPassword" ControlToCompare="txtNewPassword" 
                                Operator="Equal" ErrorMessage="Mật khẩu xác nhận không khớp" 
                                CssClass="text-danger small" Display="Dynamic" ValidationGroup="ChangePassword" />
                        </div>
                    </div>
                    
                    <div class="d-flex gap-2">
                        <asp:Button ID="btnUpdateInfo" runat="server" Text="Cập nhật thông tin" 
                            CssClass="btn btn-primary" OnClick="btnUpdateInfo_Click" ValidationGroup="UpdateAccount" />
                        <asp:Button ID="btnChangePassword" runat="server" Text="Đổi mật khẩu" 
                            CssClass="btn btn-warning" OnClick="btnChangePassword_Click" ValidationGroup="ChangePassword" />
                        <a href="Admin.aspx" class="btn btn-secondary">← Quay lại</a>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>

