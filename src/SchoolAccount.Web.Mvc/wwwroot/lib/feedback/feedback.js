function showForm () {
    const feedbackForm = document.getElementById('feedback-form');
    const feedbackInitial = document.getElementById('feedback-initial');
    const cancelButton = document.getElementById('cancel-feedback');

    feedbackForm.classList.toggle('footer-feedback__hidden', false);
    feedbackForm.classList.add('footer-feedback__row--submitted');
    feedbackForm.setAttribute('aria-hidden', 'false');
    cancelButton.classList.remove('footer-feedback__hidden');
    cancelButton.setAttribute('aria-hidden', 'false');

    feedbackInitial.classList.add('footer-feedback__hidden');
    feedbackInitial.setAttribute('aria-hidden', 'true');
}

function showInitial () {
    const feedbackForm = document.getElementById('feedback-form');
    const feedbackInitial = document.getElementById('feedback-initial');

    feedbackForm.classList.toggle('footer-feedback__hidden', true);
    feedbackForm.classList.remove('footer-feedback__row--submitted');
    feedbackForm.setAttribute('aria-hidden', 'true');

    feedbackInitial.classList.remove('footer-feedback__hidden');
    feedbackInitial.setAttribute('aria-hidden', 'false');
}

// After a submit with errors, keep the form open so its error summary shows
if (document.getElementById('feedback-form')?.dataset.hasErrors === 'true') {
    showForm();
} else {
    showInitial();
}

const openButton = document.getElementById('open-feedback');

openButton.addEventListener('click', () => {
    showForm();
})

const cancelButton = document.getElementById('cancel-feedback');

cancelButton.addEventListener('click', () => {
    showInitial();
})

