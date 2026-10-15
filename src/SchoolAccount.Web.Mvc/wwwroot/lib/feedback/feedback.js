const feedbackForm = document.getElementById('feedback-form');

if (feedbackForm) {
    const initial = document.getElementById('feedback-initial');
    const cancel = document.getElementById('cancel-feedback');
    const textArea = document.getElementById('FeedbackMessage');
    const openFeedback = document.getElementById('open-feedback');

    const showForm = (open) => {
        feedbackForm.classList.toggle('govuk-!-display-none', !open);
        initial.classList.toggle('govuk-!-display-none', open);
        textArea.focus();
    };

    cancel.classList.remove('govuk-!-display-none');

    showForm(feedbackForm.dataset.hasErrors === 'true');

    document.getElementById('open-feedback').addEventListener('click', () => showForm(true));
    cancel.addEventListener('click', () => showForm(false));
    cancel.addEventListener('click', () => openFeedback.focus())
}
