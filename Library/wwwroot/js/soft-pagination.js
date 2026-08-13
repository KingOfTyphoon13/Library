document.addEventListener('click', async function (e) {
    var link = e.target.closest('.page-link');
    if (!link || link.classList.contains('disabled')) return;

    e.preventDefault();
    await loadPage(link.href, link.closest('.pagination'));
});

document.addEventListener('submit', async function (e) {
    var form = e.target.closest('.page-jump-form');
    if (!form) return;

    e.preventDefault();
    var pagination = form.closest('.pagination');
    var input = form.querySelector('.page-jump-input');
    var pageNumber = parseInt(input.value, 10);
    if (!pageNumber || pageNumber < 1) return;

    var url = new URL(form.action, window.location.href);
    url.searchParams.set('pageNumber', pageNumber);
    await loadPage(url.toString(), pagination);
});

async function loadPage(url, pagination) {
    var containerId = pagination.dataset.container;
    var container = document.getElementById(containerId);
    if (!container) return;

    container.classList.add('fade-out');

    try {
        var response = await fetch(url, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });
        var html = await response.text();

        await new Promise(function (resolve) {
            setTimeout(resolve, 150);
        });

        container.innerHTML = html;
        container.classList.remove('fade-out');
        container.classList.add('fade-in');

        setTimeout(function () {
            container.classList.remove('fade-in');
        }, 150);

        window.history.pushState({}, '', url);
    } catch (err) {
        container.classList.remove('fade-out');
        console.error('Pagination load failed:', err);
    }
}