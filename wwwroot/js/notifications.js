// In-app notifications: wires the navbar bell to the NotificationController API.
(function () {
    var bell = document.getElementById('notificationBell');
    if (!bell) return; // not authenticated / bell not rendered

    var badge = document.getElementById('notificationBadge');
    var list = document.getElementById('notificationList');
    var markAllBtn = document.getElementById('markAllRead');

    function setBadge(count) {
        if (!badge) return;
        if (count > 0) {
            badge.textContent = count > 9 ? '9+' : String(count);
            badge.classList.remove('d-none');
        } else {
            badge.classList.add('d-none');
        }
    }

    function loadCount() {
        fetch('/Notification/GetUnreadCount', { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
            .then(function (r) { return r.ok ? r.json() : { count: 0 }; })
            .then(function (d) { setBadge(d.count || 0); })
            .catch(function () { /* ignore transient errors */ });
    }

    function emptyState() {
        list.innerHTML = '';
        var div = document.createElement('div');
        div.className = 'text-center text-muted py-3';
        div.innerHTML = '<small>No notifications</small>';
        list.appendChild(div);
    }

    function renderList(items) {
        list.innerHTML = '';
        if (!items || items.length === 0) { emptyState(); return; }

        items.forEach(function (n) {
            var a = document.createElement('a');
            a.href = n.link || '#';
            a.className = 'dropdown-item d-block border-bottom py-2 ' + (n.isRead ? '' : 'bg-light');
            a.style.whiteSpace = 'normal';

            var title = document.createElement('div');
            title.className = 'd-flex justify-content-between align-items-start';
            var titleText = document.createElement('span');
            titleText.className = n.isRead ? 'fw-normal' : 'fw-bold';
            titleText.textContent = n.title;
            var time = document.createElement('small');
            time.className = 'text-muted ms-2 flex-shrink-0';
            time.textContent = n.time;
            title.appendChild(titleText);
            title.appendChild(time);

            var msg = document.createElement('small');
            msg.className = 'text-muted d-block';
            msg.textContent = n.message;

            a.appendChild(title);
            a.appendChild(msg);

            a.addEventListener('click', function (e) {
                // Mark read first, then follow the link (so the POST isn't cancelled by navigation)
                e.preventDefault();
                fetch('/Notification/MarkAsRead?id=' + n.id, {
                    method: 'POST',
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                }).then(function () {
                    loadCount();
                    if (n.link) { window.location.href = n.link; }
                    else { renderList(); }
                }).catch(function () {
                    if (n.link) window.location.href = n.link;
                });
            });

            list.appendChild(a);
        });
    }

    function loadList() {
        fetch('/Notification/GetRecent', { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
            .then(function (r) { return r.ok ? r.json() : []; })
            .then(function (items) { renderList(items); })
            .catch(function () { emptyState(); });
    }

    // Load the list when the dropdown opens
    bell.addEventListener('show.bs.dropdown', loadList);

    if (markAllBtn) {
        markAllBtn.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            fetch('/Notification/MarkAllAsRead', {
                method: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }).then(function () { loadCount(); loadList(); });
        });
    }

    // Initial load + poll every 30s
    loadCount();
    setInterval(loadCount, 30000);
})();
