const openButton = document.getElementById('open-feedback');

openButton.addEventListener('click', () => {
    const feedbackForm = document.getElementById('feedback-form');
    const feedbackInitial = document.getElementById('feedback-initial');
    
    feedbackForm.classList.toggle('footer-feedback__hidden', false);
    feedbackForm.classList.add('footer-feedback__row--submitted');
    
    feedbackInitial.classList.add('footer-feedback__hidden');
})

const cancelButton = document.getElementById('cancel-feedback');

cancelButton.addEventListener('click', () => {
    const feedbackForm = document.getElementById('feedback-form');
    const feedbackInitial = document.getElementById('feedback-initial');

    feedbackForm.classList.toggle('footer-feedback__hidden', true);
    feedbackForm.classList.remove('footer-feedback__row--submitted');

    feedbackInitial.classList.remove('footer-feedback__hidden');
})

