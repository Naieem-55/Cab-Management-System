// Global top-bar search: debounced fetch -> grouped dropdown results.
(function () {
    var input = document.getElementById('globalSearchInput');
    var panel = document.getElementById('globalSearchResults');
    if (!input || !panel) return;

    var debounceTimer = null;
    var activeController = null;
    var lastQuery = '';

    function escapeHtml(s) {
        if (s == null) return '';
        return String(s)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

    function openPanel() { panel.classList.add('show'); }
    function closePanel() { panel.classList.remove('show'); }

    function render(results, query) {
        if (!results || results.length === 0) {
            panel.innerHTML = '<div class="px-3 py-3 text-center text-muted"><small>No results for "'
                + escapeHtml(query) + '"</small></div>';
            openPanel();
            return;
        }

        // Group by Type, preserving server order.
        var groups = {};
        var order = [];
        results.forEach(function (r) {
            if (!groups[r.type]) { groups[r.type] = []; order.push(r.type); }
            groups[r.type].push(r);
        });

        var html = '';
        order.forEach(function (type) {
            html += '<h6 class="dropdown-header text-uppercase small fw-bold">' + escapeHtml(type) + 's</h6>';
            groups[type].forEach(function (r) {
                html += '<a class="dropdown-item d-flex align-items-center gap-2 py-2" href="' + escapeHtml(r.url) + '">'
                    + '<i class="bi ' + escapeHtml(r.icon) + ' text-muted"></i>'
                    + '<span class="text-truncate">'
                    + '<span class="d-block text-truncate">' + escapeHtml(r.label) + '</span>'
                    + (r.sublabel ? '<small class="text-muted d-block text-truncate">' + escapeHtml(r.sublabel) + '</small>' : '')
                    + '</span></a>';
            });
        });
        panel.innerHTML = html;
        openPanel();
    }

    function doSearch(query) {
        if (activeController) { activeController.abort(); }
        activeController = new AbortController();
        fetch('/GlobalSearch/Search?q=' + encodeURIComponent(query), {
            signal: activeController.signal,
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
            .then(function (resp) { return resp.ok ? resp.json() : { results: [] }; })
            .then(function (data) {
                if (query !== lastQuery) return; // stale
                render(data.results, query);
            })
            .catch(function () { /* aborted or network error */ });
    }

    input.addEventListener('input', function () {
        var query = input.value.trim();
        lastQuery = query;
        if (debounceTimer) clearTimeout(debounceTimer);
        if (query.length < 2) { closePanel(); panel.innerHTML = ''; return; }
        debounceTimer = setTimeout(function () { doSearch(query); }, 250);
    });

    input.addEventListener('focus', function () {
        if (panel.innerHTML.trim() !== '') openPanel();
    });

    input.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') { closePanel(); input.blur(); }
    });

    document.addEventListener('click', function (e) {
        if (!panel.contains(e.target) && e.target !== input) closePanel();
    });
})();
