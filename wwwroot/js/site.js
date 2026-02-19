// Sidebar active link highlighting
document.addEventListener('DOMContentLoaded', function () {
    const currentPath = window.location.pathname.toLowerCase();
    const sidebarLinks = document.querySelectorAll('#sidebar .nav-link');

    sidebarLinks.forEach(function (link) {
        const href = link.getAttribute('href');
        if (href && currentPath.startsWith(href.toLowerCase())) {
            link.classList.add('active');
        }
    });

    // Initialize theme
    initializeTheme();
});

// ===== Dark Mode / Theme Toggle =====

function initializeTheme() {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme) {
        setTheme(savedTheme);
    } else {
        // Check system preference
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        setTheme(prefersDark ? 'dark' : 'light');
    }
}

function setTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('theme', theme);

    // Update toggle button icon
    const toggleBtn = document.getElementById('themeToggle');
    if (toggleBtn) {
        const icon = toggleBtn.querySelector('i');
        if (icon) {
            icon.className = theme === 'dark' ? 'bi bi-sun-fill' : 'bi bi-moon-fill';
        }
    }

    // Update Chart.js colors if charts exist
    updateChartColors(theme);
}

function toggleTheme() {
    const currentTheme = document.documentElement.getAttribute('data-theme') || 'light';
    setTheme(currentTheme === 'dark' ? 'light' : 'dark');
}

function updateChartColors(theme) {
    if (typeof Chart === 'undefined') return;

    const textColor = theme === 'dark' ? '#e4e6ea' : '#666';
    const gridColor = theme === 'dark' ? 'rgba(255, 255, 255, 0.1)' : 'rgba(0, 0, 0, 0.1)';

    Chart.defaults.color = textColor;
    Chart.defaults.borderColor = gridColor;

    // Update all existing chart instances
    Chart.helpers.each(Chart.instances, function (chart) {
        if (chart.config && chart.config.options) {
            // Update scale colors
            if (chart.config.options.scales) {
                Object.values(chart.config.options.scales).forEach(function (scale) {
                    if (scale.ticks) scale.ticks.color = textColor;
                    if (scale.grid) scale.grid.color = gridColor;
                });
            }
            // Update legend colors
            if (chart.config.options.plugins && chart.config.options.plugins.legend) {
                if (chart.config.options.plugins.legend.labels) {
                    chart.config.options.plugins.legend.labels.color = textColor;
                }
            }
        }
        chart.update('none');
    });
}

// ===== Double-Submit Prevention =====

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('form').forEach(function (form) {
        form.addEventListener('submit', function () {
            var buttons = this.querySelectorAll('button[type="submit"], input[type="submit"]');
            buttons.forEach(function (btn) {
                if (btn.disabled) return;
                btn.disabled = true;
                var originalText = btn.innerHTML;
                btn.dataset.originalText = originalText;
                btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1" role="status"></span> Processing...';
                // Re-enable after 5s as fallback (e.g. validation errors that don't navigate)
                setTimeout(function () {
                    btn.disabled = false;
                    btn.innerHTML = originalText;
                }, 5000);
            });
        });
    });
});

// ===== Confirmation Dialogs =====

// Auto-attach confirm dialogs to forms with data-confirm attribute
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('form[data-confirm]').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            if (!confirm(this.dataset.confirm)) {
                e.preventDefault();
            }
        });
    });
});

// ===== Notification Bell =====

document.addEventListener('DOMContentLoaded', function () {
    var bell = document.getElementById('notificationBell');
    if (!bell) return;

    function updateBadge(count) {
        var badge = document.getElementById('notificationBadge');
        if (!badge) return;
        if (count > 0) {
            badge.textContent = count > 99 ? '99+' : count;
            badge.classList.remove('d-none');
        } else {
            badge.classList.add('d-none');
        }
    }

    function loadNotifications() {
        fetch('/Notification/GetRecent')
            .then(function (r) { return r.json(); })
            .then(function (data) {
                var list = document.getElementById('notificationList');
                if (!list) return;
                if (!data || data.length === 0) {
                    list.innerHTML = '<div class="text-center text-muted py-3"><small>No notifications</small></div>';
                    return;
                }
                var html = '';
                data.forEach(function (n) {
                    var cls = n.isRead ? '' : 'bg-light';
                    html += '<a href="' + (n.link || '#') + '" class="dropdown-item py-2 border-bottom ' + cls + '" data-id="' + n.id + '">' +
                        '<div class="fw-semibold small">' + escapeHtml(n.title) + '</div>' +
                        '<div class="text-muted small text-truncate">' + escapeHtml(n.message) + '</div>' +
                        '<div class="text-muted" style="font-size:0.7rem;">' + n.time + '</div>' +
                        '</a>';
                });
                list.innerHTML = html;

                // Mark as read on click
                list.querySelectorAll('a[data-id]').forEach(function (a) {
                    a.addEventListener('click', function () {
                        fetch('/Notification/MarkAsRead?id=' + this.dataset.id, { method: 'POST' });
                    });
                });
            })
            .catch(function () { });
    }

    function fetchUnreadCount() {
        fetch('/Notification/GetUnreadCount')
            .then(function (r) { return r.json(); })
            .then(function (data) { updateBadge(data.count); })
            .catch(function () { });
    }

    function escapeHtml(text) {
        var div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Load on bell click
    bell.addEventListener('click', function () {
        loadNotifications();
    });

    // Mark all as read
    var markAllBtn = document.getElementById('markAllRead');
    if (markAllBtn) {
        markAllBtn.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            fetch('/Notification/MarkAllAsRead', { method: 'POST' })
                .then(function () {
                    updateBadge(0);
                    loadNotifications();
                });
        });
    }

    // Initial load + poll every 60s
    fetchUnreadCount();
    setInterval(fetchUnreadCount, 60000);
});

// Listen for system preference changes
window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function (e) {
    if (!localStorage.getItem('theme')) {
        setTheme(e.matches ? 'dark' : 'light');
    }
});
