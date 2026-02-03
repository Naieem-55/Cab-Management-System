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
});
