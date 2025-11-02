<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="NewsManagement.Login" %>

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
                                    <a class="nav-link active" id="tabLogin" href="#" onclick="showLogin(); return false;" style="border-radius: 8px; text-align: center; color: #495057;">Đăng nhập</a>
                                </li>
                                <li class="nav-item flex-fill">
                                    <a class="nav-link" id="tabRegister" href="#" onclick="showRegister(); return false;" style="border-radius: 8px; text-align: center; color: #495057;">Đăng ký</a>
                                </li>
                            </ul>
                            
                            <!-- Login Form -->
                            <div id="loginForm">
                                <div class="mb-3">
                                    <label class="form-label fw-bold">Email</label>
                                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="your@email.com" />
                                    <asp:RequiredFieldValidator ID="rfvUser" runat="server" ControlToValidate="txtUsername" ErrorMessage="Nhập email" CssClass="text-danger small" Display="Dynamic" />
                                </div>
                                <div class="mb-3">
                                    <label class="form-label fw-bold">Mật khẩu</label>
                                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="••••••••" />
                                    <asp:RequiredFieldValidator ID="rfvPass" runat="server" ControlToValidate="txtPassword" ErrorMessage="Nhập mật khẩu" CssClass="text-danger small" Display="Dynamic" />
                                </div>
                                <asp:Button ID="btnLogin" runat="server" Text="Đăng nhập" CssClass="btn btn-primary w-100" style="background-color: #2c3e50; border: none; padding: 12px; border-radius: 8px; font-weight: bold;" OnClick="btnLogin_Click" />
                                <asp:Label ID="lblMsg" runat="server" CssClass="text-danger mt-2 d-block text-center"></asp:Label>
                            </div>
                            
                            <!-- Register Form (hidden by default) -->
                            <div id="registerForm" style="display: none;">
                                <div class="mb-3">
                                    <label class="form-label fw-bold">Họ tên</label>
                                    <input type="text" class="form-control" placeholder="Nguyễn Văn A" />
                                </div>
                                <div class="mb-3">
                                    <label class="form-label fw-bold">Email</label>
                                    <input type="email" class="form-control" placeholder="your@email.com" />
                                </div>
                                <div class="mb-3">
                                    <label class="form-label fw-bold">Mật khẩu</label>
                                    <input type="password" class="form-control" placeholder="••••••••" />
                                </div>
                                <div class="mb-3">
                                    <label class="form-label fw-bold">Xác nhận mật khẩu</label>
                                    <input type="password" class="form-control" placeholder="••••••••" />
                                </div>
                                <button type="button" class="btn btn-primary w-100" style="background-color: #2c3e50; border: none; padding: 12px; border-radius: 8px; font-weight: bold;">Đăng ký</button>
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


