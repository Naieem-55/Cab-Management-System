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

// Listen for system preference changes
window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function (e) {
    if (!localStorage.getItem('theme')) {
        setTheme(e.matches ? 'dark' : 'light');
    }
});
