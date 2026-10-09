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

