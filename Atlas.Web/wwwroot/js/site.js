// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('click', function (event) {
	const clickableRow = event.target.closest('.clickable-row');

	if (!clickableRow || event.target.closest('a, button, input, select, textarea, label')) {
		return;
	}

	const targetUrl = clickableRow.getAttribute('data-href');
	if (targetUrl) {
		window.location.href = targetUrl;
	}
});

document.addEventListener('keydown', function (event) {
	const clickableRow = event.target.closest('.clickable-row');

	if (!clickableRow) {
		return;
	}

	if (event.key === 'Enter' || event.key === ' ') {
		event.preventDefault();
		const targetUrl = clickableRow.getAttribute('data-href');
		if (targetUrl) {
			window.location.href = targetUrl;
		}
	}
});
