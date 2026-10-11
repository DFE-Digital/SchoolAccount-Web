const feedbackForm = document.getElementById('feedback-form');

if (feedbackForm) {
    const initial = document.getElementById('feedback-initial');
    const cancel = document.getElementById('cancel-feedback');

    const showForm = (open) => {
        feedbackForm.classList.toggle('govuk-!-display-none', !open);
        initial.classList.toggle('govuk-!-display-none', open);
    };

    cancel.classList.remove('govuk-!-display-none');

    showForm(feedbackForm.dataset.hasErrors === 'true');

    document.getElementById('open-feedback').addEventListener('click', () => showForm(true));
    cancel.addEventListener('click', () => showForm(false));
}
