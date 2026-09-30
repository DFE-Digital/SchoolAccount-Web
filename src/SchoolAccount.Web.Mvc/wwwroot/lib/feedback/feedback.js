(function () {
    const section = document.getElementById('page-feedback');
    if (!section) return;

    section.classList.add('footer-feedback--js');

    section.addEventListener('click', (e) => {
        const opening = e.target.closest('.footer-feedback__primary');
        const cancelling = e.target.closest('.footer-feedback__cancel');
        if (!opening && !cancelling) return;

        section.classList.toggle('footer-feedback--open', !!opening);
        const button = section.querySelector('.footer-feedback__primary');
        button.setAttribute('aria-expanded', String(!!opening));
        (opening ? section.querySelector('#feedbackMessage') : button).focus();
    });

    section.addEventListener('submit', async (e) => {
        e.preventDefault();
        const form = e.target;

        try {
            const response = await fetch(form.action, {
                method: 'POST',
                body: new FormData(form),
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
            });
            if (!response.ok) throw new Error(response.statusText);

            const doc = new DOMParser().parseFromString(await response.text(), 'text/html');
            section.replaceWith(doc.getElementById('page-feedback'));
            document.getElementById('feedback-thanks')?.focus();
        } catch {
            form.submit();
        }
    });
})();