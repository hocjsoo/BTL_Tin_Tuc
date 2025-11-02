<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Admin.aspx.cs" Inherits="NewsManagement.Admin" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Quản trị tin tức</h2>
    <asp:Panel ID="pnlAuth" runat="server" Visible="false">
        <div class="alert alert-warning">Vui lòng <a href="Login.aspx">đăng nhập</a> để truy cập.</div>
    </asp:Panel>
    <asp:Panel ID="pnlMain" runat="server" Visible="false">
        <asp:Label ID="lblInfo" runat="server" CssClass="text-muted"></asp:Label>
        <hr />
        <asp:GridView ID="gvNews" runat="server" AutoGenerateColumns="false" CssClass="table table-striped"
            DataKeyNames="Id" AllowPaging="true" PageSize="10"
            OnPageIndexChanging="gvNews_PageIndexChanging"
            OnRowDataBound="gvNews_RowDataBound"
            OnRowDeleting="gvNews_RowDeleting" OnSelectedIndexChanged="gvNews_SelectedIndexChanged">
            <Columns>
                <asp:BoundField DataField="Title" HeaderText="Tiêu đề" />
                <asp:BoundField DataField="Category" HeaderText="Chuyên mục" />
                <asp:BoundField DataField="Author" HeaderText="Tác giả" />
                <asp:BoundField DataField="CreatedAt" HeaderText="Ngày" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <asp:CommandField ShowSelectButton="true" SelectText="Sửa" />
                <asp:CommandField ShowDeleteButton="true" DeleteText="Xóa" />
            </Columns>
        </asp:GridView>

        <div class="mb-3 mt-4">
            <asp:Button ID="btnAddNew" runat="server" Text="➕ Thêm bài viết mới" CssClass="btn btn-primary" OnClick="btnAddNew_Click" />
            <a href="Default.aspx" class="btn btn-secondary ms-2">← Xem trang chủ</a>
        </div>

        <!-- Form Thêm/Sửa bài viết - chỉ hiển thị khi chọn Thêm hoặc Sửa -->
        <asp:Panel ID="pnlForm" runat="server" Visible="false">
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h4 id="formTitle" runat="server">Thêm/Sửa bài viết</h4>
            </div>
            <asp:HiddenField ID="hfId" runat="server" />
            <div class="mb-3">
                <label class="form-label">Tiêu đề</label>
                <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" />
            </div>
            <div class="row mb-3">
                <div class="col-md-6">
                    <label class="form-label">Chuyên mục</label>
                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control">
                        <asp:ListItem Value="">-- Chọn chuyên mục --</asp:ListItem>
                        <asp:ListItem Value="Công nghệ">Công nghệ</asp:ListItem>
                        <asp:ListItem Value="Kinh tế">Kinh tế</asp:ListItem>
                        <asp:ListItem Value="Du lịch">Du lịch</asp:ListItem>
                        <asp:ListItem Value="Sức khỏe">Sức khỏe</asp:ListItem>
                        <asp:ListItem Value="Văn hóa">Văn hóa</asp:ListItem>
                        <asp:ListItem Value="Ẩm thực">Ẩm thực</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-6">
                    <label class="form-label">Hình ảnh</label>
                    <asp:FileUpload ID="fileUploadImage" runat="server" CssClass="form-control" accept="image/*" />
                    <small class="text-muted">Chọn ảnh từ máy tính (JPG, PNG, GIF)</small>
                </div>
            </div>
            <div class="mb-3" id="pnlCurrentImage" runat="server" visible="false">
                <label class="form-label">Ảnh hiện tại</label>
                <div>
                    <img id="imgCurrent" runat="server" src="" style="max-width: 300px; max-height: 200px; border-radius: 5px; border: 1px solid #ddd;" />
                </div>
            </div>
            <div class="mb-3">
                <label class="form-label">Tóm tắt</label>
                <asp:TextBox ID="txtSummary" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
            </div>
            <div class="mb-3">
                <label class="form-label">Nội dung</label>
                <asp:TextBox ID="txtContent" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="6" />
            </div>
            <div class="mb-3">
                <asp:Button ID="btnSave" runat="server" Text="Lưu" CssClass="btn btn-success" OnClick="btnSave_Click" />
                <asp:Button ID="btnReset" runat="server" Text="Làm mới" CssClass="btn btn-secondary ms-2" OnClick="btnReset_Click" />
                <asp:Label ID="lblMsg" runat="server" CssClass="ms-3 text-success"></asp:Label>
            </div>
        </asp:Panel>
    </asp:Panel>
</asp:Content>


