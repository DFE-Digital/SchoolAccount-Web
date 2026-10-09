function setHidden (id, hidden) {
    document.getElementById(id).classList.toggle('govuk-!-display-none', hidden);
}

function showForm () {
    setHidden('feedback-form', false);
    setHidden('cancel-feedback', false);
    setHidden('feedback-initial', true);
}

function showInitial () {
    setHidden('feedback-form', true);
    setHidden('feedback-initial', false);
}

const feedbackForm = document.getElementById('feedback-form');

// The thank-you message replaces the form, so there's nothing to set up
if (feedbackForm) {
    // After a submit with errors, keep the form open so its error summary shows
    if (feedbackForm.dataset.hasErrors === 'true') {
        showForm();
    } else {
        showInitial();
    }

    document.getElementById('open-feedback').addEventListener('click', showForm);
    document.getElementById('cancel-feedback').addEventListener('click', showInitial);
}
