<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="NewsWebsite.Login" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div style="min-height: 70vh; display: flex; align-items: center; justify-content: center; background: linear-gradient(to bottom, #e3f2fd, white); padding: 40px 0;">
        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-5">
                    <div class="card shadow-lg" style="border: none; border-radius: 15px;">
                        <div class="card-body p-5">
                            <div class="text-center mb-4">
                                <div style="font-size: 3rem; color: #2c3e50; margin-bottom: 10px;">📰</div>
                                <h2 class="mb-2" style="color: #2c3e50; font-weight: bold;">Tin Tức 24/7</h2>
                                <p class="text-muted">Đăng nhập hoặc tạo tài khoản mới</p>
                            </div>
                            
                            <!-- Tabs -->
                            <ul class="nav nav-pills mb-4" style="border-radius: 8px; background-color: #f0f0f0; padding: 5px;">
                                <li class="nav-item flex-fill">
                                    <a class="nav-link active" id="tabLogin" href="javascript:void(0);" onclick="showLogin(); return false;" style="border-radius: 8px; text-align: center; color: #495057;">Đăng nhập</a>
                                </li>
                                <li class="nav-item flex-fill">
                                    <a class="nav-link" id="tabRegister" href="javascript:void(0);" onclick="showRegister(); return false;" style="border-radius: 8px; text-align: center; color: #495057;">Đăng ký</a>
                                </li>
                            </ul>
                            
                            <!-- Login Form -->
                            <div id="loginForm">
                                <asp:ValidationSummary ID="vsLogin" runat="server" CssClass="text-danger small mb-2" ValidationGroup="LoginGroup" DisplayMode="List" ShowSummary="true" />
                                <div class="mb-3">
                                    <label class="form-label fw-bold">Email</label>
                                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="your@email.com" />
                                    <asp:RequiredFieldValidator ID="rfvUser" runat="server" ControlToValidate="txtUsername" ErrorMessage="Nhập email" CssClass="text-danger small" Display="Dynamic" ValidationGroup="LoginGroup" />
                                </div>
                                <div class="mb-3">
                                    <label class="form-label fw-bold">Mật khẩu</label>
                                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="••••••••" />
                                    <asp:RequiredFieldValidator ID="rfvPass" runat="server" ControlToValidate="txtPassword" ErrorMessage="Nhập mật khẩu" CssClass="text-danger small" Display="Dynamic" ValidationGroup="LoginGroup" />
                                </div>
                                <asp:Button ID="btnLogin" runat="server" Text="Đăng nhập" CssClass="btn btn-primary w-100" style="background-color: #2c3e50; border: none; padding: 12px; border-radius: 8px; font-weight: bold;" OnClick="btnLogin_Click" ValidationGroup="LoginGroup" />
                                <asp:Label ID="lblMsg" runat="server" CssClass="text-danger mt-2 d-block text-center"></asp:Label>
                            </div>
                            
                            <!-- Register Form (hidden by default) -->
                            <div id="registerForm" style="display: none;">
                                <asp:ValidationSummary ID="vsRegister" runat="server" CssClass="text-danger small mb-2" ValidationGroup="RegisterGroup" DisplayMode="List" ShowSummary="true" />
                                <div class="mb-3">
                                    <label class="form-label fw-bold">Họ tên</label>
                                    <asp:TextBox ID="txtRegFullName" runat="server" CssClass="form-control" placeholder="Nguyễn Văn A" />
                                    <asp:RequiredFieldValidator ID="rfvRegFullName" runat="server" ControlToValidate="txtRegFullName" ErrorMessage="Nhập họ tên" CssClass="text-danger small" Display="Dynamic" ValidationGroup="RegisterGroup" />
                                </div>
                                <div class="mb-3">
                                    <label class="form-label fw-bold">Email</label>
                                    <asp:TextBox ID="txtRegEmail" runat="server" CssClass="form-control" placeholder="your@email.com" TextMode="Email" />
                                    <asp:RequiredFieldValidator ID="rfvRegEmail" runat="server" ControlToValidate="txtRegEmail" ErrorMessage="Nhập email" CssClass="text-danger small" Display="Dynamic" ValidationGroup="RegisterGroup" />
                                    <asp:RegularExpressionValidator ID="revRegEmail" runat="server" ControlToValidate="txtRegEmail" 
                                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                                        ErrorMessage="Email không hợp lệ" CssClass="text-danger small" Display="Dynamic" ValidationGroup="RegisterGroup" />
                                </div>
                                <div class="mb-3">
                                    <label class="form-label fw-bold">Mật khẩu</label>
                                    <asp:TextBox ID="txtRegPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="••••••••" />
                                    <asp:RequiredFieldValidator ID="rfvRegPassword" runat="server" ControlToValidate="txtRegPassword" ErrorMessage="Nhập mật khẩu" CssClass="text-danger small" Display="Dynamic" ValidationGroup="RegisterGroup" />
                                </div>
                                <div class="mb-3">
                                    <label class="form-label fw-bold">Xác nhận mật khẩu</label>
                                    <asp:TextBox ID="txtRegConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="••••••••" />
                                    <asp:RequiredFieldValidator ID="rfvRegConfirmPassword" runat="server" ControlToValidate="txtRegConfirmPassword" ErrorMessage="Xác nhận mật khẩu" CssClass="text-danger small" Display="Dynamic" ValidationGroup="RegisterGroup" />
                                    <asp:CompareValidator ID="cvRegPassword" runat="server" ControlToValidate="txtRegConfirmPassword" 
                                        ControlToCompare="txtRegPassword" Operator="Equal" 
                                        ErrorMessage="Mật khẩu không khớp" CssClass="text-danger small" Display="Dynamic" ValidationGroup="RegisterGroup" />
                                </div>
                                <asp:Button ID="btnRegister" runat="server" Text="Đăng ký" CssClass="btn btn-primary w-100" style="background-color: #2c3e50; border: none; padding: 12px; border-radius: 8px; font-weight: bold;" OnClick="btnRegister_Click" ValidationGroup="RegisterGroup" />
                                <asp:Label ID="lblRegMsg" runat="server" CssClass="text-danger mt-2 d-block text-center"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <script>
        function showLogin() {
            document.getElementById('loginForm').style.display = 'block';
            document.getElementById('registerForm').style.display = 'none';
            document.getElementById('tabLogin').classList.add('active');
            document.getElementById('tabRegister').classList.remove('active');
        }
        function showRegister() {
            document.getElementById('loginForm').style.display = 'none';
            document.getElementById('registerForm').style.display = 'block';
            document.getElementById('tabLogin').classList.remove('active');
            document.getElementById('tabRegister').classList.add('active');
        }
    </script>
</asp:Content>


