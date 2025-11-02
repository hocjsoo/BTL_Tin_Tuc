// Auto-search functionality for Search.aspx
(function() {
    var searchBox = document.getElementById('<%= txtSearch.ClientID %>');
    var searchTimer;
    
    if (searchBox) {
        // Auto-search on typing with debounce
        searchBox.addEventListener('keyup', function() {
            clearTimeout(searchTimer);
            var keyword = searchBox.value.trim();
            
            // If empty, show all articles (will be handled by server)
            if (keyword.length === 0) {
                // Trigger search with empty to show all
                performSearch('');
                return;
            }
            
            // Debounce search - wait 300ms after user stops typing
            searchTimer = setTimeout(function() {
                performSearch(keyword);
            }, 300);
        });
        
        // Also search on Enter key
        searchBox.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                clearTimeout(searchTimer);
                e.preventDefault();
                var keyword = searchBox.value.trim();
                performSearch(keyword);
            }
        });
    }
    
    function performSearch(keyword) {
        // Use __doPostBack to trigger server-side search
        if (typeof __doPostBack !== 'undefined') {
            // Create a hidden button click to trigger search
            var searchButton = document.getElementById('<%= btnSearch.ClientID %>');
            if (searchButton) {
                searchButton.click();
            }
        } else {
            // Fallback: use form submission
            var form = document.forms[0];
            if (form) {
                var input = document.createElement('input');
                input.type = 'hidden';
                input.name = '__EVENTTARGET';
                input.value = '<%= btnSearch.UniqueID %>';
                form.appendChild(input);
                form.submit();
            }
        }
    }
})();

// Client-side search calling server WebMethod (legacy - kept for compatibility)
function searchNews() {
    var keyword = document.getElementById('txtSearch')?.value || '';
    var payload = JSON.stringify({ keyword: keyword });
    var results = document.getElementById('results');
    if (!results) return;
    results.innerHTML = '<div class="text-muted">Đang tìm kiếm...</div>';
    fetch('Search.aspx/DoSearch', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=utf-8'
        },
        body: payload
    })
        .then(function (r) { return r.json(); })
        .then(function (data) {
            var items = [];
            if (data && data.d) {
                items = JSON.parse(data.d);
            }
            if (!items || items.length === 0) {
                results.innerHTML = '<div class="alert alert-info">Không có kết quả.</div>';
                return;
            }
            var html = '';
            var kw = (keyword || '').trim();
            var rx = kw ? new RegExp(kw.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), 'gi') : null;
            items.forEach(function (n) {
                html += '<div class="col-md-6 mb-3 fade-in">'
                    + '<div class="card h-100">'
                    + '<div class="card-body">'
                    + '<h5 class="card-title">' + highlight(escapeHtml(n.Title), rx) + '</h5>'
                    + '<p class="card-text">' + highlight(escapeHtml(n.Summary), rx) + '</p>'
                    + '<a class="btn btn-primary" href="Details.aspx?id=' + encodeURIComponent(n.Id) + '">Xem chi tiết</a>'
                    + '</div>'
                    + '<div class="card-footer text-muted">' + n.CreatedAt + '</div>'
                    + '</div>'
                    + '</div>';
            });
            results.innerHTML = html;
            var nodes = results.querySelectorAll('.fade-in');
            nodes.forEach(function (el) {
                el.style.opacity = 0;
                setTimeout(function () {
                    el.style.transition = 'opacity 0.4s';
                    el.style.opacity = 1;
                }, 0);
            });
        })
        .catch(function () {
            results.innerHTML = '<div class="alert alert-danger">Lỗi tìm kiếm.</div>';
        });
}

function escapeHtml(s) {
    return (s || '').replace(/[&<>"']/g, function (c) {
        return ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' })[c];
    });
}

function highlight(text, rx) {
    if (!rx) return text;
    return text.replace(rx, function (m) { return '<mark>' + m + '</mark>'; });
}

// Auto-hide bootstrap-like alerts/labels messages after 2.5s
(function autoHideMessages(){
    var h = setInterval(function(){
        var el = document.querySelector('.ms-3.text-success, .ms-3.text-danger, .alert');
        if (el) {
            setTimeout(function(){
                if (el.style) {
                    el.style.transition = 'opacity .4s';
                    el.style.opacity = 0;
                }
            }, 2500);
            clearInterval(h);
        }
    }, 500);
})();
