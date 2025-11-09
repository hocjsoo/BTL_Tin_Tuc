<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="AddEditNews.aspx.cs" Inherits="NewsWebsite.AddEditNews" ValidateRequest="false" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2 id="pageTitle" runat="server">Thêm/Sửa bài viết</h2>
        <asp:Panel ID="pnlAuth" runat="server" Visible="false">
            <div class="alert alert-warning">Vui lòng <a href="Login.aspx">đăng nhập</a> với quyền Editor hoặc Admin để truy cập.</div>
        </asp:Panel>
        <asp:Panel ID="pnlMain" runat="server" Visible="false">
            <asp:Label ID="lblMsg" runat="server" CssClass="text-success"></asp:Label>
            
            <div class="card shadow-sm mt-4">
                <div class="card-body">
                    <asp:HiddenField ID="hfId" runat="server" />
                    <div class="mb-3">
                        <label class="form-label fw-bold">Tiêu đề <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvTitle" runat="server" 
                            ControlToValidate="txtTitle" ErrorMessage="Tiêu đề không được để trống" 
                            CssClass="text-danger small" Display="Dynamic" ValidationGroup="SaveNews" />
                    </div>
                    <div class="row mb-3">
                        <div class="col-md-6">
                            <label class="form-label fw-bold">Chuyên mục</label>
                            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label fw-bold">Hình ảnh</label>
                            <asp:FileUpload ID="fileUploadImage" runat="server" CssClass="form-control" accept="image/*" />
                            <small class="text-muted">Chọn ảnh từ máy tính (JPG, PNG, GIF)</small>
                        </div>
                    </div>
                    <div class="mb-3" id="pnlCurrentImage" runat="server" visible="false">
                        <label class="form-label fw-bold">Ảnh hiện tại</label>
                        <div>
                            <img id="imgCurrent" runat="server" src="" style="max-width: 300px; max-height: 200px; border-radius: 5px; border: 1px solid #ddd;" />
                        </div>
                    </div>
                    <div class="mb-3">
                        <label class="form-label fw-bold">Tóm tắt</label>
                        <asp:TextBox ID="txtSummary" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                    </div>
                    <div class="mb-3">
                        <label class="form-label fw-bold">Nội dung <span class="text-danger">*</span></label>
                        <div class="mb-2">
                            <button type="button" class="btn btn-sm btn-outline-secondary" onclick="clearContent()" title="Xóa nội dung">
                                🗑️ Xóa
                            </button>
                            <small class="text-muted ms-2">Có thể dán (Ctrl+V) hoặc kéo thả ảnh trực tiếp vào editor</small>
                        </div>
                        <div class="mb-2">
                            <label class="form-label fw-bold small">Tải ảnh lên nội dung:</label>
                            <asp:FileUpload ID="fileContentImageUpload" runat="server" CssClass="form-control" accept="image/*" />
                            <small class="text-muted">Chọn ảnh từ máy tính (JPG, PNG, GIF - Tối đa 5MB). Ảnh sẽ được thêm vào cuối nội dung khi lưu bài viết.</small>
                        </div>
                        <asp:TextBox ID="txtContent" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="15" style="display: none;" />
                        <div class="editor-toolbar">
                            <button type="button" onclick="formatText('bold'); return false;" title="Bold (Ctrl+B)"><strong>B</strong></button>
                            <button type="button" onclick="formatText('italic'); return false;" title="Italic (Ctrl+I)"><em>I</em></button>
                            <button type="button" onclick="formatText('underline'); return false;" title="Underline (Ctrl+U)"><u>U</u></button>
                            <button type="button" onclick="formatText('justifyLeft'); return false;" title="Căn trái">⬅</button>
                            <button type="button" onclick="formatText('justifyCenter'); return false;" title="Căn giữa">⬌</button>
                            <button type="button" onclick="formatText('justifyRight'); return false;" title="Căn phải">➡</button>
                            <button type="button" onclick="formatText('insertUnorderedList'); return false;" title="Danh sách">• List</button>
                            <button type="button" onclick="formatText('insertOrderedList'); return false;" title="Danh sách số">1. List</button>
                            <button type="button" onclick="formatText('removeFormat'); return false;" title="Xóa định dạng">Clear</button>
                        </div>
                        <div id="editorContent" contenteditable="true"></div>
                        <asp:RequiredFieldValidator ID="rfvContent" runat="server" 
                            ControlToValidate="txtContent" ErrorMessage="Nội dung không được để trống" 
                            CssClass="text-danger small" Display="Dynamic" ValidationGroup="SaveNews" />
                    </div>
                    
                    <div class="mb-3">
                        <label class="form-label fw-bold">Trạng thái hiện tại:</label>
                        <asp:Label ID="lblStatus" runat="server" CssClass="badge bg-secondary ms-2"></asp:Label>
                    </div>
                    
                    <div class="d-flex gap-2 flex-wrap">
                        <button type="button" class="btn btn-sm btn-outline-info" onclick="toggleFullPreview(); return false;">👁️ Xem trước</button>
                        <asp:Button ID="btnPublishNow" runat="server" Text="🚀 XUẤT BẢN NGAY" CssClass="btn btn-success" OnClick="btnPublishNow_Click" ValidationGroup="SaveNews" />
                        <asp:Button ID="btnCancel" runat="server" Text="← Hủy" CssClass="btn btn-secondary" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>
        
        <!-- Full Preview Panel (hidden by default) -->
        <div id="pnlFullPreview" class="mt-4" style="display: none;">
            <div class="card shadow-sm">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <h5 class="mb-0">👁️ Xem trước bài viết</h5>
                    <button type="button" class="btn btn-sm btn-outline-secondary" onclick="toggleFullPreview(); return false;">✕ Đóng</button>
                </div>
                <div class="card-body">
                    <div id="fullPreviewContent" class="article-preview">
                        <!-- Preview content will be inserted here -->
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <style>
        #editorContent {
            min-height: 400px;
            border: 1px solid #ced4da;
            border-radius: 0.375rem;
            background: white;
            padding: 15px;
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
            font-size: 14px;
            line-height: 1.6;
            overflow-y: auto;
        }
        #editorContent:focus {
            outline: 2px solid #007bff;
            outline-offset: -2px;
        }
        #editorContent img {
            max-width: 100%;
            height: auto;
            border-radius: 5px;
            margin: 10px 0;
            display: block;
        }
        #editorContent p {
            margin: 10px 0;
        }
        .editor-toolbar {
            background: #f8f9fa;
            border: 1px solid #ced4da;
            border-bottom: none;
            border-radius: 0.375rem 0.375rem 0 0;
            padding: 8px;
            display: flex;
            flex-wrap: wrap;
            gap: 5px;
        }
        .editor-toolbar button {
            padding: 6px 12px;
            border: 1px solid #ced4da;
            background: white;
            border-radius: 4px;
            cursor: pointer;
            font-size: 13px;
        }
        .editor-toolbar button:hover {
            background: #e9ecef;
        }
        .editor-toolbar button.active {
            background: #007bff;
            color: white;
            border-color: #007bff;
        }
    </style>
    <script type="text/javascript">
        // Simple Rich Text Editor with contenteditable
        var editorInitialized = false;
        
        // Helper function to replace deprecated unescape(encodeURIComponent(text))
        function utf8ToBinaryString(text) {
            try {
                // Modern approach: use TextEncoder if available
                if (typeof TextEncoder !== 'undefined') {
                    var encoder = new TextEncoder();
                    var bytes = encoder.encode(text);
                    var binaryString = '';
                    for (var i = 0; i < bytes.length; i++) {
                        binaryString += String.fromCharCode(bytes[i]);
                    }
                    return binaryString;
                }
                // Fallback: manual UTF-8 encoding via encodeURIComponent
                var encoded = encodeURIComponent(text);
                var binaryString = '';
                for (var i = 0; i < encoded.length; i++) {
                    if (encoded[i] === '%') {
                        var hex = encoded.substr(i + 1, 2);
                        binaryString += String.fromCharCode(parseInt(hex, 16));
                        i += 2;
                    } else if (encoded[i] === '+') {
                        binaryString += ' ';
                    } else {
                        binaryString += encoded[i];
                    }
                }
                return binaryString;
            } catch (e) {
                // Ultimate fallback: return text as-is (may cause issues with non-ASCII)
                return text;
            }
        }
        
        // Helper function to replace deprecated decodeURIComponent(escape(binaryString))
        function binaryStringToUtf8(binaryString) {
            try {
                // Modern approach: use TextDecoder if available
                if (typeof TextDecoder !== 'undefined') {
                    var bytes = new Uint8Array(binaryString.length);
                    for (var i = 0; i < binaryString.length; i++) {
                        bytes[i] = binaryString.charCodeAt(i);
                    }
                    var decoder = new TextDecoder('utf-8');
                    return decoder.decode(bytes);
                }
                // Fallback: manual UTF-8 decoding
                var utf8String = '';
                for (var i = 0; i < binaryString.length; i++) {
                    var charCode = binaryString.charCodeAt(i);
                    if (charCode < 128) {
                        utf8String += String.fromCharCode(charCode);
                    } else {
                        utf8String += '%' + charCode.toString(16).padStart(2, '0');
                    }
                }
                return decodeURIComponent(utf8String);
            } catch (e) {
                // Ultimate fallback: return binary string as-is
                return binaryString;
            }
        }
        
        function initializeEditor() {
            if (editorInitialized) return;
            
            var contentTextarea = document.getElementById('<%= txtContent.ClientID %>');
            var editorDiv = document.getElementById('editorContent');
            
            if (!contentTextarea || !editorDiv) return;
            
            // Load existing content
            if (contentTextarea.value) {
                var initialValue = contentTextarea.value;
                var isServerEncoded = contentTextarea.getAttribute('data-server-encoded') === 'true';
                var decodedValue = initialValue;
                if (isServerEncoded) {
                    try {
                        // Decode base64 and handle UTF-8 properly without deprecated escape/unescape
                        var binaryString = window.atob(initialValue);
                        decodedValue = binaryStringToUtf8(binaryString);
                    } catch (err) {
                        console.error('Failed to decode initial base64 content:', err);
                        decodedValue = initialValue;
                    }
                }
                editorDiv.innerHTML = convertImageTagsToHTML(decodedValue);
            }
            
            // Handle paste events
            editorDiv.addEventListener('paste', function(e) {
                e.preventDefault();
                handlePaste(e);
            });
            
            // Handle drag and drop images directly into editor - upload immediately (like title image)
            editorDiv.addEventListener('dragover', function(e) {
                e.preventDefault();
                e.stopPropagation();
                editorDiv.style.outline = '2px dashed #007bff';
            });
            
            editorDiv.addEventListener('dragleave', function(e) {
                e.preventDefault();
                e.stopPropagation();
                editorDiv.style.outline = '';
            });
            
            editorDiv.addEventListener('drop', function(e) {
                e.preventDefault();
                e.stopPropagation();
                editorDiv.style.outline = '';
                
                var files = e.dataTransfer.files;
                if (files && files.length > 0) {
                    for (var i = 0; i < files.length; i++) {
                        if (files[i].type.indexOf('image/') === 0) {
                            // Upload image immediately (like title image upload)
                            uploadImageFile(files[i]);
                        }
                    }
                }
            });
            
            // Handle content changes
            editorDiv.addEventListener('input', function() {
                syncEditorToTextarea();
            });
            
            // Handle keyboard shortcuts
            editorDiv.addEventListener('keydown', function(e) {
                // Ctrl+B for bold
                if (e.ctrlKey && e.key === 'b') {
                    e.preventDefault();
                    formatText('bold');
                }
                // Ctrl+I for italic
                if (e.ctrlKey && e.key === 'i') {
                    e.preventDefault();
                    formatText('italic');
                }
                // Ctrl+U for underline
                if (e.ctrlKey && e.key === 'u') {
                    e.preventDefault();
                    formatText('underline');
                }
            });
            
            editorInitialized = true;
        }
        
        function handlePaste(e) {
            var clipboardData = e.clipboardData || window.clipboardData;
            if (!clipboardData) return;
            
            var items = clipboardData.items;
            var hasImage = false;
            
            // Check for images first
            for (var i = 0; i < items.length; i++) {
                if (items[i].type.indexOf('image') !== -1) {
                    hasImage = true;
                    var blob = items[i].getAsFile();
                    uploadPastedImage(blob);
                    return;
                }
            }
            
            // If no image, handle text/HTML paste
            var pastedData = clipboardData.getData('text/html') || clipboardData.getData('text/plain');
            if (pastedData) {
                // Clean up Word HTML
                var cleanHTML = cleanWordHTML(pastedData);
                insertHTMLAtCursor(cleanHTML);
            }
        }
        
        function cleanWordHTML(html) {
            if (!html) return '';
            
            // Remove Word-specific conditional comments without regex features that old engines dislike
            var commentStart = html.indexOf('<!--[if');
            while (commentStart !== -1) {
                var commentEnd = html.indexOf('<![endif]-->', commentStart);
                if (commentEnd === -1) {
                    break;
                }
                var afterEnd = commentEnd + '<![endif]-->'.length;
                html = html.slice(0, commentStart) + html.slice(afterEnd);
                commentStart = html.indexOf('<!--[if');
            }
            
            var tempDiv = document.createElement('div');
            tempDiv.innerHTML = html;
            
            // Remove inline style/meta blocks injected by Word
            var cleanupSelectors = ['style', 'meta'];
            for (var s = 0; s < cleanupSelectors.length; s++) {
                var nodes = tempDiv.getElementsByTagName(cleanupSelectors[s]);
                while (nodes.length > 0) {
                    nodes[0].parentNode.removeChild(nodes[0]);
                }
            }
            
            // Remove Word-specific styles and attributes
            var allElements = tempDiv.querySelectorAll('*');
            for (var i = 0; i < allElements.length; i++) {
                var el = allElements[i];
                
                if (el.style) {
                    var styleText = el.getAttribute('style') || '';
                    if (styleText) {
                        var styleParts = styleText.split(';');
                        var normalizedStyles = [];
                        for (var j = 0; j < styleParts.length; j++) {
                            var style = styleParts[j].trim();
                            if (!style) continue;
                            if (style.indexOf('mso-') === 0) continue;
                            if (style.indexOf('panose-') === 0) continue;
                            if (style.indexOf('font-family') === 0) continue;
                            normalizedStyles.push(style);
                        }
                        if (normalizedStyles.length > 0) {
                            el.setAttribute('style', normalizedStyles.join('; '));
                        } else {
                            el.removeAttribute('style');
                        }
                    }
                }
                
                el.removeAttribute('class');
                el.removeAttribute('lang');
                el.removeAttribute('xml:lang');
                el.removeAttribute('o:p');
                
                var tagName = el.tagName ? el.tagName.toLowerCase() : '';
                if (tagName && tagName !== 'img' && tagName !== 'br' && tagName !== 'hr' &&
                    tagName !== 'input' && tagName !== 'meta' && tagName !== 'link') {
                    if (!el.innerHTML || el.innerHTML.trim() === '') {
                        if (el.parentNode) {
                            el.parentNode.removeChild(el);
                            i--;
                        }
                    }
                }
            }
            
            return tempDiv.innerHTML;
        }
        
        function insertHTMLAtCursor(html) {
            var selection = window.getSelection();
            if (selection.rangeCount > 0) {
                var range = selection.getRangeAt(0);
                range.deleteContents();
                
                var tempDiv = document.createElement('div');
                tempDiv.innerHTML = html;
                var frag = document.createDocumentFragment();
                while (tempDiv.firstChild) {
                    frag.appendChild(tempDiv.firstChild);
                }
                range.insertNode(frag);
                
                // Move cursor to end of inserted content
                range.setStartAfter(frag.lastChild || frag);
                range.collapse(true);
                selection.removeAllRanges();
                selection.addRange(range);
            } else {
                // If no selection, append to end
                var editorDiv = document.getElementById('editorContent');
                var tempDiv = document.createElement('div');
                tempDiv.innerHTML = html;
                while (tempDiv.firstChild) {
                    editorDiv.appendChild(tempDiv.firstChild);
                }
            }
        }
        
        function uploadPastedImage(blob) {
            if (!blob) return;
            
            if (blob.size > 5 * 1024 * 1024) {
                alert('Ảnh vượt quá 5MB. Vui lòng chọn ảnh nhỏ hơn.');
                return;
            }
            
            var formData = new FormData();
            formData.append('file', blob, 'pasted-image.png');
            
            var xhr = new XMLHttpRequest();
            xhr.open('POST', 'UploadImage.ashx', true);
            
            // Set timeout (30 seconds)
            xhr.timeout = 30000;
            
            xhr.onload = function() {
                if (xhr.status === 200) {
                    try {
                        var responseText = xhr.responseText.trim();
                        console.log('Paste upload response:', responseText);
                        
                        var response = JSON.parse(responseText);
                        if (response.success) {
                            // Insert image
                            var displaySrc = response.url.replace('~/', '');
                            var img = document.createElement('img');
                            img.src = displaySrc;
                            img.setAttribute('data-original-url', response.url); // Store original URL
                            img.alt = response.fileName || 'Image';
                            img.style.maxWidth = '100%';
                            img.style.height = 'auto';
                            img.style.borderRadius = '5px';
                            img.style.margin = '10px 0';
                            img.style.display = 'block';
                            
                            // Generate image ID
                            var imageId = 'img_' + Date.now() + '_' + Math.random().toString(36).substring(2, 11);
                            img.setAttribute('data-image-id', imageId);
                            
                            insertHTMLAtCursor(img.outerHTML);
                            syncEditorToTextarea();
                        } else {
                            alert('Lỗi upload: ' + (response.message || 'Không xác định'));
                        }
                    } catch (e) {
                        console.error('Parse error:', e, 'Response:', xhr.responseText);
                        alert('Lỗi xử lý phản hồi từ server: ' + e.message);
                    }
                } else {
                    console.error('Upload failed with status:', xhr.status, 'Response:', xhr.responseText);
                    alert('Lỗi upload ảnh (HTTP ' + xhr.status + '). Vui lòng thử lại.');
                }
            };
            
            xhr.onerror = function() {
                console.error('Upload network error');
                alert('Lỗi kết nối. Vui lòng kiểm tra kết nối mạng và thử lại.');
            };
            
            xhr.ontimeout = function() {
                console.error('Upload timeout');
                alert('Upload ảnh quá lâu. Vui lòng kiểm tra kết nối mạng và thử lại.');
            };
            
            xhr.send(formData);
        }
        
        function convertImageTagsToHTML(content) {
            if (!content) return '';
            
            // Convert [IMAGE:id:url:name] tags to HTML img tags
            // Handle data URLs specially - they contain colons and can be very long
            var result = '';
            var lastIndex = 0;
            var startTag = '[IMAGE:';
            
            while (true) {
                var startPos = content.indexOf(startTag, lastIndex);
                if (startPos === -1) {
                    // No more image tags
                    result += content.substring(lastIndex);
                    break;
                }
                
                // Add text before tag
                result += content.substring(lastIndex, startPos);
                
                // Find the end of the tag: ] after the name
                // Format: [IMAGE:id:url:name]
                // For data URLs, url can contain colons, so we need to find the last : before ]
                var tagStart = startPos + startTag.length;
                var idEnd = content.indexOf(':', tagStart);
                if (idEnd === -1) break;
                
                var id = content.substring(tagStart, idEnd);
                var urlStart = idEnd + 1;
                
                // Find the last colon before the closing bracket
                // This is tricky because data URLs contain many colons
                // We'll look for the pattern :name] where name doesn't contain ]
                var urlEnd = -1;
                var searchPos = urlStart;
                var bracketPos = content.indexOf(']', urlStart);
                
                if (bracketPos === -1) break;
                
                // Work backwards from ] to find the last :
                // The name is between the last : and ]
                for (var i = bracketPos - 1; i >= urlStart; i--) {
                    if (content.charAt(i) === ':') {
                        urlEnd = i;
                        break;
                    }
                }
                
                if (urlEnd === -1) {
                    // Malformed tag, skip it
                    lastIndex = startPos + startTag.length;
                    continue;
                }
                
                var url = content.substring(urlStart, urlEnd).trim();
                var name = content.substring(urlEnd + 1, bracketPos).trim();
                
                // Handle data URLs and regular URLs
                var originalUrl = url;
                var displaySrc = originalUrl;
                
                if (originalUrl.indexOf('~/') === 0) {
                    displaySrc = originalUrl.replace('~/', '');
                } else if (originalUrl.indexOf('data:image/') === 0) {
                    // Base64 data URL - use as is, don't modify
                    displaySrc = originalUrl;
                } else if (originalUrl.indexOf('/') !== 0 && originalUrl.indexOf('Images/') === -1) {
                    // Relative path without Images/ - assume it's in Content
                    displaySrc = originalUrl;
                    if (originalUrl.indexOf('~/') !== 0) {
                        originalUrl = '~/Images/Content/' + originalUrl;
                    }
                }
                
                // Escape HTML attributes (but preserve data URL structure)
                var escapedName = name.replace(/"/g, '&quot;').replace(/'/g, '&#39;');
                var escapedSrc = displaySrc;
                var escapedOriginal = originalUrl;
                
                if (displaySrc.indexOf('data:image/') !== 0) {
                    // Only escape non-data URLs
                    escapedSrc = displaySrc.replace(/"/g, '&quot;');
                    escapedOriginal = originalUrl.replace(/"/g, '&quot;');
                }
                
                result += '<img src="' + escapedSrc + '" alt="' + escapedName + '" data-image-id="' + id + '" data-original-url="' + escapedOriginal + '" style="max-width: 100%; height: auto;" />';
                
                lastIndex = bracketPos + 1;
            }
            
            // Convert line breaks
            result = result.replace(/\n/g, '<br>');
            
            return result;
        }
        
        function convertHTMLToImageTags(html) {
            if (!html) return '';
            
            // Convert HTML img tags back to [IMAGE:id:url:name] format
            var tempDiv = document.createElement('div');
            tempDiv.innerHTML = html;
            
            var images = tempDiv.querySelectorAll('img');
            var imageIndex = 0;
            images.forEach(function(img) {
                var imageId = img.getAttribute('data-image-id');
                if (!imageId) {
                    imageId = 'img_' + Date.now() + '_' + imageIndex + '_' + Math.random().toString(36).substring(2, 11);
                }
                imageIndex++;
                
                // Get original URL from data attribute if available, otherwise use src
                var src = img.getAttribute('data-original-url') || img.src || img.getAttribute('src') || '';
                if (src) {
                    src = src.trim();
                }
                var alt = img.alt || img.getAttribute('alt') || 'Image';
                
                if (!src) {
                    return;
                }

                // Convert full URL back to relative path if needed
                if (src.indexOf('http://') === 0 || src.indexOf('https://') === 0) {
                    // Full URL - extract relative path
                    var urlParts = src.split('/Images/');
                    if (urlParts.length > 1) {
                        src = '~/Images/' + urlParts[1];
                    } else {
                        // Try to find Images in URL
                        var imgIndex = src.indexOf('/Images/');
                        if (imgIndex >= 0) {
                            src = '~' + src.substring(imgIndex);
                        }
                    }
                } else if (src.indexOf('/Images/') === 0) {
                    // Absolute path starting with /Images/
                    src = '~' + src;
                } else if (src.indexOf('Images/') === 0) {
                    // Relative path starting with Images/
                    src = '~/Images/' + src.substring(7);
                } else if (src.indexOf('data:image/') === 0) {
                    // Base64 data URL - keep as is
                    // src remains unchanged
                } else if (src.indexOf('~/') !== 0) {
                    // Relative path without ~/
                    if (src.indexOf('/') === 0) {
                        // Absolute path from root
                        src = '~' + src;
                    } else if (src.indexOf('Images/') >= 0) {
                        // Contains Images/ somewhere
                        var imgPos = src.indexOf('Images/');
                        src = '~/Images/' + src.substring(imgPos + 7);
                    } else {
                        // Assume it's in Content folder
                        src = '~/Images/Content/' + src;
                    }
                }
                // If src already starts with ~/, keep it as is
                
                var imageTag = '[IMAGE:' + imageId + ':' + src + ':' + alt + ']';
                var textNode = document.createTextNode(imageTag);
                if (img.parentNode) {
                    img.parentNode.replaceChild(textNode, img);
                }
            });
            
            // Convert HTML to plain text with line breaks
            var text = tempDiv.innerHTML;
            
            // Replace block elements with line breaks
            text = text.replace(/<\/p>/gi, '\n');
            text = text.replace(/<p[^>]*>/gi, '');
            text = text.replace(/<\/div>/gi, '\n');
            text = text.replace(/<div[^>]*>/gi, '');
            text = text.replace(/<\/h[1-6]>/gi, '\n');
            text = text.replace(/<h[1-6][^>]*>/gi, '');
            text = text.replace(/<br\s*\/?>/gi, '\n');
            text = text.replace(/<\/li>/gi, '\n');
            text = text.replace(/<li[^>]*>/gi, '• ');
            
            // Remove remaining HTML tags but keep text and image tags
            var cleanDiv = document.createElement('div');
            cleanDiv.innerHTML = text;
            text = cleanDiv.textContent || cleanDiv.innerText || '';
            
            // Clean up multiple line breaks
            text = text.replace(/\n{3,}/g, '\n\n');
            text = text.trim();
            
            return text;
        }
        
        function formatText(command) {
            var editorDiv = document.getElementById('editorContent');
            if (!editorDiv) return;
            
            editorDiv.focus();
            
            // Try modern Selection API first (for supported browsers)
            try {
                var selection = window.getSelection();
                if (selection.rangeCount > 0) {
                    var range = selection.getRangeAt(0);
                    if (!range.collapsed) {
                        // Use modern approach if possible
                        var documentFragment = range.extractContents();
                        var wrapper = document.createElement(command === 'bold' ? 'strong' : command === 'italic' ? 'em' : command === 'underline' ? 'u' : 'span');
                        wrapper.appendChild(documentFragment);
                        range.insertNode(wrapper);
                        selection.removeAllRanges();
                        selection.addRange(range);
                        return;
                    }
                }
            } catch (err) {
                // Fall back to execCommand if modern API fails
            }
            
            // Fallback to execCommand (deprecated but still widely supported as fallback)
            if (document.execCommand) {
                document.execCommand(command, false, null);
            }
            syncEditorToTextarea();
        }
        
        function handleFormAction() {
            var button = this;
            var form = document.getElementById('aspnetForm') || document.forms[0];
            if (!form) return true;

            var evt = window.event;
            if (evt) {
                evt.preventDefault();
                evt.stopPropagation();
            }

            // Sync editor content first
            var promise = syncEditorToTextarea();
            var finish = function() {
                // After sync, trigger the button click to fire server-side event
                setTimeout(function() {
                    // Remove OnClientClick temporarily to avoid infinite loop
                    var originalOnClientClick = button.getAttribute('onclick');
                    button.removeAttribute('onclick');
                    
                    // Trigger the button click which will fire server-side OnClick
                    if (button.click) {
                        button.click();
                    } else if (typeof __doPostBack !== 'undefined') {
                        // Use __doPostBack as fallback
                        var buttonUniqueId = button.name || button.id;
                        __doPostBack(buttonUniqueId, '');
                    } else {
                        // Last resort: submit form
                    form.submit();
                    }
                    
                    // Restore onclick after a delay
                    setTimeout(function() {
                        if (originalOnClientClick) {
                            button.setAttribute('onclick', originalOnClientClick);
                        }
                    }, 100);
                }, 50);
            };

            if (promise && typeof promise.then === 'function') {
                promise.then(finish).catch(function(err) {
                    console.error('Failed to sync before submit:', err);
                    finish();
                });
            } else {
                finish();
            }

            return false;
        }

        let pendingSync = null;

        function syncEditorToTextarea() {
            var editorDiv = document.getElementById('editorContent');
            var contentTextarea = document.getElementById('<%= txtContent.ClientID %>');
            
            if (editorDiv && contentTextarea) {
                try {
                    var html = editorDiv.innerHTML;
                    
                    // Clean up any remaining Word HTML artifacts
                    html = html.replace(/<!--[iI]f[^>]*>[\s\S]*?<!\[endif\]-->/g, '');
                    
                    // Keep images as data URLs (local only, no server upload)
                    var processedHtml = html;
                        var text = convertHTMLToImageTags(processedHtml);
                    
                    // Only encode to base64 if content contains special characters or HTML
                    // For plain text, store directly
                    var needsEncoding = text.indexOf('<') >= 0 || 
                                       text.indexOf('&') >= 0 || 
                                       text.indexOf('"') >= 0 ||
                                       text.indexOf("'") >= 0 ||
                                       /[^\x20-\x7E\n\r\t]/.test(text);
                    
                    if (needsEncoding) {
                        try {
                            // Encode UTF-8 properly without using deprecated unescape
                            var binaryString = utf8ToBinaryString(text);
                            var encoded = btoa(binaryString);
                            contentTextarea.value = encoded;
                            contentTextarea.setAttribute('data-encoded', 'true');
                        } catch (e) {
                            console.error('Base64 encoding failed:', e);
                            contentTextarea.value = text;
                            contentTextarea.removeAttribute('data-encoded');
                        }
                    } else {
                        // Plain text - store directly without encoding
                        contentTextarea.value = text;
                            contentTextarea.removeAttribute('data-encoded');
                        }
                    return Promise.resolve();
                } catch (e) {
                    console.error('Error syncing editor content:', e);
                    if (contentTextarea) {
                        contentTextarea.value = editorDiv ? editorDiv.innerText || editorDiv.textContent || '' : '';
                    }
                    return Promise.resolve();
                }
            }
            return Promise.resolve();
        }

        function convertDataUrlToStoredImage(dataUrl, fileName) {
            return new Promise(function(resolve, reject) {
                try {
                    var parts = dataUrl.split(',');
                    if (parts.length < 2) {
                        resolve(null);
                        return;
                    }
                    var meta = parts[0];
                    var base64 = parts[1];
                    var mimeMatch = meta.match(/data:(image\/[a-zA-Z0-9.+-]+);base64/);
                    if (!mimeMatch) {
                        resolve(null);
                        return;
                    }
                    var mimeType = mimeMatch[1];
                    var extension = 'png';
                    if (mimeType.indexOf('jpeg') !== -1 || mimeType.indexOf('jpg') !== -1) extension = 'jpg';
                    if (mimeType.indexOf('gif') !== -1) extension = 'gif';
                    if (mimeType.indexOf('png') !== -1) extension = 'png';
                    if (mimeType.indexOf('webp') !== -1) extension = 'webp';

                    var byteCharacters = atob(base64.replace(/\s+/g, ''));
                    var byteNumbers = new Array(byteCharacters.length);
                    for (var i = 0; i < byteCharacters.length; i++) {
                        byteNumbers[i] = byteCharacters.charCodeAt(i);
                    }
                    var byteArray = new Uint8Array(byteNumbers);
                    var blob = new Blob([byteArray], { type: mimeType });
                    var generatedFileName = fileName || ('inline-image.' + extension);

                    var formData = new FormData();
                    formData.append('file', blob, generatedFileName);

                    var xhr = new XMLHttpRequest();
                    xhr.open('POST', 'UploadImage.ashx', true);
                    xhr.onload = function() {
                        if (xhr.status === 200) {
                            try {
                                var response = JSON.parse(xhr.responseText);
                                if (response.success && response.url) {
                                    resolve(response.url);
                                } else {
                                    resolve(null);
                                }
                            } catch (parseErr) {
                                console.error('Failed to parse inline upload response:', parseErr, xhr.responseText);
                                resolve(null);
                            }
                        } else {
                            console.error('Inline image upload failed with status', xhr.status, xhr.responseText);
                            resolve(null);
                        }
                    };
                    xhr.onerror = function(err) {
                        console.error('Inline image upload error:', err);
                        resolve(null);
                    };
                    xhr.send(formData);
                } catch (err) {
                    console.error('convertDataUrlToStoredImage error:', err);
                    resolve(null);
                }
            });
        }
        
        // Intercept form submission to ensure encoding happens
        function interceptFormSubmit() {
            var form = document.getElementById('aspnetForm') || document.forms[0];
            if (form) {
                form.addEventListener('submit', function(e) {
                    // Sync editor content before form submission (synchronous)
                    var editorDiv = document.getElementById('editorContent');
                    var contentTextarea = document.getElementById('<%= txtContent.ClientID %>');
                    
                    if (editorDiv && contentTextarea) {
                        try {
                            var html = editorDiv.innerHTML;
                            html = html.replace(/<!--[iI]f[^>]*>[\s\S]*?<!\[endif\]-->/g, '');
                            var text = convertHTMLToImageTags(html);
                            
                            var needsEncoding = text.indexOf('<') >= 0 || 
                                               text.indexOf('&') >= 0 || 
                                               text.indexOf('"') >= 0 ||
                                               text.indexOf("'") >= 0 ||
                                               /[^\x20-\x7E\n\r\t]/.test(text);
                            
                            if (needsEncoding) {
                                try {
                                    // Encode UTF-8 properly without using deprecated unescape
                                    var binaryString = utf8ToBinaryString(text);
                                    var encoded = btoa(binaryString);
                                    contentTextarea.value = encoded;
                                    contentTextarea.setAttribute('data-encoded', 'true');
                                } catch (e) {
                                    contentTextarea.value = text;
                                    contentTextarea.removeAttribute('data-encoded');
                                }
                            } else {
                                contentTextarea.value = text;
                                contentTextarea.removeAttribute('data-encoded');
                            }
                        } catch (e) {
                            console.error('Error syncing editor content:', e);
                        }
                    }
                    // Allow form to submit normally
                    return true;
                }, false);
            }
        }
        
        // Image upload and insertion functionality (upload immediately, no loading message in editor)
        function uploadImageFile(file) {
            if (file.size > 5 * 1024 * 1024) {
                alert('Ảnh ' + file.name + ' vượt quá 5MB. Vui lòng chọn ảnh nhỏ hơn.');
                return;
            }
            
            var formData = new FormData();
            formData.append('file', file);
            
            var xhr = new XMLHttpRequest();
            xhr.open('POST', 'UploadImage.ashx', true);
            
            // Set timeout (30 seconds)
            xhr.timeout = 30000;
            
            xhr.onload = function() {
                if (xhr.status === 200) {
                    try {
                        var responseText = xhr.responseText.trim();
                        console.log('Upload response:', responseText);
                        
                        var response = JSON.parse(responseText);
                        if (response.success) {
                            insertImageToContent(response.url, response.fileName);
                        } else {
                            alert('Lỗi upload: ' + (response.message || 'Không xác định'));
                        }
                    } catch (e) {
                        console.error('Parse error:', e, 'Response:', xhr.responseText);
                        alert('Lỗi xử lý phản hồi từ server: ' + e.message + '\nPhản hồi: ' + xhr.responseText.substring(0, 200));
                    }
                } else {
                    console.error('Upload failed with status:', xhr.status, 'Response:', xhr.responseText);
                    alert('Lỗi upload ảnh (HTTP ' + xhr.status + '). Vui lòng thử lại.');
                }
            };
            
            xhr.onerror = function() {
                console.error('Upload network error');
                alert('Lỗi kết nối. Vui lòng kiểm tra kết nối mạng và thử lại.');
            };
            
            xhr.ontimeout = function() {
                console.error('Upload timeout');
                alert('Upload ảnh quá lâu. Vui lòng kiểm tra kết nối mạng và thử lại.');
            };
            
            xhr.send(formData);
        }
        
        function insertImageToContent(imageUrl, fileName) {
            var editorDiv = document.getElementById('editorContent');
            if (editorDiv) {
                // Insert image into editor
                // Keep the full URL with ~/ for proper conversion later
                var img = document.createElement('img');
                // Store original URL in data attribute for proper conversion
                var displaySrc = imageUrl.replace('~/', '');
                img.src = displaySrc;
                img.setAttribute('data-original-url', imageUrl); // Store original URL
                img.alt = fileName;
                img.style.maxWidth = '100%';
                img.style.height = 'auto';
                img.style.borderRadius = '5px';
                img.style.margin = '10px 0';
                img.style.display = 'block';
                
                // Generate image ID if not exists
                var imageId = 'img_' + Date.now() + '_' + Math.random().toString(36).substring(2, 11);
                img.setAttribute('data-image-id', imageId);
                
                insertHTMLAtCursor(img.outerHTML);
                syncEditorToTextarea();
            } else {
                // Fallback to textarea if editor not initialized
                var contentTextarea = document.getElementById('<%= txtContent.ClientID %>');
                var imageId = 'img_' + Date.now() + '_' + Math.random().toString(36).substring(2, 11);
                var imageTag = '\n[IMAGE:' + imageId + ':' + imageUrl + ':' + fileName + ']\n';
                
                var cursorPos = contentTextarea.selectionStart;
                var textBefore = contentTextarea.value.substring(0, cursorPos);
                var textAfter = contentTextarea.value.substring(cursorPos);
                
                contentTextarea.value = textBefore + imageTag + textAfter;
            }
            
            // Update preview
            updatePreview();
        }
        
        // Drag and drop directly into editor (handled by paste event handler)
        
        // Make images in content draggable
        function makeImagesDraggable() {
            var contentTextarea = document.getElementById('<%= txtContent.ClientID %>');
            if (!contentTextarea) return;
            
            // Parse content and create visual editor
            var content = contentTextarea.value;
            var imagePattern = /\[IMAGE:([^:]+):((?:.|\r|\n)*?):([^:\]]+)\]/g;
            var matches = [];
            var match;
            
            while ((match = imagePattern.exec(content)) !== null) {
                matches.push({
                    id: match[1],
                    data: match[2],
                    name: match[3],
                    index: match.index,
                    length: match[0].length
                });
            }
        }
        
        function clearContent() {
            if (confirm('Bạn có chắc muốn xóa toàn bộ nội dung?')) {
                var editorDiv = document.getElementById('editorContent');
                if (editorDiv) {
                    editorDiv.innerHTML = '';
                }
                document.getElementById('<%= txtContent.ClientID %>').value = '';
                updatePreview();
            }
        }
        
        function togglePreview() {
            var previewDiv = document.getElementById('contentPreview');
            if (previewDiv.style.display === 'none') {
                updatePreview();
                previewDiv.style.display = 'block';
            } else {
                previewDiv.style.display = 'none';
            }
        }
        
        function updatePreview() {
            var editorDiv = document.getElementById('editorContent');
            var previewDiv = document.getElementById('previewContent');
            if (!previewDiv) return;
            
            var htmlContent = '';
            if (editorDiv) {
                // Get HTML directly from editor
                htmlContent = editorDiv.innerHTML;
                // Also sync to textarea
                syncEditorToTextarea();
            } else {
                // Fallback: get from textarea and convert image tags
                var contentTextarea = document.getElementById('<%= txtContent.ClientID %>');
                if (!contentTextarea) return;
                var content = contentTextarea.value;
                // Convert image tags to HTML for preview
                htmlContent = convertImageTagsToHTML(content);
            }
            
            // Fix image URLs in HTML (convert ~/ to relative paths)
            htmlContent = htmlContent.replace(/src="([^"]*)"/g, function(match, url) {
                if (url.indexOf('~/') === 0) {
                    return 'src="' + url.replace('~/', '') + '"';
                }
                return match;
            });
            
            previewDiv.innerHTML = htmlContent;
            
            // Make images draggable in preview
            makePreviewImagesDraggable();
        }
        
        // Toggle full preview panel (like Details page)
        function toggleFullPreview() {
            var previewPanel = document.getElementById('pnlFullPreview');
            if (!previewPanel) return;
            
            if (previewPanel.style.display === 'none' || previewPanel.style.display === '') {
                updateFullPreview();
                previewPanel.style.display = 'block';
                // Scroll to preview
                previewPanel.scrollIntoView({ behavior: 'smooth', block: 'start' });
            } else {
                previewPanel.style.display = 'none';
            }
        }
        
        // Update full preview content (like Details page)
        function updateFullPreview() {
            var previewContainer = document.getElementById('fullPreviewContent');
            if (!previewContainer) return;
            
            // Get form values
            var title = document.getElementById('<%= txtTitle.ClientID %>').value || 'Chưa có tiêu đề';
            var summary = document.getElementById('<%= txtSummary.ClientID %>').value || '';
            var categorySelect = document.getElementById('<%= ddlCategory.ClientID %>');
            var category = 'Chưa phân loại';
            if (categorySelect && categorySelect.options[categorySelect.selectedIndex]) {
                category = categorySelect.options[categorySelect.selectedIndex].text;
            }
            var titleImage = '';
            
            // Get title image
            var currentImage = document.getElementById('imgCurrent');
            if (currentImage && currentImage.src && currentImage.src.indexOf('data:') !== 0) {
                var imgSrc = currentImage.src;
                // Extract relative path if full URL
                if (imgSrc.indexOf('http') === 0) {
                    var urlParts = imgSrc.split('/');
                    var imagesIndex = urlParts.indexOf('Images');
                    if (imagesIndex >= 0) {
                        imgSrc = '/' + urlParts.slice(imagesIndex).join('/');
                    }
                }
                titleImage = '<div class="mb-4"><img src="' + imgSrc + '" alt="' + escapeHtml(title) + '" class="img-fluid" style="width: 100%; height: auto; border-radius: 10px; box-shadow: 0 4px 12px rgba(0,0,0,0.15);" /></div>';
            } else {
                // Check if new image is selected
                var fileUpload = document.getElementById('<%= fileUploadImage.ClientID %>');
                if (fileUpload && fileUpload.files && fileUpload.files.length > 0) {
                    // Show placeholder for new image
                    titleImage = '<div class="mb-4" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); min-height: 400px; display: flex; align-items: center; justify-content: center; border-radius: 10px;"><div style="text-align: center; color: white;"><div style="font-size: 5rem;">📷</div><p style="margin-top: 20px; font-size: 1.2rem;">Ảnh đã chọn (sẽ hiển thị sau khi lưu)</p></div></div>';
                } else {
                    titleImage = '<div class="mb-4" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); min-height: 400px; display: flex; align-items: center; justify-content: center; border-radius: 10px;"><div style="text-align: center; color: white;"><div style="font-size: 5rem;">🔵</div><p style="margin-top: 20px; font-size: 1.2rem;">Featured Image</p></div></div>';
                }
            }
            
            // Get content
            var editorDiv = document.getElementById('editorContent');
            var content = '';
            if (editorDiv) {
                var html = editorDiv.innerHTML;
                content = convertImageTagsToHTML(html);
                // Fix image URLs
                content = content.replace(/src="([^"]*)"/g, function(match, url) {
                    if (url.indexOf('~/') === 0) {
                        return 'src="' + url.replace('~/', '') + '"';
                    }
                    if (url.indexOf('data:image/') === 0) {
                        return match; // Keep data URLs
                    }
                    return match;
                });
            } else {
                var contentTextarea = document.getElementById('<%= txtContent.ClientID %>');
                if (contentTextarea) {
                    content = convertImageTagsToHTML(contentTextarea.value);
                }
            }
            
            // Format content images
            content = content.replace(/<img([^>]*)>/gi, function(match, attrs) {
                return '<div class="article-image mb-4" style="text-align: center;"><img' + attrs + ' class="img-fluid" style="max-width: 100%; height: auto; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);" /></div>';
            });
            
            // Build preview HTML (like Details page)
            var previewHTML = '<div class="row justify-content-center">' +
                '<div class="col-md-8">' +
                '<span class="badge bg-secondary mb-3">' + escapeHtml(category) + '</span>' +
                '<h1 class="mb-4" style="font-weight: bold; color: #2c3e50;">' + escapeHtml(title) + '</h1>' +
                '<div class="d-flex align-items-center text-muted mb-4">' +
                '<span style="margin-right: 20px;">👤 Tác giả</span>' +
                '<span>📅 ' + new Date().toLocaleDateString('vi-VN') + '</span>' +
                '</div>' +
                titleImage +
                (summary ? '<p class="lead text-muted mb-4">' + escapeHtml(summary) + '</p>' : '') +
                '<div class="article-content" style="line-height: 1.8; color: #495057; font-size: 1.1rem;">' +
                content +
                '</div>' +
                '</div>' +
                '</div>';
            
            previewContainer.innerHTML = previewHTML;
        }
        
        function escapeHtml(text) {
            var div = document.createElement('div');
            div.textContent = text;
            return div.innerHTML;
        }
        
        function makePreviewImagesDraggable() {
            var images = document.querySelectorAll('#previewContent .draggable-image');
            images.forEach(function(img) {
                img.setAttribute('draggable', 'true');
            });
        }
        
        // Drag and drop handlers for images in content
        var draggedImage = null;
        
        function dragImage(e) {
            draggedImage = e.target.closest('.draggable-image');
            e.dataTransfer.effectAllowed = 'move';
            e.dataTransfer.setData('text/html', draggedImage.outerHTML);
            draggedImage.style.opacity = '0.5';
        }
        
        function allowDrop(e) {
            e.preventDefault();
            e.stopPropagation();
            if (e.target.closest('.draggable-image') && e.target.closest('.draggable-image') !== draggedImage) {
                e.target.closest('.draggable-image').style.border = '2px dashed #007bff';
            }
        }
        
        function dropImage(e) {
            e.preventDefault();
            e.stopPropagation();
            
            var target = e.target.closest('.draggable-image');
            if (target && draggedImage && target !== draggedImage) {
                var allImages = Array.from(document.querySelectorAll('#previewContent .draggable-image'));
                var draggedIndex = allImages.indexOf(draggedImage);
                var targetIndex = allImages.indexOf(target);
                
                if (draggedIndex < targetIndex) {
                    target.parentNode.insertBefore(draggedImage, target.nextSibling);
                } else {
                    target.parentNode.insertBefore(draggedImage, target);
                }
                
                // Update content textarea with new order
                updateContentFromPreview();
            }
            
            // Reset styles
            document.querySelectorAll('#previewContent .draggable-image').forEach(function(img) {
                img.style.border = '';
                img.style.opacity = '1';
            });
            
            draggedImage = null;
        }
        
        function updateContentFromPreview() {
            var previewDiv = document.getElementById('previewContent');
            var contentTextarea = document.getElementById('<%= txtContent.ClientID %>');
            if (!previewDiv || !contentTextarea) return;
            
            // Extract text content (non-image parts)
            var contentTextarea = document.getElementById('<%= txtContent.ClientID %>');
            var originalContent = contentTextarea.value;
            
            // Get images in new order
            var images = document.querySelectorAll('#previewContent .draggable-image');
            var imageData = [];
            images.forEach(function(img) {
                var imgTag = img.querySelector('img');
                var name = img.querySelector('small').textContent;
                var id = img.getAttribute('data-image-id');
                // Get the original URL (not the resolved src)
                var data = imgTag.getAttribute('data-original-url') || imgTag.src;
                if (data) {
                    data = data.trim();
                }
                
                // If it's a full URL, convert back to relative path
                if (data.indexOf('http://') === 0 || data.indexOf('https://') === 0) {
                    // Extract relative path from full URL
                    var urlParts = data.split('/Images/');
                    if (urlParts.length > 1) {
                        data = '~/Images/' + urlParts[1];
                    }
                } else if (data.indexOf('/Images/') === 0) {
                    // Convert absolute path to relative with ~/
                    data = '~' + data;
                } else if (data.indexOf('Images/') === 0) {
                    // Convert relative path to ~/ format
                    data = '~/Images/' + data.substring(7);
                }
                // Keep base64 and ~/ format as is
                
                imageData.push({
                    id: id,
                    data: data,
                    name: name
                });
            });
            
            // Rebuild content with images in new order
            var textParts = originalContent.split(/\[IMAGE:[^\]]+\]/);
            var newContent = '';
            
            for (var i = 0; i < textParts.length; i++) {
                newContent += textParts[i];
                if (i < imageData.length) {
                    newContent += '[IMAGE:' + imageData[i].id + ':' + imageData[i].data + ':' + imageData[i].name + ']';
                }
            }
            
            contentTextarea.value = newContent;
        }
        
        function removeImage(imageId) {
            if (confirm('Bạn có chắc muốn xóa ảnh này khỏi nội dung?')) {
                var contentTextarea = document.getElementById('<%= txtContent.ClientID %>');
                var content = contentTextarea.value;
                var imagePattern = new RegExp('\\[IMAGE:' + imageId + ':[^\\]]+\\]', 'g');
                contentTextarea.value = content.replace(imagePattern, '');
                updatePreview();
            }
        }
        
        // Initialize editor when page loads
        document.addEventListener('DOMContentLoaded', function() {
            initializeEditor();
            interceptFormSubmit();
            
            // Also listen to textarea changes (fallback)
            var contentTextarea = document.getElementById('<%= txtContent.ClientID %>');
            if (contentTextarea) {
                contentTextarea.addEventListener('input', function() {
                    // Debounce preview update
                    clearTimeout(this.updateTimeout);
                    this.updateTimeout = setTimeout(function() {
                        updatePreview();
                    }, 500);
                });
            }
        });
    </script>
</asp:Content>

